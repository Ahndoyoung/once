using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class MenuSettingViewModel : ViewModelBase
    {
        #region Command

        #region AddOptionCommand

        private RelayCommand<string> _addOptionCommand;

        public RelayCommand<string> AddOptionCommand
        {
            get { return _addOptionCommand ?? (_addOptionCommand = new RelayCommand<string>(AddOption)); }
        }

        private void AddOption(string str)
        {
            var msg = new ViewModelMessage()
            {
                Text = "AddOption^ * " + str
            };
            Messenger.Default.Send(msg);
        }

        #endregion

        #region TemperatureCommand

        private RelayCommand<MenuSettingView> _temperatureCommand;

        public RelayCommand<MenuSettingView> TemperatureCommand
        {
            get { return _temperatureCommand ?? (_temperatureCommand = new RelayCommand<MenuSettingView>(Temperature)); }
        }

        private void Temperature(MenuSettingView mv)
        {
            string text = "InnerTemperature^";
            if (mv.rbIce.IsChecked == true)
            {
                text += "Ice";
            }
            else if (mv.rbHot.IsChecked == true)
            {
                text += "Hot";
            }

            var msg = new ViewModelMessage()
            {
                Text = text
            };
            Messenger.Default.Send<ViewModelMessage>(msg);
        }

        #endregion

        #region InnerSizeCommand

        private RelayCommand<MenuSettingView> _innerSizeCommand;

        public RelayCommand<MenuSettingView> InnerSizeCommand
        {
            get { return _innerSizeCommand ?? (_innerSizeCommand = new RelayCommand<MenuSettingView>(InnerSize)); }
        }

        private void InnerSize(MenuSettingView mv)
        {
            string text = "InnerSize^";
            if (mv.rbRegular.IsChecked == true)
            {
                text += "Regular";
            }
            else if (mv.rbLarge.IsChecked == true)
            {
                text += "Large";
            }

            var msg = new ViewModelMessage()
            {
                Text = text
            };
            Messenger.Default.Send<ViewModelMessage>(msg);
        }

        #endregion

        #endregion

        #region Properties

        #region IsTemperature

        private bool _isIce = true;

        public bool IsIce
        {
            get { return _isIce; }
            set
            {
                _isIce = value;
                RaisePropertyChanged("IsIce");
            }
        }

        private bool _isHot = false;

        public bool IsHot
        {
            get { return _isHot; }
            set
            {
                _isHot = value;
                RaisePropertyChanged("IsHot");
            }
        }

        #endregion

        #region IsSize

        private bool _isRegular = true;

        public bool IsRegular
        {
            get { return _isRegular; }
            set
            {
                _isRegular = value;
                RaisePropertyChanged("IsRegular");
            }
        }

        private bool _isLarge = false;

        public bool IsLarge
        {
            get { return _isLarge; }
            set
            {
                _isLarge = value;
                RaisePropertyChanged("IsLarge");
            }
        }

        #endregion

        #endregion

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] strArr = obj.Text.Split('^');
            if (strArr[0] == "Temperature")
            {
                if (strArr[1] == "Ice")
                {
                    IsIce = true;
                    IsHot = false;
                }
                else
                {
                    IsIce = false;
                    IsHot = true;
                }
            }
            else if (strArr[0] == "Size")
            {
                if (strArr[1] == "Regular")
                {
                    IsRegular = true;
                    IsLarge = false;
                }
                else if (strArr[1] == "Large")
                {
                    IsRegular = false;
                    IsLarge = true;
                }
            }
        }

        public MenuSettingViewModel()
        {
            Messenger.Default.Register<ViewModelMessage>(this, OnReceiveMessageAction);
        }
    }
}
