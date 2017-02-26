using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordTemplatePopulation;
using Loans.StringHandlers;
using Loans.Exceptions;

namespace Loans
{
    /// <summary>
    /// Stores initial info about loan in processable format
    /// </summary>
    public class Loan
    {
        public static Dictionary<string, IHandler> dict = new Dictionary<string, IHandler>();
        public int RowIndex { get; set; }
        public string Contract { get; set; }
        public string FIO { get; set; }//May be splitted to 3 parts (first,last names and patronymic)
        public decimal LoanSum { get; set; }
        /// <summary>
        /// Остаток основного долга для погашения. Изначально равен LoanSum
        /// </summary>
        public decimal TotalLeftPrincipal { get; set; }
        public decimal RateOfInterestPerDay { get; set; }//is calculated based on percents and period
        /// <summary>
        /// Кол-во процентов в первоначальном виде (в день/неделю/месяц)
        /// Хранится в виде процентов, т.е. уже делённое на 100
        /// </summary>
        public decimal InitialInterestRate { get; set; }//TODO may be required to change to private
        /// <summary>
        /// Единицы измерения первичного вида процентов
        /// </summary>
        public PeriodInDays InitialInterestPeriod { get; set; }//TODO may be required to change to private
        public PeriodInDays? IntervalOfPlannedPayments { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public List<Payments> RealPayments { get; set; }
        public List<decimal> SumsOfPayments = new List<decimal>();
        public List<DateTimeOffset> DatesOfPayments = new List<DateTimeOffset>();

        /// <summary>
        /// Initialize association of column title and its handler. So data of each column is correctly stored as initial data and processed
        /// </summary>
        static Loan()
        {
            //important jjj
            //TODO Можно ввести на форме задание этих строковых констант, чтобы можно было менять название 
            //колонок, но при этом логика их обработки оставалась прежней. Т.е. можно будет переименовать
            //колонку, но данные должны быть прежними (того же типа)
            //Можно сделать это за доп плату


            //Добавляем обработчики константных строк
            Loan.AddHandler("№", new RowIndexHandler());
            Loan.AddHandler("договор займа", new LoanContractHandler());
            Loan.AddHandler("фио должника", new FIOHandler());
            Loan.AddHandler("сумма займа", new LoanSumHandler());
            //store percents as decimal ( divided by 100)
            Loan.AddHandler("процентная ставка", new PercentHandler(100m));
            Loan.AddHandler("период", new InterestPeriodHandler());
            Loan.AddHandler("интервал выплат", new IntervalOfPlannedPaymentsHandler());
            Loan.AddHandler("дата займа", new StartDateHandler());
            Loan.AddHandler("дата возврата", new EndDateHandler());
            Loan.AddHandler("дата платежа", new DatesOfPaymentsHandler());
            Loan.AddHandler("сумма платежа", new SumsOfPaymentsHandler());
        }
        /// <summary>
        /// Заполняет данные текущего займа в соответствие с indexOfLoan строкой в excelData
        /// </summary>
        /// <param name="indexOfLoan">строка в excel файле</param>
        /// <param name="excelData">Данные excel файла</param>
        /// <exception cref="NoColumnStringHandlerException"></exception>
        public Loan(int indexOfLoan, ExcelData excelData)
        {
            try
            {
                //Go through each column title
                for (int currColumnIndex = 0; currColumnIndex < excelData.ColumnTitles.Length; currColumnIndex++)
                {
                    //let through empty columns
                    if (String.IsNullOrEmpty(excelData.ColumnTitles[currColumnIndex]))
                        continue;
                    string currColumnTitle = excelData.ColumnTitles[currColumnIndex].Trim().ToLower();
                    //Delete redundant spaces in column title
                    currColumnTitle = string.Join(" ", currColumnTitle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    string currColumnValue = excelData.data[indexOfLoan, currColumnIndex];

                    IHandler handler = null;

                    //get handler for current column title
                    bool hasHandler = dict.TryGetValue(currColumnTitle, out handler);
                    if (!hasHandler)
                        throw new NoColumnStringHandlerException("Not found handler for column " + currColumnTitle);

                    if (handler != null)
                    {
                        //Handle current column title
                        handler.Handle(this, currColumnValue);
                    }
                }
                if (SumsOfPayments.Count != DatesOfPayments.Count)
                {
                    throw new InitialTableDataIncosistency("Count of sums of payment is not equal to count of dates of payments for loan:" + indexOfLoan);
                }
                //important Assumed that date data are correct and each next follows next (dates are strongly increasing)
                //DatesOfPayments.OrderBy(x => x);
                //associate sum with date of payment (both alrea
                for (int i = 0; i < SumsOfPayments.Count(); i++)
                    RealPayments.Add(new Payments(SumsOfPayments[i], DatesOfPayments[i]));
                
                //Calculate RateOfInterestPerDay
                this.RateOfInterestPerDay = this.InitialInterestRate / (decimal)this.InitialInterestPeriod;
                this.TotalLeftPrincipal = this.LoanSum;
            }
            catch (Exception e)
            {
                //important Выдавать выше объявление для его дальнейшей обработки
                throw e;
            }
        }
        public static void AddHandler(string str, IHandler handler)
        {
            dict.Add(str, handler);
        }
        //todo each parse may be paralleled
        public static List<Loan> ParseLoansFromExcelData(ExcelData excelData)
        {
            List<Loan> loans = new List<Loan>();
            try
            {
                //Число займов = числу строк в excel (без заголовков)
                for (int i = 0; i < excelData.data.GetLength(0); i++)
                    loans[i] = new Loan(i, excelData);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return loans;
        }
    }
}

        //    #region deleted code
        //    string[] dateOfPaymentStrings =  {
        //        "дата".ToLower(),//Т.к. в exceldata всё в ToLower()
        //        "платежа".ToLower()
        //    };

        //    //Find index of 1st occurence of dates and sums of payments. If there is no such cells in excel it is short-tem loan
        //    //-1 if not found
        //    int index = Array.FindIndex(excelData.ColumnTitles, x =>
        //    {
        //        bool entry = true;
        //        foreach (var s in dateOfPaymentStrings)
        //        {
        //            entry = entry && x.Contains(s);
        //        }
        //        return entry;
        //    }
        //    );

        //    #region Parse dates and sums of expected payments
        //    //TODO Make a separate method MakeListPayments(int startColumnIndex, ExcelData excelData)
        //    List<Payments> payments = new List<Payments>();
        //    //If there are these columns in table
        //    //If index = -1 then something is wrong in input data (no payment data)
        //    if (index == -1)
        //    {

        //        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //        throw new Exception("something is wrong in input data (no payment data)");
        //        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //    }
        //    //Get list of payments (date and sum)
        //    else if (index >= 0)
        //    {
        //        try
        //        {
        //            //Go through first occurence of date of payment till the end of the columns
        //            for (int k = index; k < excelData.ColumnTitles.Length; k += 2)//+2 because 1 Payment is associated with 2 columns
        //            {
        //                //Метод DateTimeOffset.Parse (String) msdn
        //                payments.Add(new Payments
        //                {
        //                    //TODO Make user guide for correct format of data in this field
        //                    dateOfPayment = DateTimeOffset.Parse(excelData.data[i, k]),
        //                    sum = decimal.Parse(excelData.data[i, k + 1])
        //                }
        //                );
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Ошибка в формате даты или суммы выплаты. Измените excel файл и попробуйте снова.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}
        //#endregion
        //#endregion
