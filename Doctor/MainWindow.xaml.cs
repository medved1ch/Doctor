using System.Data.SQLite;
using System.Windows;
using Doctor.Connection;
using System;

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
                        Encryption f = new Encryption();
                        byte[] Pass = f.GetHashPassword(PbPass.Password);
                        connection.Open();
                        string Number = TbPhone.Text;
                        string query = $@"SELECT  COUNT(*) FROM Pacient WHERE Phone='{Number}' AND Password='{Pass}'";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 1)
                        {
                            string query2 = $@"SELECT id FROM Pacient WHERE Phone={Number}";
                            SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                            int countID = Convert.ToInt32(cmd2.ExecuteScalar());
                            connection.Close();
                            DocSelection doc = new DocSelection(countID);
                            doc.Show();
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
            Registration reg = new Registration();
            reg.Owner = this;
            reg.ShowDialog();
        }
    }
}
