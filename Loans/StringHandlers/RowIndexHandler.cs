using Loans.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Handler номера строки из изначальной таблицы Excel
    /// </summary>
    public class RowIndexHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            if (!String.IsNullOrEmpty(value))
                try
                {
                    context.RowIndex = int.Parse(value);
                }
                catch(Exception)
                {
                    throw new IncorrectRowIndexException("Can't parse row index");
                }
        }
    }
}
