using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Command

        #region OnClosingCommand

        private RelayCommand _OnClosingCommand;

        public RelayCommand OnClosingCommand
        {
            get { return _OnClosingCommand ?? (_OnClosingCommand = new RelayCommand(OnClosing)); }
        }

        private void OnClosing()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region CounterCommand

        private RelayCommand<Window> _counterCommand;

        public RelayCommand<Window> CounterCommand
        {
            get { return _counterCommand ?? (_counterCommand = new RelayCommand<Window>(Counter)); }
        }

        private void Counter(Window w)
        {
            w.Visibility = Visibility.Collapsed;
            //CounterWindow cw = new CounterWindow();
            //cw.ShowDialog();
            var msg = new ViewModelMessage()
            {
                Text = "StartPOS"
            };
            Messenger.Default.Send<ViewModelMessage>(msg);
        }

        #endregion

        #region AdjustmentCommand

        private RelayCommand<Window> _adjustmentCommand;

        public RelayCommand<Window> AdjustmentCommand
        {
            get { return _adjustmentCommand ?? (_adjustmentCommand = new RelayCommand<Window>(Adjustment)); }
        }

        private void Adjustment(Window w)
        {
            w.Visibility = Visibility.Collapsed;
            AdjustmentWindow aw = new AdjustmentWindow();
            aw.ShowDialog();
            w.Visibility = Visibility.Visible;
        }

        #endregion

        #region SettingCommand

        private RelayCommand<Window> _SettingCommand;

        public RelayCommand<Window> SettingCommand
        {
            get { return _SettingCommand ?? (_SettingCommand = new RelayCommand<Window>(Setting)); }
        }

        private void Setting(Window obj)
        {
            obj.Visibility = Visibility.Collapsed;
            MenuManagementWindow mmw = new MenuManagementWindow();
            mmw.ShowDialog();
            obj.Visibility = Visibility.Visible;
        }

        #endregion

        #endregion

        private CounterWindow counterWindow = new CounterWindow();
        
        public MainViewModel()
        {
            
        }
    }
}