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
        public Form1()
        {
            InitializeComponent();
            signControl = new SignControl();
            faceLibControl = new FaceLibControl();
            signControl.Show();
            groupBox1.Controls.Clear();
            groupBox1.Controls.Add(signControl);
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
    }
}
