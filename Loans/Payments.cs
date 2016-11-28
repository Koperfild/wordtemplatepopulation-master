using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans
{
    public class Payments
    {
        public DateTimeOffset dateOfPayment { get; set; }
        public decimal sum { get; set; }
        public Payments(decimal sumsOfPayments, DateTimeOffset datesOfPayments)
        {
            this.sum = sumsOfPayments;
            this.dateOfPayment = datesOfPayments;
        }
    }
}
