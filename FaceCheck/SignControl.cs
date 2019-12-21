using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using Baidu.Aip.Speech;
using Baidu.Aip.Face;
using System.Threading;
using System.Media;

namespace FaceCheck
{
    public partial class SignControl : UserControl
    {

        //播放声音单独线程
        static public Thread m_thread = null;
        static public CPlaySound g_playsound = new CPlaySound();
        static public string soundPath = System.Windows.Forms.Application.StartupPath;

        // 设置APPID/AK/SK
        string API_KEY = "htxG3CCVEM1qHmDNyPlXmZKW";
        string SECRET_KEY = "e0pDyckuYpLWeGsO9nkctamry9Gw1TGj";
        string API_KEY2 = "IjlWHfoHUVi1RhUBxYQ6wPtl";
        string SECRET_KEY2 = "KMNhzi71gVY5dlUHOSHVZM9FQzhj1jdl";
        Tts client2 = null;
        Face client = null;

        private bool isOpen = true;
        private int hHwnd;
        private const int port = 2000;
        public struct videohdr_tag
        {
            public byte[] lpData;
            public int dwBufferLength;
            public int dwBytesUsed;
            public int dwTimeCaptured;
            public int dwUser;
            public int dwFlags;
            public int[] dwReserved;

        }
        public void GroupGetlistDemo()
        {
            // 调用组列表查询，可能会抛出网络等异常，请使用try/catch捕获
            // 如果有可选参数
            var options = new Dictionary<string, object>{
                {"start", 0},
                {"length", 10}
            };
            // 带参数调用组列表查询
            var result = client.GroupGetlist(options);
            Console.WriteLine(result);
        }

        public delegate bool CallBack(int hwnd, int lParam);
        [DllImport("avicap32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int capCreateCaptureWindowA([MarshalAs(UnmanagedType.VBByRefStr)]   ref string lpszWindowName, int dwStyle, int x, int y, int nWidth, short nHeight, int hWndParent, int nID);
        [DllImport("avicap32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool capGetDriverDescriptionA(short wDriver, [MarshalAs(UnmanagedType.VBByRefStr)]   ref string lpszName, int cbName, [MarshalAs(UnmanagedType.VBByRefStr)]   ref string lpszVer, int cbVer);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool DestroyWindow(int hndw);
        [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, [MarshalAs(UnmanagedType.AsAny)]   object lParam);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        [DllImport("vfw32.dll")]
        public static extern string capVideoStreamCallback(int hwnd, videohdr_tag videohdr_tag);
        [DllImport("vicap32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool capSetCallbackOnFrame(int hwnd, string s);


        private void OpenCapture()
        {
            int intWidth = this.panel1.Width;
            int intHeight = this.panel1.Height;
            int intDevice = 0;
            string refDevice = intDevice.ToString();
            //创建视频窗口并得到句柄
            hHwnd = SignControl.capCreateCaptureWindowA(ref refDevice, 1342177280, 0, 0, 1024, 800, this.panel1.Handle.ToInt32(), 0);
            if (SignControl.SendMessage(hHwnd, 0x40a, intDevice, 0) > 0)
            {
                SignControl.SendMessage(this.hHwnd, 0x435, -1, 0);
                SignControl.SendMessage(this.hHwnd, 0x434, 0x42, 0);
                SignControl.SendMessage(this.hHwnd, 0x432, -1, 0);
                SignControl.SetWindowPos(this.hHwnd, 1, 0, 0, intWidth, intHeight, 6);
            }
            else
            {
                SignControl.DestroyWindow(this.hHwnd);
            }
        }
        public SignControl()
        {
            try
            {
                InitializeComponent();
                client2 = new Tts(API_KEY2, SECRET_KEY2);
                client2.Timeout = 60000;  // 修改超时时间

                client = new Face(API_KEY, SECRET_KEY);
                client.Timeout = 60000;  // 修改超时时间
                this.OpenCapture();

                g_playsound.StartThred();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.OpenCapture();
                isOpen = true;
            }catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
}

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                isOpen = false;
                //停止视频注销视频句柄
                SignControl.SendMessage(this.hHwnd, 0x40b, 0, 0);
                SignControl.DestroyWindow(this.hHwnd);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void searchImgAndRead(Object obj1)
        {
            Image image1 = (Image)((IDataObject)obj1).GetData(typeof(Bitmap));
            var image = Convert.ToBase64String(imageToByteArray(image1));
            var imageType = "BASE64";
            var groupIdList = "3,2";
            var result = client.Search(image, imageType, groupIdList);
            string audioStr = null;
            
            if (result.GetValue("error_code").ToString() == "0")
            {
                audioStr = result.GetValue("error_msg").ToString();
            }
            else
            {
                audioStr = "未能正确识别您的身份";
            }
            Thread thread = new Thread(new ParameterizedThreadStart(audioAlarm));
            thread.IsBackground = true;
            thread.Start(audioStr);
            MessageBox.Show(audioStr);
        }
        private void audioAlarm(object audioStr)
        {
            // 可选参数
            var option = new Dictionary<string, object>()
                    {
                        {"aue", "6"}, // 格式wav
                        {"spd", 5}, // 语速
                        {"vol", 7}, // 音量
                        {"per", 4}  // 发音人，4：情感度丫丫童声
                     };
            var result2 = client2.Synthesis((string)audioStr, option);
            if (result2.ErrorCode == 0)  // 或 result.Success
            {
                File.WriteAllBytes("temp.wav", result2.Data);
                g_playsound.Alarm(soundPath + "\\temp.wav");
            }
            else
            {
                MessageBox.Show(result2.ToString());
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (!isOpen)
                {
                    MessageBox.Show("摄像头未开启无法签到!");
                }
                SignControl.SendMessage(this.hHwnd, 0x41e, 0, 0);
                IDataObject obj1 = Clipboard.GetDataObject();
                if (obj1.GetDataPresent(typeof(Bitmap)))
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(searchImgAndRead));
                    thread.IsBackground = true;
                    thread.Start(obj1);
                }
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }

        
    }
}
