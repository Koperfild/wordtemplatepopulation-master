using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordTemplatePopulation
{
    public class ExcelData
    {
        public string[] ColumnTitles;
        public string[,] data;
        public ExcelData() { }
        /// <summary>
        /// Class to store initial data from Excel file. All data (strings) are stored trimmed and tolowered.
        /// </summary>
        /// <param name="n">Size of table</param>
        public ExcelData(int m, int n)
        {
            ColumnTitles = new string[n];
            data = new string[m - 1, n];//m-1 потому что m,n - размер всей таблицы, включая заголовки столбцов
        }
    }
}
