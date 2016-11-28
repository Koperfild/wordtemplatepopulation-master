using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{

    /// <summary>
    /// Процентная ставка
    /// </summary>
    public class PercentHandler : IHandler
    {
        private decimal _p;
        public PercentHandler(decimal p)
        {
            _p = p;
        }
        public void Handle(Loan context, string value)
        {
            var onlyNumbers = value.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            context.InitialInterestRate = Convert.ToDecimal(onlyNumbers) / _p;
        }
    }
}
