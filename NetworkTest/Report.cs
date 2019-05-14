using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkTest
{
    public partial class Report : Form
    {
        public Dictionary<string, string> success;
        public List<string> error;
        public List<string> timeout;
        public Report(Dictionary<string,string> suc,string[] err,string[] timeout)
        {
            InitializeComponent();
            success = suc;
            error = new List<string>(err);
            this.timeout = new List<string>(timeout);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            foreach(var str in success)
            {
                textBox1.Text += "-----" + str.Key + "连接成功!延迟" + str.Value + "毫秒\r\n";
                textBox1.Focus();
                textBox1.Select(textBox1.TextLength, 0);
                textBox1.ScrollToCaret();
            }
            MessageBox.Show("成功次数：" + success.Count(), "Msg", MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            foreach (string str in error)
            {
                textBox1.Text += "-----" + str + "连接错误!\r\n";
                textBox1.Focus();
                textBox1.Select(textBox1.TextLength, 0);
                textBox1.ScrollToCaret();
            }
            MessageBox.Show("失败次数：" + error.Count(), "Msg", MessageBoxButtons.OK);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            foreach (string str in timeout)
            {
                textBox1.Text += "-----" + str + "连接超时!\r\n";
                textBox1.Focus();
                textBox1.Select(textBox1.TextLength, 0);
                textBox1.ScrollToCaret();
            }
            MessageBox.Show("超时次数：" + timeout.Count(), "Msg", MessageBoxButtons.OK);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
