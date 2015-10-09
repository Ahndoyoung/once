using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Once_v2_2015.Class;
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class EnterPasswordViewModel : ViewModelBase
    {
        #region Command

        #region OnLoadedCommand

        private RelayCommand<EnterPasswordWindow> _OnLoadedCommand;

        public RelayCommand<EnterPasswordWindow> OnLoadedCommand
        {
            get { return _OnLoadedCommand ?? (_OnLoadedCommand = new RelayCommand<EnterPasswordWindow>(OnLoaded)); }
        }

        private void OnLoaded(EnterPasswordWindow epw)
        {
            epw.passwordBox.Focus();
        }

        #endregion

        private RelayCommand<object> _ConfirmCommand;

        public RelayCommand<object> ConfirmCommand
        {
            get { return _ConfirmCommand ?? (_ConfirmCommand = new RelayCommand<object>(Confirm)); }
        }

        private void Confirm(object obj)
        {
            // 패스워드파일 없으면 확인해봐야돼...
            string password = null;
            try
            {
                password = File.ReadAllLines("Password.once")[0];
            }
            catch
            {
                File.WriteAllText("Password.once", m_crypt.Encrypt("123123123"), Encoding.Default);
                password = File.ReadAllLines("Password.once")[0];
            }

            var epw = obj as EnterPasswordWindow;
            
            if (m_crypt.Encrypt(epw.passwordBox.Password) == password)
            {
                // 성공
                var msg = new ViewModelMessage()
                {
                    Text = "DeleteReceipt"
                };
                Messenger.Default.Send<ViewModelMessage>(msg);

                epw.Close();
            }
            else
            {
                // 실패
                MessageBox.Show("암호가 틀렸습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                epw.passwordBox.Password = null;
                epw.passwordBox.Focus();
            }
        }

        #endregion

        private WATCrypt m_crypt = new WATCrypt("onceonce"); // 암호화,복호화
        
    }
}
