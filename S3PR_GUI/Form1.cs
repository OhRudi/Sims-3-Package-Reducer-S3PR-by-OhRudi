using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OhRudi
{
    public partial class Form1 : Form
    {
        public string a = "";
        private bool stopExecution = false;
        private string lastPath = "";
        private bool isDone = false;
        private string defaultWindowTitle = "Sims 3 Package Reducer (S3PR) by OhRudi";
        private S3RC S3RC = S3RC.GetInstance;
        private S3PR S3PR = S3PR.GetInstance;
        private string pathDefault
        {
            get
            {
                string pathInternal, parentDir;
                if (Directory.Exists(pathInternal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Electronic Arts", "The Sims 3", "Mods", "Cache")))
                    return pathInternal;
                else if (Directory.Exists(parentDir = Path.GetDirectoryName(pathInternal)!))
                    return parentDir;
                return "";
            }
        }

        public Form1()
        {
            InitializeComponent();
        }


        /**
         * while loading form
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            // set window title by assembly title and assembly version
            Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? defaultWindowTitle;
            Text = $"{Text} Version {Assembly.GetEntryAssembly()?.GetName().Version?.ToString()}";

            // set icon (cause any other did not work)
            this.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

            // set last values, that user used last time
            checkBox1.Checked = Properties.Settings.Default.checkBox1;
            checkBox2.Checked = Properties.Settings.Default.checkBox2;
            checkBox3.Checked = Properties.Settings.Default.checkBox3;
            checkBox4.Checked = Properties.Settings.Default.checkBox4;
            checkBox5.Checked = Properties.Settings.Default.checkBox5;
            checkBox6.Checked = Properties.Settings.Default.checkBox6;
            textBox1.Text = Properties.Settings.Default.textBox1;

            // set default path to mods folder
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = pathDefault;
            }
        }


        /**
         * on browse button click
         */
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string initialPath;
                using FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Multiselect = true;
                dialog.SelectedPath = Directory.Exists(initialPath = textBox1.Text.Split(", ")[0]) ? initialPath : pathDefault;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    textBox1.Text = dialog.SelectedPath;
            }
            catch (Exception exception)
            {
                ShowErrorMessageBox(exception);
            }
        }


        /**
         * on edit button click
         */
        private async void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text != "Stop" && !stopExecution)
            {
                // if both compress and decompress are checked at the same time, show warning message and abort
                if (checkBox4.Checked && checkBox6.Checked)
                {
                    ShowMessageCompressAndDecompressCantBeActiveAtTheSameTime();
                    return;
                }

                // reset loading label beneath the loading bar
                UpdateLoadingLabel("");

                // save all Values from the UI Elements to the user default values
                SaveUserDefaultValues();

                // show message to warn user to have a backup, just in case
                // abort if the user dialog was negative
                if (!ShowMessageBackupWarning()) return;

                // set cursor to waiting 
                SwitchBetweenNormalAndWaitingCursor();

                // set progress bar to 2% at start, to signalise it started
                await Task.Run(() =>
                {
                    UpdateProgressBar(2);
                });


                // disable all UI input elements, to signalize it's not available
                DisableEnableAllUIInputElements(false);

                // switch edit button to stop button
                SwitchBetweenEditAndStopButton();

                // execute edit async
                await Task.Run(() =>
                {
                    ExecuteFileEdit();
                });

                // reset progress bar
                UpdateProgressBar(0);

                // reset cursor
                SwitchBetweenNormalAndWaitingCursor();

                // enable all UI input elements, to signalize it's available
                DisableEnableAllUIInputElements(true);

                // reset the stop/edit button
                SwitchBetweenEditAndStopButton();

                // reset stop execution flag
                stopExecution = false;

                // reset is done flag
                isDone = false;

                // reset skipped files
                S3PR.ResetSkippedFiles();

                // reset skipped folders
                S3PR.ResetSkippedFolders();
            }
            else
            {
                // set flag to skip all further processing
                stopExecution = true;

                // update loading label
                if (!isDone)
                {
                    UpdateLoadingLabel($"Stops after finishing: {Path.GetFileName(lastPath)} ...");
                }
            }
        }


        /**
         * the main method, that iterates through all files and edits them
         */
        private void ExecuteFileEdit()
        {
            bool removeThumbnails = checkBox1.Checked;
            bool removeIcons = checkBox2.Checked;
            bool searchRecursive = checkBox3.Checked;
            bool compressFile = checkBox4.Checked;
            bool decompressFile = checkBox6.Checked;
            string[] pathFolderList = textBox1.Text.Split(", ");
            IEnumerable<string> pathEnumerable;
            int progress = 0;
            int progressTillStopped = 0;
            double fileSizeBeforeInByte = 0;
            double fileSizeAfterInByte = 0;

            if (pathFolderList.Length <= 0)
            {
                MessageBox.Show("Please select folders with Package-Files first.", "You missed something ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!removeIcons && !removeThumbnails && !compressFile && !decompressFile)
            {
                MessageBox.Show($"Please click at least one of the options (like \"{checkBox1.Text}\", \"{checkBox2.Text}\", etc.) to edit the Package-Files.", "What to do? You missed something ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // find all package files in all folders
                pathEnumerable = S3PR.FindPackageFiles(pathFolderList, searchRecursive);
                int count = pathEnumerable.Count();

                // edit each file seperately
                foreach (string pathFile in pathEnumerable)
                {
                    UpdateLoadingLabel($"Editing {Path.GetFileName(pathFile)} ...");
                    if (!stopExecution)
                    {
                        progressTillStopped++;
                        lastPath = pathFile;
                        fileSizeBeforeInByte += (double)(new FileInfo(pathFile)).Length;
                        S3PR.EditPackage(pathFile, removeThumbnails, removeIcons);
                        if (compressFile) S3RC.Compress(pathFile);
                        if (decompressFile) S3RC.Decompress(pathFile);
                        fileSizeAfterInByte += (double)(new FileInfo(pathFile)).Length;
                    }
                    int progressBarPercentValue = (++progress * 100) / count;
                    UpdateProgressBar(progressBarPercentValue < 2 ? 2 : progressBarPercentValue);
                }

                if(progressTillStopped <= 0)
                {
                    ShowWarningNoFilesFoundMessageBox();
                    return;
                }
            }
            catch (Exception exception)
            {
                ShowErrorMessageBox(exception);
                return;
            }
            finally
            {
                isDone = true;
                S3RC.DeleteTool();
                UpdateLoadingLabel("Done.");
            }


            // if process got stopped via UI-Button
            if (stopExecution)
            {
                ShowStopMessageBox(progressTillStopped, fileSizeBeforeInByte, fileSizeAfterInByte, decompressFile);
            }

            // if process ran sucessfully
            else
            {
                ShowSuccessMessageBox(progressTillStopped, fileSizeBeforeInByte, fileSizeAfterInByte, decompressFile);
            }
        }


        /**
         * show error message box
         */
        private void ShowErrorMessageBox(Exception exception)
        {
            MessageBox.Show(
                    $"Something went wrong!\n\n" +
                    $"Error Message: {exception.Message}\n\n" +
                    $"If this happens repeatedly, please tell OhRudi (the Creator) by commenting on the download page." +
                    (
                        lastPath != "" || (exception.Source is not null && exception.Source != "")
                        ? $"\n\nPath : {(exception.Source ?? lastPath)}"
                        : ""
                    ), "Uuupsi ...", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
        }


        private void ShowWarningNoFilesFoundMessageBox()
        {
            MessageBox.Show($"The selected Folders did not contain any Package-Files or the program does not have access.\nPlease select another one.", "Nothing here ...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        /**
         * show stop message box
         */
        private void ShowStopMessageBox(int progressTillStopped, double fileSizeBeforeInByte, double fileSizeAfterInByte, bool decompressionEnabled = false)
        {
            string message = $"Process Stopped.";
            int progressMinusSkipped = progressTillStopped - S3PR.SkippedFiles.Count;
            if (progressMinusSkipped < 0) progressMinusSkipped = 0;
            if (progressMinusSkipped > 0)
            {
                if (fileSizeBeforeInByte - fileSizeAfterInByte > 0)
                {
                    message += $"\n\nBefore it stopped, it reduced {progressMinusSkipped} Package-File{(progressMinusSkipped > 1 ? "s" : "")} in total by {ConvertByteToOtherUnit(fileSizeBeforeInByte - fileSizeAfterInByte)}";
                }
                else if (decompressionEnabled && (fileSizeBeforeInByte - fileSizeAfterInByte) != 0)
                {
                    message += $"\n\nBefore it stopped, it decompressed {progressMinusSkipped} Package-File{(progressMinusSkipped > 1 ? "s" : "")} in total by {ConvertByteToOtherUnit(fileSizeAfterInByte - fileSizeBeforeInByte)}";
                }
                else
                {
                    message += $"\n\nBefore it stopped, it checked {progressMinusSkipped} Package-File{(progressMinusSkipped > 1 ? "s" : "")} but, {(progressMinusSkipped > 1 ? "they were" : "it was")} already edited{(decompressionEnabled ? " and decompressed" : "")}, so nothing changed.";
                }
            }
            message += $"\n\nLast processed File: {lastPath}";
            message += S3PR.GetSkippedFilesAndFoldersMessage();

            MessageBox.Show(
                message,
                "Oh, you stopped?",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }


        /**
         * show success message box
         */
        private void ShowSuccessMessageBox(int progressTillStopped, double fileSizeBeforeInByte, double fileSizeAfterInByte, bool decompressionEnabled = false)
        {
            string message = $"Done.";
            int progressMinusSkipped = progressTillStopped - S3PR.SkippedFiles.Count;

            if (progressMinusSkipped > 0)
            {
                if ((fileSizeBeforeInByte - fileSizeAfterInByte) > 0)
                {
                    message += $"\n\nReduced {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} in total by {ConvertByteToOtherUnit(fileSizeBeforeInByte - fileSizeAfterInByte)}";
                }
                else if (decompressionEnabled && (fileSizeBeforeInByte - fileSizeAfterInByte) != 0)
                {
                    message += $"\n\nDecompressed {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} in total by {ConvertByteToOtherUnit(fileSizeAfterInByte - fileSizeBeforeInByte)}";
                }
                else
                {
                    message += $"\n\nIt checked {progressMinusSkipped} Package-File{(progressMinusSkipped != 1 ? "s" : "")} but, {(progressMinusSkipped != 1 ? "they were" : "it was")} already edited{(decompressionEnabled ? " and decompressed" : "")}, so nothing changed.";
                }
            }
            message += S3PR.GetSkippedFilesAndFoldersMessage();

            MessageBox.Show(message,
                "Wuhuuu, it's done!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }


        /**
         * update progress bar thread safe
         */
        private void UpdateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired) progressBar1.Invoke(new Action(() => { progressBar1.Value = value; }));
            else progressBar1.Value = value;
        }


        /**
         * update loading label
         */
        private void UpdateLoadingLabel(string text)
        {
            if (label1.InvokeRequired) label1.Invoke(new Action(() => { label1.Text = text; }));
            else label1.Text = text;
        }


        /**
         * switch between normal and waiting cursor
         */
        private void SwitchBetweenNormalAndWaitingCursor()
        {
            this.Cursor = this.Cursor != Cursors.WaitCursor ? Cursors.WaitCursor : Cursors.Default;
        }


        /**
         * switch between "Edit" and "Stop" text on the execution button
         */
        private void SwitchBetweenEditAndStopButton()
        {
            if (button2.InvokeRequired) button2.Invoke(new Action(() => { button2.Text = button2.Text != "Stop" ? "Stop" : "Edit"; }));
            else button2.Text = button2.Text != "Stop" ? "Stop" : "Edit";
        }


        /**
         * toggle input UI elements between enabled and disabled
         */
        private void DisableEnableAllUIInputElements(bool enable)
        {
            if (groupBox1.InvokeRequired) groupBox1.Invoke(new Action(() => { groupBox1.Enabled = enable; }));
            else groupBox1.Enabled = enable;
            if (groupBox2.InvokeRequired) groupBox2.Invoke(new Action(() => { groupBox2.Enabled = enable; }));
            else groupBox2.Enabled = enable;
        }


        /**
         * save all Values from the UI Elements to the user default values
         */
        private void SaveUserDefaultValues()
        {
            Properties.Settings.Default.textBox1 = textBox1.Text;
            Properties.Settings.Default.checkBox1 = checkBox1.Checked;
            Properties.Settings.Default.checkBox2 = checkBox2.Checked;
            Properties.Settings.Default.checkBox3 = checkBox3.Checked;
            Properties.Settings.Default.checkBox4 = checkBox4.Checked;
            Properties.Settings.Default.checkBox5 = checkBox5.Checked;
            Properties.Settings.Default.checkBox6 = checkBox6.Checked;
            Properties.Settings.Default.Save();
        }


        /**
         * converts byte unit into a something more human friendly and adds a little funny note to it
         */
        private string ConvertByteToOtherUnit(double fileSize)
        {
            string[] units = { "Byte", "KB (Kilobyte)", "MB (Megabyte)", "GB (Gigabyte)\n\nWOW! Impressive :O", "TB (Terrabyte)\n\nHOLY COW :O Congratulations!", "PB (Petabyte)\n\nOkay, if you see this: you have way to much CC", "EB (Exabyte)\n\nIf you see this, it's probably an error or you're really into horting CC :D" };
            short f = 0;
            for (; fileSize > 1000; f++) fileSize /= 1000;
            return $"{string.Format("{0:0.0}", fileSize)} {units[f]}";
        }


        /**
         * show message, to inform user, that compressing takes way longer
         */
        private bool ShowMessageCompress()
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
         * show message, to inform user, that decompressing takes way more space
         */
        private bool ShowMessageDecompress()
        {
            return MessageBox.Show(
                $"Decompressing your Package-Files means they take up 3-times the space.\nBut it will be worthwhile.\nRead the notes on the download page if you need more information on that\n\n" +
                $"Is that okay?",
                "Decompressing takes up more space ...",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            ) == DialogResult.OK;
        }


        /**
         * show message, to inform user, that the options compress and decompress can't be active at the same time
         */
        private bool ShowMessageCompressAndDecompressCantBeActiveAtTheSameTime()
        {
            return MessageBox.Show(
                $"Sorry to interrupt: You can't have the compress and decompress option at the same time.\n\n",
                "No no, not possible, sorry.",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            ) == DialogResult.OK;
        }


        /**
         * show message, to warn user, that editing the Package-Files is permanent
         */
        private bool ShowMessageBackupWarning()
        {
            if (!checkBox5.Checked) return true;
            return MessageBox.Show(
                $"Editing your Package-Files is a one way process.\n\n" +
                $"Do You have a backup, just in case?",
                "Please read this <3",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            ) == DialogResult.Yes;
        }


        /**
         * on click compression checkbox 
         */
        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.compressWarningShown)
            {
                Properties.Settings.Default.compressWarningShown = ShowMessageCompress();
                Properties.Settings.Default.Save();
            }
            if (checkBox6.Checked && checkBox4.Checked)
            {
                checkBox6.Checked = false;
            }
        }


        /**
         * on click decompression checkbox 
         */
        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.decompressWarningShown)
            {
                Properties.Settings.Default.decompressWarningShown = ShowMessageDecompress();
                Properties.Settings.Default.Save();
            }
            if (checkBox6.Checked && checkBox4.Checked)
            {
                checkBox4.Checked = false;
            }
        }
    }
}
