using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Once_v2_2015.View;
using Once_v2_2015.Class;

namespace Once_v2_2015.ViewModel
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        #region Command

        #region ChangeCommand

        private RelayCommand<ChangePasswordWindow> _ChangeCommand;

        public RelayCommand<ChangePasswordWindow> ChangeCommand
        {
            get { return _ChangeCommand ?? (_ChangeCommand = new RelayCommand<ChangePasswordWindow>(Change)); }
        }

        private void Change(ChangePasswordWindow cpw)
        {
            var pbPre = cpw.pbPresent;
            var pbNew = cpw.pbNew;
            var pbCf = cpw.pbNewConfirm;

            if (pbPre.Password == "" || pbNew.Password == "" || pbCf.Password == "")
            {
                MessageBox.Show("빈 칸이 있습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (pbNew.Password != pbCf.Password)
            {
                MessageBox.Show("새 암호와 확인이 다릅니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            if (m_crypt.Encrypt(pbPre.Password) != password)
            {
                MessageBox.Show("현재 암호가 틀렸습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            File.WriteAllText("Password.once", m_crypt.Encrypt(pbNew.Password), Encoding.Default);
            MessageBox.Show("암호가 변경되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            cpw.Close();
        }

        #endregion

        #endregion

        private WATCrypt m_crypt = new WATCrypt("onceonce"); // 암호화,복호화
    }
}
