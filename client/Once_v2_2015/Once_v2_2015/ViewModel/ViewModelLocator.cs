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

        public CounterViewModel CounterVM
        {
            get { return Kernel.Get<CounterViewModel>("CounterVM"); }
        }

        public DiscountViewModel DiscountVM
        {
            get { return Kernel.Get<DiscountViewModel>("DiscountVM"); }
        }

        public MenuSettingViewModel MenuSettingVM
        {
            get { return Kernel.Get<MenuSettingViewModel>("MenuSettingVM"); }
        }

        public OrdersViewModel OrdersVM
        {
            get { return Kernel.Get<OrdersViewModel>("OrdersVM"); }
        }

        public MenuManagementViewModel MenuManagementVM
        {
            get { return Kernel.Get<MenuManagementViewModel>("MenuManagementVM"); }
        }

        public AdjustmentUCViewModel AdjustmentUCVM
        {
            get { return Kernel.Get<AdjustmentUCViewModel>("AdjustmentUCVM"); }
        }

        public DefaultDiscountViewModel DefaultDiscountVM
        {
            get { return Kernel.Get<DefaultDiscountViewModel>("DefaultDiscountVM"); }
        }

        public EnterPasswordViewModel EnterPasswordVM
        {
            get { return Kernel.Get<EnterPasswordViewModel>("EnterPasswordVM"); }
        }

        public ChangePasswordViewModel ChangePasswordVM
        {
            get { return Kernel.Get<ChangePasswordViewModel>("ChangePasswordVM"); }
        }

        public AdjustmentViewModel AdjustmentVM
        {
            get { return Kernel.Get<AdjustmentViewModel>("AdjustmentVM"); }
        }

        public StatisticsViewModel StatisticsVM
        {
            get { return Kernel.Get<StatisticsViewModel>("StatisticsVM"); }
        }

        public static void Cleanup()
        {
            
        }
    }
}