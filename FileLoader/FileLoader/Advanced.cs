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
    public partial class Advanced : Form
    {
        private Form1 _parentForm;
        public Advanced(Form1 parentForm)
        {
            InitializeComponent();
            _parentForm = parentForm;
        }

        private void Advanced_Load(object sender, EventArgs e)
        {
            textBox2.Text = _parentForm._customDelimiter;
            textBox3.Text = _parentForm._schema;
            checkBox1.Checked = _parentForm._hasHeaders;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _parentForm._customDelimiter = textBox2.Text;
            _parentForm._schema = textBox3.Text;
            _parentForm._hasHeaders = checkBox1.Checked;
            Close();
        }
    }
}
