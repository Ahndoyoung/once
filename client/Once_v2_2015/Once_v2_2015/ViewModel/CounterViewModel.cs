using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Once_v2_2015.Class;
using Once_v2_2015.Model;
using Once_v2_2015.View;


namespace Once_v2_2015.ViewModel
{
    public class CounterViewModel : ViewModelBase
    {
        #region Command

        #region TestCommand

        private RelayCommand _testCommand;

        public RelayCommand TestCommand
        {
            get { return _testCommand ?? (_testCommand = new RelayCommand(Test)); }
        }

        private void Test()
        {
            MessageBox.Show("test");
        }
        #endregion

        #region LoadedCommand

        private RelayCommand<CounterWindow> _loadedCommand;

        public RelayCommand<CounterWindow> LoadedCommand
        {
            get
            {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand<CounterWindow>(Loaded));
            }
        }

        private void Loaded(CounterWindow cw)
        {
            categories = LoadCategory(Properties.Resources.MenuList);
            SetCategory(cw);

            cw.grdOutterMenuSetting.Children.Add(menuSettingView);
            int idx = cw.grdOutterMenuSetting.Children.IndexOf(menuSettingView);
            cw.grdOutterMenuSetting.Children[idx].Visibility = Visibility.Collapsed;
        }
        #endregion

        #region HomeCommand

        private RelayCommand<CounterWindow> _homeCommand;

        public RelayCommand<CounterWindow> HomeCommand
        {
            get { return _homeCommand ?? (_homeCommand = new RelayCommand<CounterWindow>(Home)); }
        }

        private void Home(CounterWindow cw)
        {
            SetCategory(cw);
        }
        #endregion

        #region LoadMenuCommand

        private RelayCommand<object> _loadMenuCommand;

        public RelayCommand<object> LoadMenuCommand
        {
            get { return _loadMenuCommand ?? (_loadMenuCommand = new RelayCommand<object>(LoadMenu)); }
        }

        private void LoadMenu(object obj)
        {
            var objArr = (object[])obj;
            CounterWindow cw = (CounterWindow)objArr[0];
            int idx = (int)objArr[1];

            cw.grdInnerMenu.Children.Clear();
            cw.btnCategory.Visibility = Visibility.Visible;
            cw.btnCategory.Content = categories[idx].name;

            int row = 0;
            int col = 0;
            for (int i = 0; i < categories[idx].menuList.Count; i++)
            {
                if ((categories[idx].menuList[i].temperature == 'i' && categories[idx].menuList[i].size == 'r')
                    || (categories[idx].menuList[i].temperature == 'i' && categories[idx].menuList[i].size == null)
                    || (categories[idx].menuList[i].temperature == null && categories[idx].menuList[i].size == 'r')
                    || (categories[idx].menuList[i].temperature == null && categories[idx].menuList[i].size == null))
                {
                    if (col == MenuPosition.CntPerRow)
                        row++;
                    col = col % MenuPosition.CntPerRow;
                    
                    int left = MenuPosition.InitLeft + MenuPosition.MarginLeft * col;
                    int right = MenuPosition.InitRight - MenuPosition.MarginLeft * col;
                    int y = MenuPosition.InitY + MenuPosition.MarginTop*row;

                    Button btn = new Button();
                    btn.Width = 180;
                    btn.Height = 130;

                    Style style = Application.Current.FindResource("IvoryButton") as Style;
                    btn.Style = style;

                    btn.Content = categories[idx].menuList[i].name.Replace('^', '\n');
                    btn.FontFamily = new FontFamily("NanumBarunGothic");
                    btn.FontSize = (double) new FontSizeConverter().ConvertFrom("13pt");
                    btn.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0x3A, 0x17));

                    btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn.VerticalAlignment = VerticalAlignment.Top;
                    btn.Margin = new Thickness(left, y, right, 0);

                    btn.Command = AddMenuItemCommand;
                    string name = categories[idx].menuList[i].name;
                    object[] ob = new object[] {cw, name};
                    btn.CommandParameter = ob;

                    cw.grdInnerMenu.Children.Add(btn);

                    col++;
                }
            }
        }
        #endregion

        #region DiscountCommand

        private RelayCommand<object> _discountCommand;

        public RelayCommand<object> DiscountCommand
        {
            get { return _discountCommand ?? (_discountCommand = new RelayCommand<object>(Discount)); }
        }

        private void Discount(object obj)
        {
            var values = (object[])obj;
            Border brd = (Border)values[0];
            ListView lv = (ListView)values[1];

            if (brd.Visibility == Visibility.Collapsed)
            {
                brd.Visibility = Visibility.Visible;
                lv.Margin = new Thickness(lv.Margin.Left, lv.Margin.Top, lv.Margin.Right, lv.Margin.Bottom + 105);
            }
            else
            {
                brd.Visibility = Visibility.Collapsed;
                lv.Margin = new Thickness(lv.Margin.Left, lv.Margin.Top, lv.Margin.Right, lv.Margin.Bottom - 105);
            }
        }
        #endregion

        #region MenuSettingCommand

        private RelayCommand<CounterWindow> _menuSettingCommand;

        public RelayCommand<CounterWindow> MenuSettingCommand
        {
            get { return _menuSettingCommand ?? (_menuSettingCommand = new RelayCommand<CounterWindow>(MenuSetting)); }
        }

        private void MenuSetting(CounterWindow cw)
        {
            Grid grdOutter = cw.grdOutterMenuSetting;
            Grid grdInner = cw.grdInnerMenuSetting;

            int idx = grdOutter.Children.IndexOf(menuSettingView);

            if (grdInner.Visibility != Visibility.Visible)
            {
                grdOutter.Height -= 100;
                grdOutter.Children[idx].Visibility = Visibility.Collapsed;
                grdInner.Visibility = Visibility.Visible;
            }
            else
            {
                grdOutter.Height += 100;
                grdOutter.Children[idx].Visibility = Visibility.Visible;
                grdInner.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region AddMenuItemCommand

        private RelayCommand<object> _addMenuItemCommand;

        public RelayCommand<object> AddMenuItemCommand
        {
            get { return _addMenuItemCommand ?? (_addMenuItemCommand = new RelayCommand<object>(AddMenuItem)); }
        }

        private void AddMenuItem(object obj)
        {
            object[] values = (object[])obj;
            CounterWindow cw = (CounterWindow)values[0];
            string name = (string)values[1];

            int price = 0;

            bool isNullTemp = true;
            bool isNullSize = true;
            foreach (var category in categories)
            {
                foreach (var menuItem in category.menuList)
                {
                    if (menuItem.name == name)
                    {
                        price = menuItem.price;
                        if (menuItem.temperature != null)
                        {
                            isNullTemp = false;
                        }
                        if (menuItem.size != null)
                        {
                            isNullSize = false;
                        }
                        break;
                    }
                }
            }

            char? checkedTemp = null;
            if (!isNullTemp)
            {
                if (cw.InnerMenuSettingView.btnTemperature.Content.ToString() == "Ice")
                {
                    checkedTemp = 'i';
                }
                else if (cw.InnerMenuSettingView.btnTemperature.Content.ToString() == "Hot")
                {
                    checkedTemp = 'h';
                }
            }            
            char? checkedSize = null;
            if (!isNullSize)
            {
                if (cw.InnerMenuSettingView.btnSize.Content.ToString() == "Regular")
                {
                    checkedSize = 'r';
                }
                else if (cw.InnerMenuSettingView.btnSize.Content.ToString() == "Large")
                {
                    checkedSize = 'l';
                }
            }

            bool isExist = false;
            foreach (var sellingItem in SellingItems)
            {
                if (sellingItem.name == name && sellingItem.temperature == checkedTemp && sellingItem.size == checkedSize)
                {
                    sellingItem.quantity++;
                    sellingItem.total = sellingItem.price * sellingItem.quantity;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                SellingItem si = new SellingItem(name, checkedTemp, checkedSize, price);
                SellingItems.Add(si);
            }
            SubTotal = (int.Parse(SubTotal) + price).ToString();
            Total = (int.Parse(SubTotal) - int.Parse(DiscountPrice)).ToString();
        }
        #endregion

        #region CancelOrderCommand

        private RelayCommand _cancelOrderCommand;

        public RelayCommand CancelOrderCommand
        {
            get { return _cancelOrderCommand ?? (_cancelOrderCommand = new RelayCommand(CancelOrder));}
        }

        private void CancelOrder()
        {
            SellingItems.Clear();
            DiscountPrice = "0";
            SubTotal = "0";
            Total = "0";
        }

        #endregion

        #endregion

        #region Properties

        private ObservableCollection<SellingItem> _sellingItems = new ObservableCollection<SellingItem>();

        public ObservableCollection<SellingItem> SellingItems
        {
            get { return _sellingItems; }
        }

        private string _discountPrice = "0";

        public string DiscountPrice
        {
            get { return _discountPrice; }
            set
            {
                _discountPrice = value;
                RaisePropertyChanged("DiscountPrice");
            }
        }

        private string _subTotal = "0";

        public string SubTotal
        {
            get { return _subTotal; }
            set
            {
                _subTotal = value;
                RaisePropertyChanged("SubTotal");
            }
        }

        private string _total ="0";

        public string Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged("Total");
            }
        }

        #endregion

        public MenuSettingView menuSettingView = new MenuSettingView();

        public List<Category> categories = new List<Category>();

        private static List<Category> LoadCategory(string content)
        {
            XmlSerializer serializer = null;
            MemoryStream stream = null;
            List<Category> category = new List<Category>();
            try
            {
                serializer = new XmlSerializer(typeof(List<Category>));

                byte[] byteArr = Encoding.UTF8.GetBytes(content);
                stream = new MemoryStream(byteArr);
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

        public void SetCategory(CounterWindow cw)
        {
            cw.grdInnerMenu.Children.Clear();
            cw.btnCategory.Visibility = Visibility.Collapsed;
            
            for (int i = 0; i < categories.Count; i++)
            {
                int row = i / MenuPosition.CntPerRow;
                int col = i % 4;
                int left = MenuPosition.InitLeft + MenuPosition.MarginLeft * col;
                int right = MenuPosition.InitRight - MenuPosition.MarginLeft * col;
                int y = MenuPosition.InitY + MenuPosition.MarginTop * row;

                Button btn = new Button();
                btn.Width = 180;
                btn.Height = 130;

                Style style = Application.Current.FindResource("IvoryButton") as Style;
                btn.Style = style;

                btn.Content = categories[i].name;
                btn.FontFamily = new FontFamily("NanumBarunGothic");
                btn.FontSize = (double)new FontSizeConverter().ConvertFrom("13pt");
                btn.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0x3A, 0x17));

                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(left, y, right, 0);

                btn.Command = LoadMenuCommand;
                object[] obj = new object[] { cw, i };
                btn.CommandParameter = obj;

                cw.grdInnerMenu.Children.Add(btn);
            }
        }

        public CounterViewModel()
        {
        }
    }
}
