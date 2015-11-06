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
using SimpleCompiler;
using SimpleLang;

namespace GUI
{
    public partial class Form1 : Form
    {
        string fullFilename = null;
        bool textModified = false;
        FileLoader.GrammarType type = FileLoader.GrammarType.C;
        string formName = "OptimizedCompilersProject";

        public Form1()
        {
            InitializeComponent();
            SelectGrammar.SelectedIndex = 0;
            CreateNewFile_Click(null, EventArgs.Empty);
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();            
            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "pasn files (*.pasn)|*.pasn|cn files (*.cn)|*.cn";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    WorkingArea.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8);
                    type = FileLoader.GetGrammarType(openFileDialog1.FileName);
                    if (type == FileLoader.GrammarType.C)
                        SelectGrammar.SelectedIndex = 0;
                    else
                        SelectGrammar.SelectedIndex = 1;
                    
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
            //var file = fullFilename;

            try
            {
                ResultView.Text = string.Empty;
                string content = WorkingArea.Text;                
                var root = FileLoader.Parse(content, type);

                var codeGenerator = new SimpleLang.Gen3AddrCodeVisitor();
                codeGenerator.Visit(root);

                var code = codeGenerator.CreateCode();

                CommonSubexpressionsOptimization cso = new CommonSubexpressionsOptimization(code);
                cso.Optimize();

                ResultView.Text = code.ToString().Replace("\n", Environment.NewLine);                                
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
                Text = formName + " - " + Path.GetFileName(fullFilename);
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "pasn files (*.pasn)|*.pasn|cn files (*.cn)|*.cn";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, WorkingArea.Text, Encoding.UTF8);
                textModified = false;
                fullFilename = saveFileDialog1.FileName;
                Text = formName + " - " + Path.GetFileName(fullFilename);
            }
        }

        private void CreateNewFile_Click(object sender, EventArgs e)
        {
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
        }
    }
}
