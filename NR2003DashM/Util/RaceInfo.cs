using System;
using System.Collections.Generic;
using System.Text;
using static NR2003DashM.NR2003Types;

namespace NR2003DashM.Util
{
    public class RaceInfo
    {
        public string DriverName { get; set; }
        public string DriverID { get; set; }
        public int CurrentLap { get; set; }
        public float LastLapTime { get; set; }
        public float BestLapTime { get; set; }
        public int BestLapNumber { get; set; }

        public GaugeData GaugeData { get; set; }

        public Standings Standings { get; set; }

        public SessionInfo SessionInfo { get; set; }
    }
}
