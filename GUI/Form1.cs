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
            toolStripComboBox1.SelectedIndex = 0;
            создатьToolStripMenuItem_Click(null, EventArgs.Empty);
        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
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
                        textBox1.Text = s;
                        fullFilename = openFileDialog1.FileName;
                    }
                    myStream.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                запуститьToolStripMenuItem_Click(null, EventArgs.Empty);
                textModified = false;
                Text = formName + " - " + Path.GetFileName(fullFilename);
            }
        }

        private void запуститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var file = fullFilename;

            try
            {
                textBox2.Text = string.Empty;
                string content = textBox1.Text;
                //Console.Write("File ...\\" + file.Substring(file.LastIndexOf('\\')) + "... ");

                Scanner scanner = new Scanner();
                scanner.SetSource(content, 0);

                Parser parser = new Parser(scanner);

                if (parser.Parse())
                {
                    //Console.WriteLine("Синтаксическое дерево построено");

                    Gen3AddrCodeVisitor codeGenerator = new Gen3AddrCodeVisitor();
                    codeGenerator.Visit(parser.root);

                    // DEBUG Can watch result here
                    //Console.WriteLine(codeGenerator.Code);

                    textBox2.Text = codeGenerator.Code.ToString().Replace("\n", Environment.NewLine);
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

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fullFilename == null)
                сохранитьКакToolStripMenuItem_Click(sender, e);
            else
            {
                File.WriteAllText(fullFilename, textBox1.Text);
            }
            textModified = false;
            Text = formName + " - " + Path.GetFileName(fullFilename);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter sw = new StreamWriter(myStream);
                    sw.Write(textBox1.Text);
                    sw.Close();
                    // Code to write the stream goes here.
                    myStream.Close();
                }
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fullFilename = null;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
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
                запуститьToolStripMenuItem_Click(null, EventArgs.Empty);
        }
    }
}
