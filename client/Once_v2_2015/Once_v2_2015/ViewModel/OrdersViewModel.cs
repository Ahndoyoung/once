using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Once_v2_2015.ViewModel
{
    public class OrdersViewModel : ViewModelBase
    {
        #region Command

        #region HorizontalScrollingCommand

        private RelayCommand<MouseWheelEventArgs> _horizontalScrollingCommand;

        public RelayCommand<MouseWheelEventArgs> HorizontalScrollingCommand
        {
            get
            {
                return _horizontalScrollingCommand ??
                       (_horizontalScrollingCommand = new RelayCommand<MouseWheelEventArgs>(HorizontalScrolling));
            }
        }

        private void HorizontalScrolling(MouseWheelEventArgs e)
        {
            ScrollViewer sv = (ScrollViewer) e.Source;

            if (e.Delta > 0)
            {
                sv.LineLeft();
                sv.LineLeft();
            }
            else
            {
                sv.LineRight();
                sv.LineRight();
            }
            e.Handled = true;
        }

        #endregion

        #endregion
    }
}
