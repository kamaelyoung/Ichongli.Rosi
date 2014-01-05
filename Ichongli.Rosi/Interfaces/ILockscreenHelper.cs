using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi.Interfaces
{
    public interface ILockscreenHelper
    {
        Task SetLockscreen(string url);
    }
}
