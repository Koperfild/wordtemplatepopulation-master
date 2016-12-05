using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using WordTemplatePopulation;
using System.IO;
using Loans.StringHandlers;

namespace Loans
{
    public partial class Form1 : Form
    {
        private string _percents = "%";
        private string _penny = "penny";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var excelFilePath = WordTemplatePopulation.ExcelWork.ChooseFile();
            var excelData = WordTemplatePopulation.ExcelWork.ReadExcelFile(this, excelFilePath);

            if (excelData == null)
                return;
            
            var loans = Loan.ParseLoansFromExcelData(excelData);

            //Check excel data for each loan (each row)



            //calculation logic
            DateTimeOffset
        }

        

        //TODO делать формирование % за пользование займом, изменение основного займа (долга) и начисление пенни. Всё это зависит от выплат. Смотреть как реализовать это
        //TODO Выделить в отдельный файл, изменить название
        public void Foo(Form caller, string maskSymbol, ExcelData excelData, string wordFilePath, string dirPath)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = false;
            Word.Document WordDoc = null;

            //Cycle for rows - each row - new document
            for (int i = 0; i < excelData.data.GetLength(0); i++)
            {
                string newFilePath = dirPath + "\\" + (i + 1) + ".docx";
                File.Copy(wordFilePath, newFilePath);
                WordDoc = WordApp.Documents.Open(newFilePath);

                //string replace = WordDoc.Content.Text;

                //Word.Range rng = WordDoc.Content;
                //dynamic style = rng.get_Style();

                Word.Range rng = WordDoc.Content;
                rng.Find.ClearFormatting();
                rng.Find.Forward = true;
                rng.Find.Replacement.ClearFormatting();
                rng.Find.MatchCase = false;


                #region Fullfill LoanDetails Changes (Paste after inserting of masked strings

                #region The same as further but tried another way
                //int varIndex = 0; //Индекс первой колонки в excelData, содержащей "Дата платежа"
                //bool entry = true;
                //foreach (var s in excelData.ColumnTitles)
                //{
                //    foreach(var d in dateOfPaymentStrings)
                //    {
                //        entry = entry && s.Contains(d);
                //    }
                //    if (entry)
                //        break;
                //    varIndex++;
                //}
                #endregion

                
                #endregion

                DateTime currDate = excelData.data.


                #endregion

                //Cycle for columns. each column - data of borrower
                for (int j = 0; j < excelData.data.GetLength(1); j++)
                {
                    //As there are nulls in excelData (where there are empty strings) and ReplaceCaseInsensetive doesn't work with nulls
                    if (excelData.ColumnTitles[j] == null || excelData.data[i, j] == null)
                        continue;

                    
                    rng.Find.Text = maskSymbol + excelData.ColumnTitles[j] + maskSymbol;
                    rng.Find.Replacement.Text = excelData.data[i, j];
                    //Формирование искомой строки в шаблоне word
                    string text = maskSymbol + excelData.ColumnTitles[j] + maskSymbol;
                    string replaceText = excelData.data[i, j];

                    bool success = rng.Find.Execute(
                        text,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true,
                        Word.WdFindWrap.wdFindContinue,
                        true,
                        replaceText,
                        Word.WdReplace.wdReplaceAll,
                        false,
                        false,
                        false,
                        false);
                }
                WordDoc.Save();
                WordDoc.Close();
            }
            WordApp.Quit();
        }
        private void AddCalculatedDataToContent(Word.Document wordDoc, Loan loan)
        {
            //important stopped here
        }
        public void Calc
    }
}
