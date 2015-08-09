using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;

namespace Once_v2_2015.ViewModel
{
    public class MenuSettingViewModel : ViewModelBase
    {
        #region Command



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
