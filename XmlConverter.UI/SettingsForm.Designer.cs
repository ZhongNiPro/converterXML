namespace XmlConverter.UI
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TextHost = new TextBox();
            TextPort = new TextBox();
            TextUser = new TextBox();
            TextPassword = new TextBox();
            TextDatabase = new TextBox();
            ButtonTestAndSave = new Button();
            ButtonCreateBackup = new Button();
            ButtonRestoreBackup = new Button();
            labelHost = new Label();
            labelPassword = new Label();
            labelUser = new Label();
            labelPort = new Label();
            labelName = new Label();
            SuspendLayout();
            // 
            // TextHost
            // 
            TextHost.Location = new Point(71, 56);
            TextHost.Name = "TextHost";
            TextHost.Size = new Size(260, 31);
            TextHost.TabIndex = 0;
            // 
            // TextPort
            // 
            TextPort.Location = new Point(71, 118);
            TextPort.Name = "TextPort";
            TextPort.Size = new Size(260, 31);
            TextPort.TabIndex = 1;
            // 
            // TextUser
            // 
            TextUser.Location = new Point(71, 180);
            TextUser.Name = "TextUser";
            TextUser.Size = new Size(260, 31);
            TextUser.TabIndex = 2;
            // 
            // TextPassword
            // 
            TextPassword.Location = new Point(71, 242);
            TextPassword.Name = "TextPassword";
            TextPassword.Size = new Size(260, 31);
            TextPassword.TabIndex = 3;
            // 
            // TextDatabase
            // 
            TextDatabase.Location = new Point(71, 304);
            TextDatabase.Name = "TextDatabase";
            TextDatabase.Size = new Size(260, 31);
            TextDatabase.TabIndex = 4;
            // 
            // ButtonTestAndSave
            // 
            ButtonTestAndSave.Location = new Point(141, 362);
            ButtonTestAndSave.Name = "ButtonTestAndSave";
            ButtonTestAndSave.Size = new Size(190, 34);
            ButtonTestAndSave.TabIndex = 5;
            ButtonTestAndSave.Text = "Test and Save ";
            ButtonTestAndSave.UseVisualStyleBackColor = true;
            ButtonTestAndSave.Click += ButtonTestAndSave_Click;
            // 
            // ButtonCreateBackup
            // 
            ButtonCreateBackup.Location = new Point(744, 56);
            ButtonCreateBackup.Name = "ButtonCreateBackup";
            ButtonCreateBackup.Size = new Size(190, 34);
            ButtonCreateBackup.TabIndex = 6;
            ButtonCreateBackup.Text = "Create Backup";
            ButtonCreateBackup.UseVisualStyleBackColor = true;
            ButtonCreateBackup.Click += ButtonCreateBackup_Click;
            // 
            // ButtonRestoreBackup
            // 
            ButtonRestoreBackup.Location = new Point(744, 111);
            ButtonRestoreBackup.Name = "ButtonRestoreBackup";
            ButtonRestoreBackup.Size = new Size(190, 34);
            ButtonRestoreBackup.TabIndex = 7;
            ButtonRestoreBackup.Text = "Restore Backup";
            ButtonRestoreBackup.UseVisualStyleBackColor = true;
            ButtonRestoreBackup.Click += ButtonRestoreBackup_Click;
            // 
            // labelHost
            // 
            labelHost.AutoSize = true;
            labelHost.Location = new Point(71, 28);
            labelHost.Name = "labelHost";
            labelHost.Size = new Size(50, 25);
            labelHost.TabIndex = 8;
            labelHost.Text = "Host";
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(71, 214);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(87, 25);
            labelPassword.TabIndex = 9;
            labelPassword.Text = "Password";
            // 
            // labelUser
            // 
            labelUser.AutoSize = true;
            labelUser.Location = new Point(71, 152);
            labelUser.Name = "labelUser";
            labelUser.Size = new Size(47, 25);
            labelUser.TabIndex = 10;
            labelUser.Text = "User";
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new Point(71, 90);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(44, 25);
            labelPort.TabIndex = 11;
            labelPort.Text = "Port";
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(71, 276);
            labelName.Name = "labelName";
            labelName.Size = new Size(59, 25);
            labelName.TabIndex = 12;
            labelName.Text = "Name";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(996, 611);
            Controls.Add(labelName);
            Controls.Add(labelPort);
            Controls.Add(labelUser);
            Controls.Add(labelPassword);
            Controls.Add(labelHost);
            Controls.Add(ButtonRestoreBackup);
            Controls.Add(ButtonCreateBackup);
            Controls.Add(ButtonTestAndSave);
            Controls.Add(TextDatabase);
            Controls.Add(TextPassword);
            Controls.Add(TextUser);
            Controls.Add(TextPort);
            Controls.Add(TextHost);
            Name = "SettingsForm";
            Text = "Настройки и СУБД";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextHost;
        private TextBox TextPort;
        private TextBox TextUser;
        private TextBox TextPassword;
        private TextBox TextDatabase;
        private Button ButtonTestAndSave;
        private Button ButtonCreateBackup;
        private Button ButtonRestoreBackup;
        private Label labelHost;
        private Label labelPassword;
        private Label labelUser;
        private Label labelPort;
        private Label labelName;
    }
}