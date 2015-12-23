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
using System.Diagnostics;

namespace GUI
{
    public partial class Form1 : Form
    {
        private string mFullFilename = null;
        private bool mTextModified = false;
        private FileLoader.GrammarType mType;
        private string mFormName = "OptimizedCompilersProject";
        private ThreeAddrCode mCode;

        public Form1()
        {
            InitializeComponent();
            GrammarToolStripComboBox.SelectedIndexChanged -= GrammarToolStripComboBox_SelectedIndexChanged;
            GrammarToolStripComboBox.SelectedIndex = 0;
            GrammarToolStripComboBox.SelectedIndexChanged += GrammarToolStripComboBox_SelectedIndexChanged;
            mType = FileLoader.GrammarType.C;
            CreateNewFile_Click(null, EventArgs.Empty);
            FillingOptimizationTypes();
        }

        private void ClearErrors()
        {
            errorsTextBox.Text = "";
        }

        private void AcceptError(string error)
        {
            errorsTextBox.Text = (error + "\r\n");
            outputTabControl.SelectTab(0);
        }

        private void AcceptOutput(string message)
        {
            outputTextBox.Text = (message + "\r\n");
            outputTabControl.SelectTab(1);
        }

        private void FillingOptimizationTypes()
        {
            optimizationTypeToolStripComboBox.Items.Add("CommonSubexpressionsOptimization");
            optimizationTypeToolStripComboBox.Items.Add("DeadCodeElimination");
            optimizationTypeToolStripComboBox.Items.Add("DraggingConstantsOptimization");
            optimizationTypeToolStripComboBox.Items.Add("ReachExprOptimization");
            optimizationTypeToolStripComboBox.Items.Add("ActiveVarsOptimization");
            optimizationTypeToolStripComboBox.Items.Add("ConstantFolding");
            optimizationTypeToolStripComboBox.Items.Add("ConstantsPropagationOptimization");
            optimizationTypeToolStripComboBox.Items.Add("AllOptimizations");
        }

        private void optimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                switch (optimizationTypeToolStripComboBox.Text)
                {
                    case "CommonSubexpressionsOptimization":
                        var cso = new CommonSubexpressionsOptimization(mCode);
                        cso.Optimize();
                        break;
                    case "DraggingConstantsOptimization":
                        var dco = new DraggingConstantsOptimization(mCode);
                        dco.Optimize();
                        break;
                    case "ReachExprOptimization":
                        var reo = new ReachExprOptimization(mCode);
                        reo.Optimize();
                        break;
                    case "ActiveVarsOptimization":
                        var avo = new ActiveVarsOptimization(mCode);
                        avo.Optimize();
                        break;
                    case "ConstantFolding":
                        var cf = new ConstantFolding(mCode);
                        cf.Optimize();
                        break;
                    case "ConstantsPropagationOptimization":
                        var cpo = new ConstantsPropagationOptimization(mCode);
                        cpo.Optimize();
                        break;
                    case "DeadCodeElimination":
                        var dce = new DeadCodeElimination(mCode);
                        dce.Optimize();
                        break;
                    case "AllOptimizations":
                        var optimizer = new iCompiler.Optimizer();
                        optimizer.AddOptimization(new iCompiler.DraggingConstantsOptimization());
                        optimizer.AddOptimization(new iCompiler.ReachExprOptimization());
                        optimizer.AddOptimization(new iCompiler.ActiveVarsOptimization());
                        optimizer.AddOptimization(new iCompiler.ConstantFolding());
                        optimizer.AddOptimization(new iCompiler.CommonSubexpressionsOptimization());
                        optimizer.AddOptimization(new iCompiler.ConstantsPropagationOptimization());
                        optimizer.Assign(mCode);
                        optimizer.Optimize();
                        break;
                }


                ResultView.Text = mCode.ToString().Replace("\n", Environment.NewLine);
                ILResultView.Text = ILCodeGenerator.Generate(mCode);
            }

            catch (LexException ee)
            {
                AcceptError("Lexer error: " + ee.Message);
            }
            catch (SyntaxException ee)
            {
                AcceptError("Syntax error: " + ee.Message);
            }
            catch (SemanticException ee)
            {
                AcceptError("Semantic error: " + ee.Message);
            }
            catch (Exception ee)
            {
                string message = "Unexpected error: " + ee.Message + "\r\n";
                message += "Call stack:\r\n";
                var stackObject = new System.Diagnostics.StackTrace(ee);
                message += stackObject.ToString();
                AcceptError(message);
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mFullFilename = openFileDialog.FileName;
                    string text = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                    WorkingArea.Text = text;
                    mType = FileLoader.GetGrammarType(openFileDialog.FileName);
                    switch (mType)
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
                    mTextModified = false;
                    Text = mFormName + " - " + openFileDialog.FileName;
                    RunParser_Click(null, EventArgs.Empty);
                }
                catch (FileNotFoundException ex)
                {
                    var error = "Error: Could not read file from disk. Original error: \r\n";
                    AcceptError(error + ex.Message);
                }
            }
        }

        private void RunParser_Click(object sender, EventArgs e)
        {
            try
            {
                ResultView.Text = string.Empty;
                string content = WorkingArea.Text;
                if (mType != FileLoader.GrammarType.PASCALABCNET)
                {
                    var root = FileLoader.Parse(content, mType);

                    var codeGenerator = new iCompiler.Gen3AddrCodeVisitor();
                    codeGenerator.Visit(root);

                    if (codeGenerator.Errors.Count == 0)
                    {
                        mCode = codeGenerator.CreateCode();

                        ResultView.Text = mCode.ToString().Replace("\n", Environment.NewLine);
                        ILResultView.Text = ILCodeGenerator.Generate(mCode);
                        optimizeToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        string errors = String.Join(System.Environment.NewLine, codeGenerator.Errors.Select(x => x.ToString()));
                        AcceptError(errors);
                    }
                }
                else
                {
                    ProcessPascalABCNETCode(content);
                    optimizeToolStripMenuItem.Enabled = false;
                }
            }
            catch (FileNotFoundException ee)
            {
                AcceptError("File not found: " + ee.FileName);
            }
            catch (LexException ee)
            {
                AcceptError("Lexer error: " + ee.Message);
            }
            catch (SyntaxException ee)
            {
                AcceptError("Syntax error: " + ee.Message);
            }
            catch (SemanticException ee)
            {
                AcceptError("Semantic error: " + ee.Message);
            }
            catch (Exception ee)
            {
                string message = "Unexpected error: " + ee.Message + "\r\n";
                message += "Call stack:\r\n";
                var stackObject = new System.Diagnostics.StackTrace(ee);
                message += stackObject.ToString();
                AcceptError(message);
            }
        }

        private string CompilePascalABCNetCode(Compiler compiler, CompilerOptions options)
        {
            var path = Path.GetFullPath(options.SourceFileName);
            var output = compiler.Compile(options);
            if (compiler.ErrorsList.Count > 0)
            {
                foreach (var error in compiler.ErrorsList)
                {
                    string desc = error.ToString();
                    if (desc.Contains("PABCSystem"))
                    {
                        var temp = path.Substring(0, path.LastIndexOf('\\') + 1);
                        desc += "\r\nPABCSystem.pcu должен находиться в папке " + temp;
                    }

                    AcceptError("PascalABC.Net error: " + desc);
                }

                return "";
            }

            return output;
        }

        private void ProcessPascalABCNETCode(string content)
        {
            var filename = String.Format(@"{0}.pas", System.Guid.NewGuid());
            File.WriteAllText(filename, WorkingArea.Text);
            var path = Path.GetFullPath(filename);

            Compiler compiler = new Compiler();
            var opts = new CompilerOptions(filename, CompilerOptions.OutputType.ConsoleApplicaton);
            opts.GenerateCode = true;
            opts.Optimise = false;

            long t1 = -1, t2 = -1;
            var slow = CompilePascalABCNetCode(compiler, opts);
            if (slow.Length > 0)
            {
                var time = Task.Factory.StartNew<long>(() => ExecuteAndEvaluateTime(slow));
                t1 = time.Result;
                File.Delete(slow);
            }

            var changer = new SyntaxTreeChanger();
            compiler.SyntaxTreeChanger = changer;
            var fast = CompilePascalABCNetCode(compiler, opts);
            if (fast.Length > 0)
            {
                var time = Task.Factory.StartNew<long>(() => ExecuteAndEvaluateTime(fast));
                t2 = time.Result;
                File.Delete(fast);
                ResultView.Text = changer.Printer.Code.ToString().Replace("\n", Environment.NewLine);
            }

            if (t1 >= 0 && t2 >= 0)
            {
                ClearErrors();
                string description = "Original version: " + t1 + " ms.\r\n";
                description += "Optimized version " + t2 + " ms.";
                AcceptOutput(description);
            }

            File.Delete(path);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (mFullFilename == null)
                SaveAs_Click(sender, e);
            else
            {
                File.WriteAllText(mFullFilename, WorkingArea.Text, Encoding.UTF8);
                mTextModified = false;
                Text = mFormName + " - " + mFullFilename;
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, WorkingArea.Text, Encoding.UTF8);
                mTextModified = false;
                mFullFilename = saveFileDialog.FileName;
                Text = mFormName + " - " + mFullFilename;
            }
        }

        private void CreateNewFile_Click(object sender, EventArgs e)
        {
            if (mTextModified)
            {
                // Display a MsgBox asking the user to save changes or abort.
                DialogResult dRes = MessageBox.Show("Do you want to save changes?", mFormName, MessageBoxButtons.YesNoCancel);
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
            mFullFilename = null;
            WorkingArea.Text = string.Empty;
            ResultView.Text = string.Empty;
            Text = mFormName + " - new file";
            mTextModified = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!mTextModified)
            {
                Text += "*";
                mTextModified = true;
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
            mType = GrammarToolStripComboBox.SelectedIndex == 0 ? FileLoader.GrammarType.C : GrammarToolStripComboBox.SelectedIndex == 1
                                                               ? FileLoader.GrammarType.PASCAL : FileLoader.GrammarType.PASCALABCNET;

            hideILView(mType);

            this.toolStripStatusLabel1.Text = "";
            this.startApplicationToolStripMenuItem.Enabled = mType == FileLoader.GrammarType.PASCALABCNET;
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
                    codeSplitContainer.Panel2Collapsed = false;
                    codeSplitContainer.Panel2.Show();
                    break;

                case FileLoader.GrammarType.PASCALABCNET:

                    codeSplitContainer.Panel2Collapsed = true;
                    codeSplitContainer.Panel2.Hide();
                    break;

                default:
                    codeSplitContainer.Panel2Collapsed = false;
                    codeSplitContainer.Panel2.Show();
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mTextModified)
            {
                // Display a MsgBox asking the user to save changes or abort.
                DialogResult dRes = MessageBox.Show("Do you want to save changes?", mFormName, MessageBoxButtons.YesNoCancel);
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

        private long ExecuteAndEvaluateTime(string executable)
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            Process.Start(executable).WaitForExit();
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        private void startApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = CompileILCode(ILResultView.Text);

            if (p.StartInfo.FileName == null)
            {
                AcceptError("Ошибка компиляции IL-кода");
            }
            else
            {
                p.Start();
                p.WaitForExit();

                ClearErrors();
                var output = p.StandardOutput.ReadToEnd();
                output += "\r\nSuccessfully finished";
                AcceptOutput(output);
            }
        }

        private string CompileILCode(string ilcode)
        {
            var version = Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.VersionLatest;
            string ilcomp = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkFile("ILAsm.exe", version);
            string tmp_fn = "_tmp";
            string tmp_ilfn = tmp_fn + ".il";
            string tmp_exefn = tmp_fn + ".exe";
            StreamWriter sw = new StreamWriter(tmp_ilfn);
            sw.Write(ilcode);
            sw.Close();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = ilcomp;
            p.StartInfo.Arguments = tmp_ilfn;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            File.Delete(tmp_fn);
            if (p.StandardOutput.ReadToEnd().Contains("Operation completed successfully"))
                return (new FileInfo(tmp_exefn)).FullName;
            else
                return null;
        }
    }
}
