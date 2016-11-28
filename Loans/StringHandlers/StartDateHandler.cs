using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Дата займа
    /// </summary>

    public class StartDateHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            try
            {
                context.StartDate = DateTime.Parse(value).Date;
            }
            catch (Exception e)
            {
                throw new Exception("Incorrect start date");
            }
        }
    }
}
