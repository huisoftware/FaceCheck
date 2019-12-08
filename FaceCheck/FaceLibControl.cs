using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceCheck
{
    public partial class FaceLibControl : UserControl
    {
        private string selectGroupID;
        public FaceLibControl()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void addGroupButton_Click(object sender, EventArgs e)
        {

        }

        private void addUserButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择图片";
            dialog.Filter = "图片文件(*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var files = dialog.FileNames;
                foreach (string file in files)
                {

                }

            }
        }

        private static string GetGuid()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            return guid.ToString();
        }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
