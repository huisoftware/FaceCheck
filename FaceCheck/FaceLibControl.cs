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
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void updateGroupList()
        {
            var result = GroupGetlistDemo();
            // 每次查询前清空dataGridView1数据
            while (this.dataGridView1.Rows.Count != 0)
            {
                this.dataGridView1.Rows.RemoveAt(0);
            }

            //对dataGridView1赋予新的值
            for (int i = 0; i <= result["group_id_list"].ToArray().Length - 1; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = result["group_id_list"][i];
            }

        }

        //创建用户组列表
        List<string> groupList = new List<string>();

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
            var groupId = "group1";

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
    }
}
