namespace GTNH_Manager
{
    partial class GTNHManager
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
            versionListBox = new ListBox();
            chooseVersionLabel = new Label();
            downloadButton = new Button();
            multiMCPathTextBox = new TextBox();
            multiMCPathLabel = new Label();
            progressBar = new ProgressBar();
            progressLabel = new Label();
            statusLabel = new Label();
            optifineCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // versionListBox
            // 
            versionListBox.FormattingEnabled = true;
            versionListBox.ItemHeight = 15;
            versionListBox.Location = new Point(12, 186);
            versionListBox.Name = "versionListBox";
            versionListBox.Size = new Size(120, 94);
            versionListBox.TabIndex = 0;
            // 
            // chooseVersionLabel
            // 
            chooseVersionLabel.AutoSize = true;
            chooseVersionLabel.Location = new Point(12, 168);
            chooseVersionLabel.Name = "chooseVersionLabel";
            chooseVersionLabel.Size = new Size(97, 15);
            chooseVersionLabel.TabIndex = 1;
            chooseVersionLabel.Text = "Choose a version";
            // 
            // downloadButton
            // 
            downloadButton.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            downloadButton.Location = new Point(12, 27);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(201, 94);
            downloadButton.TabIndex = 2;
            downloadButton.Text = "Install";
            downloadButton.UseVisualStyleBackColor = true;
            downloadButton.Click += downloadButton_Click;
            // 
            // multiMCPathTextBox
            // 
            multiMCPathTextBox.Location = new Point(12, 142);
            multiMCPathTextBox.Name = "multiMCPathTextBox";
            multiMCPathTextBox.Size = new Size(207, 23);
            multiMCPathTextBox.TabIndex = 3;
            // 
            // multiMCPathLabel
            // 
            multiMCPathLabel.AutoSize = true;
            multiMCPathLabel.Location = new Point(12, 124);
            multiMCPathLabel.Name = "multiMCPathLabel";
            multiMCPathLabel.Size = new Size(81, 15);
            multiMCPathLabel.TabIndex = 4;
            multiMCPathLabel.Text = "MultiMC path";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(219, 27);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(569, 23);
            progressBar.TabIndex = 5;
            // 
            // progressLabel
            // 
            progressLabel.AutoSize = true;
            progressLabel.Location = new Point(219, 53);
            progressLabel.Name = "downloadProgressLabel";
            progressLabel.Size = new Size(0, 15);
            progressLabel.TabIndex = 6;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(219, 68);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(26, 15);
            statusLabel.TabIndex = 7;
            statusLabel.Text = "Idle";
            // 
            // optifineCheckBox
            // 
            optifineCheckBox.AutoSize = true;
            optifineCheckBox.Location = new Point(142, 186);
            optifineCheckBox.Name = "optifineCheckBox";
            optifineCheckBox.Size = new Size(103, 19);
            optifineCheckBox.TabIndex = 8;
            optifineCheckBox.Text = "Install Optifine";
            optifineCheckBox.UseVisualStyleBackColor = true;
            // 
            // GTNHManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(optifineCheckBox);
            Controls.Add(statusLabel);
            Controls.Add(progressLabel);
            Controls.Add(progressBar);
            Controls.Add(multiMCPathLabel);
            Controls.Add(multiMCPathTextBox);
            Controls.Add(downloadButton);
            Controls.Add(chooseVersionLabel);
            Controls.Add(versionListBox);
            Name = "GTNHManager";
            Text = "GTNH Manager " + version;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox versionListBox;
        private Label chooseVersionLabel;
        private Button downloadButton;
        private TextBox multiMCPathTextBox;
        private Label multiMCPathLabel;
        private ProgressBar progressBar;
        private Label progressLabel;
        private Label statusLabel;
        private CheckBox optifineCheckBox;
    }
}