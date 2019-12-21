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
    public partial class AddUserGroup : Form
    {
        Tts client2 = null;
        Face client = null;
        public AddUserGroup(Face clientAll, Tts client2All)
        {
            InitializeComponent();
            client = clientAll;
            client2 = client2All;
        }
        
        private void Button1_Click(object sender, EventArgs e)
        {
            var groupId = textBox1.Text;
            if (groupId.Length==0)
            {
                MessageBox.Show("请填写用户组id！");
            }
            var result = client.GroupAdd(groupId);
            FaceLibControl.form.updateGroupList();
            this.Close();
        }
    }
}
