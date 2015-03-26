using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkersLibrary;

namespace WorkerEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Worker> Workers { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Workers = new ObservableCollection<Worker>();
            Workers.Add(new Manager("Светлана", 25, 78461532,  9));
            Workers.Add(new Driver("Вася", 32, 488653, "Audi", 340));
            Workers.Add(new Manager("Елена", 27, 448666, 19));
            Workers.Add(new Driver("Петя", 45, 789542, "BMW", 256));
            Workers.Add(new Manager("Екатерина", 23, 78985622, 15));
            Workers.Add(new Driver("Саша", 45, 789542, "BMW", 256));

            //загрузка информации из БД

            //создаем подклчение к БД
            OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Environment.CurrentDirectory + "\\WorkersDB.accdb");
            //открываем соединение к базе
            con.Open();
            //создаем новый запрос
            OleDbCommand select = new OleDbCommand();
            //инициализируем его с помощью команды языка SQL
            select = new OleDbCommand("SELECT SNN, ManName, Age, ProjCount FROM Managers", con);
            //select = new OleDbCommand("SELECT * FROM Managers", con); эта запись аналогична предыдущей
            OleDbDataReader reader = select.ExecuteReader(); //запускаем чтеца, который выберет нужную информацию из запроса
            while (reader.Read()) //пока есть что читать
            {
                //добавляем в список работников нового.
                //чтец вернет массив объектов, из которого можно по индексу извлечь нужные элементы.
                //обратите внимание, что порядок значений в таблице отличается от того, который идет в конструкторе
                Workers.Add(new Manager(reader[1].ToString(), Convert.ToInt32(reader[2]), Convert.ToInt64(reader[0]), Convert.ToInt32(reader[3])));
            }
            //select = new OleDbCommand("SELECT SNN, DrName, Age, CarType, Hours FROM Drivers"); эта запись аналогична той, что ниже
            select = new OleDbCommand("SELECT * FROM Drivers", con);
            reader = select.ExecuteReader(); //опять запускаем чтеца
            while (reader.Read()) 
            {
                Workers.Add(new Driver(reader[1].ToString(), Convert.ToInt32(reader[2]), Convert.ToInt64(reader[0]), reader[3].ToString(), Convert.ToInt32(reader[4])));
            }


            con.Close(); //закрываем соединение

            DataContext = this;
        }

        private void workersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Worker worker = ((sender as ListBox).SelectedItem as Worker);
            if (worker != null)
            {
                detailsPanel.DataContext = worker;
                if (worker is Driver)
                {
                    managerInfo.Visibility = System.Windows.Visibility.Collapsed;
                    driverInfo.Visibility = System.Windows.Visibility.Visible;
                }
                if (worker is Manager)
                {
                    managerInfo.Visibility = System.Windows.Visibility.Visible;
                    driverInfo.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        

        /// <summary>
        /// Метод для добавления в список и БД нового сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            //если добавляем водителя
            if (projCountInfo.Text == "")
            {
                try
                {
                    //создаем объект, в который заносится информация из текстовых полей
                    Driver dr = new Driver(nameInfo.Text, int.Parse(ageInfo.Text), Int64.Parse(snnInfo.Text), carTypeInfo.Text, int.Parse(hoursInfo.Text));
                    //создае подключение к БД. Указываем тип СУБД и адрес, по которому лежит файл базы данных
                    OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Environment.CurrentDirectory + "\\WorkersDB.accdb");
                    //создаем новый запрос
                    OleDbCommand insert = new OleDbCommand();
                    //инициализируем этот запрос командой языка SQL
                    insert = new OleDbCommand("INSERT INTO Drivers (`SNN`, `DrName`, `Age`, `CarType`, `Hours`) VALUES (@snn, @drName, @age, @carType, @hours)", con);
                    
                    insert.Parameters.AddWithValue("@snn", dr.Snn);               //указываем какие данные нужно занести в БД
                    insert.Parameters.AddWithValue("@drName", dr.Name);           //указываем какие данные нужно занести в БД
                    insert.Parameters.AddWithValue("@age", dr.Age);               //указываем какие данные нужно занести в БД
                    insert.Parameters.AddWithValue("@carType", dr.CarType);       //указываем какие данные нужно занести в БД
                    insert.Parameters.AddWithValue("@hours", dr.Hours);           //указываем какие данные нужно занести в БД
                    con.Open(); //открываем соединение
                    insert.ExecuteNonQuery(); //выполняем запрос языка SQL
                    con.Close(); //закрываем соединение
                    Workers.Add(dr); //добавлем работника в наш листбокс
                    MessageBox.Show("Добавили \n" + dr.Name);
                    ClearInfoTable(); //зачищаем текстовые поля нашего приложения
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Ошибка ввода \n" + ex.Message);
                }
            }
                //то же самое, только для менеджера
            else
            {
                try
                {
                    Manager mn = new Manager(nameInfo.Text, int.Parse(ageInfo.Text), Int64.Parse(snnInfo.Text), int.Parse(projCountInfo.Text));
                    OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Environment.CurrentDirectory + "\\WorkersDB.accdb");
                    OleDbCommand insert = new OleDbCommand();
                    insert = new OleDbCommand("INSERT INTO Managers (`SNN`, `ManName`, `Age`, `ProjCount`) VALUES (@snn, @manName, @age, @projCount)", con);
                    insert.Parameters.AddWithValue("@snn", mn.Snn);
                    insert.Parameters.AddWithValue("@manName", mn.Name);
                    insert.Parameters.AddWithValue("@age", mn.Age);
                    insert.Parameters.AddWithValue("@carType", mn.ProjectsCount);
                    con.Open();
                    insert.ExecuteNonQuery();
                    con.Close();
                    Workers.Add(mn);
                    MessageBox.Show("Добавили \n" + mn.Name);
                    ClearInfoTable();
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Ошибка ввода \n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Метод, который зачищает текстовые поля после добавления нового сотрудника 
        /// </summary>
        private void ClearInfoTable()
        {
            nameInfo.Text = "";
            ageInfo.Text = "";
            snnInfo.Text = "";
            projCountInfo.Text = "";
            carTypeInfo.Text = "";
            hoursInfo.Text = "";
        }

        /// <summary>
        /// Удаление работика из БД и списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Worker delWorker = (workersList.SelectedItem as Worker);
            if (delWorker != null)
            {
                Workers.Remove(delWorker);
                workersList.SelectedIndex = 0;

                //опять же создаем подключение
                OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Environment.CurrentDirectory + "\\WorkersDB.accdb");

                con.Open(); //открываем соединение с БД
                OleDbCommand delete = new OleDbCommand();

                if (delWorker is Driver)
                {
                    //заносим в команду запрос на удаление 
                    delete = new OleDbCommand("DELETE FROM Drivers WHERE SNN =@snn", con);
                    delete.Parameters.AddWithValue("@snn", delWorker.Snn); //параметр, по которому будем выбирать работника для удаления
                }
                if (delWorker is Manager)
                {
                    delete = new OleDbCommand("DELETE FROM Managers WHERE SNN =@snn", con);
                    delete.Parameters.AddWithValue("@snn", delWorker.Snn);
                }
                delete.ExecuteNonQuery(); //выполняем запрос
                con.Close();
                MessageBox.Show("Удален + \n" + delWorker.Name);
            }
        }

        
    }
}
