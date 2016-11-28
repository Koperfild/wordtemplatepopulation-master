using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Добавление фактической выплаты в список
    /// </summary>

    public class SumsOfPaymentsHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            //Check for null and emptiness as there can be empty cells at the end cells (each loan has different count of payments (sum+date)
            if (!String.IsNullOrEmpty(value))
                context.SumsOfPayments.Add(Convert.ToDecimal(value.Trim().Replace(',', '.'), CultureInfo.InvariantCulture));
        }
    }
}
