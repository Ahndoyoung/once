﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using Microsoft.Office.Interop.Word;
using Once_v2_2015.Class;
using Once_v2_2015.Model;
using Once_v2_2015.View;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using Category = Once_v2_2015.Model.Category;
using Style = System.Windows.Style;
using Border = System.Windows.Controls.Border;
using ContextMenu = System.Windows.Controls.ContextMenu;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.MessageBox;
using SelectionMode = System.Windows.Controls.SelectionMode;
using VerticalAlignment = System.Windows.VerticalAlignment;


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
            categories = LoadCategory();
            SetCategory(cw);
            
            menuSettingView = new MenuSettingView();
            cw.grdOutterMenuSetting.Children.Add(menuSettingView);
            int idx = cw.grdOutterMenuSetting.Children.IndexOf(menuSettingView);
            cw.grdOutterMenuSetting.Children[idx].Visibility = Visibility.Collapsed;

            // init
            CheckDateTime();
            InitProperties();
            counterWindow = cw; // need modify
        }

        private void ReloadMenu()
        {
            // need modify
            if (counterWindow != null)
            {
                categories = LoadCategory();
                SetCategory(counterWindow);
            }
        }
        #endregion

        #region OnClosingCommand

        private RelayCommand<CancelEventArgs> _OnClosingCommand;

        public RelayCommand<CancelEventArgs> OnClosingCommand
        {
            get { return _OnClosingCommand ?? (_OnClosingCommand = new RelayCommand<CancelEventArgs>(OnClosing)); }
        }

        private void OnClosing(CancelEventArgs obj)
        {
            obj.Cancel = true;
            CounterWindowVisible = Visibility.Collapsed;
            var msg = new ViewModelMessage()
            {
                Text = "ShowMain"
            };
            Messenger.Default.Send<ViewModelMessage>(msg);
        }

        #endregion

        #region ShowCommand

        private RelayCommand _ShowCommand;

        public RelayCommand ShowCommand
        {
            get { return _ShowCommand ?? (_ShowCommand = new RelayCommand(Show)); }
        }

        private void Show()
        {
            CounterWindowVisible = Visibility.Visible;
            var msg = new ViewModelMessage()
            {
                Text = "HideMain"
            };
            Messenger.Default.Send<ViewModelMessage>(msg);
        }

        #endregion

        #region ShutdownCommand

        private RelayCommand _ShutdownCommand;

        public RelayCommand ShutdownCommand
        {
            get { return _ShutdownCommand ?? (_ShutdownCommand = new RelayCommand(Shutdown)); }
        }

        private void Shutdown()
        {
            SaveDateTime();
            Application.Current.Shutdown();
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
                if ((categories[idx].menuList[i].temperature == 'I' && categories[idx].menuList[i].size == 'R')
                    || (categories[idx].menuList[i].temperature == 'I' && categories[idx].menuList[i].size == null)
                    || (categories[idx].menuList[i].temperature == null && categories[idx].menuList[i].size == 'R')
                    || (categories[idx].menuList[i].temperature == null && categories[idx].menuList[i].size == null))
                {
                    if (col == MenuPosition.CntPerRow)
                        row++;
                    col = col % MenuPosition.CntPerRow;

                    int left = MenuPosition.InitLeft + MenuPosition.MarginLeft * col;
                    int right = MenuPosition.InitRight - MenuPosition.MarginLeft * col;
                    int y = MenuPosition.InitY + MenuPosition.MarginTop * row;

                    Button btn = new Button();
                    btn.Width = MenuPosition.BtnWidth;
                    btn.Height = MenuPosition.BtnHeight;

                    Style style = (categories[idx].menuList[i].isWhipping == true) ? Application.Current.FindResource("WhippingButton") as Style : Application.Current.FindResource("IvoryButton") as Style;
                    btn.Style = style;

                    btn.Content = categories[idx].menuList[i].name.Replace('^', '\n');
                    btn.FontFamily = new FontFamily("NanumBarunGothic");
                    btn.FontSize = (double)new FontSizeConverter().ConvertFrom("11pt");
                    btn.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0x3A, 0x17));

                    btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn.VerticalAlignment = VerticalAlignment.Top;
                    btn.Margin = new Thickness(left, y, right, 0);

                    //btn.Command = AddMenuItemCommand;
                    //string name = categories[idx].menuList[i].name;
                    //object[] ob = new object[] {cw, name};
                    //btn.CommandParameter = ob;

                    var triggers = Interaction.GetTriggers(btn);

                    string name = categories[idx].menuList[i].name;
                    string whip = categories[idx].menuList[i].isWhipping == true ? "whipping" : "none";
                    object[] left_ob = new object[] { cw, name, whip };
                    var invoke_left = new InvokeCommandAction { CommandParameter = left_ob };
                    var binding_left = new Binding { Path = new PropertyPath("AddMenuItemCommand") };
                    BindingOperations.SetBinding(invoke_left, InvokeCommandAction.CommandProperty, binding_left);
                    var event_left = new System.Windows.Interactivity.EventTrigger
                    {
                        EventName = "PreviewMouseLeftButtonDown"
                    };
                    event_left.Actions.Add(invoke_left);
                    triggers.Add(event_left);

                    if (categories[idx].menuList[i].isWhipping == true)
                    {
                        object[] right_ob = new object[] { cw, name, "none" };
                        var invoke_right = new InvokeCommandAction { CommandParameter = right_ob };
                        var binding_right = new Binding { Path = new PropertyPath("AddMenuItemCommand") };
                        BindingOperations.SetBinding(invoke_right, InvokeCommandAction.CommandProperty, binding_right);
                        var event_right = new System.Windows.Interactivity.EventTrigger
                        {
                            EventName = "PreviewMouseRightButtonDown"
                        };
                        event_right.Actions.Add(invoke_right);
                        triggers.Add(event_right);
                    }

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
            string way = (string) values[2];

            int price = 0;

            bool isNullTemp = true;
            bool isNullSize = true;
            // 메뉴이름 찾기
            foreach (var category in categories)
            {
                if (cw.btnCategory.Content.ToString() == category.name)
                {
                    foreach (var menuItem in category.menuList)
                    {
                        if (menuItem.name == name)
                        {
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
            }

            // 온도, 사이즈 체크
            char? checkedTemp = null;
            if (!isNullTemp)
            {
                if (cw.InnerMenuSettingView.btnTemperature.Content.ToString() == "Ice")
                {
                    checkedTemp = 'I';
                }
                else if (cw.InnerMenuSettingView.btnTemperature.Content.ToString() == "Hot")
                {
                    checkedTemp = 'H';
                }
            }
            char? checkedSize = null;
            if (!isNullSize)
            {
                if (cw.InnerMenuSettingView.btnSize.Content.ToString() == "Regular")
                {
                    checkedSize = 'R';
                }
                else if (cw.InnerMenuSettingView.btnSize.Content.ToString() == "Large")
                {
                    checkedSize = 'L';
                }
            }

            // 가격 체크
            foreach (var category in categories)
            {
                if (cw.btnCategory.Content.ToString() == category.name)
                {
                    foreach (var menuItem in category.menuList)
                    {
                        if (menuItem.name == name && menuItem.temperature == checkedTemp && menuItem.size == checkedSize)
                        {
                            price = menuItem.price;
                        }
                    }
                }
            }

            if (way == "whipping")
                name += "\n * 휘핑크림";
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
                cw.lvSelling.ScrollIntoView(cw.lvSelling.Items[cw.lvSelling.Items.Count - 1]);
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

            ShowDetailVisible = Visibility.Collapsed;
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

        private RelayCommand<object> _sendOrderCommand;

        public RelayCommand<object> SendOrderCommand
        {
            get { return _sendOrderCommand ?? (_sendOrderCommand = new RelayCommand<object>(SendOrder)); }
        }

        private void SendOrder(object obj)
        {
            object[] values = (object[]) obj;
            CounterWindow cw = (CounterWindow) values[0];
            OrdersUC ov = (OrdersUC) values[1];

            var items = new ObservableCollection<SellingItem>();
            foreach (var si in SellingItems)
            {
                items.Add(si);
            }
            

            if (items.Count != 0)
            {
                string way = cw.rbCash.IsChecked == true ? "현금" : "카드";
                MessageBoxResult mbr = MessageBox.Show(Total + "원을 " + way + "(으)로 계산하시겠습니까?", "계산확인", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (mbr == MessageBoxResult.Yes)
                {
                    Border brd = new Border();
                    Grid grd = new Grid();
                    // TextBlock
                    TextBlock tb = new TextBlock();
                    tb.Margin = new Thickness(OrderPosition.InitTbLeft, OrderPosition.InitTbTop,
                        OrderPosition.InitTbRight,
                        0);
                    tb.VerticalAlignment = VerticalAlignment.Top;
                    tb.TextAlignment = TextAlignment.Center;

                    tb.Text = "Order #" + OrderNumber.ToString();
                    if (ShowDetailVisible == Visibility.Visible)
                        tb.Text += " (M)";
                    tb.FontSize = (double) new FontSizeConverter().ConvertFrom("22pt");
                    tb.FontFamily = new FontFamily("Segoe Print");
                    tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0x3A, 0x17));
                    grd.Children.Add(tb);

                    TextBlock tb1 = new TextBlock();
                    tb1.Margin = new Thickness(0, 0, OrderPosition.InitTbWayRight, OrderPosition.InitTbBottom);
                    tb1.VerticalAlignment = VerticalAlignment.Bottom;
                    tb1.HorizontalAlignment = HorizontalAlignment.Right;
                    tb1.TextAlignment = TextAlignment.Center;

                    tb1.Text = cw.rbCash.IsChecked == true ? "현금" : "카드";
                    tb1.FontSize = (double) new FontSizeConverter().ConvertFrom("20pt");
                    tb1.FontFamily = new FontFamily("Segoe UI Semibold");
                    tb1.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x59, 0x59, 0x59));
                    grd.Children.Add(tb1);

                    TextBlock tb2 = new TextBlock();
                    tb2.Margin = new Thickness(0, 0, OrderPosition.InitTbPriceRight, OrderPosition.InitTbBottom);
                    tb2.VerticalAlignment = VerticalAlignment.Bottom;
                    tb2.HorizontalAlignment = HorizontalAlignment.Right;
                    tb2.TextAlignment = TextAlignment.Right;

                    tb2.Text = Total;
                    tb2.FontSize = (double) new FontSizeConverter().ConvertFrom("20pt");
                    tb2.FontFamily = new FontFamily("Segoe UI Semibold");
                    tb2.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x59, 0x59, 0x59));
                    grd.Children.Add(tb2);

                    // ListView
                    ListView lv = new ListView();
                    lv.ItemsSource = items;
                    lv.Margin = new Thickness(OrderPosition.InitLvLeft, OrderPosition.InitLvTop,
                        OrderPosition.InitLvRight,
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

                    btnM.Command = ModifyOrderCommand;
                    object[] obj_btnM = new object[] { OrderNumber, cw, items, Total, SubTotal, DiscountPrice, way, ov, brd };
                    btnM.CommandParameter = obj_btnM;

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

                    btnC.Command = CompleteOrderCommand;
                    object[] obj_btnC = new object[] {ov, brd};
                    btnC.CommandParameter = obj_btnC;

                    grd.Children.Add(btnM);
                    grd.Children.Add(btnC);

                    // Border
                    brd.BorderBrush = Brushes.Black;
                    brd.BorderThickness = new Thickness(1);
                    brd.HorizontalAlignment = HorizontalAlignment.Left;
                    brd.Width = 450;
                    brd.Margin =
                        new Thickness(OrderPosition.InitBorderLeft + OrderPosition.MarginBorderLeft*(ExistingOrder++),
                            OrderPosition.InitBorderTop, 0, OrderPosition.InitBorderBottom);
                    brd.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF7, 0xF1, 0xE1));

                    brd.Child = grd;

                    ov.grdOrders.Children.Add(brd);
                    OrderNumber++;

                    // JSON
                    try
                    {
                        string request = JsonConvert.SerializeObject(items);
                        //string uri = "";
                        //WebClient webClient = new WebClient();
                        //webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                        //webClient.Encoding = UTF8Encoding.UTF8;
                        //string response = webClient.UploadString(uri, request);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.ToString());
                    }

                    // init
                    SellingItems.Clear();
                    DiscountPrice = "0";
                    SubTotal = "0";

                    ShowDetailVisible = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region CompleteOrderCommand

        private RelayCommand<object> _completeOrderCommand;

        public RelayCommand<object> CompleteOrderCommand
        {
            get { return _completeOrderCommand ?? (_completeOrderCommand = new RelayCommand<object>(CompleteOrder)); }
        }

        private void CompleteOrder(object obj)
        {
            object[] values = (object[]) obj;
            OrdersUC ov = (OrdersUC) values[0];
            Border brd = (Border) values[1];

            bool isFired = false;
            for (int i = 0; i < ov.grdOrders.Children.Count; i++)
            {
                if (brd == ov.grdOrders.Children[i])
                {
                    ov.grdOrders.Children.RemoveAt(i);
                    ExistingOrder--;
                    isFired = true;
                }

                if (isFired && i != ov.grdOrders.Children.Count)
                {
                    Border tmp = (Border)ov.grdOrders.Children[i];
                    tmp.Margin = new Thickness(tmp.Margin.Left - OrderPosition.MarginBorderLeft, tmp.Margin.Top, 0, tmp.Margin.Bottom);
                }
            }
        }

        #endregion

        #region ModifyOrderCommand

        private RelayCommand<object> _modifyOrderCommand;

        public RelayCommand<object> ModifyOrderCommand
        {
            get { return _modifyOrderCommand ?? (_modifyOrderCommand = new RelayCommand<object>(ModifyOrder)); }
        }

        private void ModifyOrder(object obj)
        {
            object[] values = (object [])obj;
            int idx = (int) values[0];
            CounterWindow cw = (CounterWindow) values[1];
            ObservableCollection<SellingItem> items = (ObservableCollection<SellingItem>)values[2];
            string total = (string)values[3];
            string subTotal = (string)values[4];
            string discount = (string)values[5];
            string payment = (string)values[6];
            OrdersUC ov = (OrdersUC) values[7];
            Border brd = (Border) values[8];


            MessageBoxResult mbr = MessageBox.Show("Order #" + idx.ToString() + "을(를) 수정하시겠습니까?", "수정확인",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                // ShowDetail
                ShowDetailVisible = Visibility.Visible;

                object[] param = new object[] {idx, total, subTotal, discount, payment};
                cw.btnShowDetail.CommandParameter = param;

                // FireOrder
                bool isFired = false;
                for (int i = 0; i < ov.grdOrders.Children.Count; i++)
                {
                    if (brd == ov.grdOrders.Children[i])
                    {
                        ov.grdOrders.Children.RemoveAt(i);
                        ExistingOrder--;
                        isFired = true;
                    }

                    if (isFired && i != ov.grdOrders.Children.Count)
                    {
                        Border tmp = (Border)ov.grdOrders.Children[i];
                        tmp.Margin = new Thickness(tmp.Margin.Left - OrderPosition.MarginBorderLeft, tmp.Margin.Top, 0, tmp.Margin.Bottom);
                    }
                }

                // Change Window
                ViewOrders();

                // Pass Parameters
                SellingItems.Clear();
                foreach (var item in items)
                {
                    SellingItems.Add(item);
                }
                Total = total;
                SubTotal = subTotal;
                DiscountPrice = discount;
                if (payment == "현금")
                    cw.rbCash.IsChecked = true;
                else if (payment == "카드")
                    cw.rbCard.IsChecked = true;
            }
        }

        #endregion

        #region ShowDetailCommand

        private RelayCommand<object> _showdetailCommand;

        public RelayCommand<object> ShowDetailCommand
        {
            get { return _showdetailCommand ?? (_showdetailCommand = new RelayCommand<object>(ShowDetail)); }
        }

        private void ShowDetail(object obj)
        {
            object[] values = (object[]) obj;
            int idx = (int) values[0];
            string total = (string) values[1];
            string subTotal = (string) values[2];
            string discount = (string) values[3];
            string payment = (string) values[4];

            MessageBox.Show(
                "Order #" + idx.ToString() + "\n\nSubTotal : " + subTotal + "\nDiscounts : " + discount +
                "\n-------------------------\n"
                + "Total : " + total + "\nPayment : " + payment, "원본 내역", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region ReadDocxCommand

        private RelayCommand _ReadDocxCommand;

        public RelayCommand ReadDocxCommand
        {
            get { return _ReadDocxCommand ?? (_ReadDocxCommand = new RelayCommand(ReadDocx)); }
        }

        private void ReadDocx()
        {
            Microsoft.Office.Interop.Word.Application ap = new Microsoft.Office.Interop.Word.Application();

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                Document doc = ap.Documents.Open(path + "Recipe.docx", ReadOnly: true, Visible: false);
                doc.Activate();
                ap.Visible = true;
            }
            catch (Exception err)
            {
                
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
        
        private Visibility _CounterWindowVisible = Visibility.Collapsed;

        public Visibility CounterWindowVisible 
        {
            get { return _CounterWindowVisible; }
            set
            {
                _CounterWindowVisible = value;
                RaisePropertyChanged("CounterWindowVisible");
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

        private Visibility _showDetailVisible = Visibility.Collapsed;

        public Visibility ShowDetailVisible
        {
            get { return _showDetailVisible; }
            set
            {
                _showDetailVisible = value;
                RaisePropertyChanged("ShowDetailVisible");
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

        public CounterWindow counterWindow = null; // need modify
        public MenuSettingView menuSettingView = null;
        private string date_today = null;

        public List<Category> categories = new List<Category>();

        private static List<Category> LoadCategory()
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

        public void SetCategory(CounterWindow cw)
        {
            cw.grdInnerMenu.Children.Clear();
            cw.btnCategory.Visibility = Visibility.Collapsed;
            
            for (int i = 0; i < categories.Count; i++)
            {
                int row = i / MenuPosition.CntPerRow;
                int col = i % MenuPosition.CntPerRow;
                int left = MenuPosition.InitLeft + MenuPosition.MarginLeft * col;
                int right = MenuPosition.InitRight - MenuPosition.MarginLeft * col;
                int y = MenuPosition.InitY + MenuPosition.MarginTop * row;

                Button btn = new Button();
                btn.Width = MenuPosition.BtnWidth;
                btn.Height = MenuPosition.BtnHeight;

                Style style = Application.Current.FindResource("IvoryButton") as Style;
                btn.Style = style;

                btn.Content = categories[i].name;
                btn.FontFamily = new FontFamily("NanumBarunGothic");
                btn.FontSize = (double)new FontSizeConverter().ConvertFrom("11pt");
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

        private void InitProperties()
        {
            SellingItems.Clear();

            SubTotal = "0";
            DiscountPrice = "0";
            ExistingOrder = 0;

            CounterVisible = Visibility.Visible;
            OrdersVisible = Visibility.Collapsed;
        }

        private void CheckDateTime()
        {
            try
            {
                date_today = DateTime.Today.ToShortDateString();
                string[] values = File.ReadAllLines("DateTime_Sale.txt");

                if (values[0] == DateTime.Today.ToShortDateString())
                {
                    // 오늘과 날짜가 같으면
                    OrderNumber = int.Parse(values[1]);
                }
                else
                {
                    // 다르면
                    File.WriteAllText("DateTime_Sale.txt", date_today + "\n1", Encoding.Default);
                    CheckDateTime();
                }
            }
            catch (Exception)
            {
                File.WriteAllText("DateTime_Sale.txt", date_today + "\n1", Encoding.Default);
                CheckDateTime();
            }
        }

        private void SaveDateTime()
        {
            File.WriteAllText("DateTime_Sale.txt", date_today + "\n" + OrderNumber.ToString(), Encoding.Default);
        }

        private void OnReceiveMessageAction(ViewModelMessage obj)
        {
            string[] arr = obj.Text.Split('^');

            switch (arr[0])
            {
                case "StartPOS":
                    CounterWindowVisible = Visibility.Visible;
                    break;

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

                case "ReloadMenu":
                    ReloadMenu();
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
