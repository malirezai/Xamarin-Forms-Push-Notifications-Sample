using System;
using Xamarin.Forms;
using pushsample.ViewModel;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace pushsample.Pages
{
    public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
    {

		T viewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:WellsDairyCrmPoc.BaseContentPage`1"/> class.
		/// </summary>
		protected BaseContentPage()
		{
			this.ViewModel = Activator.CreateInstance<T>();
		}

		/// <summary>
		/// Gets or sets the view model.
		/// </summary>
		/// <value>The view model.</value>
		public T ViewModel
		{
			get { return this.viewModel; }
			set
			{
				viewModel = value;
				BindingContext = viewModel;
                this.SetBinding(TitleProperty, nameof(ViewModel.Title));
				Task.Run(async () =>
				{
					await this.Init();
				});
			}
		}

		private async Task Init()
		{
			await this.ViewModel.InitAsync();
		}

		//public abstract void SubscribeEventHandlers();
        public virtual void SubscribeEventHandlers()
        {
			if (!CrossConnectivity.Current.IsConnected)
			{
				//TODO: handle network lost
			}

			CrossConnectivity.Current.ConnectivityChanged += OnConnectivityChanged;
		}

        //public abstract void UnsubscribeEventHandlers();
        public virtual void UnsubscribeEventHandlers()
        {
			
			CrossConnectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            SubscribeEventHandlers();
        }

        void OnConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {

            if (!e.IsConnected)
            {
                //todo handle this
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UnsubscribeEventHandlers();
		}
    }
}
