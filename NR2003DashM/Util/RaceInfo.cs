﻿using System;
using System.Collections.Generic;
using System.Text;
using static NR2003DashM.NR2003Types;
using System.Linq;

namespace NR2003DashM.Util
{
    public class RaceInfo
    {
        public int SessionCookie { get; set; }
        public int SessionID { get; set; }
        public string DriverName { get; set; }
        public string DriverID { get; set; }
        public int CurrentLap { get; set; }
        public float LastLapTime { get; set; }
        public float BestLapTime { get; set; }
        public int BestLapNumber { get; set; }

        public GaugeData GaugeData { get; set; }

        public Standings Standings { get; set; }

        /// <summary>
        /// Because we have to clear our cache (per doc) when session num or cookie change, we clear the data in the setter.
        /// </summary>
        public SessionInfo SessionInfo
        {
            get
            {
                return SessionInfo;
            }
            set
            {

                if (value.sessionCookie != this.SessionCookie || value.sessionNum != this.SessionID)
                {
                    // reset values because our session chnaged.
                    this.LapCrossings = new List<LapCrossing>();
                    this.OpponentCarDatas = new List<OpponentCarData>();
                    this.PitLaps = new List<int>();
                    this.GaugeData = new GaugeData();
                    this.Standings = new Standings();

                }
                
                this.SessionInfo = value;

                this.SessionCookie = value.sessionCookie;
                this.SessionID = value.sessionNum;

            }
        }

        public DriverInput DriverInput { get; set; }

        public List<int> PitLaps { get; set; }

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
            int LastPitLap = 0;
            try
            {
                if (this.PitLaps.Count > 0)
                {
                    LastPitLap = this.PitLaps.Last();
                }

            }
            catch (Exception ex)
            {

            }
            return CurrentLap - LastPitLap;
        }

    }

}
