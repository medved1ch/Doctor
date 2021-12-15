using System.Data.SQLite;
using System.Windows;
using System.Windows.Shapes;
using Doctor.Connection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Doctor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnSign_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TbPhone.Text) || String.IsNullOrEmpty(PbPass.Password))
            {
                MessageBox.Show("Заполните поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                    try
                    {
                        byte[] Pass;
                        Encryption f = new Encryption();
                        Pass = f.GetHashPassword(PbPass.Password);
                        connection.Open();
                        string query = $@"SELECT  COUNT(*) FROM Pacient WHERE Phone=@Phone AND Password=@Pass";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        string LoginLower = TbPhone.Text.ToLower();
                        cmd.Parameters.AddWithValue("@Phone", TbPhone.Text.ToLower());
                        cmd.Parameters.AddWithValue("@Pass", Pass);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 1)
                        {
                            string query2 = $@"SELECT id FROM User WHERE Phone=@Phone";
                            SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                            cmd2.Parameters.AddWithValue("@Phone", TbPhone.Text.ToLower());
                            int countID = Convert.ToInt32(cmd2.ExecuteScalar());
                            connection.Close();
                            MessageBox.Show("Добро пожаловать!");
                            MainWindow menu = new MainWindow();
                            menu.Show();
                            this.Close();

                        }
                        else
                        {
                            MessageBox.Show("Неверное имя пользователя или пароль");
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
            }
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Owner = this;
            registration.ShowDialog();
        }
    }
}
