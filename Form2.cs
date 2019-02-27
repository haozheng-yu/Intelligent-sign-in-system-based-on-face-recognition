using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcsoftFaceTest
{
    public partial class Form2 : Form
    {
        public string groupName = "";				//组名
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupName = textBox1.Text.Trim();
            this.Close();					//关闭窗体
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();			//关闭此窗体
        }
    }
}
