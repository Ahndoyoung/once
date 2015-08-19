using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup.Primitives;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Once_v2_2015.Model;
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class MenuManagementViewModel : ViewModelBase
    {
        #region Command

        #region OnLoadedCommand

        private RelayCommand _onLoadedCommand;

        public RelayCommand OnLoadedCommand
        {
            get { return _onLoadedCommand ?? (_onLoadedCommand = new RelayCommand(OnLoaded)); }
        }

        private void OnLoaded()
        {
            // xml 불러오기
        }

        #endregion

        #region SaveCommand

        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save)); }
        }

        private void Save()
        {

        }

        #endregion
        
        #region CancelCommand

        private RelayCommand<MenuManagementWindow> _cancelCommand;

        public RelayCommand<MenuManagementWindow> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<MenuManagementWindow>(Cancel)); }
        }

        private void Cancel(MenuManagementWindow mw)
        {
            mw.Close();
            
        }

        #endregion

        #region AddCategory

        #endregion

        #endregion

        #region Properties

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();

        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
        }

        private ObservableCollection<MenuItemProto> _menuItems = new ObservableCollection<MenuItemProto>();

        public ObservableCollection<MenuItemProto> MenuItems
        {
            get { return _menuItems; }
        }

        #endregion
    }
}
