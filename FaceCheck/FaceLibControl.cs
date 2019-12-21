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
        public AddUserGroup addUserGroup;
        public static FaceLibControl form;
        Tts client2 = null;
        Face client = null;
        public FaceLibControl(Face clientAll, Tts client2All)
        {
            InitializeComponent();
            client = clientAll;
            client2 = client2All;
            form = this;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        public void updateGroupList()
        {
            var result = GroupGetlistDemo();
            // 每次查询前清空dataGridView1数据
            this.dataGridView1.Rows.Clear();
            /*while (this.dataGridView1.Rows.Count > 1)
            {
              
                this.dataGridView1.Rows.Cl(1);
            }*/

            //对dataGridView1赋予新的值
            for (int i = 0; i <= result.GetValue("result")["group_id_list"].ToArray().Length - 1; i++)
            {
                var index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = result.GetValue("result")["group_id_list"][i];
            }

        }

        //创建用户组列表
        public static List<string> groupList = new List<string>();

        public Newtonsoft.Json.Linq.JObject GroupGetlistDemo()
        {
            // 调用组列表查询，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GroupGetlist();
            // 如果有可选参数
            var options = new Dictionary<string, object>{
            {"start", 0},
            {"length", 1000}
                                         };
            // 带参数调用组列表查询
            result = client.GroupGetlist(options);
            return result;

        }
        public void GroupDeleteDemo()
        {
            var groupId = dataGridView1.CurrentRow.Cells[0].Value.ToString();

            // 调用删除用户组，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GroupDelete(groupId);
            int i = Convert.ToInt32(result["error_code"]);
            if ( i == 0)
            {
                MessageBox.Show("删除成功！");
            }
            else
            {
                MessageBox.Show("删除失败！");
            }
  
            //刷新dataGridview
            updateGroupList();

        }
       

        private void FaceLibControl_Load(object sender, EventArgs e)
        {
            updateGroupList();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            /*if (e.RowIndex > -1)
            {
                this.textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            }*/
        }

        // 刷新首页用户组，增加删除用户组可在一级窗口刷新显示默认10个
        public void renovate(List<string> group_list)
        {
            Form1 child = new Form1(group_list);
            child.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addUserGroup = new AddUserGroup(client, client2);
            addUserGroup.Show();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //选择多行进行删除用户组
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("无用户组！！");
                return;
            }

            DialogResult dr = MessageBox.Show("确定要删除选中的用户组吗？","提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                GroupDeleteDemo();
            }

        }

        private void stopGroupBt_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("无用户组！！");
                return;
            }
            string groupID;
            DialogResult dr = MessageBox.Show("确定要停用选中的用户组吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                groupID = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                groupList.Remove(groupID);
            }
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
