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

using ParsePABC;
using PascalABCCompiler;
using PascalABCCompiler.Errors;

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
            this.toolStripStatusLabel1.Text = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fullFilename = openFileDialog.FileName;
                    WorkingArea.Text = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                    type = FileLoader.GetGrammarType(openFileDialog.FileName);
                    switch (type)
                    {
                        case FileLoader.GrammarType.C:
                            GrammarToolStripComboBox.SelectedIndex = 0;
                            break;
                        case FileLoader.GrammarType.PASCAL:
                            GrammarToolStripComboBox.SelectedIndex = 1;
                            break;
                        case FileLoader.GrammarType.PASCALABCNET:
                            GrammarToolStripComboBox.SelectedIndex = 2;
                            break;
                    }
                    textModified = false;
                    Text = formName + " - " + openFileDialog.FileName;
                    RunParser_Click(null, EventArgs.Empty);                
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void RunParser_Click(object sender, EventArgs e)
        {
            try
            {
                ResultView.Text = string.Empty;
                string content = WorkingArea.Text;
                if (type != FileLoader.GrammarType.PASCALABCNET)
                {
                    var root = FileLoader.Parse(content, type);

                    var codeGenerator = new iCompiler.Gen3AddrCodeVisitor();
                    codeGenerator.Visit(root);

                    var code = codeGenerator.CreateCode();

                    CommonSubexpressionsOptimization cso = new CommonSubexpressionsOptimization(code);
                    cso.Optimize();

                    ResultView.Text = code.ToString().Replace("\n", Environment.NewLine);
                    ILResultView.Text = ILCodeGenerator.Generate(code);
                }
                else
                {
                    ProcessPascalABCNETCode(content);
                }
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

        private void ProcessPascalABCNETCode(string content)
        {
            var filename = String.Format(@"{0}.pas", System.Guid.NewGuid());
            File.WriteAllText(filename, WorkingArea.Text);

            Compiler compiler = new Compiler();
            var changer = new SyntaxTreeChanger();
            compiler.SyntaxTreeChanger = changer;
            var opts = new CompilerOptions(filename, CompilerOptions.OutputType.ConsoleApplicaton);
            //opts.GenerateCode = true;
            compiler.Compile(opts);

            ResultView.Text = changer.Code.ToString().Replace("\n", Environment.NewLine);
            File.Delete(filename);
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
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, WorkingArea.Text, Encoding.UTF8);
                textModified = false;
                fullFilename = saveFileDialog.FileName;
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
            type = GrammarToolStripComboBox.SelectedIndex == 0 ? FileLoader.GrammarType.C : GrammarToolStripComboBox.SelectedIndex == 1 
                                                               ? FileLoader.GrammarType.PASCAL : FileLoader.GrammarType.PASCALABCNET;

            hideILView(type);

            this.toolStripStatusLabel1.Text = "";
            this.startApplicationToolStripMenuItem.Enabled = type == FileLoader.GrammarType.PASCALABCNET;
            openFileDialog.FilterIndex = saveFileDialog.FilterIndex = GrammarToolStripComboBox.SelectedIndex + 1;
            ResultView.Text = string.Empty;
            ILResultView.Text = string.Empty;
        }

        private void hideILView(FileLoader.GrammarType type)
        {
            switch (type)
            {
                case FileLoader.GrammarType.C:
                case FileLoader.GrammarType.PASCAL:
                    ILResultView.Visible = true;
                    IlCode.Visible = true;
                    break;

                case FileLoader.GrammarType.PASCALABCNET:
                    ILResultView.Visible = false;
                    IlCode.Visible = false;
                    break;

                default:
                    ILResultView.Visible = false;
                    IlCode.Visible = false;
                    break;
            }
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

        private Tuple<long, TOUT> ExecuteAndEvaluateTime<TIN, TOUT>(Func<TIN, TOUT> operation, TIN in_value)
        {
            System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
            s.Start();
            TOUT ret = operation(in_value);
            s.Stop();
            return new Tuple<long, TOUT>(s.ElapsedMilliseconds, ret);
        }

        private void startApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            //NEEDS REWORK: call parameters, can use anything for it.
            Task.Factory.StartNew(() =>
                {
                    this.toolStripStatusLabel1.Text = "Execution speed comparison is in progress...";
                    string path_vanilla="";
                    string path_optimized="";
                    var t1 = Task.Factory.StartNew<Tuple<long, bool>>(() => ExecuteAndEvaluateTime(RunVanilla, path_vanilla));
                    var t2 = Task.Factory.StartNew<Tuple<long, bool>>(() => ExecuteAndEvaluateTime(RunOptimized, path_optimized));
                    Task.WaitAll(t1, t2);
                    if (t1.Result.Item1 > t2.Result.Item1)
                        this.toolStripStatusLabel1.Text = "Comparison complete. " + "Optimized compared to vanilla code: " + (t1.Result.Item1 - t2.Result.Item1) + "ms faster";
                    else
                        this.toolStripStatusLabel1.Text = "Comparison complete. " + "Optimized compared to vanilla code: " + (t1.Result.Item1 - t2.Result.Item1) + "ms slower";
                });            
        }

        /// <summary>
        /// Starts optimized version of a program
        /// NEEDS REWORK
        /// </summary>
        /// <param name="arg">param for program start; path probably</param>
        /// <returns>exit code?</returns>
        private bool RunOptimized(string arg)
        {
            System.Threading.Thread.Sleep(2000);
            return true;
        }


        /// <summary>
        /// Starts vanilla version of a program
        /// NEEDS REWORK
        /// </summary>
        /// <param name="arg">param for program start; path probably</param>
        /// <returns>exit code?</returns>
        private bool RunVanilla(string arg)
        {
            System.Threading.Thread.Sleep(3000);
            return true;
        }
    }
}
