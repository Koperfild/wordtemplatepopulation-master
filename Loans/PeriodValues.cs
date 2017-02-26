using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loans.Exceptions;

namespace Loans
{
    /// <summary>
    /// Содержит текущее состояние займа, т.е. хранит 
    /// </summary>
    class PeriodValues
    {
        /// <summary>
        /// Платы относящиеся к текущему периоду. 
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
        /// Если заёмщик оплачивает больше нужного, то заносим лишние средства сюда. Доступны всем периодам
        /// </summary>
        public static decimal PaymentInAdvance { get; set; }

        /// <summary>
        /// Fills dates and CurrPrincipalPayment. CurrPercents is not filled
        /// </summary>
        /// <param name="dateBegin">dateEnd - dateBegin may be not equal to
        /// loan.IntervalOfPlannedPayments for short term loans or last period.
        /// So pass either full interval or part
        /// </param>
        /// <param name="loan"></param>
        public PeriodValues(DateTime dateBegin, DateTime dateEnd, Loan loan)
        {

            this.DateBegin = dateBegin.Date;
            this.DateEnd = dateEnd.Date;

            int periodTermInDays = (DateEnd - DateBegin).Duration().Days+1;//+1 т.к. считается и день выдачи и день погашения

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

            int loanTermIndays = (loan.EndDate.Date - loan.StartDate.Date).Duration().Days+1; //+1 т.к. считается и день выдачи и день погашения
            //important Спрашивать как рассчитывается сумма выплат основного долга в каждый период
            //т.к. если у всех периодов считать первый день и не считать последний, то в последнем
            //периоде дата погашения займа не будет считаться, т.е. мы потеряем 1 день оплаты основного долга
            this.CurrPrincipalPayment = loan.LoanSum * (periodTermInDays / loanTermIndays);
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
                return pvList;
            }
            else
            {
                DateTime prevTmp = currLoan.StartDate.Date;
                //-1 т.к. мы включаем первый день периода для начисления процентов. 
                var deltaTime = (int)currLoan.IntervalOfPlannedPayments.Value - 1;
                //IMPORTANT переделывать. Ибо не +deltaTime а плюсовать день/месяц/год
                DateTime currTmp = currLoan.StartDate.Date.AddDays(deltaTime);

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
                    prevTmp = currTmp.AddDays(1);
                    currTmp = prevTmp.AddDays(deltaTime);

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
                if (period.Payments == null)
                    period.Payments = new List<Loans.Payments>();
                
                period.Payments.AddRange(
                    currLoan.RealPayments.Where(x => x.dateOfPayment >= period.DateBegin && x.dateOfPayment <= period.DateEnd).OrderBy(x=>x.dateOfPayment).ToList());

                //Смотрим что у нас по платежам
                var timeInterval = period.DateEnd - period.DateBegin;
                //period.CurrPercents = currLoan.RateOfInterestPerDay * timeInterval.Days;
                PeriodValues.CurrentPrincipalPaymentsSumObligation += period.CurrPrincipalPayment;

                //Разбираемся с поступившими платежами
                //Считаем набегающие проценты
                period.CurrPercents = 0;
                DateTime prevPaymentDateTmp = period.DateBegin.Date;
                DateTime currPaymentDateTmp = period.DateBegin.Date;
                                //List<DateTimeOffset> datesOfPayments = new List<DateTimeOffset>();
                //Можно сделать по другому. Проход не между платежами а пройтись по каждому дню. Начислять проценты
                //посуточно и смотреть есть ли в текущий день платежи.
                foreach (var payment in period.Payments)
                {
                    ////Пропускаем последний платёж, чтобы рассчитать по нему погашение 2-4 пунктов
                    ////Реализовано ниже 
                    //if (payment.dateOfPayment.Date == period.DateEnd.Date)
                    //    continue;

                    //Запоминаем дату последней выплаты
                    currPaymentDateTmp = payment.dateOfPayment.Date;

                    #region Расчёт набежавших процентов до последнего платежа в периоде
                    //IMPORTANT read next
                    //Дни между платежами в данном периоде или с первого дня периода до текущего платежа (без учёта одного дня)
                    int currTimeInterval = (currPaymentDateTmp.Date - prevPaymentDateTmp.Date).Days;// (payment.dateOfPayment.Date - period.DateBegin.Date).Days;
                    //Если это первый платёж, то учитываем первый день периода
                    if (prevPaymentDateTmp.Date == period.DateBegin.Date)
                        currTimeInterval++;
                    //Начисляем проценты за дни до даты платежа
                    period.CurrPercents += currLoan.RateOfInterestPerDay* currTimeInterval * currLoan.TotalLeftPrincipal;
                    #endregion Расчёт набежавших процентов до последнего платежа в периоде

                    #region Оплата платежом имеющихся задолженностей
                    ////Если платёж превышает набежавшие проценты, то погашаем часть основного долга
                    //if (payment.sum > period.CurrPercents)
                    //{
                    //    var exceededPercentsSum = Math.Abs(period.CurrPercents - payment.sum);
                    //    period.CurrPercents = 0;
                    //    if (exceededPercentsSum <= currLoan.TotalLeftPrincipal)
                    //    {
                    //        currLoan.TotalLeftPrincipal -= exceededPercentsSum;
                    //        PeriodValues.PaymentInAdvance -= exceededPercentsSum;
                    //    }
                    //    else if (exceededPercentsSum > currLoan.TotalLeftPrincipal)
                    //    {
                    //        var exceededTotalLeftPrincipal = Math.Abs(exceededPercentsSum - currLoan.TotalLeftPrincipal);
                    //        currLoan.TotalLeftPrincipal = 0;
                    //        //Погашаем задолженности по процентам за предыдущие периоды и предыдущие долги по основному долгу
                    //        //Пункты 3 и 4
                    //        if (exceededTotalLeftPrincipal <= PeriodValues.PrevPercents)
                    //        {
                    //            PeriodValues.PrevPercents -= exceededTotalLeftPrincipal;
                    //        }
                    //        else if (exceededTotalLeftPrincipal > PeriodValues.PrevPercents)
                    //        {
                    //            var exceededPrevPercents = Math.Abs(exceededTotalLeftPrincipal - PeriodValues.PrevPercents);
                    //            PeriodValues.PrevPercents = 0;
                    //            if (exceededPrevPercents <= PeriodValues.PrevPrincipalPayments)
                    //            {
                    //                PeriodValues.PrevPrincipalPayments -= exceededPrevPercents;
                    //            }
                    //            else if (exceededPrevPercents > PeriodValues.PrevPrincipalPayments)
                    //            {
                    //                var overPayedSum = Math.Abs(exceededPrevPercents - PeriodValues.PrevPrincipalPayments);
                    //                PeriodValues.PrevPrincipalPayments = 0;
                    //                throw new Exception("WTF dude. Why you pay so much" + "OverPayment is: " + overPayedSum);
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    period.CurrPercents -= payment.sum;
                    //}
                    #endregion Оплата платежом имеющихся задолженностей

                    #region Переписанная оплата платежом имеющихся задолженностей

                    List<decimal> orderedCosts = new List<decimal>();
                    orderedCosts.Add(period.CurrPercents);
                    orderedCosts.Add(currLoan.TotalLeftPrincipal);
                    orderedCosts.Add(PeriodValues.PrevPercents);
                    orderedCosts.Add(PeriodValues.PrevPrincipalPayments);
                    int currIndexOfCosts = 0;
                    while(payment.sum >0 && currIndexOfCosts < orderedCosts.Count())
                    {
                        //Если погашаем основной долг, то в текущем периоде понижаем и 
                        if (orderedCosts[currIndexOfCosts] == currLoan.TotalLeftPrincipal)
                        {
                            //Оплачиваем п. 2 - долг по основному долгу
                            var tmp1 = Math.Min(period.CurrPrincipalPayment, payment.sum);
                            currLoan.TotalLeftPrincipal -= tmp1;
                            period.CurrPrincipalPayment -= tmp1;
                            payment.sum -= tmp1;
                            //Если это не последний день периода, то оплачиваем основной долг по максимуму
                            if (payment.dateOfPayment != period.DateEnd)
                            {
                                var tmp2 = Math.Min(currLoan.TotalLeftPrincipal, payment.sum);
                                currLoan.TotalLeftPrincipal -= tmp2;
                                payment.sum -= tmp2;
                            }
                            //IMPORTANT обсуждать момент когда переплата большая. Ведь надо пересчитывать все CurrPrincipalPayment для каждого периода
                            //PeriodValues.PaymentInAdvance += Math.
                        }
                       
                        var tmp = Math.Min(payment.sum, orderedCosts[currIndexOfCosts]);
                        payment.sum -= tmp;
                        orderedCosts[currIndexOfCosts] -= tmp;

                        currIndexOfCosts++;
                    }
                    //currIndexOfCosts - после цикла хранит либо индекс за пределами числа задолженностей либо индекс частично погашенной задолженности
                    if (orderedCosts.All(x=>x == 0) && payment.sum > 0)
                    {
                        throw new OverPaymentException("Too much money were payed");
                    }
                    #endregion Переписанная оплата платежом имеющихся задолженностей

                    prevPaymentDateTmp = currPaymentDateTmp;

                }
                //После последнего платежа надо начислить ещё проценты, начисленные до конца периода. 
                period.CurrPercents += (period.DateEnd.Date - prevPaymentDateTmp.Date).Days * currLoan.RateOfInterestPerDay * currLoan.TotalLeftPrincipal;


                //Перенос задолженностей в задолженности за прошлые периоды

                PeriodValues.PrevPercents += period.CurrPercents;



                //Производим погашение пунктов 2-4 в условленный день погашения текущего периода
                var lastPeriodDayPayment = period.Payments.LastOrDefault(x => x.dateOfPayment == period.DateEnd);//.Where(x => x.dateOfPayment == period.DateEnd);
                if (lastPeriodDayPayment != default(Payments))
                {
                    #region пункт 2
                    //Если переплаты в течение периода не обнулили всю плату по основному долгу
                    if (period.CurrPrincipalPayment > 0)
                    {
                        if (lastPeriodDayPayment.sum <= period.CurrPrincipalPayment)
                        {

                        }
                    }
                }


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
