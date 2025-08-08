using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Shared.Helpers
{
    public static class FormattingHelper
    {

        /// <summary>
        /// Returns a number of milliseconds converted into a duration string, such as "10 min 15 sec". Includes rounding to match the behaviour of the Azure Media Player.
        /// </summary>
        /// <param name="durationInMilliseconds">The number of milliseconds.</param>
        /// <returns>The duration string.</returns>
        public static string GetDurationText(int durationInMilliseconds)
        {
            if (durationInMilliseconds > 0)
            {
                // Azure media player rounds duration to nearest second. e.g. 8:59.88 becomes 9:00. LH needs to match.
                int nearestSecond = (int)Math.Round(((double)durationInMilliseconds) / 1000);
                var duration = new TimeSpan(0, 0, nearestSecond);
                string returnValue = string.Empty;

                // If duration greater than an hour, don't return the seconds part.
                if (duration.Hours > 0)
                {
                    returnValue = $"{duration.Hours} hr {duration.Minutes} min ";

                    // Exclude "0 min" from the return value.
                    if (returnValue.EndsWith(" 0 min "))
                    {
                        returnValue = returnValue.Replace("0 min ", string.Empty);
                    }
                }
                else
                {
                    returnValue = $"{duration.Minutes} min {duration.Seconds} sec ";

                    // Exclude "0 min" and "0 sec" from the return value.
                    if (returnValue.StartsWith("0 min "))
                    {
                        returnValue = returnValue.Replace("0 min ", string.Empty);
                    }

                    if (returnValue.EndsWith(" 0 sec "))
                    {
                        returnValue = returnValue.Replace("0 sec ", string.Empty);
                    }
                }

                return returnValue;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
