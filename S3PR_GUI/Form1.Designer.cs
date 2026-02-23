namespace S3PR_GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            button1 = new Button();
            groupBox1 = new GroupBox();
            checkBox5 = new CheckBox();
            checkBox3 = new CheckBox();
            groupBox2 = new GroupBox();
            checkBox4 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            groupBox3 = new GroupBox();
            label1 = new Label();
            progressBar1 = new ProgressBar();
            button2 = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Enabled = false;
            textBox1.Location = new Point(20, 43);
            textBox1.Margin = new Padding(4, 2, 4, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(951, 39);
            textBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(986, 39);
            button1.Margin = new Padding(4, 2, 4, 2);
            button1.Name = "button1";
            button1.Size = new Size(150, 47);
            button1.TabIndex = 1;
            button1.Text = "Browse";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox5);
            groupBox1.Controls.Add(checkBox3);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Location = new Point(11, 21);
            groupBox1.Margin = new Padding(4, 2, 4, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 2, 4, 2);
            groupBox1.Size = new Size(1136, 132);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Step 1: Select Folders containing Package Files";
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Checked = true;
            checkBox5.CheckState = CheckState.Checked;
            checkBox5.Location = new Point(290, 98);
            checkBox5.Margin = new Padding(4, 2, 4, 2);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(349, 36);
            checkBox5.TabIndex = 3;
            checkBox5.Text = "Remind me to have backups";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Location = new Point(20, 98);
            checkBox3.Margin = new Padding(4, 2, 4, 2);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(256, 36);
            checkBox3.TabIndex = 2;
            checkBox3.Text = "Include Subfolders?";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(checkBox4);
            groupBox2.Controls.Add(checkBox2);
            groupBox2.Controls.Add(checkBox1);
            groupBox2.Location = new Point(11, 164);
            groupBox2.Margin = new Padding(4, 2, 4, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 2, 4, 2);
            groupBox2.Size = new Size(1136, 102);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Step 2: What to do?";
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(492, 47);
            checkBox4.Margin = new Padding(4, 2, 4, 2);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(357, 36);
            checkBox4.TabIndex = 2;
            checkBox4.Text = "Compress Files (takes longer)";
            checkBox4.UseVisualStyleBackColor = true;
            checkBox4.CheckedChanged += checkBox4_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Location = new Point(290, 47);
            checkBox2.Margin = new Padding(4, 2, 4, 2);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(194, 36);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "Remove Icons";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(20, 47);
            checkBox1.Margin = new Padding(4, 2, 4, 2);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(263, 36);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Remove Thumbnails";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(progressBar1);
            groupBox3.Controls.Add(button2);
            groupBox3.Location = new Point(11, 271);
            groupBox3.Margin = new Padding(4, 2, 4, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 2, 4, 2);
            groupBox3.Size = new Size(1136, 128);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Step 3: Click Reduce";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 88);
            label1.Name = "label1";
            label1.Size = new Size(0, 32);
            label1.TabIndex = 3;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(20, 38);
            progressBar1.Margin = new Padding(4, 2, 4, 2);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(951, 47);
            progressBar1.TabIndex = 2;
            // 
            // button2
            // 
            button2.Location = new Point(986, 38);
            button2.Margin = new Padding(4, 2, 4, 2);
            button2.Name = "button2";
            button2.Size = new Size(150, 47);
            button2.TabIndex = 1;
            button2.Text = "Reduce";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(1164, 409);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Margin = new Padding(4, 2, 4, 2);
            MaximumSize = new Size(1190, 480);
            MinimumSize = new Size(1190, 480);
            Name = "Form1";
            Text = "Sims 3 Package Reducer (S3PR) by OhRudi";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private GroupBox groupBox3;
        private Button button2;
        private CheckBox checkBox3;
        private ProgressBar progressBar1;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private Label label1;
    }
}
