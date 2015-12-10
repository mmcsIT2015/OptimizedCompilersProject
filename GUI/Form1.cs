using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

using CompilerExceptions;
using iCompiler;

namespace GUI
{
    public partial class Form1 : Form
    {
        string fullFilename = null;
        bool textModified = false;
        FileLoader.GrammarType type;
        string formName = "OptimizedCompilersProject";

        public Form1()
        {
            InitializeComponent();
            GrammarToolStripComboBox.SelectedIndexChanged -= GrammarToolStripComboBox_SelectedIndexChanged;
            GrammarToolStripComboBox.SelectedIndex = 0;
            GrammarToolStripComboBox.SelectedIndexChanged += GrammarToolStripComboBox_SelectedIndexChanged;
            type = FileLoader.GrammarType.C;
            CreateNewFile_Click(null, EventArgs.Empty);
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fullFilename = openFileDialog1.FileName;
                    WorkingArea.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8);
                    type = FileLoader.GetGrammarType(openFileDialog1.FileName);
                    GrammarToolStripComboBox.SelectedIndex = type == FileLoader.GrammarType.C ? 0 : 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                RunParser_Click(null, EventArgs.Empty);
                textModified = false;
                Text = formName + " - " + openFileDialog1.FileName;
            }
        }

        private void RunParser_Click(object sender, EventArgs e)
        {
            try
            {
                ResultView.Text = string.Empty;
                string content = WorkingArea.Text;                
                var root = FileLoader.Parse(content, type);

                var codeGenerator = new iCompiler.Gen3AddrCodeVisitor();
                codeGenerator.Visit(root);

                var code = codeGenerator.CreateCode();

                CommonSubexpressionsOptimization cso = new CommonSubexpressionsOptimization(code);
                cso.Optimize();

                ResultView.Text = code.ToString().Replace("\n", Environment.NewLine);
                
                //MessageBox.Show(ILCodeGeneration.GenILCode(code));
            }
            catch (FileNotFoundException ee)
            {
                MessageBox.Show("File not found: " + ee.FileName);
            }
            catch (LexException ee)
            {
                MessageBox.Show("Lexer error: " + ee.Message);
            }
            catch (SyntaxException ee)
            {
                MessageBox.Show("Syntax error: " + ee.Message);
            }
            catch (Exception ee)
            {
                MessageBox.Show("Unexpected error: " + ee.Message);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (fullFilename == null)
                SaveAs_Click(sender, e);
            else
            {
                File.WriteAllText(fullFilename, WorkingArea.Text, Encoding.UTF8);
                textModified = false;
                Text = formName + " - " + fullFilename;
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, WorkingArea.Text, Encoding.UTF8);
                textModified = false;
                fullFilename = saveFileDialog1.FileName;
                Text = formName + " - " + fullFilename;
            }
        }

        private void CreateNewFile_Click(object sender, EventArgs e)
        {
            if (textModified)
            {
                // Display a MsgBox asking the user to save changes or abort.
                DialogResult dRes = MessageBox.Show("Do you want to save changes?", formName, MessageBoxButtons.YesNoCancel);
                if (dRes == DialogResult.Yes)
                {
                    // Call method to save file...
                    Save_Click(null, EventArgs.Empty);
                }
                else if (dRes == DialogResult.Cancel)
                {
                    return;
                }
            }
            fullFilename = null;
            WorkingArea.Text = string.Empty;
            ResultView.Text = string.Empty;
            Text = formName + " - new file";
            textModified = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textModified)
            {
                Text += "*";
                textModified = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                RunParser_Click(null, EventArgs.Empty);
            else if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            {
                Save_Click(null, EventArgs.Empty);
                // Do what you want here
                e.SuppressKeyPress = true;  // Stops bing! Also sets handled which stop event bubbling
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                OpenFile_Click(null, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                CreateNewFile_Click(null, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        private void GrammarToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = GrammarToolStripComboBox.SelectedIndex == 0 ? FileLoader.GrammarType.C : FileLoader.GrammarType.PASCAL;
            openFileDialog1.FilterIndex = saveFileDialog1.FilterIndex = GrammarToolStripComboBox.SelectedIndex + 1;
            ResultView.Text = string.Empty;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textModified)
            {
                // Display a MsgBox asking the user to save changes or abort.
                DialogResult dRes = MessageBox.Show("Do you want to save changes?", formName, MessageBoxButtons.YesNoCancel);
                if (dRes == DialogResult.Yes)
                {
                    // Call method to save file...
                    Save_Click(null, EventArgs.Empty);
                }
                else if (dRes == DialogResult.Cancel)
                {
                    // Cancel the Closing event from closing the form.
                    e.Cancel = true;
                }
            }
        }
    }
}
