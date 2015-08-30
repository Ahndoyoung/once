using System.Diagnostics;
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

        #region OnLoadedCommand

        private RelayCommand _OnLoadedCommand;

        public RelayCommand OnLoadedCommand
        {
            get { return _OnLoadedCommand ?? (_OnLoadedCommand = new RelayCommand(OnLoaded)); }
        }

        private void OnLoaded()
        {
            // 중복실행 방지
            Process[] procs = Process.GetProcessesByName("Once_v2_2015");
            if (procs.Length > 1)
            {
                MessageBox.Show("이미 실행 중입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        #endregion

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

        private RelayCommand _counterCommand;

        public RelayCommand CounterCommand
        {
            get { return _counterCommand ?? (_counterCommand = new RelayCommand(Counter)); }
        }

        private void Counter()
        {
            MainWindowVisible = Visibility.Collapsed;
            var msg = new ViewModelMessage()
            {
                Text = "StartPOS"
            };
            Messenger.Default.Send<ViewModelMessage>(msg);
        }

        #endregion

        #region AdjustmentCommand

        private RelayCommand _adjustmentCommand;

        public RelayCommand AdjustmentCommand
        {
            get { return _adjustmentCommand ?? (_adjustmentCommand = new RelayCommand(Adjustment)); }
        }

        private void Adjustment()
        {
            MainWindowVisible = Visibility.Collapsed;
            AdjustmentWindow aw = new AdjustmentWindow();
            aw.ShowDialog();
            MainWindowVisible = Visibility.Visible;
        }

        #endregion

        #region SettingCommand

        private RelayCommand _SettingCommand;

        public RelayCommand SettingCommand
        {
            get { return _SettingCommand ?? (_SettingCommand = new RelayCommand(Setting)); }
        }

        private void Setting()
        {
            MainWindowVisible = Visibility.Collapsed;
            MenuManagementWindow mmw = new MenuManagementWindow();
            mmw.ShowDialog();
            MainWindowVisible = Visibility.Visible;
        }

        #endregion

        #endregion

        #region Properties
        
        private Visibility _MainWindowVisible = Visibility.Visible;

        public Visibility MainWindowVisible
        {
            get { return _MainWindowVisible; }
            set
            {
                try
                {
                    _MainWindowVisible = value;
                    RaisePropertyChanged("MainWindowVisible");
                }
                catch
                {
                }
            }
        }

        #endregion

        private CounterWindow counterWindow = new CounterWindow();

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] arr = obj.Text.Split('^');

            switch (arr[0])
            {
                case "ShowMain":
                    MainWindowVisible = Visibility.Visible;
                    break;
                case "HideMain":
                    MainWindowVisible = Visibility.Collapsed;
                    break;
            }
        }

        public MainViewModel()
        {
            Messenger.Default.Register<ViewModelMessage>(this, OnReceiveMessageAction);
        }
    }
}