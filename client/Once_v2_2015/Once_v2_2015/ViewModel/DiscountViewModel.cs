using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Once_v2_2015.ViewModel
{
    public class DiscountViewModel : ViewModelBase
    {
        #region Command

        #region CloseCommand

        private RelayCommand<Window> _closeCommand;

        public RelayCommand<Window> CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand<Window>(Close)); }
        }

        private void Close(Window wd)
        {
            wd.Close();
        }
        #endregion

        #region DiscountCommand

        #endregion

        #endregion

        public DiscountViewModel()
        {
            
        }
    }
}
