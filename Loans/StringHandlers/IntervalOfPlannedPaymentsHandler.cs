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
                        context.IntervalOfPlannedPayments = PeriodInDays.Day;
                        break;
                    case "месяц":
                        context.IntervalOfPlannedPayments = PeriodInDays.Month;
                        break;
                    case "год":
                        context.IntervalOfPlannedPayments = PeriodInDays.Year;
                        break;
                    default:
                        context.IntervalOfPlannedPayments = null;
                        throw new Exception("Incorrect period");
                        break;
                }
            }
        }
    }
}
