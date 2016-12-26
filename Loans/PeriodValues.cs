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
        /// Платы относящиеся к текущему периоду
        /// </summary>
        public List<Payments> Payments = new List<Payments>();
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
        /// Текущая плата за основной долг за весь данный период
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

        #region replaced to Loan
        /// <summary>
        /// Остаток основного долга. 
        /// ПОСЛЕ ИНИЦИАЛИЗАЦИИ ВСЕХ PERIODVALUES ЭКЗЕМПЛЯРОВ ДОЛЖЕН
        /// ПРИСВАИВАТЬСЯ ЗНАЧЕНИЮ LOAN.SUMOFLOAN
        /// </summary>
        //public static decimal TotalLeftPrincipal { get; set; }
        #endregion

        /// <summary>
        /// Текущая сумма, которую надо оплатить в расчётные периоды, чтобы не начислялись пенни
        /// </summary>
        public static decimal CurrentPrincipalPaymentsSumObligation { get; set; }
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

            int timeInterval = (DateEnd - DateBegin).Days+1;//+1 т.к. считается и день выдачи и день погашения

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

            int loanTermIndays = (loan.EndDate.Date - loan.StartDate.Date).Days+1; //+1 т.к. считается и день выдачи и день погашения
            //important Спрашивать как рассчитывается сумма выплат основного долга в каждый период
            //т.к. если у всех периодов считать первый день и не считать последний, то в последнем
            //периоде дата погашения займа не будет считаться, т.е. мы потеряем 1 день оплаты основного долга
            this.CurrPrincipalPayment = loan.LoanSum * (timeInterval / loanTermIndays);
        }

        /// <summary>
        /// Fills PeriodValues with DateBegin and DateEnd according to currloan.StartDate and currloan.EndDate
        /// </summary>
        /// <param name="currLoan"></param>
        /// <returns></returns>
        private List<PeriodValues> DivideLoanIntervalIntoPeriods(Loan currLoan)
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
                //IMPORTANT переделывать. Ибо не +deltaTime а плюсовать день/месяц/год
                DateTimeOffset currTmp = currLoan.StartDate.Date + deltaTime;

                //Ориентируемся по левой части временного интервала, т.е. пока начало периода меньше конца займа
                while (prevTmp < currLoan.EndDate.Date)
                {
                    PeriodValues pv;
                    //Если стандартный интервал выплат укладывается
                    if (currTmp < currLoan.EndDate.Date)
                    {
                        pv = new PeriodValues(prevTmp, currTmp, currLoan);
                        pvList.Add(pv);
                    }
                    //Если последний интервал, то он может быть как ровно по размеры интервала выплат так и короче
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
            //Делаем расчёт процентов CurrPercents для текущего периода, 
            //Плюсуем CurrPrincipalPayment для текущего периода к CurrentPrincipalPaymentsSumObligation
            //считаем PeriodValues.CurrentPrincipalPaymentsSumObligation
            //списываем TotalLeftPrincipal 

            //init some static fields

            //currLoan.TotalLeftPrincipal = currLoan.LoanSum;
            PeriodValues.PrevPercents = 0;
            PeriodValues.PrevPrincipalPayments = 0;
            PeriodValues.CurrentPrincipalPaymentsSumObligation = 0;
            
            //Разбиваем время займа на интервалы - периоды выплат (PeriodValues)
            List<PeriodValues> periods = DivideLoanIntervalIntoPeriods(currLoan);

            foreach(var period in periods)
            {
                //Добавляем к периоду связанные платежи
                period.Payments.AddRange(
                    currLoan.RealPayments.Where(x => x.dateOfPayment >= period.DateBegin && x.dateOfPayment <= period.DateEnd).ToList());

                //Смотрим что у нас по платежам
                var timeInterval = period.DateEnd - period.DateBegin;
                //period.CurrPercents = currLoan.RateOfInterestPerDay * timeInterval.Days;
                PeriodValues.CurrentPrincipalPaymentsSumObligation += period.CurrPrincipalPayment;

                //Разбираемся с поступившими платежами
                //Считаем набегающие проценты
                period.CurrPercents = 0;
                DateTimeOffset prevPaymentDateTmp = period.DateBegin.Date;
                DateTimeOffset currPaymentDateTmp = period.DateBegin.Date;
                                //List<DateTimeOffset> datesOfPayments = new List<DateTimeOffset>();
                foreach (var payment in period.Payments)
                {
                    //Запоминаем дату последней выплаты
                    currPaymentDateTmp = payment.dateOfPayment.Date;
                    //IMPORTANT read next
                    //Дни между платежами в данном периоде или с первого дня периода до текущего платежа (без учёта одного дня)
                    int currTimeInterval = (currPaymentDateTmp.Date - prevPaymentDateTmp.Date).Days;// (payment.dateOfPayment.Date - period.DateBegin.Date).Days;
                    //Начисляем проценты за дни до даты платежа
                    period.CurrPercents += currLoan.RateOfInterestPerDay* currTimeInterval * currLoan.TotalLeftPrincipal;
                    //Если платёж превышает набежавшие проценты, то погашаем часть основного долга
                    if (payment.sum > period.CurrPercents)
                    {
                        period.CurrPercents = 0;
                        //Погашаем часть основного долга
                        currLoan.TotalLeftPrincipal -= Math.Abs(period.CurrPercents - payment.sum);
                        if (currLoan.TotalLeftPrincipal < 0)
                            throw new Exception("WTF dude. Why you pay so much");
                    }
                    else
                    {
                        period.CurrPercents -= payment.sum;
                    }

                    prevPaymentDateTmp = currPaymentDateTmp;

                }
                //После последнего платежа надо начислить ещё проценты, 
                period.CurrPercents += (period.DateEnd.Date - prevPaymentDateTmp.Date).Days * currLoan.RateOfInterestPerDay * currLoan.TotalLeftPrincipal;



                Рассчитать CurrentPrincipalPaymentsSumObligation и сравнить с PrevPrincipalPayments
            }

            //IMPORTANT fff
            //После прохода всех периодов необходимо захватить последний неучтённый день. Т.к. все CurrPercents,
            //CurrPrincipalPayment считаются с первого до предпоследнего дня.



































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
