using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using Ninject;
using Once_v2_2015.Container;
using Once_v2_2015.View;

namespace Once_v2_2015.ViewModel
{
    public class ViewModelLocator
    {
        public static StandardKernel Kernel;

        public ViewModelLocator()
        {
            Kernel = new StandardKernel(new DiContainer());
        }

        public MainViewModel MainVM
        {
            get { return Kernel.Get<MainViewModel>("MainVM"); }
        }

        public CounterViewModel CounterVM
        {
            get { return Kernel.Get<CounterViewModel>("CounterVM"); }
        }

        public DiscountViewModel DiscountVM
        {
            get { return Kernel.Get<DiscountViewModel>("DiscountVM"); }
        }

        public InnerMenuSettingViewModel InnerMenuSettingVM
        {
            get { return Kernel.Get<InnerMenuSettingViewModel>("InnerMenuSettingVM"); }
        }

        public MenuSettingViewModel MenuSettingVM
        {
            get { return Kernel.Get<MenuSettingViewModel>("MenuSettingVM"); }
        }

        public OrdersViewModel OrdersVM
        {
            get { return Kernel.Get<OrdersViewModel>("OrdersVM"); }
        }
        
        public static void Cleanup()
        {
            
        }
    }
}