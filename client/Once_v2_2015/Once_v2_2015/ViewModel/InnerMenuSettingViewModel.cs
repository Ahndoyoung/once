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

        private RelayCommand<InnerMenuSettingUC> _temperatureCommand;

        public RelayCommand<InnerMenuSettingUC> TemperatureCommand
        {
            get
            {
                return _temperatureCommand ?? (_temperatureCommand = new RelayCommand<InnerMenuSettingUC>(Temperature));
            }
        }

        private void Temperature(InnerMenuSettingUC ims)
        {
            if (ims.btnTemperature.Content.ToString() == "Ice")
            {
                ims.btnTemperature.Content = "Hot";
                Style style = Application.Current.FindResource("HotIvoryButton") as Style;
                ims.btnTemperature.Style = style;
            }
            else
            {
                ims.btnTemperature.Content = "Ice";
                Style style = Application.Current.FindResource("IceIvoryButton") as Style;
                ims.btnTemperature.Style = style;
            }

            var msg = new ViewModelMessage()
            {
                Text = "Temperature^" + ims.btnTemperature.Content.ToString()
            };
            Messenger.Default.Send(msg);
        }

        #endregion

        #region SizeCommand

        private RelayCommand<InnerMenuSettingUC> _sizeCommand;

        public RelayCommand<InnerMenuSettingUC> SizeCommand
        {
            get
            {
                return _sizeCommand ?? (_sizeCommand = new RelayCommand<InnerMenuSettingUC>(Size));
            }
        }

        private void Size(InnerMenuSettingUC ims)
        {
            if (ims.btnSize.Content.ToString() == "Regular")
            {
                ims.btnSize.Content = "Large";
                Style style = Application.Current.FindResource("YellowIvoryButton") as Style;
                ims.btnSize.Style = style;
            }
            else
            {
                ims.btnSize.Content = "Regular";
                Style style = Application.Current.FindResource("GrayIvoryButton") as Style;
                ims.btnSize.Style = style;
            }

            var msg = new ViewModelMessage()
            {
                Text = "Size^" + ims.btnSize.Content.ToString()
            };
            Messenger.Default.Send(msg);
        }

        #endregion

        #endregion

        public InnerMenuSettingViewModel()
        {
            
        }
    }
}
