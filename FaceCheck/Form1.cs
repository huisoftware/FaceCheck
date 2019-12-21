using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceCheck
{
    public partial class Form1 : Form
    {
        public SignControl signControl;
        public FaceLibControl faceLibControl;
        public static List<string> signList = new List<string>();//创建了一个空签到列表
        public Form1()
        {
            InitializeComponent();
            signControl = new SignControl();
            faceLibControl = new FaceLibControl();
            signControl.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(signControl);
        }

        public Form1(List<string> groupList)  //跨窗口引用变量
        {
            InitializeComponent();
            for (int i = 0; i < 10; ++i)
            {
                userGroup_listBox.Items.Add(groupList[i]);
            }


        }
        private void button1_Click(object sender, EventArgs e)
        {
            signControl.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(signControl);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            faceLibControl.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(faceLibControl);
        }
        public Boolean AddSignNum(string id)
        {
            //判断id已存在list中，并更新list
            if (signList.Contains(id))
                return false;
            else
            {
                signList.Add(id);
                string str = Convert.ToString(signList.Count());
                richTextBox1.Text = str;

                return true;
             }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            signList.Clear();
            string str = Convert.ToString(0);
            richTextBox1.Text = str;

            //页面数量=0
            //刷新用户列表



            


        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox1.SelectionAlignment = HorizontalAlignment.Center;  //RichTextBox中的文本居中对齐
        }
    }
}
