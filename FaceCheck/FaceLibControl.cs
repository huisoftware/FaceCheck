﻿using System;
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
using System.Threading;

namespace FaceCheck
{
    public partial class FaceLibControl : UserControl
    {
        string userListUrl = Util.soundPath + "\\data\\user\\userlist.txt";

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
            var groupId = textBox1.Text;

            updateUserList(groupId);

        }

        public void updateUserList(string groupId)
        {
            // 每次查询前判断是否已选择用户组
            if (string.IsNullOrWhiteSpace(textBox1.Text))

            {
                MessageBox.Show("请选择相应的用户组！");
                return;
            }

            // 如果有可选参数
            var options = new Dictionary<string, object>{
                        {"start", 0},
                        {"length", 100}
                                                         };
            // 带参数调用获取用户组列表
            var result = client.GroupGetusers(groupId, options);

            userList.Rows.Clear();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (File.Exists(userListUrl))
            {
                string[] userFileLine = File.ReadAllLines(userListUrl, Encoding.UTF8);
                for (int i = 0; i < userFileLine.Length; i++)
                {
                    string[] oneLine = userFileLine[i].Split(' ');
                    dic.Add(oneLine[0], oneLine[1]);
                }
            }
            

            for (int i = 0; i <= result.GetValue("result")["user_id_list"].ToArray().Length - 1; i++)
            {

                var uid = result.GetValue("result")["user_id_list"][i].ToString();
                var index = userList.Rows.Add();
                // 获取用户组用户ID
                userList.Rows[index].Cells[0].Value = result.GetValue("result")["user_id_list"][i];
                var userName = "";
                if (dic.ContainsKey(uid))
                {
                    userName = dic[uid];
                }
                userList.Rows[index].Cells[1].Value = userName;
                // 获取用户组用户名    备用字段
                //userList.Rows[index].Cells[1].Value = result["user_id_list"][i];
                //获取用户组用户是否签到   
                // Form[signList]
                string isSign = "否";
                if (Form1.signList.Contains(uid))
                {
                    isSign = "是";
                }
                userList.Rows[index].Cells[2].Value = isSign;

            }
        }

        public void updateGroupList()
        {
            var result = GroupGetlistDemo();
            // 每次查询前清空dataGridView1数据
            this.dataGridView1.Rows.Clear();
            

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
                Form1.form.listBoxDel(groupId);
                groupList.Remove(groupId);
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
        }

        // 刷新首页用户组，增加删除用户组可在一级窗口刷新显示默认10个
        public void renovate(List<string> group_list)
        {
            Form1.form.listBoxAdd(group_list);
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
                updateUserList(groupID);
            }
            
        }

        

        private void register(string[] imgPaths, string groupID, string name)
        {
            var uid = Util.GenerateStringID();
            if (!Directory.Exists(Util.soundPath + "\\data\\user\\"))
            {
                Directory.CreateDirectory(Util.soundPath + "\\data\\user\\");
            }
            File.AppendAllText(userListUrl, uid + " " + name + Environment.NewLine, Encoding.UTF8);

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
                Thread.Sleep(350);
                int i = Convert.ToInt32(result["error_code"]);
                if (i == 0)
                {

                }
                else
                {
                    MessageBox.Show("注册失败！");
                    return;
                }
            }

        }

        private string readImg(string img)
        {
            return Convert.ToBase64String(File.ReadAllBytes(img));
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
                update(files, groupID, uid);
            }
            MessageBox.Show("更新成功！");
        }

        private void update(string[] imgPaths, string groupID, string uid)
        {
            var key = 1;
            foreach (string img in imgPaths)
            {
                var image = readImg(img);

                var imageType = "BASE64";

                var groupId = groupID;

                var userId = uid;

                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (File.Exists(userListUrl))
                {
                    string[] userFileLine = File.ReadAllLines(userListUrl, Encoding.UTF8);
                    for (int j = 0; j < userFileLine.Length; j++)
                    {
                        string[] oneLine = userFileLine[j].Split(' ');
                        dic.Add(oneLine[0], oneLine[1]);
                    }
                }
                var userName = "";
                if (dic.ContainsKey(uid))
                {
                    userName = dic[uid];
                }

                // 如果有可选参数
                var options = new Dictionary<string, object>{
                    {"user_info", userName},
                };
                if (key == 1)
                {
                    options.Add("action_type", "REPLACE");
                }
                // 带参数调用人脸注册
                var result = client.UserUpdate(image, imageType, groupId, userId, options);
                key++;
                Thread.Sleep(350);
                int i = Convert.ToInt32(result["error_code"]);
                if (i == 0)
                {
                    
                }
                else
                {
                    MessageBox.Show("更新失败！");
                    return;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("无用户组！！");
                return;
            }
            string groupID;
            DialogResult dr = MessageBox.Show("确定要使用选中的用户组吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                groupID = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                if (!groupList.Contains(groupID))
                {
                    groupList.Add(groupID);
                    renovate(groupList);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //选择多行进行删除用户组
            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("无用户组！！");
                return;
            }

            DialogResult dr = MessageBox.Show("确定要删除选中的用户组吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                UserDelete();
            }
        }
        public void UserDelete()
        {
            var groupId = textBox1.Text;
            var userId = userList.CurrentRow.Cells[0].Value.ToString();

            var result = client.UserDelete(groupId, userId);
            int i = Convert.ToInt32(result["error_code"]);
            if (i == 0)
            {
                MessageBox.Show("删除成功！");
            }
            else
            {
                MessageBox.Show("删除失败！");
            }


            //刷新
            updateUserList(groupId);

        }
    }
}
