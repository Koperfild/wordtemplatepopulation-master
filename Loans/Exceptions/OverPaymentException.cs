using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.Exceptions
{
    public class OverPaymentException:Exception
    {
        public OverPaymentException(string message) : base(message)
        {
        }
    }
}
