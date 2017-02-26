using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.Exceptions
{
    class InitialTableDataIncosistency : Exception
    {
        public InitialTableDataIncosistency(string message) : base(message)
        {
        }
    }
}
