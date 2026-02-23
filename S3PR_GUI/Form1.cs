using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms.VisualStyles;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace S3PR_GUI
{
    public partial class Form1 : Form
    {

        private bool stopExecution = false;
        private string lastFile = "";
        private bool isDone = false;

        public Form1()
        {
            InitializeComponent();
        }

        /**
         * while loading form
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            string path_default = "";
            checkBox1.Checked = Properties.Settings.Default.checkBox1;
            checkBox2.Checked = Properties.Settings.Default.checkBox2;
            checkBox3.Checked = Properties.Settings.Default.checkBox3;
            checkBox4.Checked = Properties.Settings.Default.checkBox4;
            checkBox5.Checked = Properties.Settings.Default.checkBox5;
            textBox1.Text = Properties.Settings.Default.textBox1;
            if (string.IsNullOrEmpty(textBox1.Text) &&
                Path.Exists(path_default = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Electronic Arts", "The Sims 3", "Mods")))
            {
                textBox1.Text = path_default;
            }
        }

        /**
         * on browse button click
         */
        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = string.Join(", ", dialog.SelectedPaths);
            }
        }


        /**
         * on reduce button click
         */
        private async void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text != "Stop" && !stopExecution)
            {
                // reset loading label beneath the loading bar
                updateLoadingLabel("");

                // save all Values from the UI Elements to the user default values
                saveUserDefaultValues();

                // show message to warn user to have a backup, just in case
                // abort if the user dialog was negative
                if (!showMessageBackupWarning()) return;

                // set cursor to waiting 
                switchBetweenNormalAndWaitingCursor();

                // set progress bar to 2% at start, to signalise it started
                await Task.Run(() =>
                {
                    updateProgressBar(2);
                });


                // disable all UI input elements, to signalize it's not available
                disableEnableAllUIInputElements(false);

                // switch reduce button to stop button
                switchBetweenReduceAndStopButton();

                // execute reduce async
                await Task.Run(() =>
                {
                    executeReduce();
                });

                // reset progress bar
                updateProgressBar(0);

                // reset cursor
                switchBetweenNormalAndWaitingCursor();

                // enable all UI input elements, to signalize it's available
                disableEnableAllUIInputElements(true);

                // reset the stop/reduce button
                switchBetweenReduceAndStopButton();

                // reset stop execution flag
                stopExecution = false;

                // reset is done flag
                isDone = false;
            }
            else
            {
                // set flag to skip all further processing
                stopExecution = true;

                // update loading label
                if (!isDone)
                {
                    updateLoadingLabel($"Stops after finishing: {Path.GetFileName(lastFile)} ...");
                }
            }
        }


        /**
         * the main method, that iterates through all files and reduces them
         */
        private void executeReduce()
        {
            bool removeThumbnails = checkBox1.Checked;
            bool removeIcons = checkBox2.Checked;
            bool searchRecursive = checkBox3.Checked;
            bool compressFile = checkBox4.Checked;
            string[] pathFolderList = textBox1.Text.Split(", ");
            List<string> pathFileList = new List<string>();
            int progress = 0;
            int progessTillStopped = 0;
            double fileSizeBeforeInByte = 0;
            double fileSizeAfterInByte = 0;
            string pathS3RC = ExtractS3RC();

            if (pathFolderList.Length <= 0)
            {
                MessageBox.Show("Please select folders with Package-Files first.", "No Folders found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (removeIcons == false && removeThumbnails == false && compressFile == false)
            {
                MessageBox.Show($"Please click \"{checkBox1.Text}\" or \"{checkBox2.Text}\" or both to remove anything from Package-Files.", "What should be removed?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // find all package files in all folders
                foreach (string pathFolder in pathFolderList)
                {
                    pathFileList = pathFileList.Concat(from file in Directory.GetFiles(pathFolder, "*.*", searchRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                                                       where file.EndsWith(".package", StringComparison.OrdinalIgnoreCase)
                                                       select file).ToList<string>();
                }

                // warn that no package files exist in the source folder
                if (pathFileList.Count <= 0)
                {
                    MessageBox.Show($"The selected folder{(pathFolderList.Length > 1 ? "s do" : " does")} not contain any Package-Files. Please select another Folder.", "No Package Files found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // reduce each file seperately
                foreach (string pathFile in pathFileList)
                {
                    updateLoadingLabel($"Reducing {Path.GetFileName(pathFile)} ...");
                    if (!stopExecution)
                    {
                        progessTillStopped++;
                        lastFile = pathFile;
                        fileSizeBeforeInByte += (double)(new FileInfo(pathFile)).Length;
                        S3PR.S3PR.RemoveThumbnail = removeThumbnails;
                        S3PR.S3PR.RemoveIcon = removeIcons;
                        S3PR.S3PR.ReducePackages([pathFile]);
                        if (compressFile) S3RC(pathFile, pathS3RC);
                        fileSizeAfterInByte += (double)(new FileInfo(pathFile)).Length;
                    }
                    int progressBarPercentValue = (++progress * 100) / pathFileList.Count;
                    updateProgressBar(progressBarPercentValue < 2 ? 2 : progressBarPercentValue);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Something went wrong!\n\n" +
                    $"If this happens repeatedly, please tell OhRudi (the Creator) by commenting on the download page.\n\n" +
                    $"Error Message: {exception.Message}\n\n" +
                    $"Path: {lastFile}", "Uuupsi ...", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                return;
            }
            finally
            {
                isDone = true;
                try { File.Delete(pathS3RC); }
                catch { /* Do nothing */ }
                updateLoadingLabel("Done.");
            }


            // if process got stopped via UI-Button
            if (stopExecution)
            {
                MessageBox.Show(
                    $"Process Stopped.\n\n" +
                    (
                        fileSizeBeforeInByte == fileSizeAfterInByte || (fileSizeBeforeInByte - fileSizeAfterInByte) < 0
                        ? $"Before it stopped, it checked {progessTillStopped} Package-File{(progessTillStopped > 1 ? "s" : "")} but, {(progessTillStopped > 1 ? "they were" : "it was")} already reduced, so nothing changed.\n\n"
                        : $"Before it stopped, it reduced {progessTillStopped} Package-File{(progessTillStopped > 1 ? "s" : "")} in total by {convertByteToOtherUnit(fileSizeBeforeInByte - fileSizeAfterInByte)}\n\n"
                    ) +
                    $"Last processed File: {lastFile}",
                    "Oh, you stopped?",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            // if process ran sucessfully
            else
            {
                MessageBox.Show($"Done.\n\n" +
                    (
                        fileSizeBeforeInByte == fileSizeAfterInByte || (fileSizeBeforeInByte - fileSizeAfterInByte) < 0
                        ? $"It checked {pathFileList.Count} Package-File{(progessTillStopped > 1 ? "s" : "")} but, {(progessTillStopped > 1 ? "they were" : "it was")} already reduced, so nothing changed."
                        : $"Reduced {pathFileList.Count} Package-File{(progessTillStopped > 1 ? "s" : "")} in total by {convertByteToOtherUnit(fileSizeBeforeInByte - fileSizeAfterInByte)}"
                    ),
                    "Wuhuuu, it's done!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }


        /**
         * update progress bar thread safe
         */
        private void updateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired) progressBar1.Invoke(new Action(() => { progressBar1.Value = value; }));
            else progressBar1.Value = value;
        }


        private void updateLoadingLabel(string text)
        {
            if (label1.InvokeRequired) label1.Invoke(new Action(() => { label1.Text = text; }));
            else label1.Text = text;
        }


        /**
         * switch between "Reduce" and "Stop" text on the execution button
         */
        private void switchBetweenNormalAndWaitingCursor()
        {
            this.Cursor = this.Cursor != Cursors.WaitCursor ? Cursors.WaitCursor : Cursors.Default;
        }


        /**
         * switch between "Reduce" and "Stop" text on the execution button
         */
        private void switchBetweenReduceAndStopButton()
        {
            if (button2.InvokeRequired) button2.Invoke(new Action(() => { button2.Text = button2.Text != "Stop" ? "Stop" : "Reduce"; }));
            else button2.Text = button2.Text != "Stop" ? "Stop" : "Reduce";
        }


        /**
         * toggle input UI elements between enabled and disabled
         */
        private void disableEnableAllUIInputElements(bool enable)
        {
            if (groupBox1.InvokeRequired) groupBox1.Invoke(new Action(() => { groupBox1.Enabled = enable; }));
            else groupBox1.Enabled = enable;
            if (groupBox2.InvokeRequired) groupBox2.Invoke(new Action(() => { groupBox2.Enabled = enable; }));
            else groupBox2.Enabled = enable;
        }


        /**
         * save all Values from the UI Elements to the user default values
         */
        private void saveUserDefaultValues()
        {
            Properties.Settings.Default.textBox1 = textBox1.Text;
            Properties.Settings.Default.checkBox1 = checkBox1.Checked;
            Properties.Settings.Default.checkBox2 = checkBox2.Checked;
            Properties.Settings.Default.checkBox3 = checkBox3.Checked;
            Properties.Settings.Default.checkBox4 = checkBox4.Checked;
            Properties.Settings.Default.checkBox5 = checkBox5.Checked;
            Properties.Settings.Default.Save();
        }


        /**
         * converts byte unit into a something more human friendly and adds a little funny note to it
         */
        private string convertByteToOtherUnit(double fileSize)
        {
            string[] units = { "Byte", "KB (Kilobyte)", "MB (Megabyte)", "GB (Gigabyte)\n\nWOW! Impressive :O", "TB (Terrabyte)\n\nHOLY COW :O Congratulations!", "PB (Petabyte)\n\nOkay, if you see this: you have way to much CC", "EB (Exabyte)\n\nIf you see this, it's probably an error or you're really into horting CC :D" };
            short f = 0;
            for (; fileSize > 1000; f++) fileSize /= 1000;
            return $"{string.Format("{0:0.0}", fileSize)} {units[f]}";
        }


        /**
         * run Sims 3 Recompressor Executeable
         */
        private void S3RC(string inputPathFile, string exePath)
        {
            if (!File.Exists(exePath))
            {
                throw new Exception("Crucial Files of this Software are missing. Did you delete something? This can't be fixed manually. Please reinstall this program.");
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
                Arguments = $"\"{inputPathFile}\"",
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
            }
        }


        /**
         * show message, to inform user, that compressing takes way longer
         */
        private bool showMessageCompress()
        {
            return MessageBox.Show(
                $"Compressing your Package-Files may take some time to process. But it will be worthwhile.\n\n" +
                $"Is that okay?",
                "Compressing takes longer ...",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            ) == DialogResult.OK;
        }


        /**
         * show message, to warn user, that reducing the Package-Files is permanent
         */
        private bool showMessageBackupWarning()
        {
            if (!checkBox5.Checked) return true;
            return MessageBox.Show(
                $"Reducing your Package-Files is a one way process.\n\n" +
                $"Do You have a backup, just in case?",
                "Please read this <3",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            ) == DialogResult.Yes;
        }


        /**
         * on click checkbox 
         */
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.compressWarningShown)
            {
                Properties.Settings.Default.compressWarningShown = showMessageCompress();
                Properties.Settings.Default.Save();
            }
        }


        /**
         * extract the Sims 3 Recompressor Tool (s3rc.exe) into the temp folder from the packaged exe file of this tool
         */
        private string ExtractS3RC()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"s3rc_{Guid.NewGuid()}.exe");

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "S3PR_GUI.s3rc.exe";

            using (Stream resource = assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null) throw new Exception("Embedded tool not found.");

                using (FileStream file = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }

            return tempPath;
        }
    }
}
