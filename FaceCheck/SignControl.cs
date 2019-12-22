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
//using AForge
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Controls;

namespace FaceCheck
{
    public partial class SignControl : UserControl
    {
        public static SignControl form;
        //播放声音单独线程
        static public Thread m_thread = null;
        static public CPlaySound g_playsound = new CPlaySound();

        Tts client2 = null;
        Face client = null;

        private FilterInfoCollection videoDevices;  //摄像头设备
        private VideoCaptureDevice videoSource;     //视频的来源选择
        private VideoSourcePlayer videoSourcePlayer;    //AForge控制控件
        private VideoFileWriter writer;     //写入到视频
        System.Timers.Timer timer_count;
        int tick_num = 0;


        private bool isOpen = true;//摄像头是否打开
        
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
        /// <summary>保存图片框的句柄</summary>
        private IntPtr pbHWND;
        /// <summary>临时图片，用于保存到视频</summary>
        private Bitmap tmpBmp;
        
        public SignControl(Face clientAll, Tts client2All)
        {
            try
            {
                InitializeComponent();
                client = clientAll;
                client2 = client2All;

                g_playsound.StartThred();

                pbHWND = pictureBox1.Handle;
                tmpBmp = new Bitmap(640, 480);
                openCapture();

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
            form = this;
        }
        private void openCapture()
        {
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.videoSource = new VideoCaptureDevice();
            this.writer = new VideoFileWriter();
            //设置视频来源
            try
            {
                // 枚举所有视频输入设备
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();   //没有找到摄像头设备

                //foreach (FilterInfo device in videoDevices)
                //{
                //    this.comboBox_camera.Items.Add(device.Name);
                //}
                //this.comboBox_camera.SelectedIndex = 0;   //注释掉，选择摄像头来源的时候才会才会触发显示摄像头信息

                int selected_index = 0;
                this.videoSource = new VideoCaptureDevice(videoDevices[selected_index].MonikerString);
                // set NewFrame event handler
                videoSource.NewFrame += new NewFrameEventHandler(show_video);
                videoSource.Start();
                videoSourcePlayer.VideoSource = videoSource;
                videoSourcePlayer.Start();
                isOpen = true;
            }
            catch (ApplicationException)
            {
                videoDevices = null;
                MessageBox.Show("没有找到摄像头设备", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            tick_num = 0;
            //秒表
            this.timer_count = new System.Timers.Timer();   //实例化Timer类，设置间隔时间为10000毫秒；
            this.timer_count.Elapsed += new System.Timers.ElapsedEventHandler(tick_count);   //到达时间的时候执行事件；
            this.timer_count.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；
            this.timer_count.Interval = 1000;

        }
        
        //新帧的触发函数
        private void show_video(object sender, NewFrameEventArgs eventArgs)
        {
            
            //Bitmap bitmap = eventArgs.Frame;    //获取到一帧图像
            //pictureBox1.Image = Image.FromHbitmap(bitmap.GetHbitmap());
            Graphics g = Graphics.FromHwnd(pbHWND);
            g.DrawImage(eventArgs.Frame, 0, 0, eventArgs.Frame.Width, eventArgs.Frame.Height);
            g.Dispose();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                openCapture();
                
            }catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
}

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                closeCapture();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }
        public void closeCapture()
        {
            isOpen = false;
            //停止视频
            this.videoSource.SignalToStop();
            this.videoSource.WaitForStop();
            this.videoSourcePlayer.SignalToStop();
            this.videoSourcePlayer.WaitForStop();
            pictureBox1.Image = null;
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
            Image image1 = (Image)(obj1);
            var imageId = Util.GenerateStringID();
            if (!Directory.Exists(Util.soundPath + "\\data\\image\\"))
            {
                Directory.CreateDirectory(Util.soundPath + "\\data\\image\\");
            }
            string imageUrl = Util.soundPath + "\\data\\image\\" + imageId + ".jpg";
            image1.Save(imageUrl);
            var image = Convert.ToBase64String(File.ReadAllBytes(imageUrl));
            var imageType = "BASE64";
            List<string> groupList = FaceLibControl.groupList;
            if (groupList.Count==0)
            {
                MessageBox.Show("没有启用的用户组");
                return;
            }
            StringBuilder stb = new StringBuilder();
            for(int i=0;i< groupList.Count; i++)
            {
                if (groupList.Count==i+1)
                {
                    stb.Append(groupList[i]);
                }
                else
                {
                    stb.Append(groupList[i] + ",");
                }
            }
            var options = new Dictionary<string, object>{
                {"match_threshold", 80},
            };
            var result = client.Search(image, imageType, stb.ToString(), options);
            string audioStr = null;
            
            if (result.GetValue("error_code").ToString() == "0")
            {
                var userList = result.GetValue("result")["user_list"][0];
                var userInfo = userList["user_info"];
                var userId = userList["user_id"];
                //根据userId查询是否签到成功
                //设置签到状态
                Boolean isSign = Form1.form.AddSignNum(userId.ToString());
                if (isSign)
                {
                    audioStr = "签到成功，欢迎" + userInfo;
                }
                else
                {
                    audioStr = userInfo + "您已签到过，请勿重复签到";
                }
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
                g_playsound.Alarm(Util.soundPath + "\\temp.wav");
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
                if (this.videoSource.IsRunning && this.videoSourcePlayer.IsRunning)
                {
                    Bitmap bitmap = this.videoSourcePlayer.GetCurrentVideoFrame();
                    Thread thread = new Thread(new ParameterizedThreadStart(searchImgAndRead));
                    thread.IsBackground = true;
                    thread.Start(bitmap);
                }
                else
                {
                    MessageBox.Show("摄像头没有运行", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }

        //计时器响应函数
        public void tick_count(object source, System.Timers.ElapsedEventArgs e)
        {
            tick_num++;
        }
    }
}
