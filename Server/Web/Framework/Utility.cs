using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Microsoft.Xna.Framework;

namespace WebGame
{
    public static class Utility
    {
        public static Random Random = new Random();

        /// <summary>
        /// Returns the specified time measured in the number of seconds.
        /// </summary>
        /// <param name="dateTime">The System.DateTime to obtain the number of seconds from.</param>
        /// <returns>Returns the specified time measured in the number of seconds since January 1 1970 00:00:00 GMT.</returns>
        public static int UnixTimestamp(DateTime dateTime)
        {
            long initialTicks = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;
            long dateTicks = dateTime.ToUniversalTime().Ticks;
            int elapsedSeconds = System.Convert.ToInt32((dateTicks - initialTicks) / 10000000);
            return elapsedSeconds;
        }

        /// <summary>
        /// Creates a new System.DateTime from the specified timestamp (time measured in the number of seconds).
        /// </summary>
        /// <param name="unixTimestamp">The timestamp that represents the date to be created.</param>
        /// <returns>Returns a new System.DateTime instance created from the specified timestamp.</returns>
        public static DateTime FromUnixTimestamp(int unixTimestamp)
        {
            long initialTicks = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;
            long elapsedUtcTicks = System.Convert.ToInt64(unixTimestamp) * 10000000;
            elapsedUtcTicks += initialTicks;
            System.DateTime theDate = new System.DateTime(elapsedUtcTicks);//.ToLocalTime();
            return theDate;
        }

        public static string PrintTimeSpan(this TimeSpan time, bool isInPast = true)
        {
            string suffix;
            if (isInPast)
                suffix = "ago";
            else
                suffix = "left";

            if (time.Days > 365)
                return String.Format("{0} yrs, {1} days {2}", time.Days / 365, time.Days % 365, suffix);
            else if (time.Days >= 1)
                return String.Format("{0} days, {1} hrs {2}", time.Days, time.Hours, suffix);
            else if (time.Hours >= 1)
                return String.Format("{0} hrs, {1} min {2}", time.Hours, time.Minutes, suffix);
            else if (time.Minutes >= 1)
                return String.Format("{0} min, {1} sec {2}", time.Minutes, time.Seconds, suffix);
            return String.Format("{0} sec {1}", time.Seconds, suffix);
        }

        public static Vector3 Multiply(this Vector3 vector, double coefficient)
        {
            return new Vector3((float)(vector.X * coefficient), (float)(vector.Y * coefficient), (float)(vector.Z * coefficient));
        }

        private static double _NormalizeAngle(this double angle, double lowerBound, double upperBound, double range = 2 * Math.PI)
        {
            while (angle >= upperBound)
            {
                angle -= range;
            }
            while (angle < lowerBound)
            {
                angle += range;
            }
            return angle;
        }

        public static double NormalizeOrientation(this double angle)
        {
            return angle._NormalizeAngle(0, 2 * Math.PI);
        }

        public static double NormalizeTurn(this double angle)
        {
            return angle._NormalizeAngle(-Math.PI, Math.PI);
        }

        public static double Quadrance(this Vector3 vector)
        {
            return (double)vector.X * vector.X + (double)vector.Y * vector.Y + (double)vector.Z * vector.Z;
        }

        public static double Magnitude(this Vector3 vector)
        {
            return Math.Sqrt(vector.Quadrance());
        }

        public static bool ApproximatelyEqual(double left, double right, double epsilon = 0.001)
        {
            return Math.Abs(left - right) < epsilon;
        }

        //public static double NormalizeAlign(this double angle)
        //{
        //    return angle._NormalizeAngle(-Math.PI / 2, Math.PI / 2, Math.PI);
        //}
    }
}