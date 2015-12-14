namespace GUI
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateNew = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFileAs = new System.Windows.Forms.ToolStripMenuItem();
            this.GrammarToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.RunItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WorkingArea = new System.Windows.Forms.TextBox();
            this.ResultView = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.startApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.GrammarToolStripComboBox,
            this.RunItem,
            this.startApplicationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(985, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateNew,
            this.OpenFile,
            this.SaveFile,
            this.SaveFileAs});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(37, 24);
            this.FileMenu.Text = "File";
            // 
            // CreateNew
            // 
            this.CreateNew.Name = "CreateNew";
            this.CreateNew.Size = new System.Drawing.Size(152, 22);
            this.CreateNew.Text = "Create new";
            this.CreateNew.Click += new System.EventHandler(this.CreateNewFile_Click);
            // 
            // OpenFile
            // 
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(152, 22);
            this.OpenFile.Text = "Open";
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(152, 22);
            this.SaveFile.Text = "Save";
            this.SaveFile.Click += new System.EventHandler(this.Save_Click);
            // 
            // SaveFileAs
            // 
            this.SaveFileAs.Name = "SaveFileAs";
            this.SaveFileAs.Size = new System.Drawing.Size(152, 22);
            this.SaveFileAs.Text = "Save as";
            this.SaveFileAs.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // GrammarToolStripComboBox
            // 
            this.GrammarToolStripComboBox.AutoCompleteCustomSource.AddRange(new string[] {
            "С/С++",
            "Pascal"});
            this.GrammarToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GrammarToolStripComboBox.Items.AddRange(new object[] {
            "С/С++",
            "Pascal",
            "PascalABC.NET"});
            this.GrammarToolStripComboBox.Name = "GrammarToolStripComboBox";
            this.GrammarToolStripComboBox.Size = new System.Drawing.Size(92, 24);
            this.GrammarToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.GrammarToolStripComboBox_SelectedIndexChanged);
            this.GrammarToolStripComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // RunItem
            // 
            this.RunItem.Image = global::GUI.Properties.Resources.run;
            this.RunItem.Name = "RunItem";
            this.RunItem.Size = new System.Drawing.Size(130, 24);
            this.RunItem.Text = "Start compilation";
            this.RunItem.Click += new System.EventHandler(this.RunParser_Click);
            // 
            // WorkingArea
            // 
            this.WorkingArea.AcceptsTab = true;
            this.WorkingArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WorkingArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WorkingArea.Location = new System.Drawing.Point(0, 26);
            this.WorkingArea.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.WorkingArea.MaxLength = 0;
            this.WorkingArea.Multiline = true;
            this.WorkingArea.Name = "WorkingArea";
            this.WorkingArea.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.WorkingArea.Size = new System.Drawing.Size(553, 561);
            this.WorkingArea.TabIndex = 1;
            this.WorkingArea.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.WorkingArea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // ResultView
            // 
            this.ResultView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ResultView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultView.Location = new System.Drawing.Point(553, 26);
            this.ResultView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ResultView.Multiline = true;
            this.ResultView.Name = "ResultView";
            this.ResultView.ReadOnly = true;
            this.ResultView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultView.Size = new System.Drawing.Size(432, 560);
            this.ResultView.TabIndex = 2;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "cn files (*.cn)|*.cn|pasn files (*.pasn)|*.pasn|PABC.NET files(*.pas)|*.pas";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "cn files (*.cn)|*.cn|pasn files (*.pasn)|*.pasn|PABC.NET files(*.pas)|*.pas";
            // 
            // startApplicationToolStripMenuItem
            // 
            this.startApplicationToolStripMenuItem.Name = "startApplicationToolStripMenuItem";
            this.startApplicationToolStripMenuItem.Size = new System.Drawing.Size(105, 24);
            this.startApplicationToolStripMenuItem.Text = "Start application";
            this.startApplicationToolStripMenuItem.Click += new System.EventHandler(this.startApplicationToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 586);
            this.Controls.Add(this.ResultView);
            this.Controls.Add(this.WorkingArea);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem OpenFile;
        private System.Windows.Forms.TextBox WorkingArea;
        private System.Windows.Forms.TextBox ResultView;
        private System.Windows.Forms.ToolStripMenuItem SaveFile;
        private System.Windows.Forms.ToolStripMenuItem SaveFileAs;
        private System.Windows.Forms.ToolStripMenuItem RunItem;
        private System.Windows.Forms.ToolStripMenuItem CreateNew;
        private System.Windows.Forms.ToolStripComboBox GrammarToolStripComboBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem startApplicationToolStripMenuItem;
    }
}

