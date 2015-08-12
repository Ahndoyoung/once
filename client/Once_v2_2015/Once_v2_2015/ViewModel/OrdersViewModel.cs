using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;

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
            if (e.Source.GetType().Name == "ScrollViewer")
            {
                ScrollViewer sv = (ScrollViewer)e.Source;

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
        }

        #endregion

        #endregion

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] arr = obj.Text.Split('^');

            switch (arr[0])
            {
                case "t":

                    break;

                default:
                    break;
            }
        }

        public OrdersViewModel()
        {
            Messenger.Default.Register<ViewModelMessage>(this, OnReceiveMessageAction);
        }
    }
}
