using System;
using System.Windows.Forms;

namespace WordTemplatePopulation
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.excelFilePath = ExcelWork.ChooseFile();
            this.excelTable = ExcelWork.ReadExcelFile(this, this.excelFilePath);

            if (this.excelFilePath != null)
                this.label1.Text = "Выбран " + this.excelFilePath;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.wordFilePath = WordWork.openWordFile(this);
            if (this.wordFilePath != null)
            this.label2.Text = "Выбран " + this.wordFilePath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //if excel or word file wasn't chosen before this button pressed
            if (this.excelFilePath == null || this.wordFilePath == null)
            {
                MessageBox.Show("Не выбран excel или word файл","Сообщение");
                return;
            }
            string dirPath = WordWork.CreateDirNearFile(this, this.wordFilePath);
            //if user've chosen not to replace existing folder
            if (dirPath == null)
                return;
            WordWork.CreateOutputFiles(this, "@", this.excelTable, this.wordFilePath, dirPath);

            this.label3.Text = "Результат сохранен в " + dirPath;
            this.label3.Visible = true;
            MessageBox.Show(this, "Файлы успешно созданы","Сообщение");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string help="Программа предназначена для автоматического заполнения word шаблона данными из exel таблицы.";
            help += "\n\r" + "\n\r" + "При работе программы встречаемые в word файле слова, выделенные c начала и конца знаком @ (пример: @поле@), будут заменены на значения из таблицы exel с соответсвующими заголовкоми столбцов.";
            help += "\n\r" + "\n\r" + "В ходе работы, в директории, в которой изначально лежал word файл, будет создана соответсвующая папка, в которой будут находится копии word файла с заполнеными значениями.";
            help += "\n\r" + "\n\r" + "Автор программы Леоненко Евгений\nContacts: +79153682750 email: avalancher2@rambler.ru";
            MessageBox.Show(help, "Помощь");
        }

       
    }
}
