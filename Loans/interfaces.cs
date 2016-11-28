using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans
{
    public interface IHandler
    {
        void Handle(Loan context, string value);
    }
}
