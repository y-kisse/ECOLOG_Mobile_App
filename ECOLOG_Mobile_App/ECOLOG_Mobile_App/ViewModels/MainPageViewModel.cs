using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ECOLOG_Mobile_App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public ICommand NaviToDataInsertionPageCom { get; }
        public MainPageViewModel(INavigationService navigationService) 
            : base (navigationService)
        {
            Title = "Main Page";
            _navigationService = navigationService;
            NaviToDataInsertionPageCom = new DelegateCommand(() =>
            {
                _navigationService.NavigateAsync("DataInsertionPage");
            });
        }
    }
}
