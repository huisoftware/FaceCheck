using Baidu.Aip.Face;
using Baidu.Aip.Speech;
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
        // 设置APPID/AK/SK
        string API_KEY = "htxG3CCVEM1qHmDNyPlXmZKW";
        string SECRET_KEY = "e0pDyckuYpLWeGsO9nkctamry9Gw1TGj";
        string API_KEY2 = "IjlWHfoHUVi1RhUBxYQ6wPtl";
        string SECRET_KEY2 = "KMNhzi71gVY5dlUHOSHVZM9FQzhj1jdl";
        Tts client2 = null;
        Face client = null;

        public static Form1 form;

        public SignControl signControl;
        public FaceLibControl faceLibControl;
        public static List<string> signList = new List<string>();//创建了一个空签到列表
        public Form1()
        {
            InitializeComponent();

            client2 = new Tts(API_KEY2, SECRET_KEY2);
            client2.Timeout = 60000;  // 修改超时时间
            client = new Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间

            signControl = new SignControl(client,client2);
            faceLibControl = new FaceLibControl(client, client2);
            signControl.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(signControl);

            form = this;
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
            var result = faceLibControl.GroupGetlistDemo();
            
            var list = result.GetValue("group_list").ToArray();
            var k = 0;

            FaceLibControl.groupList.Clear();
            foreach (string groupID in list)
            {
                if (k > 9) break;
                FaceLibControl.groupList.Add(groupID);
                k++;
            }
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
