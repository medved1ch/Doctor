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
using System.Data;

namespace Doctor
{
    /// <summary>
    /// Логика взаимодействия для DocSelection.xaml
    /// </summary>
    public partial class DocSelection : Window
    {
        DataTable dt1 = new DataTable();
        public DocSelection()
        {
            InitializeComponent();
            dt1.Columns.Add("Time", typeof(String));
        }

        private void DpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dt1.Clear();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    var Date_in = DpDate.Text;
                    var Doc_in = CbDoc.SelectedItem;
                    string query = $@"SELECT ScheduleStart FROM DocSchedule WHERE DocID = '2'  AND Day = '{Date_in}'";
                    string query1 = $@"SELECT ScheduleEND FROM DocSchedule WHERE DocID = '2'  AND Day = '{Date_in}'";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                    string start = Convert.ToString(cmd.ExecuteScalar());
                    string end = Convert.ToString(cmd1.ExecuteScalar());
                    DateTime schstart = DateTime.Parse(start);
                    DateTime schend = DateTime.Parse(end);
                    for (DateTime ii11 = schstart; ii11 < schend; ii11 = ii11.AddHours(0.25))
                    {
                        DataRow _ravi = dt1.NewRow();
                        //MessageBox.Show(ii11.ToString("t"));
                        _ravi["Time"] = ii11.ToString("t");
                        dt1.Rows.Add(_ravi);
                    }
                    string query2 = $@"SELECT TimeApp FROM Appointment WHERE DocID = '2'  AND DateApp = '{Date_in}'";
                    SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                    DataTable dt2 = new DataTable();
                    dt2.Load(cmd2.ExecuteReader());
                    foreach(DataRow ttt in dt2.Rows)
                    {
                        string time = Convert.ToString(ttt["TimeApp"]);
                        for (int i = dt1.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = dt1.Rows[i];
                            if (dr["Time"].ToString() == time)
                                dr.Delete();
                        }
                    }
                    CbTime.ItemsSource = dt1.DefaultView;
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                connection.Open();
                if (CbSpec.SelectedIndex == -1 || CbDoc.SelectedIndex == -1 || String.IsNullOrEmpty(DpDate.Text) || CbTime.SelectedIndex == -1)
                {
                    MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    int IdStatus, IdPost;
                    bool NameStat = int.TryParse(CbStat.SelectedValue.ToString(), out IdStatus);
                    bool NamePost = int.TryParse(CbPost.SelectedValue.ToString(), out IdPost);

                    string query = $@"INSERT INTO Appointment('FirstName', 'SecondName', 'MiddleName', 'Dateofbirth', 'Phone', 'idPost', 'idStatus') values (@FN, @SN, @MN, @Date, @Phone, '{IdPost}', '{IdStatus}')";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    try
                    {
                        cmd.Parameters.AddWithValue("@SN", TbSN.Text);
                        cmd.Parameters.AddWithValue("@FN", TbFN.Text);
                        cmd.Parameters.AddWithValue("@MN", TbMN.Text);
                        cmd.Parameters.AddWithValue("@Date", DpB.Text);
                        cmd.Parameters.AddWithValue("@Phone", TbPhone.Text);
                        /*cmd.Parameters.AddWithValue("@Post", CbPost.SelectedItem);
                        cmd.Parameters.AddWithValue("@Stat", CbStat.SelectedItem);*/
                        cmd.ExecuteNonQuery();
                        MessageBoxResult result = MessageBox.Show("Внести ещё одного сотрудника? ", "Пользователь добавлен", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            TbFN.Clear();
                            TbSN.Clear();
                            TbMN.Clear();
                            TbPhone.Clear();
                            DpB.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");
                            TbPhone.Clear();
                            CbStat.SelectedIndex = -1;
                            CbPost.SelectedIndex = -1;
                        }
                        else
                        {
                            this.Close();
                        }

                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }

            }
        }
    }
}
