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
        private int _CurrentLap;
        public int CurrentLap
        {
            get
            {
                if (this.LapCrossings.Count > 0)
                {
                    // get the most recent crossing where our carIDx = 0 (our car)
                    var laps = LapCrossings.Where(x => BitConverter.ToInt32(x.carIdx) == 0).ToList();
                    if (laps.Count > 0)
                    {
                        _CurrentLap = BitConverter.ToInt32(laps.Last().lapNum);
                    }
                    
                }
                return _CurrentLap;
            }
            // no set for this.
        }
        public float LastLapTime { get; set; }
        public float BestLapTime { get; set; }
        public int BestLapNumber { get; set; }

        public GaugeData GaugeData { get; set; }

        public Standings Standings { get; set; }

        private SessionInfo _sessionInfo;
        /// <summary>
        /// Because we have to clear our cache (per doc) when session num or cookie change, we clear the data in the setter.
        /// </summary>
        public SessionInfo SessionInfo
        {
            get
            {
                return this._sessionInfo;
            }
            set
            {

                if (value.sessionCookie != this._sessionInfo.sessionCookie || value.sessionNum != this._sessionInfo.sessionNum)
                {
                    // reset values because our session chnaged.
                    this.LapCrossings = new List<LapCrossing>();
                    this.OpponentCarDatas = new List<OpponentCarData>();
                    this.PitLaps = new List<int>();
                    this.GaugeData = new GaugeData();
                    this.Standings = new Standings();

                }

                this._sessionInfo = value;

            }
        }

        public DriverInput DriverInput { get; set; }

        public List<int> PitLaps { get; set; }
        public int LastPitLap { get; set; }

        public List<OpponentCarData> OpponentCarDatas { get; set; }

        public List<LapCrossing> LapCrossings { get; set; }
        public RaceInfo()
        {
            this.PitLaps = new List<int>();
            this.OpponentCarDatas = new List<OpponentCarData>();
            this.LapCrossings = new List<LapCrossing>();
        }

        public int GetLapsSinceLastPit()
        {            
            return this.CurrentLap - this.LastPitLap;
        }

    }

}
