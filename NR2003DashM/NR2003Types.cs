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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public StandingsEntry[] position;
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

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct PitStop
        {
            public struct tire // LF, RF, LR, RR        
            {
                public bool changed;   // Was this tire changed?
                public float pressure; // (cold, gauge) tire pressure (PSI)
            }
            public float wedge;        // cross weight (lbs - positive = more RF<->LR)
            public float grilleTape;   // percentage of air inlet area taped up

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public float[] trackBar;// track bar height (inches LR, RR)
            public float fuel;     // gallons
        }

        //------------------------------------------------------------------------
        // World coordinates:
        //		- right handed, with +z = up
        // From the driver's point of view sitting at the center of gravity of the car,..
        // Car coordinates:
        //		- right handed, with +z = up, +x = towards nose, +y towards left side
        //		- +yaw rotates the +x axis into the +y axis
        //		- +pitch rotates the +z axis into the +x axis
        //		- +roll rotates the +y axis into the +z axis
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct ChassisData
        {
            public float trkPct;           // Percentage of way around the track
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] r;         // position of car c.o.g. [x,y,z] (meters)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] v;         // velocity [x,y,z] (meters/sec)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] q;         // orientation [yaw,pitch,roll] (radians)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] w;         // angular velocity [yaw,pitch,roll] (radians/sec)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] N;         // unit normal to track surface [x,y,z]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public float[] H;         // unit track heading vector [x,y]
            public float steerT;			// torque on steering shaft (N*m, +=clockwise)
        }
        // NOTE:
        // For the chassis orientation, yaw is applied first, then pitch, then roll.
        // You can generate a 3x3 rotation matrix from car coordinates to world
        // coordinates from ChassisData.q as follows...
        //		- Let the prefix 'c' mean cos(x), and the prefix 's' mean sin(x)
        //		- Let the suffix 'y' mean yaw, 'p' mean pitch, and 'r' mean roll
        // so that, for example, 'cp' means cos(pitch), or cos(ChassisData.q[1]).
        // Then...
        //		m[0][0] = cp*cy;
        //		m[0][1] = cy*sp*sr - cr*sy;
        //		m[0][2] = cr*cy*sp + sr*sy;
        //		m[1][0] = cp*sy;
        //		m[1][1] = cr*cy + sp*sr*sy;
        //		m[1][2] = cr*sp*sy - cy*sr;
        //		m[2][0] = -sp;
        //		m[2][1] = cp*sr;
        //		m[2][2] = cp*cr;
        // Multiplying a vector by this matrix will rotate it from the car's
        // coordinate system into the world coordinate system.  Multiplying
        // a vector by the transpose of this matrix goes the other way.
        //
        // See the sample program for an example.  It computes lateral,
        // longitudinal, and normal G loads from (sequences of) this data.

        //------------------------------------------------------------------------
        // See the comments for ChassisData for details of these coordinate values.
        // Telemetry of this type is for all (available) cars except the local
        // player's car. That is, this telemetry will include the pace car,
        // AI cars, and remote player cars.  The pointer returned by
        // AppGetSimData(kOpponentCarData) will be to an array of these structs.
        // Use (AppGetSimDataSize(kOpponentCarData)/sizeof(OpponentCarData)) to
        // determine the number of valid elements in the array.
        // If you are collecting this telemetry from a client in a multiplayer
        // race, you should be aware that, due to bandwidth limitations between
        // the client and the server, you are likely to only receive this telemetry
        // for a subset of the cars participating in the race.
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct OpponentCarData
        {
            public int carIdx;
            public float trkPct;       // Percentage of way around the track
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] r;     // position of car c.o.g. [x,y,z] (meters)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] v;     // velocity [x,y,z] (meters/sec)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] q;		// orientation [yaw,pitch,roll] (radians)
        }
        
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct DriverInput
        {
            public float throttle; // Percent applied
            public float brake;        // Percent applied
            public float clutch;       // Percent released (1 => foot off clutch)
            public float steer;        // radians at lf wheel (no ackerman)
            public int gear;		// reverse = <0, neutral = 0, fwd gear = >0
        }

        //------------------------------------------------------------------------
        // You will get a DriverEntry each time a "driver" joins the race.
        // Drivers can be either human players, or computer controlled
        // cars (AI).  AI drivers are so flagged in the driverFlags field.
        // The pace car is also registered as a driver, and is also flagged.
        // Any other telemetry that refers to a specific car or driver
        // will use an integer index that matches the carIdx field of
        // the DriverEntry.
        // carIdx's can range from 0..(kMaxCars-1), inclusive.
        // kCarIdxInvalid is used as an 'invalid' car index.
        public enum eDriveRFlags
        {
            kDriverIsPaceCar = 0x00000001,
            kDriverIsAI = 0x00000002,
            kDriverIsBoss = 0x00000004,
            kDriverIsAdmin = 0x00000008
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct DriverEntry
        {
            public eDriveRFlags flags;      // bitwise or of eDriverFlags
            public int carIdx;     // index (used in lap crossings, ...)
            public int carMake;    // 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] carNum; // Painted on car
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public char[] name;  // driver name
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] carFile;// .car file name
        }

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

        public static IntPtr GetIntPtrFromStructArray<T>(T[] array) where T : new()
        {
            int arrayLen = array.Length;
            int structSize = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocCoTaskMem(arrayLen * structSize);
            for (int i = 0; i < arrayLen; i++)
                Marshal.StructureToPtr(array[i], (IntPtr)(ptr.ToInt32() + i * structSize), false);
            return ptr;
        }
        public static T[] GetStructArrayFromIntPtr<T>(IntPtr ptr, int len) where T : new()
        {            
            int structSize = Marshal.SizeOf(typeof(T));
           // len = structSize;
            T[] array = new T[len];
            IntPtr ptrPos = ptr;
            for (int i = 0; i < len; i++)
            {
                array[i] = new T();
                //Marshal.PtrToStructure(ptrPos, array[i]);                
                array[i] = Marshal.PtrToStructure<T>(ptrPos);
                Marshal.DestroyStructure(ptrPos, typeof(T));
                ptrPos = (IntPtr)(ptrPos.ToInt32() + structSize);
            }
            //Marshal.FreeCoTaskMem(ptr);
            return array;
        }
    }
}

