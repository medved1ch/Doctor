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
        DataTable dt1 = new DataTable("Time");
        public DocSelection()
        {
            InitializeComponent();
        }

        private void DpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dt1.Clear();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    connection.Open();
                    var Date_in = DpDate.SelectedDate;
                    var Doc_in = CbDoc.SelectedItem;
                    string query = $@"SELECT ScheduleStart FROM DocSchedule WHERE DocID = {Doc_in}  AND Day = {Date_in}";
                    string query1 = $@"SELECT ScheduleEND FROM DocSchedule WHERE DocID = {Doc_in}  AND Day = {Date_in}";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                    string start = Convert.ToString(cmd.ExecuteReader());
                    string end = Convert.ToString(cmd1.ExecuteReader());
                    DateTime schstart = DateTime.Parse(start);
                    DateTime schend = DateTime.Parse(end);

                    for (DateTime ii11 = schstart; ii11 < schend; schstart.AddMinutes(15))
                    {
                        dt1.Rows.Add(ii11);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
