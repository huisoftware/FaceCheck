using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Baidu.Aip.Speech;
using Baidu.Aip.Face;
using System.IO;

namespace FaceCheck
{
    public partial class FaceLibControl : UserControl
    {

        // 设置APPID/AK/SK
        // 设置APPID/AK/SK
        string API_KEY = "htxG3CCVEM1qHmDNyPlXmZKW";
        string SECRET_KEY = "e0pDyckuYpLWeGsO9nkctamry9Gw1TGj";
        string API_KEY2 = "IjlWHfoHUVi1RhUBxYQ6wPtl";
        string SECRET_KEY2 = "KMNhzi71gVY5dlUHOSHVZM9FQzhj1jdl";
        Tts client2 = null;
        Face client = null;
        public FaceLibControl()
        {
            InitializeComponent();
            client = new Face(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
        public void GroupGetlistDemo()
        {
            // 调用组列表查询，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GroupGetlist();
            Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
            {"start", 0},
            {"length", 1000}
                                         };
            // 带参数调用组列表查询
            result = client.GroupGetlist(options);


            for (int i = 0; i <= result["group_id_list"].ToArray().Length - 1; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = result["group_id_list"][i];
            }

        }
        List<string> groupList = new List<string>();
        private void FaceLibControl_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        // 刷新首页用户组，增加删除用户组可在一级窗口刷新显示默认10个
        public void renovate(List<string> group_list)
        {
            Form1 child = new Form1(group_list);
            child.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupList.Add("需要填写");
            renovate(groupList);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupList.Remove("需要删除");
            renovate(groupList);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请先选择用户组", "错误");
                return;
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择图片";
            dialog.Filter = "图片文件(*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] files = dialog.FileNames;
                var groupID = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                string userName = string.Empty;
                InputName.Show(out userName);
                register(files, groupID, userName);
            }
            
        }

        

        private string register(string[] imgPaths, string groupID, string name)
        {
            var uid = GenerateStringID();
            foreach (string img in imgPaths)
            {
                var image = readImg(img);

                var imageType = "BASE64";

                var groupId = groupID;

                var userId = uid;

                // 如果有可选参数
                var options = new Dictionary<string, object>{
                    {"user_info", name},
                };
                // 带参数调用人脸注册
                var result = client.UserAdd(image, imageType, groupId, userId, options);
            }
            return uid;
        }

        private string readImg(string img)
        {
            return Convert.ToBase64String(File.ReadAllBytes(img));
        }

        private string GenerateStringID()

        {

            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())

            {

                i *= ((int)b + 1);

            }

            return string.Format("{0:x}", i - DateTime.Now.Ticks);

        }

        private void updateFaceBt_Click(object sender, EventArgs e)
        {
            if (userList.SelectedRows.Count < 1)
            {
                MessageBox.Show("请先选择用户", "错误");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择图片";
            dialog.Filter = "图片文件(*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] files = dialog.FileNames;
                var groupID = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                var uid = userList.CurrentRow.Cells[0].Value.ToString();
                string userName = string.Empty;
                InputName.Show(out userName);
                update(files, groupID, userName, uid);
            }
        }

        private string update(string[] imgPaths, string groupID, string name, string uid)
        {
            var key = 1;
            foreach (string img in imgPaths)
            {
                var image = readImg(img);

                var imageType = "BASE64";

                var groupId = groupID;

                var userId = uid;

                // 如果有可选参数
                var options = new Dictionary<string, object>{
                    {"user_info", name},
                };
                if (key == 1)
                {
                    options.Add("action_type", "REPLACE ");
                }
                // 带参数调用人脸注册
                var result = client.UserUpdate(image, imageType, groupId, userId, options);
                key++;
            }
            return uid;
        }
    }
}
