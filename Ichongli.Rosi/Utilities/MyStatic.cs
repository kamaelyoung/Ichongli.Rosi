using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Utilities
{
    public class MyStatic
    {
        public static string strcolor = "";
        public static ProgressIndicator pi;

        static MyStatic()
        {
        }

        public static void WaitState(string str, double opacity)
        {
            try
            {
                if (MyStatic.pi != null)
                    MyStatic.pi = (ProgressIndicator)null;
                MyStatic.pi = new ProgressIndicator();
                SystemTray.ProgressIndicator = (MyStatic.pi);
                SystemTray.Opacity = (opacity);
                MyStatic.pi.Text = (str);
                MyStatic.pi.IsIndeterminate = (true);
                MyStatic.pi.IsVisible = (true);
            }
            catch
            {
            }
        }

        public static void Cancel()
        {
            try
            {
                SystemTray.Opacity = (0.0);
                MyStatic.pi.IsVisible = (false);
            }
            catch
            {
            }
        }

        public static string GetMouth(int num)
        {
            return new Dictionary<int, string>()
      {
        {
          1,
          "January"
        },
        {
          2,
          "February"
        },
        {
          3,
          "march"
        },
        {
          4,
          "April"
        },
        {
          5,
          "May"
        },
        {
          6,
          "June"
        },
        {
          7,
          "July"
        },
        {
          8,
          "August"
        },
        {
          9,
          "September"
        },
        {
          10,
          "October"
        },
        {
          11,
          "November"
        },
        {
          12,
          "December"
        }
      }[num];
        }
    }
}
