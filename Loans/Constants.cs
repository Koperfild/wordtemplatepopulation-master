using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans
{
    public class Constants
    {
        public static Dictionary<PeriodInDays, TimeSpan> PeriodDict = new Dictionary<PeriodInDays, TimeSpan>();
        public static int Day = 1;
        public static int Month = 30;
        public static int Quarter = 90;
        public static int Year = 366;

        static Constants()
        {
            PeriodDict.Add(PeriodInDays.Day, TimeSpan.FromDays(1));
            PeriodDict.Add(PeriodInDays.Month, TimeSpan.FromDays(30));
            PeriodDict.Add(PeriodInDays.Quarter, TimeSpan.FromDays(90));
            PeriodDict.Add(PeriodInDays.Year, TimeSpan.FromDays(366));
        }
    }
}
