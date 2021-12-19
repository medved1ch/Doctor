using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        DataTable dtTime = new DataTable();
        DataTable dtDocSpec = new DataTable();
        DataTable dtDocName = new DataTable();
        int PacientID;
        public DocSelection()
        {
            InitializeComponent();
            dtTime.Columns.Add("Time", typeof(String));
            Load_Spec();
        }

        public DocSelection(int id)
        {
            InitializeComponent();
            PacientID = id;
            dtTime.Columns.Add("Time", typeof(String));
            Load_Spec();
        }

        private void Load_Spec()
        {
            dtDocSpec.Clear();
            CbSpec.ItemsSource = dtDocSpec.DefaultView;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    string query = $@"SELECT id, PostName FROM Post";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    dtDocSpec.Load(cmd.ExecuteReader());
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Load_DocName()
        {
            dtDocName.Clear();
            CbDoc.ItemsSource = dtDocName.DefaultView;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    string PostID = Convert.ToString(dtDocSpec.Rows[CbSpec.SelectedIndex][0]);
                    string query = $@"SELECT id, FirstName, SecondName, MiddleName FROM Doctor WHERE PostID = {PostID}";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    dtDocName.Load(cmd.ExecuteReader());
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dtTime.Clear();
            CbTime.ItemsSource = dtTime.DefaultView;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    string Date_in = DpDate.Text;
                    string DocID = Convert.ToString(dtDocName.Rows[CbDoc.SelectedIndex][0]);
                    string query = $@"SELECT ScheduleStart FROM DocSchedule WHERE DocID = '{DocID}'  AND Day = '{Date_in}'";
                    string query1 = $@"SELECT ScheduleEND FROM DocSchedule WHERE DocID = '{DocID}'  AND Day = '{Date_in}'";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                    string start = Convert.ToString(cmd.ExecuteScalar());
                    string end = Convert.ToString(cmd1.ExecuteScalar());
                    DateTime schstart = DateTime.Parse(start);
                    DateTime schend = DateTime.Parse(end);
                    for (DateTime ii11 = schstart; ii11 < schend; ii11 = ii11.AddHours(0.25))
                    {
                        DataRow _ravi = dtTime.NewRow();
                        //MessageBox.Show(ii11.ToString("t"));
                        _ravi["Time"] = ii11.ToString("t");
                        dtTime.Rows.Add(_ravi);
                    }
                    string query2 = $@"SELECT TimeApp FROM Appointment WHERE DocID = '2'  AND DateApp = '{Date_in}'";
                    SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                    DataTable dt2 = new DataTable();
                    dt2.Load(cmd2.ExecuteReader());
                    foreach (DataRow ttt in dt2.Rows)
                    {
                        string time = Convert.ToString(ttt["TimeApp"]);
                        for (int i = dtTime.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = dtTime.Rows[i];
                            if (dr["Time"].ToString() == time)
                            {
                                dr.Delete();
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    string DocID = Convert.ToString(dtDocName.Rows[CbDoc.SelectedIndex][0]);
                    string Date_in = DpDate.Text;
                    string Time_in = CbTime.Text;
                    string query = $@"insert into Appointment(PacientID, DocID, DateApp, TimeApp) values ('{PacientID}', '{DocID}', '{Date_in}', '{Time_in}')";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    if (MessageBox.Show("Вы успешно записались! Хотите распечатать талон?", "Печать", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        AppPrint appPrint = new AppPrint();
                        appPrint.Owner = this;
/*                        appPrint.Close();*/
                    } 
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CbSpec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load_DocName();
        }

        private void CbDoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DpDate.BlackoutDates.Clear();
            var dates = new List<DateTime>{};
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    string DocID = Convert.ToString(dtDocName.Rows[CbDoc.SelectedIndex][0]);
                    string query = $@"SELECT Day FROM DocSchedule WHERE DocID = {DocID}";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToDateTime(dr[0]) >= DateTime.Today)
                        {
                            dates.Add(Convert.ToDateTime(dr[0]));
                        }
                    }
                    var firstDate = DateTime.Today.AddDays(-30);
                    var lastDate = DateTime.Today.AddDays(30);
                    var dateCounter = dates.First();
                    DpDate.BlackoutDates.Add(new CalendarDateRange(firstDate, dateCounter.AddDays(-1)));
                    foreach (var d in dates.Skip(1))
                    {
                        if (d.AddDays(-1).Date != dateCounter.Date)
                        {
                            DpDate.BlackoutDates.Add(new CalendarDateRange(dateCounter.AddDays(1), d.AddDays(-1)));
                        }
                        dateCounter = d;
                    }
                    DpDate.BlackoutDates.Add(new CalendarDateRange(dateCounter.AddDays(1), lastDate));
                    DpDate.DisplayDateStart = firstDate;
                    DpDate.DisplayDateEnd = lastDate;
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
