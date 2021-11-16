using System;
using System.Collections.Generic;
using System.Text;
using static NR2003DashM.NR2003Types;
using System.Linq;

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

        public DriverInput DriverInput { get; set; }

        public List<int> PitLaps { get; set; }

        public List<OpponentCarData> OpponentCarDatas { get;set;}

        public RaceInfo()
        {
            this.PitLaps = new List<int>();
            this.OpponentCarDatas = new List<OpponentCarData>();
        }

        public int GetLapsSinceLastPit()
        {
            int LastPitLap = 0;
            try
            {
                if (this.PitLaps.Count > 0)
                {                    
                    LastPitLap = this.PitLaps.Last();
                }
                
            } catch (Exception ex)
            {

            }
            return CurrentLap - LastPitLap;
        }

    }    
    
}
