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
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class InnerMenuSettingViewModel : ViewModelBase
    {
        #region Command

        #region TemperatureCommand

        private RelayCommand _temperatureCommand;

        public RelayCommand TemperatureCommand
        {
            get
            {
                return _temperatureCommand ?? (_temperatureCommand = new RelayCommand(Temperature));
            }
        }

        private void Temperature()
        {
            if (StrTemp == "Ice")
            {
                StrTemp = "Hot";
                BtnTempStyle = Application.Current.FindResource("HotIvoryButton") as Style;
            }
            else
            {
                StrTemp = "Ice";
                BtnTempStyle = Application.Current.FindResource("IceIvoryButton") as Style;
            }

            var msg = new ViewModelMessage()
            {
                Text = "Temperature^" + StrTemp
            };
            Messenger.Default.Send(msg);
        }

        #endregion

        #region SizeCommand

        private RelayCommand _sizeCommand;

        public RelayCommand SizeCommand
        {
            get
            {
                return _sizeCommand ?? (_sizeCommand = new RelayCommand(Size));
            }
        }

        private void Size()
        {
            if (StrSize == "Regular")
            {
                StrSize = "Large";
                BtnSizeStyle = Application.Current.FindResource("YellowIvoryButton") as Style;
            }
            else
            {
                StrSize = "Regular";
                BtnSizeStyle = Application.Current.FindResource("GrayIvoryButton") as Style;
            }

            var msg = new ViewModelMessage()
            {
                Text = "Size^" + StrSize
            };
            Messenger.Default.Send(msg);
        }

        #endregion

        #endregion

        #region Properties

        private Style _btnTempStyle = Application.Current.FindResource("IceIvoryButton") as Style;

        public Style BtnTempStyle
        {
            get { return _btnTempStyle; }
            set
            {
                _btnTempStyle = value;
                RaisePropertyChanged("BtnTempStyle");
            }
        }

        private Style _btnSizeStyle = Application.Current.FindResource("GrayIvoryButton") as Style;

        public Style BtnSizeStyle
        {
            get { return _btnSizeStyle; }
            set
            {
                _btnSizeStyle = value;
                RaisePropertyChanged("BtnSizeStyle");
            }
        }

        private string _strTemp = "Ice";

        public string StrTemp
        {
            get { return _strTemp; }
            set
            {
                _strTemp = value;
                RaisePropertyChanged("StrTemp");
            }
        }

        private string _strSize = "Regular";

        public string StrSize
        {
            get { return _strSize; }
            set
            {
                _strSize = value;
                RaisePropertyChanged("StrSize");
            }
        }

        #endregion

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] strArr = obj.Text.Split('^');
            switch (strArr[0])
            {
                case "InnerTemperature":
                    if (strArr[1] == "Ice")
                    {
                        StrTemp = "Ice";
                        BtnTempStyle = Application.Current.FindResource("IceIvoryButton") as Style;
                    }
                    else if (strArr[1] == "Hot")
                    {
                        StrTemp = "Hot";
                        BtnTempStyle = Application.Current.FindResource("HotIvoryButton") as Style;
                    }
                    break;
                case "InnerSize":
                    if (strArr[1] == "Regular")
                    {
                        StrSize = "Regular";
                        BtnSizeStyle = Application.Current.FindResource("GrayIvoryButton") as Style;
                    }
                    else if (strArr[1] == "Large")
                    {
                        StrSize = "Large";
                        BtnSizeStyle = Application.Current.FindResource("YellowIvoryButton") as Style;
                    }
                    break;
                default:
                    break;
            }
        }

        public InnerMenuSettingViewModel()
        {
            Messenger.Default.Register<ViewModelMessage>(this, OnReceiveMessageAction);
        }
    }
}
