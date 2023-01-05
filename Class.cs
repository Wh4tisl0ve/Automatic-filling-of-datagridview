using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Resources
{
    public class fillDGVClass//создаем класс для заполнения datagridview
    {
        DataBase dataBase = new DataBase();

        public List<String> ListColumns = new List<String>();//для запоминания столбцов таблицы
        /*метод для заполнения столбцов DataGridView(чтобы не прописывать каждый раз заново)*/
        public object DataGridLoadColumns(string TableName, int i)//загрузка столбцов автоматическим методом
        {//передаем название таблиц для вывода стобцов
            dataBase.OpenConnection();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();//для формирования таблицы с выводом запроса

            string queryString = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}'";//запрос для выборки колонок из каждой таблицы

            SqlCommand command = new SqlCommand(queryString, dataBase.GetConnection());//выполнение запроса с передачей строки запроса и строки подсключения

            adapter.SelectCommand = command;
            adapter.Fill(table);//заполняем таблицу по условиям выборки

            SqlDataReader reader = command.ExecuteReader();//переменная класса для чтения результата запроса
            while (reader.Read())//читаем результат массива и заполняем List
            {
                ListColumns.Add(reader.GetString(0));
            }
            dataBase.CloseConnection();
            return ListColumns.ElementAt(i);//вывод нужного элемента
        }

        public void ReadSingleRow(DataGridView dataGridView_Table, IDataRecord record, int CountCols, int i)//метод для заполнения данными datagridview
        {
            dataGridView_Table.Rows.Add();//каждый раз при выводе данного метода добавляем новую строку
            try
            {
                for (int j = 0; j < CountCols; j++)
                {
                    dataGridView_Table[j, i].Value = record.GetValue(j);//заполняем данными из запроса на выборку
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RefreshDataGrid(DataGridView dataGridView_Table, string queryString, int CountColumns)//принимаем таблицу и строку для заполнения
        {
            try
            {
                dataGridView_Table.Rows.Clear();
                dataBase.OpenConnection();
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable table = new DataTable();//для формирования таблицы с выводом запроса

                SqlCommand command = new SqlCommand(queryString, dataBase.GetConnection());//выполнение запроса с передачей строки запроса и строки подсключения

                adapter.SelectCommand = command;
                adapter.Fill(table);//заполняем таблицу по условиям выборки

                SqlDataReader reader = command.ExecuteReader();//переменная класса для чтения результата запроса
                int i = 0;//номер строки
                while (reader.Read())//читаем результат массива и заполняем List
                {

                    ReadSingleRow(dataGridView_Table, reader, CountColumns, i);//передаем datagridview, результат чтения запроса, номер строки
                    i++;
                }

                reader.Close();
                dataBase.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int ColumnCount(string TableName)//узнаем количество столбцов переданной таблицы          
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable table = new DataTable();//для формирования таблицы с выводом запроса
            string queryCountColumns = $"SELECT Count(*) FROM information_schema.COLUMNS WHERE TABLE_NAME = '{TableName}'";//запрос на подсчет количества столбцов, дял динамического заполнения
            dataBase.OpenConnection();
            SqlCommand command = new SqlCommand(queryCountColumns, dataBase.GetConnection());//выполнение запроса с передачей строки запроса и строки подсключения
            adapter.SelectCommand = command;
            adapter.Fill(table);//заполняем таблицу по условиям выборки

            SqlDataReader reader = command.ExecuteReader();//переменная класса для чтения результата запроса

            reader.Read();//читаем результат запроса
            int Count = (int)reader[0];

            reader.Close();
            dataBase.CloseConnection();

            return Count;//возвращаем количество столбцов

        }
    }
}
