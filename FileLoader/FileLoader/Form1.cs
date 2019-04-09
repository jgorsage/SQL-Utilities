using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileLoader
{
    public partial class Form1 : Form
    {
        public bool _hasHeaders = true;
        public string _schema = "";
        public string _customDelimiter = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
                MessageBox.Show("Please select a file to load.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                try
                {
                    ProcessInput pi = new ProcessInput(textBox3.Text, textBox1.Text, textBox2.Text, this);
                    pi.LoadFile();
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), new object[] { text });
                return;
            }
            richTextBox1.Text += "- " + text;
            richTextBox1.Text += Environment.NewLine;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Advanced frm = new Advanced(this);
            frm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
