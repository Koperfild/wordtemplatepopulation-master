using System;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;

namespace WordTemplatePopulation
{
    public class ExcelWork
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>filepath of chosen file</returns>
        public static string ChooseFile()
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = @"E:\GitHub\WordTemplatePopulation";
            openFileDialog1.Filter = "Exel files|*.xls;*.xlsx";
            //openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string file = null;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;
            }
            return file;
        }
        /// <summary>
        /// Opens, read Excel table and save data to ExcelData. All strings are stored ToLowered
        /// </summary>
        /// <returns>data from Excel table or null if table is empty or was error during opening</returns>
        public static ExcelData ReadExcelFile(Form caller, string filePath)
        {

            if (filePath == null)
                return null;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var ExcelApp = new Excel.Application();
            CultureInfo oldCi = System.Threading.Thread.CurrentThread.CurrentCulture;

            //!!!!!!!!!!!!!!!Делать проверку на правильность открытия Open
            Excel.Workbook wrkBook=null;
            try
            {
                wrkBook = ExcelApp.Workbooks.Open(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(caller, "Данный Excel файл уже открыт. Закройте файл и попробуйте снова");
                ExcelApp.Quit();
                return null;
            }
            if (wrkBook == null)
            {
                MessageBox.Show("Не удалось открыть WorkBook");
                return null;
            }


            Excel.Worksheet wrkSheet = ((Excel.Worksheet)wrkBook.Worksheets[1]);
            //System.Threading.Thread.CurrentThread.CurrentCulture = oldCi;
            Excel.Range usedRange = wrkSheet.UsedRange;
            //Тут делать ExcelData(n), т.е. найти n из usedRange
            ExcelData excelTable = new ExcelData(usedRange.Rows.Count, usedRange.Columns.Count);

            #region ~~~~~~~~~~Experiments~~~~~~~~~~~
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~Experiments~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //int m = usedRange.Rows.Count;
            //int n = usedRange.Columns.Count;
            //Excel.Range tmp1 = usedRange.Offset[1, 0];
            //string tmp_1 = usedRange.Cells[2, 3].Value2.ToString();
            //string tmp_2 = tmp1.Cells[2, 3].Value2.ToString();
            //m = usedRange.Rows.Count;
            //n = usedRange.Columns.Count;
            //tmp_1 = Convert.ToString(usedRange.Cells[usedRange.Rows.Count, usedRange.Columns.Count].Value2);
            //tmp_2 = Convert.ToString(tmp1.Cells[usedRange.Rows.Count-1, usedRange.Columns.Count-1].Value2);
            //tmp_2 = Convert.ToString(tmp1.Cells[usedRange.Rows.Count, usedRange.Columns.Count].Value2);
            //tmp1 = usedRange.Resize[usedRange.Rows.Count - 1, Type.Missing];

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~Experiments~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            #endregion

            //Если таблица пустая
            if (usedRange.Rows.Count <= 1)
            {
                MessageBox.Show("Вы ввели пустую таблицу Excel, выберите другой файл Excel");
                wrkBook.Close();//Проверять метод Close. Т.к. могут быть изменения в файле при простом открытии файла
                if (ExcelApp != null) ExcelApp.Quit();
                return null;
            }

            for (int i = 0; i < usedRange.Columns.Count; i++)
            {
                excelTable.ColumnTitles[i] = (Convert.ToString(((Excel.Range)usedRange.Cells[1, i + 1]).Value2)).Trim().ToLower();
            }

            //Сдвигаем диапазон вниз на 1 строку
            Excel.Range dataRange = usedRange.Offset[1, 0];
            //Убираем получившийся лишним при сдвиге вниз последний ряд
            dataRange = (Excel.Range)dataRange.Resize[usedRange.Rows.Count - 1, Type.Missing];

            //Заполняем таблицу данных данными
            for (int i = 0; i < dataRange.Rows.Count; i++)
            {
                for (int j = 0; j < dataRange.Columns.Count; j++)
                {
                    object a = ((Excel.Range)dataRange.Cells[i + 1, j + 1]).Value2;
                    excelTable.data[i, j] = (Convert.ToString(a)).Trim();
                }
            }
            wrkBook.Close();
            ExcelApp.Quit();
            return excelTable;
        }
    }
    class WordWork
    {
        //экземпляр ворда

        //~~~~~~~~~~~~~~~~~~~DELETE NAHYU~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //public static string WordPath;
        //public static string ResultPath;
        //~~~~~~~~~~~~~~~~~~~DELETE NAHYU END~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Creates directory near specified file
        /// </summary>
        /// <param name="filePath">File near which directory is created</param>
        /// <param name="caller">Form that called this method. Used to create modal form</param>
        /// <returns>Path to created directory</returns>
        public static string CreateDirNearFile(Form caller, string filePath)
        {

            int a = filePath.LastIndexOf("\\");
            string name = filePath.Remove(0, a + 1);
            string dir = filePath.Remove(a) + "\\Result_" + name;
            if (Directory.Exists(dir))
            {
                //MessageBox.Show()
                DialogResult dialRes = MessageBox.Show(caller, "Папка с именем как у выбранного Word файла уже существует. Перезаписать папку (внимание старая папка будет удалена со всем содержимым)", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialRes == DialogResult.Yes)
                {
                    Directory.Delete(dir,true);
                }
                else
                {
                    return null;
                }
            }
            //Because sometimes CreateDirectory doesn't really create directory (maybe because of work of Directory.Delete)
            Thread.Sleep(100);
            Directory.CreateDirectory(dir);
            //ResultPath = dir;
            return dir;
        }

        /// <summary>
        /// Call filedialog to choose word file
        /// </summary>
        /// <param name="caller">Form that called this method. Used to create modal form</param>
        /// <returns>full file path of chosen file or null if not chosen</returns>
        public static string openWordFile(Form caller)
        {
            //Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = @"E:\GitHub\WordTemplatePopulation";

            //TODO: фильтр только doc, docx файлов
            openFileDialog1.Filter = "Word files|*.doc;*.docx";
            //openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string file = null;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;
                //CreateDirNearFile(caller, file);
                //WordPath = file;
            }            
            return file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskSymbol">Symbol enclosing inserting field</param>
        /// <param name="excelData">data from Excel table</param>
        /// <param name="wordFilePath">Path of word file to open</param>
        /// <param name="dirPath">Directory path to save results (created files)</param>
        public static void CreateOutputFiles(Form caller, string maskSymbol, ExcelData excelData, string wordFilePath, string dirPath)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var WordApp = new Microsoft.Office.Interop.Word.Application();
            WordApp.Visible = false;
            Word.Document WordDoc=null;

            #region ~~~~~~~~~~old version~~~~~~~~~~~~~
            //try
            //{
            //    WordDoc = WordApp.Documents.Open(wordFilePath);
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show(caller, "Данный Word файл уже открыт. Закройте файл и попробуйте снова");
            //    WordApp.Quit();
            //    return;
            //}
            #endregion

            //Делать проверку скопировался ли файл

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

                for (int j = 0; j < excelData.data.GetLength(1); j++)
                {
                    //As there are nulls in excelData (where there are empty strings) and ReplaceCaseInsensetive doesn't work with nulls
                    if (excelData.ColumnTitles[j] == null || excelData.data[i, j] == null)
                        continue;

                    rng.Find.Text = maskSymbol + excelData.ColumnTitles[j] + maskSymbol;
                    rng.Find.Replacement.Text = excelData.data[i, j];

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
                        
                          

                    //Work but with case sensetive
                    //replace = replace.Replace(maskSymbol + excelData.ColumnTitles[j] + maskSymbol, excelData.data[i, j]);

                    
                    //replace = replace.ReplaceCaseInsensetive(maskSymbol + excelData.ColumnTitles[j] + maskSymbol, excelData.data[i, j]);

                }
                //Word.Document saveme = new Word.Document();
                //saveme.Content.set_Style(ref style);

                //WordDoc.Content.Text = replace;
                WordDoc.Save();
                WordDoc.Close();


                //saveme.SaveAs(dirPath + "\\" + (i+1) +".docx");
                //saveme.Close();
            }
            //WordDoc.Close();
            WordApp.Quit();
        }
    }

    public static class StringExtension
    {
        public static string ReplaceCaseInsensetive(this String str, string findMe,
    string newValue)
        {
            return Regex.Replace(str,
                Regex.Escape(findMe),
                Regex.Replace(newValue, "\\$[0-9]+", @"$$$0"),
                RegexOptions.IgnoreCase);
        }
    }
    public class CellPointer
    {
        public CellPointer(int m, int n)
        {
            this.Row = m;
            this.Column = n;
        }
        public int Row;
        public int Column;
    }
}





