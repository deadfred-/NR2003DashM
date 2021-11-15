using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NR2003DashM
{
    public class NR2003Types
    {
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct GaugeData
        {
            public float rpm;
            public float waterTemp;
            public float fuelPress;
            public float oilTemp;
            public float oilPress;
            public float voltage;
            public byte warnings;

            public static bool operator ==(GaugeData x, GaugeData y)
            {
                return (x.rpm == y.rpm &&
                        x.waterTemp == y.waterTemp &&
                        x.fuelPress == y.fuelPress &&
                        x.oilTemp == y.oilTemp &&
                        x.oilPress == y.oilPress &&
                        x.voltage == y.voltage &&
                        x.warnings == y.warnings);

            }

            public static bool operator !=(GaugeData x, GaugeData y)
            {
                return (x.rpm != y.rpm ||
                        x.waterTemp != y.waterTemp ||
                        x.fuelPress != y.fuelPress ||
                        x.oilTemp != y.oilTemp ||
                        x.oilPress != y.oilPress ||
                        x.voltage != y.voltage ||
                        x.warnings != y.warnings);

            }

        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct CurrentWeekend
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool atTrack;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string track;
            public float trackLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sessions;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string options;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct LapCrossing
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] carIdx;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] lapNum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] flags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] crossedAt;

            public static bool operator ==(LapCrossing x, LapCrossing y)
            {
                return (BitConverter.ToInt32(x.carIdx, 0) == BitConverter.ToInt32(y.carIdx, 0) &&
                        BitConverter.ToInt32(x.lapNum, 0) == BitConverter.ToInt32(y.lapNum, 0) &&
                        BitConverter.ToInt32(x.flags, 0) == BitConverter.ToInt32(y.flags, 0) &&
                        BitConverter.ToDouble(x.crossedAt, 0) == BitConverter.ToDouble(y.crossedAt, 0));
            }

            public static bool operator !=(LapCrossing x, LapCrossing y)
            {
                return (BitConverter.ToInt32(x.carIdx, 0) != BitConverter.ToInt32(y.carIdx, 0) ||
                        BitConverter.ToInt32(x.lapNum, 0) != BitConverter.ToInt32(y.lapNum, 0) ||
                        BitConverter.ToInt32(x.flags, 0) != BitConverter.ToInt32(y.flags, 0) ||
                        BitConverter.ToDouble(x.crossedAt, 0) != BitConverter.ToDouble(y.crossedAt, 0));
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct StandingsEntry
        {
            public int carIdx;     // as in DriverEntry, or -1 if invalid
            public float time;     // behind leader in race, lap time otherwise
            public int lap;        // lap number
            public int lapsLead;   // if race session, number of laps lead
            public int reasonOut;  // eReasonOut
            public int incidents;	// number of 'incidents' for this driver.
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Standings
        {
            public int flags;
            public int sessionNum;
            public float averageLapTime;
            public int lapsComplete;       // race session only
            public int numCautionFlags;    // race session only
            public int numCautionLaps;     // race session only
            public int numLeadChanges;     // race session only
            public StandingsEntry fastestLap;
            //public StandingsEntry[] position;
        }


        // You will get a SessionInfo periodically whenever a race
        // weekend is active.
        // - sessionNum is the session index (the first sessionNum is 0).
        // - sessionCookie will change whenever a session is restarted.
        // It's pretty important that you pay attention to these two.
        // Whenever the sessionNum or sessionCookie changes, you'll want
        // to clear out any information you may have cached for this
        // and any later sessions (like lap crossings and standings -
        // see below).
        // - sessionType is one of the eSessionType values above.
        // - sessionState is one of the eSessionState values above.
        // - sessionFlag is one of the eSessionFlag values above.
        // - currentTime is the current session time - the number of
        // elapsed simulation seconds since the start of the session.
        // Simulation time does not advance while the game is paused.
        // Simulation time (generally) advances more quickly than
        // real time when the sim is running in 'accelerated' time.
        // - timeRemaining reflects what is shown on the "time remaining"
        // field on the race weekend screen.  It is the number of seconds
        // remaining in the current state of the current session.  It is
        // possible that this value will be -1 immediately after a state
        // transition
        // - lapsRemaining will only be valid during sessions whose length
        // is limited to a particular number of laps (qualifying and race).
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SessionInfo
        {
            public int sessionNum;
            public int sessionCookie;  // changes if session restarted
            public int sessionType;    // eSessionType
            public int sessionState;   // eSessionState
            public int sessionFlag;    // eSessionFlag
            public float currentTime;
            public float timeRemaining;
            public int lapsRemaining;
        }

        public class UtilFunctions
        {
            public static float CelsiusToFarenheit(float celsius)
            {
                return celsius * 1.8f + 32.0f;
            }

            public static float KPAToPSI(float KPA)
            {
                return KPA * 14.5038f;
            }
        }
    }
}
