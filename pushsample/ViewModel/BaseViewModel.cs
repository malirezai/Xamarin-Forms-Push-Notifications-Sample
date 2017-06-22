using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace pushsample.ViewModel
{
	public class BaseViewModel : INotifyPropertyChanged
    {
		protected Page page;
		public BaseViewModel(Page page)
		{
			this.page = page;
		}

        public BaseViewModel(){

        }

		public virtual Task InitAsync()
		{
			return Task.FromResult(true);
		}

		string title = string.Empty;
		
		/// <summary>
		/// Gets or sets the "Title" property
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return title; }
			set { SetProperty(ref title, value); }
		}

		string subTitle = string.Empty;
		/// <summary>
		/// Gets or sets the "Subtitle" property
		/// </summary>
		public string Subtitle
		{
			get { return subTitle; }
			set { SetProperty(ref subTitle, value); }
		}

        string icon;
		/// <summary>
		/// Gets or sets the "Icon" of the viewmodel
		/// </summary>
		public string Icon
		{
			get { return icon; }
			set { SetProperty(ref icon, value); }
		}

		bool isBusy;
		/// <summary>
		/// Gets or sets if the view is busy.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				SetProperty(ref isBusy, value);
				SetProperty(ref isBusyRev, !isBusy, nameof(IsBusyRev));
			}
		}

		private bool isBusyRev;
		/// <summary>
		/// Gets or sets the reverse of  isbusy. Handy for hiding views during busy times.
		/// </summary>
		public bool IsBusyRev
		{
			get { return isBusyRev; }
			set { SetProperty(ref isBusyRev, value); }
		}

		protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
		{

			if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			onChanged?.Invoke();

			OnPropertyChanged(propertyName);
		}

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
