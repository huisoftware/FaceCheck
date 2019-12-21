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

        private void button3_Click(object sender, EventArgs e)
        {




        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
