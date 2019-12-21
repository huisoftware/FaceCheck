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
        public static UserControl form;
        Tts client2 = null;
        Face client = null;
        public FaceLibControl(Face clientAll, Tts client2All)
        {
            InitializeComponent();

            Face client = clientAll;
            Tts client2 = client2All;
            form = this;
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
        public static List<string> groupList = new List<string>();
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
    }
}
