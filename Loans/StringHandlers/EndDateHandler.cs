using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans.StringHandlers
{
    /// <summary>
    /// Дата окончания договора
    /// </summary>
    public class EndDateHandler : IHandler
    {
        public void Handle(Loan context, string value)
        {
            try
            {
                context.EndDate = DateTime.Parse(value).Date;
            }
            catch (Exception e)
            {
                throw new Exception("Incorrect end date");
            }
        }
    }
}
