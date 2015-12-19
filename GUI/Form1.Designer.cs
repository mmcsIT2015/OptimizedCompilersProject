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
            this.startApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WorkingArea = new System.Windows.Forms.TextBox();
            this.ResultView = new System.Windows.Forms.TextBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ILResultView = new System.Windows.Forms.TextBox();
            this.taCodeGroupBox = new System.Windows.Forms.GroupBox();
            this.ilGroupBox = new System.Windows.Forms.GroupBox();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.codeSplitContainer = new System.Windows.Forms.SplitContainer();
            this.optimizationTypeToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.optimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.taCodeGroupBox.SuspendLayout();
            this.ilGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.codeSplitContainer)).BeginInit();
            this.codeSplitContainer.Panel1.SuspendLayout();
            this.codeSplitContainer.Panel2.SuspendLayout();
            this.codeSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.GrammarToolStripComboBox,
            this.RunItem,
            this.startApplicationToolStripMenuItem,
            this.optimizationTypeToolStripComboBox,
            this.optimizeToolStripMenuItem});
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
            // startApplicationToolStripMenuItem
            // 
            this.startApplicationToolStripMenuItem.Name = "startApplicationToolStripMenuItem";
            this.startApplicationToolStripMenuItem.Size = new System.Drawing.Size(105, 24);
            this.startApplicationToolStripMenuItem.Text = "Start application";
            this.startApplicationToolStripMenuItem.Click += new System.EventHandler(this.startApplicationToolStripMenuItem_Click);
            // 
            // WorkingArea
            // 
            this.WorkingArea.AcceptsTab = true;
            this.WorkingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorkingArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WorkingArea.Location = new System.Drawing.Point(0, 0);
            this.WorkingArea.Margin = new System.Windows.Forms.Padding(2);
            this.WorkingArea.MaxLength = 0;
            this.WorkingArea.Multiline = true;
            this.WorkingArea.Name = "WorkingArea";
            this.WorkingArea.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.WorkingArea.Size = new System.Drawing.Size(600, 536);
            this.WorkingArea.TabIndex = 1;
            this.WorkingArea.WordWrap = false;
            this.WorkingArea.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.WorkingArea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // ResultView
            // 
            this.ResultView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResultView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultView.Location = new System.Drawing.Point(3, 16);
            this.ResultView.Margin = new System.Windows.Forms.Padding(2);
            this.ResultView.Multiline = true;
            this.ResultView.Name = "ResultView";
            this.ResultView.ReadOnly = true;
            this.ResultView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ResultView.Size = new System.Drawing.Size(375, 254);
            this.ResultView.TabIndex = 2;
            this.ResultView.WordWrap = false;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "cn files (*.cn)|*.cn|pasn files (*.pasn)|*.pasn|PABC.NET files(*.pas)|*.pas";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "cn files (*.cn)|*.cn|pasn files (*.pasn)|*.pasn|PABC.NET files(*.pas)|*.pas";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 564);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(985, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // ILResultView
            // 
            this.ILResultView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ILResultView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ILResultView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ILResultView.Location = new System.Drawing.Point(3, 16);
            this.ILResultView.Margin = new System.Windows.Forms.Padding(2);
            this.ILResultView.Multiline = true;
            this.ILResultView.Name = "ILResultView";
            this.ILResultView.ReadOnly = true;
            this.ILResultView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ILResultView.Size = new System.Drawing.Size(375, 240);
            this.ILResultView.TabIndex = 4;
            this.ILResultView.WordWrap = false;
            // 
            // taCodeGroupBox
            // 
            this.taCodeGroupBox.Controls.Add(this.ResultView);
            this.taCodeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taCodeGroupBox.Location = new System.Drawing.Point(0, 0);
            this.taCodeGroupBox.Name = "taCodeGroupBox";
            this.taCodeGroupBox.Size = new System.Drawing.Size(381, 273);
            this.taCodeGroupBox.TabIndex = 7;
            this.taCodeGroupBox.TabStop = false;
            this.taCodeGroupBox.Text = "TA Code";
            // 
            // ilGroupBox
            // 
            this.ilGroupBox.Controls.Add(this.ILResultView);
            this.ilGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ilGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ilGroupBox.Name = "ilGroupBox";
            this.ilGroupBox.Size = new System.Drawing.Size(381, 259);
            this.ilGroupBox.TabIndex = 8;
            this.ilGroupBox.TabStop = false;
            this.ilGroupBox.Text = "IL Code";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 28);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.WorkingArea);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.codeSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(985, 536);
            this.mainSplitContainer.SplitterDistance = 600;
            this.mainSplitContainer.TabIndex = 9;
            // 
            // codeSplitContainer
            // 
            this.codeSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.codeSplitContainer.Name = "codeSplitContainer";
            this.codeSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // codeSplitContainer.Panel1
            // 
            this.codeSplitContainer.Panel1.Controls.Add(this.taCodeGroupBox);
            // 
            // codeSplitContainer.Panel2
            // 
            this.codeSplitContainer.Panel2.Controls.Add(this.ilGroupBox);
            this.codeSplitContainer.Size = new System.Drawing.Size(381, 536);
            this.codeSplitContainer.SplitterDistance = 273;
            this.codeSplitContainer.TabIndex = 0;
            // 
            // optimizationTypeToolStripComboBox
            // 
            this.optimizationTypeToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optimizationTypeToolStripComboBox.Name = "optimizationTypeToolStripComboBox";
            this.optimizationTypeToolStripComboBox.Size = new System.Drawing.Size(200, 24);
            // 
            // optimizeToolStripMenuItem
            // 
            this.optimizeToolStripMenuItem.Enabled = false;
            this.optimizeToolStripMenuItem.Name = "optimizeToolStripMenuItem";
            this.optimizeToolStripMenuItem.Size = new System.Drawing.Size(67, 24);
            this.optimizeToolStripMenuItem.Text = "Optimize";
            this.optimizeToolStripMenuItem.Click += new System.EventHandler(this.optimizeToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 586);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.taCodeGroupBox.ResumeLayout(false);
            this.taCodeGroupBox.PerformLayout();
            this.ilGroupBox.ResumeLayout(false);
            this.ilGroupBox.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel1.PerformLayout();
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.codeSplitContainer.Panel1.ResumeLayout(false);
            this.codeSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.codeSplitContainer)).EndInit();
            this.codeSplitContainer.ResumeLayout(false);
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
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem startApplicationToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox ILResultView;
        private System.Windows.Forms.GroupBox taCodeGroupBox;
        private System.Windows.Forms.GroupBox ilGroupBox;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer codeSplitContainer;
        private System.Windows.Forms.ToolStripComboBox optimizationTypeToolStripComboBox;
        private System.Windows.Forms.ToolStripMenuItem optimizeToolStripMenuItem;
    }
}

