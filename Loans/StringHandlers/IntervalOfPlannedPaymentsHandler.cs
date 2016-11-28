using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Интервал выплат
    /// </summary>
    public class IntervalOfPlannedPaymentsHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            //TODO смотреть норм ли делать IntervalOfPlannedPayments?  nullable
            if (value == null)
                context.IntervalOfPlannedPayments = null;
            else
            {
                switch (value)
                {
                    case "день":
                        context.Period = PeriodInDays.day;
                        break;
                    case "месяц":
                        context.Period = PeriodInDays.month;
                        break;
                    case "год":
                        context.Period = PeriodInDays.year;
                        break;
                    default:
                        throw new Exception("Incorrect period");
                }
            }
        }
    }
}
