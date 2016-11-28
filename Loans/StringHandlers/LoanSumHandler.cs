using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Сумма займа
    /// </summary>
    public class LoanSumHandler : IHandler
    {
        public LoanSumHandler()
        {

        }
        public void Handle(Loan context, string value)
        {
            //запомнить
            context.LoanSum = Convert.ToDecimal(value.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
        }
    }
}
