using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//用来加提示音，要用音箱
using System.Threading;

namespace FaceCheck
{
    public class CPlaySound
    {
        protected const int SND_SYNC = 0x0;
        protected const int SND_FILENAME = 0x20000;
        protected Queue<string> m_queueAlarm = new Queue<string>();//存放警示音的队列

        [DllImport("winmm.dll", CharSet = CharSet.Auto, EntryPoint = "PlaySound")]
        private static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);       //调用此函数来播放声音
        public CPlaySound() { }
        public Queue<string> Flush()//将队列中的值赋给另一个新建队列，只要在赋值的时候锁一下就可以了，缩短时间。（小白开始在遍历刷新的时候一直锁着资源，就会卡界面~）
        {
            lock (m_queueAlarm)
            {
                Queue<string> ret = m_queueAlarm;
                m_queueAlarm = new Queue<string>();
                return ret;
            }
        }
        public void Alarm(string s_musicname)//将音乐文件入队列，也只要如队列的时候锁一下资源就可以了
        {
            lock (m_queueAlarm)
            {
                m_queueAlarm.Enqueue(s_musicname);
            }
        }
        public void StartThred()//程序最开始运行的时候，创建线程，然后就一直运行着，直到软件关闭
        {
            if (SignControl.m_thread != null) return;

            SignControl.m_thread = new Thread(new ThreadStart(SignControl.g_playsound.Run));//创建线程
            SignControl.m_thread.IsBackground = true;//线程会随着主线程的关闭而关闭
            SignControl.m_thread.Start();//线程开启
        }
        public void Run()//这是被外部真正调用一直用来运行的函数
        {
            while (true)
            {
                Queue<string> queue = Flush();      //取队列 
                
                if (queue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                while (queue.Count > 0)
                {
                    PlaySound(queue.Dequeue(), IntPtr.Zero, SND_FILENAME | SND_SYNC);//播放声音
                }
            }
        }
    }
}
