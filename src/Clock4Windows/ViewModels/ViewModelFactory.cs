using System;
using Caliburn.Micro;

namespace Clock4Windows.ViewModels
{
    public class ViewModelFactory
    {
        private readonly SimpleContainer _container;

        public ViewModelFactory(SimpleContainer container)
        {
            _container = container;
        }


        public TViewModel Create<TViewModel>(Action<TViewModel> initCallback = null)
        {
            var viewModel = _container.GetInstance<TViewModel>();

            initCallback?.Invoke(viewModel);

            return viewModel;
        }


    }
}
