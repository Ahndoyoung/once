using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class AdjustmentViewModel : ViewModelBase
    {
        #region Command

        private RelayCommand<AdjustmentWindow> _ChangeModeCommand;

        public RelayCommand<AdjustmentWindow> ChangeModeCommand
        {
            get { return _ChangeModeCommand ?? (_ChangeModeCommand = new RelayCommand<AdjustmentWindow>(ChangeMode)); }
        }

        private void ChangeMode(AdjustmentWindow aw)
        {
            if (AdjustmentVisible == Visibility.Visible)
            {
                AdjustmentVisible = Visibility.Collapsed;
                StatisticsVisible = Visibility.Visible;

                aw.btnChangeMode.Content = "Adjustment";
            }
            else
            {
                AdjustmentVisible = Visibility.Visible;
                StatisticsVisible = Visibility.Collapsed;

                aw.btnChangeMode.Content = "Statistics";
            }
        }

        #endregion

        #region Properties

        private Visibility _AdjustmentVisible = Visibility.Visible;

        public Visibility AdjustmentVisible
        {
            get { return _AdjustmentVisible; }
            set
            {
                _AdjustmentVisible = value;
                RaisePropertyChanged("AdjustmentVisible");
            }
        }

        private Visibility _StatisticsVisible = Visibility.Collapsed;

        public Visibility StatisticsVisible
        {
            get { return _StatisticsVisible; }
            set
            {
                _StatisticsVisible = value;
                RaisePropertyChanged("StatisticsVisible");
            }
        }

        #endregion
    }
}
