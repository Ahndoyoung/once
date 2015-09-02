using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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

        }

        #endregion

        #endregion

        #region Properties

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
