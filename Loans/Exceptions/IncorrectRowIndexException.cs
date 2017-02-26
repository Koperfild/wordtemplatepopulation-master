using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.Exceptions
{
    class IncorrectRowIndexException : Exception
    {
        public IncorrectRowIndexException(string message) : base(message)
        {
        }
    }
}
