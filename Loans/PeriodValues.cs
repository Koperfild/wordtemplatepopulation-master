using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loans
{
    /// <summary>
    /// Содержит текущее состояние займа, т.е. хранит 
    /// </summary>
    class PeriodValues
    {
        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTimeOffset DateBegin { get; set; }
        /// <summary>
        /// Дата конца периода
        /// </summary>
        public DateTimeOffset DateEnd { get; set; }
        /// <summary>
        /// 1.
        /// Сумма процентов по текущему периоду (последнему)
        /// </summary>
        public decimal CurrPercents { get; set; }
        /// <summary>
        /// 2.
        /// Текущая плата за основной долг
        /// Рассчитывается по мере изменения TotalLeftPrincipal
        /// </summary>
        public decimal CurrPrincipalPayment { get; set; }
        /// <summary>
        /// 3.
        /// Сумма процентов за все предыдущие периоды
        /// </summary>
        public static decimal PrevPercents { get; set; }
        /// <summary>
        /// 4.
        /// Сумма всех предшествовших плат за основной долг
        /// </summary>
        public static decimal PrevPrincipalPayments { get; set; }
        /// <summary>
        /// Накопленные пенни за просрочку платежей
        /// </summary>
        public decimal Pennies { get; set; }
        /// <summary>
        /// Остаток основного долга. 
        /// ПОСЛЕ ИНИЦИАЛИЗАЦИИ ВСЕХ PERIODVALUES ЭКЗЕМПЛЯРОВ ДОЛЖЕН
        /// ПРИСВАИВАТЬСЯ ЗНАЧЕНИЮ LOAN.SUMOFLOAN
        /// </summary>
        public static decimal TotalLeftPrincipal { get; set; }

        /// <summary>
        /// Fills all initial data
        /// </summary>
        /// <param name="dateBegin">dateEnd - dateBegin may be not equal to
        /// loan.IntervalOfPlannedPayments for short term loans or last period.
        /// So pass either full interval or part
        /// </param>
        /// <param name="loan"></param>
        public PeriodValues(DateTimeOffset dateBegin, DateTimeOffset dateEnd, Loan loan)
        {

            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.
            this.CurrPercents = loan.LoanSum loan.RateOfInterestPerDay * (DateEnd.Date - DateBegin.Date).Days;
            this.
        }


        /// <summary>
        /// Get conditions for all borrowers
        /// </summary>
        /// <param name="loans"></param>
        /// <returns></returns>
        public static List<PeriodValues> GetAllPeriodsValues(Loan currLoan)
        {
            List<PeriodValues> res = new List<PeriodValues>();

            //Создаём период. В static TotalLeftPrincipal пишем currLoan.LoanSum
            //Находим все currLoan.RealPayments, чья дата в интервале текущего периода
            //Делаем расчёт процентов CurrPercents, списываем TotalLeftPrincipal 

            //init some static fields
            PeriodValues.TotalLeftPrincipal = currLoan.LoanSum;
            PeriodValues.PrevPercents = 0;
            PeriodValues.PrevPrincipalPayments = 0;

            var startDate = currLoan.StartDate.Date;
            DateTimeOffset endDate;

            if (!currLoan.IntervalOfPlannedPayments.HasValue)
                //if it's short term loan
                endDate = currLoan.EndDate.Date;
            //Compare date+ interval with enddate of loan
            else if ((startDate.AddDays(currLoan.IntervalOfPlannedPayments.Value.Days)) < currLoan.EndDate)
                endDate = startDate.AddDays(currLoan.IntervalOfPlannedPayments.Value.Days);
            else
                //case of termination of loan
                endDate = currLoan.EndDate.Date;

            PeriodValues pv = new PeriodValues(startDate, endDate, currLoan);
            
            int periodDurationInDays = (pv.DateEnd - pv.DateBegin).Days;
            //different for start, middle and last periods
            pv.CurrPrincipalPayment = currLoan.LoanSum * currLoan.RateOfInterestPerDay * periodDurationInDays;
            pv.CurrPercents = PeriodValues.TotalLeftPrincipal * currLoan.RateOfInterestPerDay * periodDurationInDays;
            

            //for (int i=0;i<loans.Count;i++)
            //{
            //    var currLoan = loans[i];

            //periodValues.DateBegin = currLoan.StartDate;
            PeriodValues periodValues = new PeriodValues
            {
                
            }

            var startDate = currLoan.StartDate;
            if (currLoan.IntervalOfPlannedPayments == null)
            {
                

            }
            var endDate = startDate + currLoan.IntervalOfPlannedPayments;





            DateTimeOffset plannedPaymentDate = currLoan.StartDate;
            if (currLoan.IntervalOfPlannedPayments == null)
                SHORTTERM LOAN
            
            if (plannedPaymentDate+currLoan.IntervalOfPlannedPayments == currLoan.RealPayments.;
            var totalPrincipal = currLoan.LoanSum;
            decimal percents = 0;//Накопленные проценты
            //cycle for payments of each borrower
            for (int j=0;j<currLoan.RealPayments.Count;j++)
            {
                var currRealPayment = currLoan.RealPayments[j];
                var periodValues = new PeriodValues
                {
                    date = currRealPayment.dateOfPayment,
                         
                }
                if ()

            }



            }
        }
        private PeriodValues
    }
}
