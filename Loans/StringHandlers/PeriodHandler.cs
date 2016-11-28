using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Период указанной процентной ставки
    /// </summary>
    public class InterestPeriodHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            switch (value)
            {
                case "день":
                    context.Period = PeriodInDays.Day;
                    break;
                case "месяц":
                    context.Period = PeriodInDays.Month;
                    break;
                case "квартал":
                    context.Period = PeriodInDays.Quarter;
                    break;
                case "год":
                    context.Period = PeriodInDays.Year;
                    break;
                default:
                    throw new Exception("Incorrect period");
            }

        }
    }
}
