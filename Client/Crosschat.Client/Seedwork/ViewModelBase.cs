﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using SharedSquawk.Client.Properties;
using Xamarin.Forms;
using SharedSquawk.Client.Views;

namespace SharedSquawk.Client.Seedwork
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isBusy;
        protected static Page _currentPage;
        
        [NotifyPropertyChangedInvocator]
        protected virtual ViewModelBase SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            return this;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual ViewModelBase Raise(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ViewModelBase CurrentViewModel { get; private set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public void OnAppearing(Page page)
        {
            _currentPage = page;
            CurrentViewModel = this;
            OnShown();
        }

        protected virtual void OnShown() {}

        public Task ShowAsync()
        {
            //auto-wiring VM with view like MvvmCross and Caliburn do
            string viewModelName = "SharedSquawk.Client.Views." + GetType().Name.Replace("ViewModel", "") + "Page";
            var page = Activator.CreateInstance(Type.GetType(viewModelName), this) as Page;
            return _currentPage.Navigation.PushAsync(page);
        }

		public Task ShowModalAsync()
		{
			//auto-wiring VM with view like MvvmCross and Caliburn do
			string viewModelName = "SharedSquawk.Client.Views." + GetType().Name.Replace("ViewModel", "") + "Page";
			var page = Activator.CreateInstance(Type.GetType(viewModelName), this) as Page;
			return _currentPage.Navigation.PushModalAsync(page);
		}

		public Task ShowModalAsyncWithNavPage()
		{
			//auto-wiring VM with view like MvvmCross and Caliburn do
			string viewModelName = "SharedSquawk.Client.Views." + GetType().Name.Replace("ViewModel", "") + "Page";
			var page = Activator.CreateInstance(Type.GetType(viewModelName), this) as Page;
			var navPage = new SquawkNavigationPage (page);
			navPage.BindingContext = page;
			navPage.SetBinding (Page.TitleProperty, new Binding ("Title"));
			navPage.SetBinding(Page.IconProperty, new Binding("Icon"));
			return _currentPage.Navigation.PushModalAsync(navPage);
		}

		public Task PopToRootAsync()
		{
			if (_currentPage.Navigation != null && _currentPage.Navigation.NavigationStack.Count > 0)
			{
				return _currentPage.Navigation.PopToRootAsync ();
			}
			return Task.FromResult(false);
		}

		public Task PopAsync()
		{
			if (_currentPage.Navigation != null && _currentPage.Navigation.NavigationStack.Count > 0)
			{
				return _currentPage.Navigation.PopAsync();
			}
			return Task.FromResult(false);
		}

        public ICommand ShowCommand
        {
            get { return new Command(() => ShowAsync());}
        }

        public Task<bool> Ask(string title, string text)
        {
            if (_currentPage == null)
                return Task.FromResult(false);

            return _currentPage.DisplayAlert(title, text, "Yes", "No");
        }

        public Task Notify(string title, string text)
        {
            if (_currentPage == null)
                return Task.FromResult(false);
			
            return _currentPage.DisplayAlert(title, text, "Ok");
        }

		public Task<string> Action(string title, string cancel, string destruction, params string[] buttons)
		{
			if (_currentPage == null)
				return Task<string>.FromResult("");

			return _currentPage.DisplayActionSheet (title, cancel, destruction, buttons);
		}
    }
}
