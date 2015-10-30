using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Once_v2_2015.View;
using Once_v2_2015.Class;
using MessageBox = System.Windows.MessageBox;

namespace Once_v2_2015.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        #region Command

        #region LookupSales

        private RelayCommand<StatisticsUC> _LookupSalesCommand;

        public RelayCommand<StatisticsUC> LookupSalesCommand
        {
            get { return _LookupSalesCommand ?? (_LookupSalesCommand = new RelayCommand<StatisticsUC>(LookupSales)); }
        }

        private void LookupSales(StatisticsUC obj)
        {
            ChartTitle = "판매량";
            ChartHeader = "판매량";
            ChartHeader2 = "지난 판매량";

            MyCollection.Clear();
            MyCollection2.Clear();

            // 기간
            DateTime start = DateTime.Today;
            DateTime prevDT = DateTime.Today;
            if (obj.btn1Week_s.IsChecked == true)
            {
                start = DateTime.Today.AddDays(-7);
                prevDT = start.AddDays(-7);
                ChartTitle += " ( 1주 )";
            }
            else if (obj.btn1Month_s.IsChecked == true)
            {
                start = DateTime.Today.AddMonths(-1);
                prevDT = start.AddMonths(-1);
                ChartTitle += " ( 1개월 )";
            }
            else if (obj.btn3Month_s.IsChecked == true)
            {
                start = DateTime.Today.AddMonths(-3);
                prevDT = start.AddMonths(-3);
                ChartTitle += " ( 3개월 )";
            }
            else if (obj.btn6Month_s.IsChecked == true)
            {
                start = DateTime.Today.AddMonths(-6);
                prevDT = start.AddMonths(-6);
                ChartTitle += " ( 6개월 )";
            }
            else if (obj.btn1Year_s.IsChecked == true)
            {
                start = DateTime.Today.AddYears(-1);
                prevDT = start.AddYears(-1);
                ChartTitle += " ( 1년 )";
            }
            DateTime end = DateTime.Today;

            string query =
                string.Format("SELECT * FROM SALE, RECEIPT WHERE SALE.RECEIPT_NUM = RECEIPT.RECEIPT_NUM AND (Format([RECEIPT_DATE], \"yyyy-mm-dd\") >= '{0}' AND Format([RECEIPT_DATE], \"yyyy-mm-dd\") <= '{1}')",
                    string.Format("{0:yyyy-MM-dd}", start), string.Format("{0:yyyy-MM-dd}", end));
            OleDbConnection conn = new OleDbConnection(OleDB.connPath);
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                conn.Open();

                // 판매량
                var read = cmd.ExecuteReader();
                Dictionary<string, int> dic = new Dictionary<string, int>();
                while (read.Read())
                {
                    // read[1] = Name
                    // read[6] = Quantity
                    // read[9] = DateTime
                    string name = (string) read[1];
                    name = name.Trim();
                    name = name.Replace("^", " ");
                    name = name.Replace(" * 휘핑크림", "");
                    int qty = (int) read[6];
                    DateTime dt = (DateTime) read[9];

                    // 시간 체크
                    if ((obj.btnTime0.IsChecked == false && 10 <= dt.Hour && dt.Hour < 13) ||
                        (obj.btnTime1.IsChecked == false && 13 <= dt.Hour && dt.Hour < 16) ||
                        (obj.btnTime2.IsChecked == false && 16 <= dt.Hour && dt.Hour < 19) ||
                        (obj.btnTime3.IsChecked == false && 19 <= dt.Hour && dt.Hour < 23) ||
                        (0 <= dt.Hour && dt.Hour < 10) || dt.Hour == 23)
                        continue;

                    // 요일 체크
                    if ((obj.btnDow0.IsChecked == false && dt.DayOfWeek == DayOfWeek.Monday) ||
                        (obj.btnDow1.IsChecked == false && dt.DayOfWeek == DayOfWeek.Tuesday) ||
                        (obj.btnDow2.IsChecked == false && dt.DayOfWeek == DayOfWeek.Wednesday) ||
                        (obj.btnDow3.IsChecked == false && dt.DayOfWeek == DayOfWeek.Thursday) ||
                        (obj.btnDow4.IsChecked == false && dt.DayOfWeek == DayOfWeek.Friday) ||
                        (obj.btnDow5.IsChecked == false && dt.DayOfWeek == DayOfWeek.Saturday) ||
                        (obj.btnDow6.IsChecked == false && dt.DayOfWeek == DayOfWeek.Sunday))
                        continue;

                    if (dic.ContainsKey(name) == false)
                        dic.Add(name, 0);
                    dic[name] += qty;
                }

                var list = from pair in dic
                           orderby pair.Value descending
                           select pair;
                int i = 0;
                foreach (KeyValuePair<string, int> pair in list)
                {
                    if (i == 10)
                        break;
                    MyCollection.Add(pair);
                    i++;
                }

                read.Close();

                // 전 판매량
                query =
                string.Format("SELECT * FROM SALE, RECEIPT WHERE SALE.RECEIPT_NUM = RECEIPT.RECEIPT_NUM AND (Format([RECEIPT_DATE], \"yyyy-mm-dd\") >= '{0}' AND Format([RECEIPT_DATE], \"yyyy-mm-dd\") <= '{1}')",
                    string.Format("{0:yyyy-MM-dd}", prevDT.AddDays(-1)), string.Format("{0:yyyy-MM-dd}", start.AddDays(-1)));
                cmd.CommandText = query;
                read = cmd.ExecuteReader();
                dic = new Dictionary<string, int>();
                while (read.Read())
                {
                    string name = (string)read[1];
                    name = name.Trim();
                    name = name.Replace("^", " ");
                    name = name.Replace(" * 휘핑크림", "");
                    int qty = (int)read[6];
                    DateTime dt = (DateTime)read[9];

                    // 시간 체크
                    if ((obj.btnTime0.IsChecked == false && 10 <= dt.Hour && dt.Hour < 13) ||
                        (obj.btnTime1.IsChecked == false && 13 <= dt.Hour && dt.Hour < 16) ||
                        (obj.btnTime2.IsChecked == false && 16 <= dt.Hour && dt.Hour < 19) ||
                        (obj.btnTime3.IsChecked == false && 19 <= dt.Hour && dt.Hour < 23) ||
                        (0 <= dt.Hour && dt.Hour < 10) || dt.Hour == 23)
                        continue;

                    // 요일 체크
                    if ((obj.btnDow0.IsChecked == false && dt.DayOfWeek == DayOfWeek.Monday) ||
                        (obj.btnDow1.IsChecked == false && dt.DayOfWeek == DayOfWeek.Tuesday) ||
                        (obj.btnDow2.IsChecked == false && dt.DayOfWeek == DayOfWeek.Wednesday) ||
                        (obj.btnDow3.IsChecked == false && dt.DayOfWeek == DayOfWeek.Thursday) ||
                        (obj.btnDow4.IsChecked == false && dt.DayOfWeek == DayOfWeek.Friday) ||
                        (obj.btnDow5.IsChecked == false && dt.DayOfWeek == DayOfWeek.Saturday) ||
                        (obj.btnDow6.IsChecked == false && dt.DayOfWeek == DayOfWeek.Sunday))
                        continue;

                    // 본 판매량에 없으면
                    bool flag = false;
                    foreach (var pair in MyCollection)
                    {
                        if (pair.Key == name)
                            flag = true;
                    }
                    if (flag == false)
                        continue;

                    if (dic.ContainsKey(name) == false)
                        dic.Add(name, 0);
                    dic[name] += qty;
                }

                var list2 = dic.ToList();
                i = 0;
                foreach (KeyValuePair<string, int> pair in list2)
                {
                    if (i == 10)
                        break;
                    MyCollection2.Add(pair);
                    i++;
                }

                read.Close();
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region LookupProfit

        private RelayCommand<StatisticsUC> _LookupProfitCommand;

        public RelayCommand<StatisticsUC> LookupProfitCommand
        {
            get { return _LookupProfitCommand ?? (_LookupProfitCommand = new RelayCommand<StatisticsUC>(LookupProfit)); }
        }

        private void LookupProfit(StatisticsUC obj)
        {
            ChartTitle = "매출량";
            ChartHeader = "매출량";
            ChartHeader2 = "*";

            MyCollection.Clear();
            MyCollection2.Clear();

            // 기간
            DateTime start = DateTime.Today;
            if (obj.btn1Week_p.IsChecked == true)
            {
                start = DateTime.Today.AddDays(-7);
                ChartTitle += " ( 1주 )";
            }
            else if (obj.btn1Month_p.IsChecked == true)
            {
                start = DateTime.Today.AddMonths(-1);
                ChartTitle += " ( 1개월 )";
            }
            else if (obj.btn3Month_p.IsChecked == true)
            {
                start = DateTime.Today.AddMonths(-3);
                ChartTitle += " ( 3개월 )";
            }
            else if (obj.btn6Month_p.IsChecked == true)
            {
                start = DateTime.Today.AddMonths(-6);
                ChartTitle += " ( 6개월 )";
            }
            else if (obj.btn1Year_p.IsChecked == true)
            {
                start = DateTime.Today.AddYears(-1);
                ChartTitle += " ( 1년 )";
            }
            DateTime end = DateTime.Today;

            // 단위
            int unit = -1;
            if (obj.btnTime_p.IsChecked == true)
            {
                unit = 0;
                ChartTitle += " ( 시간 )";
            }
            else if (obj.btnDay_p.IsChecked == true)
            {
                unit = 1;
                ChartTitle += " ( 요일 )";
            }
            else if (obj.btnMonth_p.IsChecked == true)
            {
                unit = 2;
                ChartTitle += " ( 월 )";
            }

            string query =
                string.Format("SELECT * FROM RECEIPT WHERE Format([RECEIPT_DATE], \"yyyy-mm-dd\") >= '{0}' AND Format([RECEIPT_DATE], \"yyyy-mm-dd\") <= '{1}' ORDER BY RECEIPT_NUM",
                    string.Format("{0:yyyy-MM-dd}", start), string.Format("{0:yyyy-MM-dd}", end));
            OleDbConnection conn = new OleDbConnection(OleDB.connPath);
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                conn.Open();
                var read = cmd.ExecuteReader();
                if (unit == 0)
                {
                    int[] values = new int[4];
                    DateTime dt;
                    while (read.Read())
                    {
                        // #, datetime, type, discount, subtotal, amount
                        dt = (DateTime) read[1];
                        if (10 <= dt.Hour && dt.Hour < 13)
                            values[0] += (int) read[5];
                        else if (13 <= dt.Hour && dt.Hour < 16)
                            values[1] += (int) read[5];
                        else if (16 <= dt.Hour && dt.Hour < 19)
                            values[2] += (int) read[5];
                        else if (19 <= dt.Hour && dt.Hour < 23)
                            values[3] += (int) read[5];
                    }

                    KeyValuePair<string, int>[] kvp = new KeyValuePair<string, int>[4];
                    kvp[0] = new KeyValuePair<string, int>("10~1시", values[0]);
                    kvp[1] = new KeyValuePair<string, int>("1~4시", values[1]);
                    kvp[2] = new KeyValuePair<string, int>("4~7시", values[2]);
                    kvp[3] = new KeyValuePair<string, int>("7~11시", values[3]);

                    for(int i=0; i<4; i++)
                        MyCollection.Add(kvp[i]);
                }
                else if (unit == 1)
                {
                    int[] values = new int[7];
                    DateTime dt;
                    while (read.Read())
                    {
                        dt = (DateTime)read[1];
                        if (dt.DayOfWeek == DayOfWeek.Monday)
                            values[0] += (int)read[5];
                        else if (dt.DayOfWeek == DayOfWeek.Tuesday)
                            values[1] += (int)read[5];
                        else if (dt.DayOfWeek == DayOfWeek.Wednesday)
                            values[2] += (int)read[5];
                        else if (dt.DayOfWeek == DayOfWeek.Thursday)
                            values[3] += (int)read[5];
                        else if (dt.DayOfWeek == DayOfWeek.Friday)
                            values[4] += (int)read[5];
                        else if (dt.DayOfWeek == DayOfWeek.Saturday)
                            values[5] += (int)read[5];
                        else if (dt.DayOfWeek == DayOfWeek.Sunday)
                            values[6] += (int)read[5];
                    }

                    KeyValuePair<string, int>[] kvp = new KeyValuePair<string, int>[7];
                    kvp[0] = new KeyValuePair<string, int>("월", values[0]);
                    kvp[1] = new KeyValuePair<string, int>("화", values[1]);
                    kvp[2] = new KeyValuePair<string, int>("수", values[2]);
                    kvp[3] = new KeyValuePair<string, int>("목", values[3]);
                    kvp[4] = new KeyValuePair<string, int>("금", values[4]);
                    kvp[5] = new KeyValuePair<string, int>("토", values[5]);
                    kvp[6] = new KeyValuePair<string, int>("일", values[6]);

                    for (int i = 0; i < 7; i++)
                        MyCollection.Add(kvp[i]);
                }
                else if (unit == 2)
                {
                    int[] values = new int[12];
                    DateTime dt;
                    while (read.Read())
                    {
                        dt = (DateTime) read[1];
                        if (dt.Month == 1)
                            values[0] += (int) read[5];
                        else if (dt.Month == 2)
                            values[1] += (int)read[5];
                        else if (dt.Month == 3)
                            values[2] += (int)read[5];
                        else if (dt.Month == 4)
                            values[3] += (int)read[5];
                        else if (dt.Month == 5)
                            values[4] += (int)read[5];
                        else if (dt.Month == 6)
                            values[5] += (int)read[5];
                        else if (dt.Month == 7)
                            values[6] += (int)read[5];
                        else if (dt.Month == 8)
                            values[7] += (int)read[5];
                        else if (dt.Month == 9)
                            values[8] += (int)read[5];
                        else if (dt.Month == 10)
                            values[9] += (int)read[5];
                        else if (dt.Month == 11)
                            values[10] += (int)read[5];
                        else if (dt.Month == 12)
                            values[11] += (int)read[5];
                    }

                    KeyValuePair<string,int>[] kvp = new KeyValuePair<string, int>[12];
                    for (int i = 0; i < 12; i++)
                    {
                        kvp[i] = new KeyValuePair<string, int>((i + 1).ToString() + "월", values[i]);
                        MyCollection.Add(kvp[i]);
                    }
                }
                read.Close();
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #endregion


        #region Properties

        private ObservableCollection<KeyValuePair<string, int>> _MyCollection = new ObservableCollection<KeyValuePair<string, int>>();

        public ObservableCollection<KeyValuePair<string, int>> MyCollection
        {
            get { return _MyCollection; }
        }

        private ObservableCollection<KeyValuePair<string, int>> _MyCollection2 = new ObservableCollection<KeyValuePair<string, int>>();

        public ObservableCollection<KeyValuePair<string, int>> MyCollection2
        {
            get { return _MyCollection2; }
        }

        private string _ChartTitle = "*";

        public string ChartTitle
        {
            get { return _ChartTitle; }
            set
            {
                _ChartTitle = value;
                RaisePropertyChanged("ChartTitle");
            }
        }

        private string _ChartHeader = "*";

        public string ChartHeader
        {
            get { return _ChartHeader; }
            set
            {
                _ChartHeader = value;
                RaisePropertyChanged("ChartHeader");
            }
        }

        private string _ChartHeader2 = "*";

        public string ChartHeader2
        {
            get { return _ChartHeader2; }
            set
            {
                _ChartHeader2 = value;
                RaisePropertyChanged("ChartHeader2");
            }
        }

        #endregion
    }
}
