using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Договор займа
    /// </summary>
    public class LoanContractHandler : IHandler
    {
        public LoanContractHandler()
        {

        }
        public void Handle(Loan context, string value)
        {
            context.Contract = value;
        }
    }
}
