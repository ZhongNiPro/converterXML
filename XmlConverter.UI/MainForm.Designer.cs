namespace XmlConverter.UI
{
    partial class MainForm
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
            ButtonStart = new Button();
            ButtonCancel = new Button();
            myProgressBar = new ProgressBar();
            GridCadastralObjects = new DataGridView();
            TextSearchCadastralNumber = new TextBox();
            ButtonSearchObjects = new Button();
            TabCadastralObjectDetails = new TabControl();
            pageMainProperties = new TabPage();
            TextMainProperties = new RichTextBox();
            pagePropertyRights = new TabPage();
            TextPropertyRights = new RichTextBox();
            ButtonSelectFiles = new Button();
            LabelFilesCount = new Label();
            ButtonOpenSettings = new Button();
            ((System.ComponentModel.ISupportInitialize)GridCadastralObjects).BeginInit();
            TabCadastralObjectDetails.SuspendLayout();
            pageMainProperties.SuspendLayout();
            pagePropertyRights.SuspendLayout();
            SuspendLayout();
            // 
            // ButtonStart
            // 
            ButtonStart.Location = new Point(1053, 680);
            ButtonStart.Name = "ButtonStart";
            ButtonStart.Size = new Size(211, 34);
            ButtonStart.TabIndex = 0;
            ButtonStart.Text = "Загрузить в базу";
            ButtonStart.UseVisualStyleBackColor = true;
            ButtonStart.Click += ButtonStart_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(1289, 680);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(211, 34);
            ButtonCancel.TabIndex = 1;
            ButtonCancel.Text = "Отмена";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // myProgressBar
            // 
            myProgressBar.Location = new Point(44, 745);
            myProgressBar.Name = "myProgressBar";
            myProgressBar.Size = new Size(1456, 34);
            myProgressBar.TabIndex = 2;
            // 
            // GridCadastralObjects
            // 
            GridCadastralObjects.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridCadastralObjects.Location = new Point(44, 22);
            GridCadastralObjects.Name = "GridCadastralObjects";
            GridCadastralObjects.RowHeadersWidth = 62;
            GridCadastralObjects.Size = new Size(680, 602);
            GridCadastralObjects.TabIndex = 3;
            // 
            // TextSearchCadastralNumber
            // 
            TextSearchCadastralNumber.Location = new Point(763, 26);
            TextSearchCadastralNumber.Name = "TextSearchCadastralNumber";
            TextSearchCadastralNumber.Size = new Size(501, 31);
            TextSearchCadastralNumber.TabIndex = 4;
            // 
            // ButtonSearchObjects
            // 
            ButtonSearchObjects.Location = new Point(1289, 26);
            ButtonSearchObjects.Name = "ButtonSearchObjects";
            ButtonSearchObjects.Size = new Size(211, 34);
            ButtonSearchObjects.TabIndex = 5;
            ButtonSearchObjects.Text = "Поиск";
            ButtonSearchObjects.UseVisualStyleBackColor = true;
            ButtonSearchObjects.Click += ButtonSearchObjects_Click;
            // 
            // TabCadastralObjectDetails
            // 
            TabCadastralObjectDetails.Controls.Add(pageMainProperties);
            TabCadastralObjectDetails.Controls.Add(pagePropertyRights);
            TabCadastralObjectDetails.Location = new Point(763, 87);
            TabCadastralObjectDetails.Name = "TabCadastralObjectDetails";
            TabCadastralObjectDetails.SelectedIndex = 0;
            TabCadastralObjectDetails.Size = new Size(737, 537);
            TabCadastralObjectDetails.TabIndex = 6;
            // 
            // pageMainProperties
            // 
            pageMainProperties.Controls.Add(TextMainProperties);
            pageMainProperties.Location = new Point(4, 34);
            pageMainProperties.Name = "pageMainProperties";
            pageMainProperties.Padding = new Padding(3);
            pageMainProperties.Size = new Size(729, 499);
            pageMainProperties.TabIndex = 0;
            pageMainProperties.Text = "Основные характеристики";
            pageMainProperties.UseVisualStyleBackColor = true;
            // 
            // TextMainProperties
            // 
            TextMainProperties.Location = new Point(0, 0);
            TextMainProperties.Name = "TextMainProperties";
            TextMainProperties.Size = new Size(733, 503);
            TextMainProperties.TabIndex = 0;
            TextMainProperties.Text = "";
            // 
            // pagePropertyRights
            // 
            pagePropertyRights.Controls.Add(TextPropertyRights);
            pagePropertyRights.Location = new Point(4, 34);
            pagePropertyRights.Name = "pagePropertyRights";
            pagePropertyRights.Padding = new Padding(3);
            pagePropertyRights.Size = new Size(729, 499);
            pagePropertyRights.TabIndex = 1;
            pagePropertyRights.Text = "Сведения о правах";
            pagePropertyRights.UseVisualStyleBackColor = true;
            // 
            // TextPropertyRights
            // 
            TextPropertyRights.Location = new Point(3, 3);
            TextPropertyRights.Name = "TextPropertyRights";
            TextPropertyRights.Size = new Size(730, 500);
            TextPropertyRights.TabIndex = 0;
            TextPropertyRights.Text = "";
            // 
            // ButtonSelectFiles
            // 
            ButtonSelectFiles.Location = new Point(1289, 640);
            ButtonSelectFiles.Name = "ButtonSelectFiles";
            ButtonSelectFiles.Size = new Size(211, 34);
            ButtonSelectFiles.TabIndex = 7;
            ButtonSelectFiles.Text = "Выбрать файлы";
            ButtonSelectFiles.UseVisualStyleBackColor = true;
            ButtonSelectFiles.Click += ButtonSelectFiles_Click;
            // 
            // LabelFilesCount
            // 
            LabelFilesCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LabelFilesCount.AutoSize = true;
            LabelFilesCount.Location = new Point(1264, 645);
            LabelFilesCount.Name = "LabelFilesCount";
            LabelFilesCount.Size = new Size(0, 25);
            LabelFilesCount.TabIndex = 8;
            LabelFilesCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ButtonOpenSettings
            // 
            ButtonOpenSettings.AutoSize = true;
            ButtonOpenSettings.Image = Properties.Resources.set_but_img;
            ButtonOpenSettings.Location = new Point(1516, 745);
            ButtonOpenSettings.Name = "ButtonOpenSettings";
            ButtonOpenSettings.Size = new Size(40, 40);
            ButtonOpenSettings.TabIndex = 9;
            ButtonOpenSettings.UseVisualStyleBackColor = true;
            ButtonOpenSettings.Click += ButtonOpenSettings_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1577, 813);
            Controls.Add(ButtonOpenSettings);
            Controls.Add(LabelFilesCount);
            Controls.Add(ButtonSelectFiles);
            Controls.Add(TabCadastralObjectDetails);
            Controls.Add(ButtonSearchObjects);
            Controls.Add(TextSearchCadastralNumber);
            Controls.Add(GridCadastralObjects);
            Controls.Add(myProgressBar);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonStart);
            Name = "MainForm";
            Text = "MainForm ";
            ((System.ComponentModel.ISupportInitialize)GridCadastralObjects).EndInit();
            TabCadastralObjectDetails.ResumeLayout(false);
            pageMainProperties.ResumeLayout(false);
            pagePropertyRights.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ButtonStart;
        private Button ButtonCancel;
        private ProgressBar myProgressBar;
        private DataGridView GridCadastralObjects;
        private TextBox TextSearchCadastralNumber;
        private Button ButtonSearchObjects;
        private TabControl TabCadastralObjectDetails;
        private TabPage pageMainProperties;
        private TabPage pagePropertyRights;
        private RichTextBox TextMainProperties;
        private RichTextBox TextPropertyRights;
        private Button ButtonSelectFiles;
        private Label LabelFilesCount;
        private Button ButtonOpenSettings;
    }
}
