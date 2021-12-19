using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;

namespace Doctor
{
    /// <summary>
    /// Логика взаимодействия для AppPrint.xaml
    /// </summary>
    public partial class AppPrint : Window
    {
        public AppPrint(DataTable dt)
        {
            InitializeComponent();
            foreach (DataRow dr in dt.Rows)
            {
                AppNumLB.Content = AppNumLB.Content + dr["id"].ToString();
                AppDateLB.Content = dr["DateApp"].ToString();
                WeekDayLB.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Convert.ToDateTime(dr["DateApp"]).DayOfWeek));
                TimeLB.Content = dr["TimeApp"].ToString();
                PostLB.Content = dr["PostName"].ToString();
                FioLB.Content = dr["SecondName"].ToString() + " " + dr["FirstName"].ToString() + " " + dr["MiddleName"].ToString();
                NumCabLB.Content = NumCabLB.Content + dr["CabNumber"].ToString();
            }
            PrintDialog p = new PrintDialog();
            if (p.ShowDialog() == true)
            {
                p.PrintVisual(PrintGrid, "Печать");
            }
        }
    }
}
