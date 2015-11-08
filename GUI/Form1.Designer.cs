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
<<<<<<< HEAD
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateNew = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFileAs = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectGrammar = new System.Windows.Forms.ToolStripComboBox();
            this.RunItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WorkingArea = new System.Windows.Forms.TextBox();
            this.ResultView = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.SelectGrammar,
            this.RunItem});
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
            this.CreateNew.Size = new System.Drawing.Size(133, 22);
            this.CreateNew.Text = "Create new";
            this.CreateNew.Click += new System.EventHandler(this.CreateNewFile_Click);
            // 
            // OpenFile
            // 
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(133, 22);
            this.OpenFile.Text = "Open";
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(133, 22);
            this.SaveFile.Text = "Save";
            this.SaveFile.Click += new System.EventHandler(this.Save_Click);
            // 
            // SaveFileAs
            // 
            this.SaveFileAs.Name = "SaveFileAs";
            this.SaveFileAs.Size = new System.Drawing.Size(133, 22);
            this.SaveFileAs.Text = "Save as";
            this.SaveFileAs.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // SelectGrammar
            // 
            this.SelectGrammar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectGrammar.Enabled = false;
            this.SelectGrammar.Items.AddRange(new object[] {
            "С/С++",
            "Pascal"});
            this.SelectGrammar.Name = "SelectGrammar";
            this.SelectGrammar.Size = new System.Drawing.Size(92, 24);
            // 
            // RunItem
            // 
            this.RunItem.Image = global::GUI.Properties.Resources.run;
            this.RunItem.Name = "RunItem";
            this.RunItem.Size = new System.Drawing.Size(63, 24);
            this.RunItem.Text = "Start";
            this.RunItem.Click += new System.EventHandler(this.RunParser_Click);
            // 
            // WorkingArea
            // 
            this.WorkingArea.AcceptsTab = true;
            this.WorkingArea.Dock = System.Windows.Forms.DockStyle.Left;
            this.WorkingArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WorkingArea.Location = new System.Drawing.Point(0, 28);
            this.WorkingArea.Margin = new System.Windows.Forms.Padding(2);
            this.WorkingArea.Multiline = true;
            this.WorkingArea.Name = "WorkingArea";
            this.WorkingArea.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.WorkingArea.Size = new System.Drawing.Size(564, 558);
            this.WorkingArea.TabIndex = 1;
            this.WorkingArea.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.WorkingArea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // ResultView
            // 
            this.ResultView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultView.Location = new System.Drawing.Point(564, 28);
            this.ResultView.Margin = new System.Windows.Forms.Padding(2);
            this.ResultView.Multiline = true;
            this.ResultView.Name = "ResultView";
            this.ResultView.ReadOnly = true;
            this.ResultView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultView.Size = new System.Drawing.Size(421, 558);
            this.ResultView.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 586);
            this.Controls.Add(this.ResultView);
            this.Controls.Add(this.WorkingArea);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

=======
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateNew = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFileAs = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectGrammar = new System.Windows.Forms.ToolStripComboBox();
            this.RunItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WorkingArea = new System.Windows.Forms.TextBox();
            this.ResultView = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.SelectGrammar,
            this.RunItem});
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
            this.CreateNew.Size = new System.Drawing.Size(133, 22);
            this.CreateNew.Text = "Create new";
            this.CreateNew.Click += new System.EventHandler(this.CreateNewFile_Click);
            // 
            // OpenFile
            // 
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(133, 22);
            this.OpenFile.Text = "Open";
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(133, 22);
            this.SaveFile.Text = "Save";
            this.SaveFile.Click += new System.EventHandler(this.Save_Click);
            // 
            // SaveFileAs
            // 
            this.SaveFileAs.Name = "SaveFileAs";
            this.SaveFileAs.Size = new System.Drawing.Size(133, 22);
            this.SaveFileAs.Text = "Save as";
            this.SaveFileAs.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // SelectGrammar
            // 
            this.SelectGrammar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectGrammar.Enabled = false;
            this.SelectGrammar.Items.AddRange(new object[] {
            "С/С++",
            "Pascal"});
            this.SelectGrammar.Name = "SelectGrammar";
            this.SelectGrammar.Size = new System.Drawing.Size(92, 24);
            // 
            // RunItem
            // 
            this.RunItem.Image = global::GUI.Properties.Resources.run;
            this.RunItem.Name = "RunItem";
            this.RunItem.Size = new System.Drawing.Size(63, 24);
            this.RunItem.Text = "Start";
            this.RunItem.Click += new System.EventHandler(this.RunParser_Click);
            // 
            // WorkingArea
            // 
            this.WorkingArea.AcceptsTab = true;
            this.WorkingArea.Dock = System.Windows.Forms.DockStyle.Left;
            this.WorkingArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.WorkingArea.Location = new System.Drawing.Point(0, 28);
            this.WorkingArea.Margin = new System.Windows.Forms.Padding(2);
            this.WorkingArea.Multiline = true;
            this.WorkingArea.Name = "WorkingArea";
            this.WorkingArea.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.WorkingArea.Size = new System.Drawing.Size(564, 558);
            this.WorkingArea.TabIndex = 1;
            this.WorkingArea.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.WorkingArea.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // ResultView
            // 
            this.ResultView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultView.Location = new System.Drawing.Point(564, 28);
            this.ResultView.Margin = new System.Windows.Forms.Padding(2);
            this.ResultView.Multiline = true;
            this.ResultView.Name = "ResultView";
            this.ResultView.ReadOnly = true;
            this.ResultView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultView.Size = new System.Drawing.Size(421, 558);
            this.ResultView.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 586);
            this.Controls.Add(this.ResultView);
            this.Controls.Add(this.WorkingArea);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

>>>>>>> 5ed01454fc05e4df8cd497e3fbd6fc8385e6541a
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
        private System.Windows.Forms.ToolStripComboBox SelectGrammar;
        private System.Windows.Forms.ToolStripMenuItem CreateNew;
    }
}

