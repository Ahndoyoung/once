using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;
using Once_v2_2015.Model;
using Once_v2_2015.View;

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
            DateTime today = DateTime.Today;
            string[] end = string.Format("{0:yyyy-MM-dd}", today).Split('-');
            EndYear = int.Parse(end[0]);
            EndMonth = int.Parse(end[1]);
            EndDay = int.Parse(end[2]);

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

            string[] start = string.Format("{0:yyyy-MM-dd}", startDT).Split('-');
            StartYear = int.Parse(start[0]);
            StartMonth = int.Parse(start[1]);
            StartDay = int.Parse(start[2]);
        }

        #endregion

        #region OnPreviewMouseUpCommand
        /* 마우스 포커싱 없애기 */

        private RelayCommand _OnPreviewMouseUpCommand;

        public RelayCommand OnPreviewMouseUpCommand
        {
            get { return _OnPreviewMouseUpCommand ?? (_OnPreviewMouseUpCommand = new RelayCommand(OnPreviewMouseUp)); }
        }

        private void OnPreviewMouseUp()
        {
            if (Mouse.Captured is Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                Mouse.Capture(null);
            }
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
            System.Windows.Input.Mouse.Capture(null);

            var list = obj.ToList();
            list.Sort();

            string[] start = string.Format("{0:yyyy-MM-dd}", list[0]).Split('-');
            StartYear = int.Parse(start[0]);
            StartMonth = int.Parse(start[1]);
            StartDay = int.Parse(start[2]);

            string[] end = string.Format("{0:yyyy-MM-dd}", list[list.Count - 1]).Split('-');
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
                string.Format("SELECT * FROM RECEIPT WHERE Format([RECEIPT_DATE], \"yyyy-mm-dd\") >= '{0}' AND Format([RECEIPT_DATE], \"yyyy-mm-dd\") <= '{1}' ORDER BY RECEIPT_NUM",
                    string.Format("{0:yyyy-MM-dd}", start), string.Format("{0:yyyy-MM-dd}", end));
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
                        date = string.Format("{0:yyyy-MM-dd}", (DateTime)read[1]),
                        time = string.Format("{0:T}", (DateTime)read[1]),
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

        #region DeleteCommand

        private RelayCommand<object> _DeleteCommand;

        public RelayCommand<object> DeleteCommand
        {
            get { return _DeleteCommand ?? (_DeleteCommand = new RelayCommand<object>(Delete)); }
        }

        private void Delete(object obj)
        {
            System.Collections.IList items = (System.Collections.IList)obj;
            var coll = items.Cast<Receipt>();
            List<Receipt> receipts = coll.ToList();

            if (receipts.Count == 0) return;

            // 암호확인
            EnterPasswordWindow epw = new EnterPasswordWindow();
            epw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            epw.ShowDialog();

            // 삭제작업
            if (isDeleted == false)
                return;
            OleDbConnection conn = new OleDbConnection(OleDB.connPath);
            OleDbCommand cmd = new OleDbCommand(null, conn);
            conn.Open();
            foreach (var receipt in receipts)
            {
                Receipts.Remove(receipt);

                // Sale제거, Receipt 제거
                try
                {
                    cmd.CommandText = string.Format("DELETE FROM SALE WHERE RECEIPT_NUM = {0}", receipt.num);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("DELETE FROM RECEIPT WHERE RECEIPT_NUM = {0}", receipt.num);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            conn.Close();
            SellingItems.Clear();
            isDeleted = false;
        }

        #endregion

        #region ChangeCommand

        private RelayCommand _ChangeCommand;

        public RelayCommand ChangeCommand
        {
            get { return _ChangeCommand ?? (_ChangeCommand = new RelayCommand(Change)); }
        }

        private void Change()
        {
            ChangePasswordWindow cpw = new ChangePasswordWindow();
            cpw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            cpw.ShowDialog();
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

        private bool isDeleted = false;

        public AdjustmentUCViewModel()
        {
            Messenger.Default.Register<ViewModelMessage>(this, OnReceiveMessageAction);

            InitPeriod();
        }

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] arr = obj.Text.Split('^');

            switch (arr[0])
            {
                case "DeleteReceipt":
                    isDeleted = true;
                    break;
            }
        }

        private void InitPeriod()
        {
            DateTime today = DateTime.Today;
            string[] day = string.Format("{0:yyyy-MM-dd}", today).Split('-');
            StartYear = int.Parse(day[0]);
            StartMonth = int.Parse(day[1]);
            StartDay = int.Parse(day[2]);
            EndYear = int.Parse(day[0]);
            EndMonth = int.Parse(day[1]);
            EndDay = int.Parse(day[2]);
        }
    }
}
