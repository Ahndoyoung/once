using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Once_v2_2015.Class;
using Once_v2_2015.Model;

namespace Once_v2_2015.ViewModel
{
    public class AdjustmentUCViewModel : ViewModelBase
    {
        #region Command

        #region PeriodCommand

        private RelayCommand<string> _PeriodCommand;

        public RelayCommand<string> PeriodCommand
        {
            get { return _PeriodCommand ?? (_PeriodCommand = new RelayCommand<string>(Period)); }
        }

        private void Period(string obj)
        {
            string[] end = DateTime.Today.ToShortDateString().Split('-');
            EndYear = int.Parse(end[0]);
            EndMonth = int.Parse(end[1]);
            EndDay = Int32.Parse(end[2]);

            DateTime startDT = DateTime.Today;
            switch (obj)
            {
                case "1week":
                    startDT = DateTime.Today.AddDays(-7);
                    break;
                case "1month":
                    startDT = DateTime.Today.AddMonths(-1);
                    break;
                case "3month":
                    startDT = DateTime.Today.AddMonths(-3);
                    break;
                case "6month":
                    startDT = DateTime.Today.AddMonths(-6);
                    break;
                case "1year":
                    startDT = DateTime.Today.AddYears(-1);
                    break;
            }

            string[] start = startDT.ToShortDateString().Split('-');
            StartYear = int.Parse(start[0]);
            StartMonth = int.Parse(start[1]);
            StartDay = int.Parse(start[2]);
        }

        #endregion

        #region OnSelectedDatesChangedCommand

        private RelayCommand<SelectedDatesCollection> _OnSelectedDatesChangeCommand;

        public RelayCommand<SelectedDatesCollection> OnSelectedDatesChangedCommand
        {
            get { return _OnSelectedDatesChangeCommand ?? (_OnSelectedDatesChangeCommand = new RelayCommand<SelectedDatesCollection>(OnSelectedDatesChanged)); }
        }

        private void OnSelectedDatesChanged(SelectedDatesCollection obj)
        {
            var list = obj.ToList();
            list.Sort();

            string[] start = list[0].ToShortDateString().Split('-');
            StartYear = int.Parse(start[0]);
            StartMonth = int.Parse(start[1]);
            StartDay = int.Parse(start[2]);

            string[] end = list[list.Count - 1].ToShortDateString().Split('-');
            EndYear = int.Parse(end[0]);
            EndMonth = int.Parse(end[1]);
            EndDay = int.Parse(end[2]);
        }

        #endregion

        #region LookupCommand

        private RelayCommand _LookupCommand;

        public RelayCommand LookupCommand
        {
            get { return _LookupCommand ?? (_LookupCommand = new RelayCommand(Lookup)); }
        }

        private void Lookup()
        {
            Receipts.Clear();
            SellingItems.Clear();
            MoneySales = 0;
            CardSales = 0;

            DateTime start = new DateTime(StartYear, StartMonth, StartDay);
            DateTime end = new DateTime(EndYear, EndMonth, EndDay);

            string query =
                string.Format("SELECT * FROM RECEIPT WHERE Format([RECEIPT_DATE], \"yyyy-mm-dd\") >= '{0}' AND Format([RECEIPT_DATE], \"yyyy-mm-dd\") <= '{1}'",
                    start.ToShortDateString(), end.ToShortDateString());
            OleDbConnection conn = new OleDbConnection(OleDB.connPath);
            OleDbCommand cmd = new OleDbCommand(query, conn);
            try
            {
                conn.Open();
                var read = cmd.ExecuteReader();
                while (read.Read())
                {
                    // #, date, time, type, discount, subtotal, amount
                    Receipt r = new Receipt()
                    {
                        num = (int)read[0],
                        date = ((DateTime)read[1]).ToShortDateString(),
                        time = ((DateTime)read[1]).ToShortTimeString(),
                        type = (string)read[2],
                        discount = (int)read[3],
                        subtotal = (int)read[4],
                        amount = (int)read[5]
                    };
                    Receipts.Add(r);

                    if (r.type.Replace(" ", "") == "현금")
                        MoneySales += r.amount;
                    else if (r.type.Replace(" ", "") == "카드")
                        CardSales += r.amount;
                }
                read.Close();

                TotalSales = MoneySales + CardSales;
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region OnSelectedReceiptChangedCommand

        private RelayCommand _OnSelectedReceiptChangedCommand;

        public RelayCommand OnSelectedReceiptChangedCommand
        {
            get { return _OnSelectedReceiptChangedCommand ?? (_OnSelectedReceiptChangedCommand = new RelayCommand(OnSelectedReceiptChanged)); }
        }

        private void OnSelectedReceiptChanged()
        {
            if (SelectedReceipt != null)
            {
                SellingItems.Clear();
                MenusSales = 0;

                string query =
                    string.Format("SELECT * FROM SALE WHERE RECEIPT_NUM = {0}", SelectedReceipt.num);
                OleDbConnection conn = new OleDbConnection(OleDB.connPath);
                OleDbCommand cmd = new OleDbCommand(query, conn);
                try
                {
                    conn.Open();
                    var read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        // #, MENU_NAME, MENU_TEMP, MENU_SIZE, MENU_WHIP, MENU_PRICE, SALE_QUANTITY, 
                        SellingItem si = new SellingItem(read[1].ToString(), read[2].ToString()[0], read[3].ToString()[0], int.Parse(read[5].ToString()), int.Parse(read[6].ToString()));
                        SellingItems.Add(si);
                    }
                    read.Close();

                    SellingItem dis = new SellingItem("# Discount", null, null, -SelectedReceipt.discount, 1);
                    SellingItems.Add(dis);
                    MenusSales = SelectedReceipt.amount;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        #endregion

        #endregion

        #region Properties

        private ObservableCollection<Receipt> _Receipts = new ObservableCollection<Receipt>();

        public ObservableCollection<Receipt> Receipts
        {
            get { return _Receipts; }
            set
            {
                _Receipts = value;
                RaisePropertyChanged("Receipts");
            }
        }

        private ObservableCollection<SellingItem> _SellingItems = new ObservableCollection<SellingItem>();

        public ObservableCollection<SellingItem> SellingItems
        {
            get { return _SellingItems; }
            set
            {
                _SellingItems = value;
                RaisePropertyChanged("SellingItems");
            }
        }

        private Receipt _SelectedReceipt;

        public Receipt SelectedReceipt
        {
            get { return _SelectedReceipt; }
            set
            {
                _SelectedReceipt = value;
                RaisePropertyChanged("SelectedReceipt");
            }
        }

        private int _StartYear;

        public int StartYear
        {
            get { return _StartYear; }
            set
            {
                _StartYear = value;
                RaisePropertyChanged("StartYear");
            }
        }

        private int _StartMonth;

        public int StartMonth
        {
            get { return _StartMonth; }
            set
            {
                _StartMonth = value;
                RaisePropertyChanged("StartMonth");
            }
        }

        private int _StartDay;

        public int StartDay
        {
            get { return _StartDay; }
            set
            {
                _StartDay = value;
                RaisePropertyChanged("StartDay");
            }
        }

        private int _EndYear;

        public int EndYear
        {
            get { return _EndYear; }
            set
            {
                _EndYear = value;
                RaisePropertyChanged("EndYear");
            }
        }

        private int _EndMonth;

        public int EndMonth
        {
            get { return _EndMonth; }
            set
            {
                _EndMonth = value;
                RaisePropertyChanged("EndMonth");
            }
        }

        private int _EndDay;

        public int EndDay
        {
            get { return _EndDay; }
            set
            {
                _EndDay = value;
                RaisePropertyChanged("EndDay");
            }
        }
        
        private int _TotalSales = 0;

        public int TotalSales
        {
            get { return _TotalSales; }
            set
            {
                _TotalSales = value;
                RaisePropertyChanged("TotalSales");
            }
        }

        private int _MoneySales = 0;

        public int MoneySales
        {
            get { return _MoneySales; }
            set
            {
                _MoneySales = value;
                RaisePropertyChanged("MoneySales");
            }
        }

        private int _CardSales = 0;

        public int CardSales
        {
            get { return _CardSales; }
            set
            {
                _CardSales = value;
                RaisePropertyChanged("CardSales");
            }
        }

        private int _MenusSales = 0;

        public int MenusSales
        {
            get { return _MenusSales; }
            set
            {
                _MenusSales = value;
                RaisePropertyChanged("MenusSales");
            }
        }

        #endregion

        private void InitPeriod()
        {
            string[] day = DateTime.Today.ToShortDateString().Split('-');
            StartYear = int.Parse(day[0]);
            StartMonth = int.Parse(day[1]);
            StartDay = int.Parse(day[2]);
            EndYear = int.Parse(day[0]);
            EndMonth = int.Parse(day[1]);
            EndDay = int.Parse(day[2]);
        }

        public AdjustmentUCViewModel()
        {
            InitPeriod();
        }
    }
}
