using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Добавление даты фактического платежа
    /// </summary>
    public class DatesOfPaymentsHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            if (!String.IsNullOrEmpty(value))
                context.DatesOfPayments.Add(DateTime.Parse(value));
        }
    }
}
