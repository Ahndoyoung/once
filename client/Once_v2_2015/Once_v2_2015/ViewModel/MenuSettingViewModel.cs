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

        #endregion

        #region Properties

        #endregion

        public MenuSettingViewModel()
        {
        }
    }
}
