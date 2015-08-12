using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ninject.Modules;
using Once_v2_2015.ViewModel;

namespace Once_v2_2015.Container
{
    public class DiContainer : NinjectModule
    {
        public override void Load()
        {
            Bind<MainViewModel>().ToSelf().InSingletonScope().Named("MainVM");
            Bind<CounterViewModel>().ToSelf().InSingletonScope().Named("CounterVM");
            Bind<DiscountViewModel>().ToSelf().InSingletonScope().Named("DiscountVM");
            Bind<InnerMenuSettingViewModel>().ToSelf().InSingletonScope().Named("InnerMenuSettingVM");
            Bind<MenuSettingViewModel>().ToSelf().InSingletonScope().Named("MenuSettingVM");
            Bind<OrdersViewModel>().ToSelf().InSingletonScope().Named("OrdersVM");
        }
    }
}
