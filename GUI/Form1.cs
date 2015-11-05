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

using SimpleCompiler;
using SimpleScanner;
using SimpleParser;
using SimpleLang;

namespace GUI
{
    public partial class Form1 : Form
    {
        string fullFilename = null;
        bool textModified = false;
        string formName = "OptimizedCompilersProject";

        public Form1()
        {
            InitializeComponent();
            SelectGrammar.SelectedIndex = 0;
            CreateNewFile_Click(null, EventArgs.Empty);
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        string s = new StreamReader(myStream, Encoding.UTF8).ReadToEnd();
                        WorkingArea.Text = s;
                        fullFilename = openFileDialog1.FileName;
                    }
                    myStream.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                RunParser_Click(null, EventArgs.Empty);
                textModified = false;
                Text = formName + " - " + Path.GetFileName(fullFilename);
            }
        }

        private void RunParser_Click(object sender, EventArgs e)
        {
            //var file = fullFilename;

            try
            {
                ResultView.Text = string.Empty;
                string content = WorkingArea.Text;
                //Console.Write("File ...\\" + file.Substring(file.LastIndexOf('\\')) + "... ");

                Scanner scanner = new Scanner();
                scanner.SetSource(content, 0);

                Parser parser = new Parser(scanner);

                if (parser.Parse())
                {
                    //Console.WriteLine("Синтаксическое дерево построено");

                    var codeGenerator = new SimpleLang.Gen3AddrCodeVisitor();
                    codeGenerator.Visit(parser.root);

                    var code = codeGenerator.CreateCode();

                    // DEBUG Can watch result here
                    //Console.WriteLine(codeGenerator.Code);

                    ResultView.Text = code.ToString().Replace("\n", Environment.NewLine);
                }
                else MessageBox.Show("Ошибка");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл не найден");
            }
            catch (LexException ee)
            {
                MessageBox.Show("Лексическая ошибка. " + ee.Message);
            }
            catch (SyntaxException ee)
            {
                MessageBox.Show("Синтаксическая ошибка. " + ee.Message);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (fullFilename == null)
                SaveAs_Click(sender, e);
            else
            {
                File.WriteAllText(fullFilename, WorkingArea.Text);
                textModified = false;
                Text = formName + " - " + Path.GetFileName(fullFilename);
            }
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, WorkingArea.Text);
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
            Text = formName + " - новый файл";
            textModified = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textModified)
            {
                Text += "*";
                textModified = true;
            }
            Console.Write("TextChengedc");
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
