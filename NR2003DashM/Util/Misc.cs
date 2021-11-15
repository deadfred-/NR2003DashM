using System;
using System.Collections.Generic;
using System.Text;

namespace NR2003DashM.Util
{
    public static class Misc
    {
        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }
    }
}
