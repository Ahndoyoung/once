using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using Ninject;
using Once_v2_2015.Container;

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
        
        public static void Cleanup()
        {
            
        }
    }
}