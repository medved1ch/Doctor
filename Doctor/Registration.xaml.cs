using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.SQLite;
using Doctor.Connection;

namespace Doctor
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {

                if (String.IsNullOrEmpty(TbPhone.Text) || String.IsNullOrEmpty(PbPass.Password) || String.IsNullOrEmpty(TbPolis.Text))
                {
                    MessageBox.Show("Заполните поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (TbPhone.Text.Length <= 3)
                {
                    MessageBox.Show(" Логин должен быть больше 3", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (PbPass.Password.Length <= 3)
                {
                    MessageBox.Show(" Пароль должен быть больше 3", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    connection.Open();
                    string query = $@"SELECT  COUNT(*) FROM Pacient WHERE Phone=@Phone";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Phone", TbPhone.Text.ToLower());
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 1)
                    {
                        MessageBox.Show("Логин занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        query = $@"INSERT INTO Pacient ('Phone','Polis','Password', 'SecondName', 'FirstName', 'MiddleName') VALUES (@Phone, @Polis, @Pass, @SN, @FN, @MN)";
                        cmd = new SQLiteCommand(query, connection);

                        byte[] Pass;
                        Encryption f = new Encryption();
                        Pass = f.GetHashPassword(PbPass.Password);
                        cmd.Parameters.AddWithValue("@Phone", TbPhone.Text.ToLower());
                        cmd.Parameters.AddWithValue("@Pass", Pass);
                        cmd.Parameters.AddWithValue("@Polis", TbPolis.Text);
                        cmd.Parameters.AddWithValue("@FN", TbFN.Text);
                        cmd.Parameters.AddWithValue("@SN", TbSN.Text);
                        cmd.Parameters.AddWithValue("@MN", TbMN.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Аккаунт зарегистрирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    } 
                }
            }
        }
    }
}
