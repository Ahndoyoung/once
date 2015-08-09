using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;

namespace Once_v2_2015.ViewModel
{
    public class DiscountViewModel : ViewModelBase
    {
        #region Command

        #region ClearDiscountCommand

        private RelayCommand _clearDiscountCommand;

        public RelayCommand ClearDiscountCommand
        {
            get { return _clearDiscountCommand ?? (_clearDiscountCommand = new RelayCommand(ClearDiscount)); }
        }

        private void ClearDiscount()
        {
            var msg = new ViewModelMessage()
            {
                Text = "ClearDiscount"
            };
            Messenger.Default.Send(msg);
        }

        #endregion

        #region ApplyDiscountCommand

        private RelayCommand<string> _applyDiscountCommand;

        public RelayCommand<string> ApplyDiscountCommand
        {
            get { return _applyDiscountCommand ?? (_applyDiscountCommand = new RelayCommand<string>(ApplyDiscount)); }
        }

        private void ApplyDiscount(string price)
        {
            var msg = new ViewModelMessage()
            {
                Text = "ApplyDiscount^" + price
            };
            Messenger.Default.Send(msg);
        }
        #endregion

        #endregion

        public DiscountViewModel()
        {
            
        }
    }
}
