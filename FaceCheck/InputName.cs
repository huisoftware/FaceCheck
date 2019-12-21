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

    public partial class InputName : Form
    {
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;
        public InputName()
        {
            InitializeComponent();
        }

        private void userNameButton_Click(object sender, EventArgs e)
        {
            if (null != TextHandler)
            {
                TextHandler.Invoke(userNameText.Text);
                DialogResult = DialogResult.OK;
            }
        }
        public static DialogResult Show(out string strText)
        {
            string strTemp = string.Empty;

            InputName inputDialog = new InputName();
            inputDialog.TextHandler = (str) => { strTemp = str; };

            DialogResult result = inputDialog.ShowDialog();
            strText = strTemp;

            return result;
        }
    }
}
