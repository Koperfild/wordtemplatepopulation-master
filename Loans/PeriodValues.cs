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
        /// Плата вперёд.
        /// Если заёмщик оплачивает больше нужного, то заносим лишние средства сюда.
        /// </summary>
        public decimal PaymentInAdvance { get; set; }

        /// <summary>
        /// Fills dates and CurrPrincipalPayment. CurrPercents is not filled
        /// </summary>
        /// <param name="dateBegin">dateEnd - dateBegin may be not equal to
        /// loan.IntervalOfPlannedPayments for short term loans or last period.
        /// So pass either full interval or part
        /// </param>
        /// <param name="loan"></param>
        public PeriodValues(DateTimeOffset dateBegin, DateTimeOffset dateEnd, Loan loan)
        {

            this.DateBegin = dateBegin.Date;
            this.DateEnd = dateEnd.Date;

            int timeInterval = (DateEnd - DateBegin).Days;

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            Эта штука будет рассчитываться исходя из loan.realPayments;
                //important jjjj
            ////this.CurrPercents = loan.RateOfInterestPerDay * timeInterval * PeriodValues.TotalLeftPrincipal;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            int loanTermIndays = (loan.EndDate.Date - loan.StartDate.Date).Days;
            //important Спрашивать как рассчитывается сумма выплат основного долга в каждый период
            this.CurrPrincipalPayment = loan.LoanSum * (timeInterval / loanTermIndays);
        }

        /// <summary>
        /// Fills PeriodValues with DateBegin and DateEnd according to currloan.StartDate and currloan.EndDate
        /// </summary>
        /// <param name="currLoan"></param>
        /// <returns></returns>
        private List<PeriodValues> InitPeriodValuesWithDatesAndCurrPrincipalPayment(Loan currLoan)
        {
            List<PeriodValues> pvList = new List<PeriodValues>();

            //If it's short term loan then we create only 1 periodValues
            if (!currLoan.IntervalOfPlannedPayments.HasValue)
            {//if it's short term loan
                PeriodValues pv = new PeriodValues(currLoan.StartDate.Date, currLoan.EndDate.Date, currLoan);
                pvList.Add(pv);
            }
            else
            {
                DateTimeOffset prevTmp = currLoan.StartDate.Date;
                var deltaTime = TimeSpan.FromDays(currLoan.IntervalOfPlannedPayments.Value.Days);
                DateTimeOffset currTmp = currLoan.StartDate.Date + deltaTime;

                while (prevTmp < currLoan.EndDate.Date)
                {
                    PeriodValues pv;
                    if (currTmp < currLoan.EndDate.Date)
                    {
                        pv = new PeriodValues(prevTmp, currTmp, currLoan);
                        pvList.Add(pv);
                    }
                    else
                    {
                        pv = new PeriodValues(prevTmp, currLoan.EndDate.Date,currLoan);
                        pvList.Add(pv);
                    }
                    prevTmp = currTmp;
                    currTmp += deltaTime;

                }
            }
            return pvList;
        }
        private class 
        /// <summary>
        /// Initialize and fills data for all periods of loan
        /// </summary>
        /// <param name="loans">Current loan</param>
        /// <returns></returns>
        public List<PeriodValues> GetAllPeriodsValuesForLoan(Loan currLoan)
        {
            List<PeriodValues> res = new List<PeriodValues>();

            //Создаём период. В static TotalLeftPrincipal пишем currLoan.LoanSum
            //Находим все currLoan.RealPayments, чья дата в интервале текущего периода
            //Делаем расчёт процентов CurrPercents, списываем TotalLeftPrincipal 

            //init some static fields
            PeriodValues.TotalLeftPrincipal = currLoan.LoanSum;
            PeriodValues.PrevPercents = 0;
            PeriodValues.PrevPrincipalPayments = 0;

            List<PeriodValues> pvs = InitPeriodValuesWithDatesAndCurrPrincipalPayment(currLoan);
            var q = pvs.GroupJoin(currLoan.RealPayments, x => new { x.DateBegin, x.DateEnd }, x => new { DateBegin = x.dateOfPayment, DateEnd =x.dateOfPayment }, (x, y) => new { PeriodValue = x, RealPayments = y },
                new IEqualityComparer(var x,var y) => y.dateOfPayment > x.DateBegin && y.dateOfPayment <= x.DateEnd);

            //startdate и endDate пока не нужны
            var startDate = currLoan.StartDate.Date;
            DateTimeOffset endDate;

            var q =  currLoan.RealPayments

            //If it's short term loan then we create only 1 periodValues
            if (!currLoan.IntervalOfPlannedPayments.HasValue)
            {//if it's short term loan
                PeriodValues period = new PeriodValues()
                endDate = currLoan.EndDate.Date;


                Fill this Period with data

                res.Add(jdjdjdjdj);
            }
            //Else we iterate with currLoan.IntervalOfPlannedPayments step
            else
            {
                for (var endPerioDate = startDate;endPerioDate<endDate) 
            
            //Проверка последнего периода, т.к. он может быть короче currLoan.IntervalOfPlannedPayments
            //Compare date+ interval with enddate of loan
            else if ((startDate.AddDays(currLoan.IntervalOfPlannedPayments.Value.Days)) < currLoan.EndDate)
                    endDate = startDate.AddDays(currLoan.IntervalOfPlannedPayments.Value.Days);
                else
                    //case of termination of loan
                    endDate = currLoan.EndDate.Date;
            }


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
