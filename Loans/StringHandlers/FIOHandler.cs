using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// FIO
    /// </summary>
    public class FIOHandler : IHandler
    {
        public FIOHandler()
        {

        }
        public void Handle(Loan context, string value)
        {
            context.FIO = value;
        }
    }
}
