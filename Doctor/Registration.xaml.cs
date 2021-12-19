using System;
using System.Windows;
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
                    MessageBox.Show("Логин должен быть больше 3 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (PbPass.Password.Length <= 3)
                {
                    MessageBox.Show("Пароль должен быть больше 3 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    connection.Open();
                    string query = $@"SELECT  COUNT(*) FROM Pacient WHERE Phone=@Phone";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Phone", TbPhone.Text.ToLower());
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        string Phone = TbPhone.Text;
                        string Polis = TbPolis.Text;
                        Encryption f = new Encryption();
                        byte[] Pass = f.GetHashPassword(PbPass.Password);
                        string FN = TbFN.Text;
                        string SN = TbSN.Text;
                        string MN = TbMN.Text;
                        query = $@"INSERT INTO Pacient ('Phone','Polis','Password', 'SecondName', 'FirstName', 'MiddleName') VALUES ('{Phone}', '{Polis}', '{Pass}', '{SN}', '{FN}', '{MN}')";
                        cmd = new SQLiteCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Аккаунт зарегистрирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Логин занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    } 
                }
            }
        }
    }
}
