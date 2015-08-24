using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup.Primitives;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Once_v2_2015.Model;
using Once_v2_2015.View;
// ReSharper disable InconsistentNaming

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
            LoadCategory();
        }

        #endregion

        #region SaveCommand

        private RelayCommand<MenuManagementWindow> _saveCommand;

        public RelayCommand<MenuManagementWindow> SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand<MenuManagementWindow>(Save)); }
        }

        private void Save(MenuManagementWindow mw)
        {
            MessageBoxResult mbr = MessageBox.Show("저장 후 종료하시겠습니까?\nYes : 저장 후 종료\nNo : 저장\nCancel : 취소", "알림", MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (mbr == MessageBoxResult.Yes)
            {
                SaveCategory();
                mw.Close();
            }
            else if (mbr == MessageBoxResult.No)
            {
                SaveCategory();
            }
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
            MessageBoxResult mbr = MessageBox.Show("취소할 경우 저장이 되지 않습니다.\n취소하시겠습니까?", "알림", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
                mw.Close();
        }

        #endregion

        #region AddCategoryCommand

        private RelayCommand _addCategoryCommand;

        public RelayCommand AddCategoryCommand
        {
            get { return _addCategoryCommand ?? (_addCategoryCommand = new RelayCommand(AddCategory)); }
        }

        private void AddCategory()
        {
            if (CategoryName != null)
            {
                // 중복체크
                foreach (var category in Categories)
                {
                    if (category.name == CategoryName)
                    {
                        MessageBox.Show("이미 있는 이름입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                CategoryProto ct = new CategoryProto()
                {
                    name = CategoryName
                };
                Categories.Add(ct);

                CategoryName = null;
            }
        }

        #endregion

        #region ModifyCategoryCommand

        private RelayCommand _modifyCategoryCommand;

        public RelayCommand ModifyCategoryCommand
        {
            get { return _modifyCategoryCommand ?? (_modifyCategoryCommand = new RelayCommand(ModifyCategory)); }
        }

        private void ModifyCategory()
        {
            if (CategoryName != null && SelectedCategory != null)
            {
                // 중복체크
                foreach (var category in Categories)
                {
                    if (category.name == CategoryName && CategoryName != SelectedCategory.name)
                    {
                        MessageBox.Show("이미 있는 이름입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                SelectedCategory.name = CategoryName;

                CategoryName = null;
            }
        }

        #endregion

        #region DeleteCategoryCommand

        private RelayCommand _deleteCategoryCommand;

        public RelayCommand DeleteCategoryCommand
        {
            get { return _deleteCategoryCommand ?? (_deleteCategoryCommand = new RelayCommand(DeleteCategory)); }
        }

        private void DeleteCategory()
        {
            if (SelectedCategory != null)
            {
                MessageBoxResult mbr = MessageBox.Show("정말로 삭제하시겠습니까?\n" + SelectedCategory.name, "알림",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (mbr == MessageBoxResult.Yes)
                {
                    Categories.Remove(SelectedCategory);
                    MenuItems.Clear();
                }
            }
        }

        #endregion

        #region ChangeCategoryCommand

        private RelayCommand<string> _changeCategoryCommand;

        public RelayCommand<string> ChangeCategoryCommand
        {
            get { return _changeCategoryCommand ?? (_changeCategoryCommand = new RelayCommand<string>(ChangeCategory)); }
        }

        private void ChangeCategory(string obj)
        {
            if (SelectedCategory != null)
            {
                switch (obj)
                {
                    case "up":
                        if (Categories.IndexOf(SelectedCategory) != 0)
                        {
                            int idx = Categories.IndexOf(SelectedCategory);
                            Categories.Move(idx, idx - 1);
                        }
                        break;
                    case "down":
                        if (Categories.IndexOf(SelectedCategory) != Categories.Count - 1)
                        {
                            int idx = Categories.IndexOf(SelectedCategory);
                            Categories.Move(idx, idx + 1);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region OnSelectedCategoryChangedCommand

        private RelayCommand _OnSelectedCategoryChangedCommand;

        public RelayCommand OnSelectedCategoryChangedCommand
        {
            get { return _OnSelectedCategoryChangedCommand ?? (_OnSelectedCategoryChangedCommand = new RelayCommand(OnSelectedCategoryChanged)); }
        }

        private void OnSelectedCategoryChanged()
        {
            if (SelectedCategory != null)
            {
                MenuItems.Clear();

                foreach (var menuItemProto in SelectedCategory.menuProtoList)
                {
                    MenuItems.Add(menuItemProto);
                }
            }
        }

        #endregion

        #region AddMenuCommand

        private RelayCommand _AddMenuCommand;

        public RelayCommand AddMenuCommand
        {
            get { return _AddMenuCommand ?? (_AddMenuCommand = new RelayCommand(AddMenu)); }
        }

        private void AddMenu()
        {
            if (MenuName != null && SelectedCategory != null)
            {
                // 유효성검사
                string chk = Regex.Replace(MenuPrice, @"[0-9]", "");
                string chkL = Regex.Replace(MenuPriceL, @"[0-9]", "");
                if (chk.Length > 0 || chkL.Length > 0)
                {
                    MessageBox.Show("가격에 문자가 포함되어 있습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 중복체크
                foreach (var menuItemProto in MenuItems)
                {
                    if (menuItemProto.name == MenuName)
                    {
                        MessageBox.Show("이미 있는 이름입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                MenuItemProto mip = new MenuItemProto()
                {
                    name = MenuName,
                    price = int.Parse(MenuPrice),
                    size = IsSize,
                    temp = IsTemp,
                    whip = !IsNoWhipping
                };
                if (mip.size == false)
                {
                    mip.priceL = 0;
                }
                else
                {
                    mip.priceL = int.Parse(MenuPriceL);
                }

                SelectedCategory.menuProtoList.Add(mip);
                MenuItems.Add(mip);

                Clear();
            }
        }

        #endregion

        #region ModifyMenuCommand
        
        private RelayCommand _ModifyMenuCommand;

        public RelayCommand ModifyMenuCommand
        {
            get { return _ModifyMenuCommand ?? (_ModifyMenuCommand = new RelayCommand(ModifyMenu)); }
        }

        private void ModifyMenu()
        {
            if (MenuName != null && SelectedMenuItem != null)
            {
                // 유효성검사
                string chk = Regex.Replace(MenuPrice, @"[0-9]", "");
                string chkL = Regex.Replace(MenuPriceL, @"[0-9]", "");
                if (chk.Length > 0 || chkL.Length > 0)
                {
                    MessageBox.Show("가격에 문자가 포함되어 있습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 중복체크
                foreach (var menuItemProto in MenuItems)
                {
                    if (menuItemProto.name == MenuName && MenuName != SelectedMenuItem.name)
                    {
                        MessageBox.Show("이미 있는 이름입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                SelectedMenuItem.name = MenuName;
                SelectedMenuItem.price = int.Parse(MenuPrice);
                SelectedMenuItem.size = IsSize;
                SelectedMenuItem.temp = IsTemp;
                SelectedMenuItem.whip = !IsNoWhipping;
                if (SelectedMenuItem.size == true)
                {
                    SelectedMenuItem.priceL = int.Parse(MenuPriceL);
                }
                else
                {
                    SelectedMenuItem.priceL = 0;
                }

                Clear();
            }
        }

        #endregion

        #region DeleteMenuCommand

        private RelayCommand _deleteMenuCommand;

        public RelayCommand DeleteMenuCommand
        {
            get { return _deleteMenuCommand ?? (_deleteMenuCommand = new RelayCommand(DeleteMenu)); }
        }

        private void DeleteMenu()
        {
            if (SelectedMenuItem != null)
            {
                MessageBoxResult mbr = MessageBox.Show("정말로 삭제하시겠습니까?\n" + SelectedMenuItem.name, "알림",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (mbr == MessageBoxResult.Yes)
                {
                    SelectedCategory.menuProtoList.Remove(SelectedMenuItem);
                    MenuItems.Remove(SelectedMenuItem);
                }
            }
        }

        #endregion

        #region ChangeMenuCommand

        private RelayCommand<string> _changeMenuCommand;

        public RelayCommand<string> ChangeMenuCommand
        {
            get { return _changeMenuCommand ?? (_changeMenuCommand = new RelayCommand<string>(ChangeMenu)); }
        }

        private void ChangeMenu(string obj)
        {
            if (SelectedMenuItem != null)
            {
                switch (obj)
                {
                    case "up":
                        if (MenuItems.IndexOf(SelectedMenuItem) != 0)
                        {
                            int idx = MenuItems.IndexOf(SelectedMenuItem);
                            var tmp = SelectedCategory.menuProtoList[idx];
                            SelectedCategory.menuProtoList[idx] = SelectedCategory.menuProtoList[idx - 1];
                            SelectedCategory.menuProtoList[idx - 1] = tmp;
                            MenuItems.Move(idx, idx - 1);
                        }
                        break;
                    case "down":
                        if (MenuItems.IndexOf(SelectedMenuItem) != MenuItems.Count - 1)
                        {
                            int idx = MenuItems.IndexOf(SelectedMenuItem);
                            var tmp = SelectedCategory.menuProtoList[idx];
                            SelectedCategory.menuProtoList[idx] = SelectedCategory.menuProtoList[idx + 1];
                            SelectedCategory.menuProtoList[idx + 1] = tmp;
                            MenuItems.Move(idx, idx + 1);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region ClearCommand

        private RelayCommand _ClearCommand;

        public RelayCommand ClearCommand
        {
            get { return _ClearCommand ?? (_ClearCommand = new RelayCommand(Clear)); }
        }

        private void Clear()
        {
            MenuName = null;
            IsTemp = true;
            IsSize = true;
            IsNoWhipping = true;
            MenuPrice = "0";
            MenuPriceL = "0";
        }

        #endregion

        #endregion

        #region Properties

        private ObservableCollection<CategoryProto> _categories = new ObservableCollection<CategoryProto>();

        public ObservableCollection<CategoryProto> Categories
        {
            get { return _categories; }
        }

        private ObservableCollection<MenuItemProto> _menuItems = new ObservableCollection<MenuItemProto>();

        public ObservableCollection<MenuItemProto> MenuItems
        {
            get { return _menuItems; }
        }

        private CategoryProto _selectedCategory;

        public CategoryProto SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }

        private MenuItemProto _selectedMenuItem;

        public MenuItemProto SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                RaisePropertyChanged("SelectedMenuItem");
            }
        }


        private string _categoryName = null;

        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                _categoryName = value;
                RaisePropertyChanged("CategoryName");
            }
        }

        #region MenuProperties

        private string _menuName = null;

        public string MenuName
        {
            get { return _menuName; }
            set
            {
                _menuName = value;
                RaisePropertyChanged("MenuName");
            }
        }


        private bool _IsTemp = true;

        public bool IsTemp
        {
            get { return _IsTemp; }
            set
            {
                _IsTemp = value;
                RaisePropertyChanged("IsTemp");
            }
        }
        
        private bool _IsSize = true;

        public bool IsSize
        {
            get { return _IsSize; }
            set
            {
                _IsSize = value;
                RaisePropertyChanged("IsSize");
            }
        }

        private bool _IsNoWhipping = true;

        public bool IsNoWhipping
        {
            get { return _IsNoWhipping; }
            set
            {
                _IsNoWhipping = value;
                RaisePropertyChanged("IsNoWhipping");
            }
        }

        private string _MenuPrice = "0";

        public string MenuPrice
        {
            get { return _MenuPrice; }
            set
            {
                _MenuPrice = value;
                RaisePropertyChanged("MenuPrice");
            }
        }
        
        private string _MenuPriceL = "0";

        public string MenuPriceL
        {
            get { return _MenuPriceL; }
            set
            {
                _MenuPriceL = value;
                RaisePropertyChanged("MenuPriceL");
            }
        }

        #endregion

        #endregion

        public List<Category> categories = new List<Category>();
        public List<CategoryProto> cateProtoes = new List<CategoryProto>();

        private static List<Category> LoadXML()
        {
            XmlSerializer serializer = null;
            FileStream stream = null;
            List<Category> category = new List<Category>();
            try
            {
                serializer = new XmlSerializer(typeof(List<Category>));

                stream = new FileStream("MenuList.xml", FileMode.Open);
                category = (List<Category>)serializer.Deserialize(stream);
            }
            catch
            {
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return category;
        }

        public void SaveXML(List<Category> category)
        {
            XmlSerializer serializer = null;
            FileStream stream = null;
            
            try
            {
                serializer = new XmlSerializer(typeof(List<Category>));

                stream = new FileStream("MenuList.xml", FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, category);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private void SaveCategory()
        {
            // List<Category> 완성시켜야됨
            List<Category> saveCate = new List<Category>();
            foreach (var categoryProto in Categories)
            {
                Category cate = new Category();
                cate.name = categoryProto.name;
                foreach (var menuItemProto in categoryProto.menuProtoList)
                {
                    if (menuItemProto.temp == true && menuItemProto.size == true)
                    {
                        // IR IL HR HL
                        MenuItem mi1 = new MenuItem();
                        mi1.name = menuItemProto.name;
                        mi1.temperature = 'I';
                        mi1.size = 'R';
                        mi1.isWhipping = menuItemProto.whip;
                        mi1.price = mi1.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi1);

                        MenuItem mi2 = new MenuItem();
                        mi2.name = menuItemProto.name;
                        mi2.temperature = 'I';
                        mi2.size = 'L';
                        mi2.isWhipping = menuItemProto.whip;
                        mi2.price = mi2.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi2);

                        MenuItem mi3 = new MenuItem();
                        mi3.name = menuItemProto.name;
                        mi3.temperature = 'H';
                        mi3.size = 'R';
                        mi3.isWhipping = menuItemProto.whip;
                        mi3.price = mi3.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi3);

                        MenuItem mi4 = new MenuItem();
                        mi4.name = menuItemProto.name;
                        mi4.temperature = 'I';
                        mi4.size = 'L';
                        mi4.isWhipping = menuItemProto.whip;
                        mi4.price = mi4.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi4);
                    }
                    else if(menuItemProto.temp == true && menuItemProto.size == false)
                    {
                        // IN HN
                        MenuItem mi1 = new MenuItem();
                        mi1.name = menuItemProto.name;
                        mi1.temperature = 'I';
                        mi1.size = null;
                        mi1.isWhipping = menuItemProto.whip;
                        mi1.price = mi1.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi1);

                        MenuItem mi2 = new MenuItem();
                        mi2.name = menuItemProto.name;
                        mi2.temperature = 'H';
                        mi2.size = null;
                        mi2.isWhipping = menuItemProto.whip;
                        mi2.price = mi2.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi2);
                    }
                    else if (menuItemProto.temp == false && menuItemProto.size == true)
                    {
                        // NR NL
                        MenuItem mi1 = new MenuItem();
                        mi1.name = menuItemProto.name;
                        mi1.temperature = null;
                        mi1.size = 'R';
                        mi1.isWhipping = menuItemProto.whip;
                        mi1.price = mi1.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi1);

                        MenuItem mi2 = new MenuItem();
                        mi2.name = menuItemProto.name;
                        mi2.temperature = null;
                        mi2.size = 'L';
                        mi2.isWhipping = menuItemProto.whip;
                        mi2.price = mi2.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi2);
                    }
                    else if (menuItemProto.temp == false && menuItemProto.size == false)
                    {
                        MenuItem mi1 = new MenuItem();
                        mi1.name = menuItemProto.name;
                        mi1.temperature = null;
                        mi1.size = null;
                        mi1.isWhipping = menuItemProto.whip;
                        mi1.price = mi1.size == 'L' ? menuItemProto.priceL : menuItemProto.price;
                        cate.menuList.Add(mi1);
                    }
                }
                saveCate.Add(cate);
            }

            SaveXML(saveCate);
        }

        private void LoadCategory()
        {
            Categories.Clear();
            categories.Clear();
            cateProtoes.Clear();

            categories = LoadXML();

            foreach (var category in categories)
            {
                CategoryProto cp = new CategoryProto();
                cp.name = category.name;

                for (int i = 0; i < category.menuList.Count; i++)
                {
                    MenuItemProto mip = new MenuItemProto();
                    mip.name = category.menuList[i].name;
                    mip.temp = category.menuList[i].temperature != null ? true : false;
                    mip.size = category.menuList[i].size != null ? true : false;
                    mip.whip = category.menuList[i].isWhipping;
                    mip.price = category.menuList[i].price;
                    mip.priceL = mip.size == true ? category.menuList[i + 1].price : 0;

                    cp.menuProtoList.Add(mip);

                    if (mip.temp == true && mip.size == true)
                        i += 3;
                    else if (mip.temp == true && mip.size == false)
                        i += 1;
                    else if (mip.temp == false && mip.size == true)
                        i += 1;
                }

                cateProtoes.Add(cp);
            }

            foreach (var categoryProto in cateProtoes)
            {
                Categories.Add(categoryProto);
            }
        }
    }
}
