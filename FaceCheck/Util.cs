using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceCheck
{
    public class Util
    {
        static public string soundPath = System.Windows.Forms.Application.StartupPath;

        public static string GenerateStringID()

        {

            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())

            {

                i *= ((int)b + 1);

            }

            return string.Format("{0:x}", i - DateTime.Now.Ticks);

        }
    }
}
