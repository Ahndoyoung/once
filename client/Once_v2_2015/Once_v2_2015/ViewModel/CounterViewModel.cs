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
        }

        #endregion

        #region FireMenuItemCommand

        private RelayCommand _fireMenuItemCommand;

        public RelayCommand FireMenuItemCommand
        {
            get { return _fireMenuItemCommand ?? (_fireMenuItemCommand = new RelayCommand(FireMenuItem)); }
        }

        private void FireMenuItem()
        {
            if (SelectedSellingItem == null) return;


            SubTotal = (int.Parse(SubTotal) - SelectedSellingItem.price).ToString();

            if (SelectedSellingItem.quantity != 1)
            {
                SelectedSellingItem.quantity--;
            }
            else
            {
                SellingItems.Remove(SelectedSellingItem);
            }
        }

        #endregion

        #region ViewOrdersCommand

        private RelayCommand _viewOrdersCommand;

        public RelayCommand ViewOrdersCommand
        {
            get { return _viewOrdersCommand ?? (_viewOrdersCommand = new RelayCommand(ViewOrders)); }
        }

        private void ViewOrders()
        {
            if (CounterVisible == Visibility.Visible)
            {
                CounterVisible = Visibility.Collapsed;
                OrdersVisible = Visibility.Visible;

            }
            else
            {
                CounterVisible = Visibility.Visible;
                OrdersVisible = Visibility.Collapsed;
            }
        }

        #endregion

        #region SendOrderCommand

        private RelayCommand<OrdersUC> _sendOrderCommand;

        public RelayCommand<OrdersUC> SendOrderCommand
        {
            get { return _sendOrderCommand ?? (_sendOrderCommand = new RelayCommand<OrdersUC>(SendOrder)); }
        }

        private void SendOrder(OrdersUC ov)
        {
            var items = new ObservableCollection<SellingItem>();
            foreach (var si in SellingItems)
            {
                items.Add(si);
            }
            

            if (items.Count != 0)
            {
                Grid grd = new Grid();
                // TextBlock
                TextBlock tb = new TextBlock();
                tb.Margin = new Thickness(OrderPosition.InitTbLeft, OrderPosition.InitTbTop, OrderPosition.InitTbRight,
                    0);
                tb.VerticalAlignment = VerticalAlignment.Top;
                tb.TextAlignment = TextAlignment.Center;

                tb.Text = "Order #" + (OrderNumber++).ToString();
                tb.FontSize = (double) new FontSizeConverter().ConvertFrom("22pt");
                tb.FontFamily = new FontFamily("Segoe Print");
                tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0x3A, 0x17));
                grd.Children.Add(tb);

                // ListView
                ListView lv = new ListView();
                lv.ItemsSource = items;
                lv.Margin = new Thickness(OrderPosition.InitLvLeft, OrderPosition.InitLvTop, OrderPosition.InitLvRight,
                    OrderPosition.InitLvBottom);
                lv.SelectionMode = SelectionMode.Single;
                lv.FontSize = (double) new FontSizeConverter().ConvertFrom("13pt");
                lv.FontFamily = new FontFamily("NanumBarunGothic");
                lv.Foreground = Brushes.Black;
                Style style = Application.Current.FindResource("ColorfulListView") as Style;
                lv.ItemContainerStyle = style;
                lv.AlternationCount = 2;

                GridView gv = new GridView();
                GridViewColumn[] gvc = new GridViewColumn[2];
                gvc[0] = new GridViewColumn();
                gvc[1] = new GridViewColumn();
                GridViewColumnHeader[] gvch = new GridViewColumnHeader[2];
                gvch[0] = new GridViewColumnHeader();
                gvch[1] = new GridViewColumnHeader();

                gvc[0].Width = 260;
                gvc[0].DisplayMemberBinding = new Binding("content");
                gvch[0].Content = "Name";
                gvch[0].FontSize = (double) new FontSizeConverter().ConvertFrom("13pt");
                gvch[0].FontFamily = new FontFamily("Segoe UI");
                gvc[0].Header = gvch[0];
                gvc[1].Width = 100;
                gvc[1].DisplayMemberBinding = new Binding("quantity");
                gvch[1].Content = "Qty";
                gvch[1].FontSize = (double) new FontSizeConverter().ConvertFrom("13pt");
                gvch[1].FontFamily = new FontFamily("Segoe UI");
                gvc[1].Header = gvch[1];

                gv.Columns.Add(gvc[0]);
                gv.Columns.Add(gvc[1]);

                lv.View = gv;
                grd.Children.Add(lv);

                // Button
                Button btnM = new Button();
                btnM.Width = 112;
                btnM.Height = 56;

                Style style1 = Application.Current.FindResource("IvoryButton") as Style;
                btnM.Style = style1;

                btnM.Content = "Modify";
                btnM.FontFamily = new FontFamily("NanumBarunGothic");
                btnM.FontSize = (double) new FontSizeConverter().ConvertFrom("11pt");
                btnM.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0x3A, 0x17));

                btnM.HorizontalAlignment = HorizontalAlignment.Right;
                btnM.VerticalAlignment = VerticalAlignment.Bottom;
                btnM.Margin = new Thickness(0, 0, OrderPosition.InitBtnMRight, OrderPosition.InitBtnBottom);

                //btnM.Command = LoadMenuCommand;
                //object[] obj = new object[] { cw, i };
                //btnM.CommandParameter = obj;

                Button btnC = new Button();
                btnC.Width = 112;
                btnC.Height = 56;

                Style style2 = Application.Current.FindResource("BrownButton") as Style;
                btnC.Style = style2;

                btnC.Content = "Complete";
                btnC.FontFamily = new FontFamily("NanumBarunGothic");
                btnC.FontSize = (double) new FontSizeConverter().ConvertFrom("11pt");
                btnC.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFB, 0xFB, 0xEF));

                btnC.HorizontalAlignment = HorizontalAlignment.Right;
                btnC.VerticalAlignment = VerticalAlignment.Bottom;
                btnC.Margin = new Thickness(0, 0, OrderPosition.InitBtnCRight, OrderPosition.InitBtnBottom);

                //btnC.Command = LoadMenuCommand;
                //object[] obj = new object[] { cw, i };
                //btnC.CommandParameter = obj;

                grd.Children.Add(btnM);
                grd.Children.Add(btnC);

                // Border
                Border brd = new Border();
                brd.BorderBrush = Brushes.Black;
                brd.BorderThickness = new Thickness(1);
                brd.HorizontalAlignment = HorizontalAlignment.Left;
                brd.Width = 450;
                brd.Margin = new Thickness(OrderPosition.InitBorderLeft + OrderPosition.MarginBorderLeft * (ExistingOrder++),
                    OrderPosition.InitBorderTop, 0, OrderPosition.InitBorderBottom);
                brd.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF1, 0xE1));

                brd.Child = grd;

                ov.grdOrders.Children.Add(brd);

                SellingItems.Clear();
                DiscountPrice = "0";
                SubTotal = "0";
            }
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

                Total = (int.Parse(SubTotal) - int.Parse(DiscountPrice)).ToString();
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

                Total = (int.Parse(SubTotal) - int.Parse(DiscountPrice)).ToString();
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

        private SellingItem _selectedSellingItem;

        public SellingItem SelectedSellingItem
        {
            get { return _selectedSellingItem; }
            set
            {
                _selectedSellingItem = value;
                RaisePropertyChanged("SelectedSellingItem");
            }
        }

        private Visibility _counterVisible = Visibility.Visible;

        public Visibility CounterVisible
        {
            get { return _counterVisible; }
            set
            {
                _counterVisible = value;
                RaisePropertyChanged("CounterVisible");
            }
        }

        private Visibility _ordersVisible = Visibility.Collapsed;

        public Visibility OrdersVisible
        {
            get { return _ordersVisible; }
            set
            {
                _ordersVisible = value;
                RaisePropertyChanged("OrdersVisible");
            }
        }

        private Visibility _cntVisible = Visibility.Collapsed;

        public Visibility CntVisible
        {
            get { return _cntVisible; }
            set
            {
                _cntVisible = value;
                RaisePropertyChanged("CntVisible");
            }
        }

        private int _orderNumber = 1;

        public int OrderNumber
        {
            get { return _orderNumber; }
            set
            {
                _orderNumber = value;
                RaisePropertyChanged("OrderNumber");

                StrOrderNumber = "Order #" + _orderNumber.ToString();
            }
        }

        private string _strOrderNumber = "Order #1";

        public string StrOrderNumber
        {
            get { return _strOrderNumber; }
            set
            {
                _strOrderNumber = value;
                RaisePropertyChanged("StrOrderNumber");
            }
        }

        private int _existingOrder = 0;

        public int ExistingOrder
        {
            get { return _existingOrder; }
            set
            {
                _existingOrder = value;
                if (_existingOrder == 0)
                {
                    CntVisible = Visibility.Collapsed;
                }
                else
                {
                    CntVisible = Visibility.Visible;
                }
                RaisePropertyChanged("ExistingOrder");
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

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] arr = obj.Text.Split('^');

            switch (arr[0])
            {
                case "ClearDiscount":
                    DiscountPrice = "0";
                    break;

                case "ApplyDiscount":
                    DiscountPrice = (int.Parse(DiscountPrice) + int.Parse(arr[1])).ToString();
                    break;

                case "AddOption":
                    SellingItem si = new SellingItem(arr[1], null, null, 500);
                    SellingItems.Add(si);

                    SubTotal = (int.Parse(SubTotal) + 500).ToString();
                    break;

                default:
                    break;
            }
        }

        public CounterViewModel()
        {
            Messenger.Default.Register<ViewModelMessage>(this, OnReceiveMessageAction);
        }
    }
}
