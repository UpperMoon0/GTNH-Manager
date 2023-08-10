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
            installButton = new Button();
            multiMCPathTextBox = new TextBox();
            multiMCPathLabel = new Label();
            progressBar = new ProgressBar();
            progressLabel = new Label();
            statusLabel = new Label();
            optifineCheckBox = new CheckBox();
            creditLabel = new Label();
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
            // installButton
            // 
            installButton.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            installButton.Location = new Point(12, 27);
            installButton.Name = "installButton";
            installButton.Size = new Size(201, 94);
            installButton.TabIndex = 2;
            installButton.Text = "Install";
            installButton.UseVisualStyleBackColor = true;
            installButton.Click += installButton_Click;
            // 
            // multiMCPathTextBox
            // 
            multiMCPathTextBox.Location = new Point(12, 142);
            multiMCPathTextBox.Name = "multiMCPathTextBox";
            multiMCPathTextBox.Size = new Size(233, 23);
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
            progressLabel.Name = "progressLabel";
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
            // creditLabel
            // 
            creditLabel.AutoSize = true;
            creditLabel.Location = new Point(680, 426);
            creditLabel.Name = "creditLabel";
            creditLabel.Size = new Size(108, 15);
            creditLabel.TabIndex = 9;
            creditLabel.Text = "Developed by Nhat";
            // 
            // GTNHManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(creditLabel);
            Controls.Add(optifineCheckBox);
            Controls.Add(statusLabel);
            Controls.Add(progressLabel);
            Controls.Add(progressBar);
            Controls.Add(multiMCPathLabel);
            Controls.Add(multiMCPathTextBox);
            Controls.Add(installButton);
            Controls.Add(chooseVersionLabel);
            Controls.Add(versionListBox);
            Name = "GTNHManager";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox versionListBox;
        private Label chooseVersionLabel;
        private Button installButton;
        private TextBox multiMCPathTextBox;
        private Label multiMCPathLabel;
        private ProgressBar progressBar;
        private Label progressLabel;
        private Label statusLabel;
        private CheckBox optifineCheckBox;
        private Label creditLabel;
    }
}