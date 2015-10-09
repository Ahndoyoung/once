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
    public class DefaultDiscountViewModel : ViewModelBase
    {
        #region Command

        #region SaveCommand

        private RelayCommand<DefaultDiscountWindow> _SaveCommand;

        public RelayCommand<DefaultDiscountWindow> SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new RelayCommand<DefaultDiscountWindow>(Save)); }
        }

        private void Save(DefaultDiscountWindow ddw)
        {

            ddw.Close();
        }

        #endregion

        #region CancelCommand

        private RelayCommand<DefaultDiscountWindow> _CancelCommand;

        public RelayCommand<DefaultDiscountWindow> CancelCommand
        {
            get { return _CancelCommand ?? (_CancelCommand = new RelayCommand<DefaultDiscountWindow>(Cancel)); }
        }

        private void Cancel(DefaultDiscountWindow ddw)
        {
            ddw.Close();
        }

        #endregion

        #endregion

        #region Properties

        #endregion
    }
}
