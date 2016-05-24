using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace Capp2
{
	public class SwitcherPageViewModel : BaseViewModel
	{
		public SwitcherPageViewModel()
		{
			var background = Color.FromHex (Values.CAPPTUTORIALCOLOR_Orange); 
			List<HomeViewModel> list = new List<HomeViewModel> ();
			list.Add (new HomeViewModel {
				Title = "Where'd my CAPP Sheet go?",
				Background = background,
				ImageSource = "HowToCappScreenshot.png"
			});
			list.Add (new HomeViewModel { Title = "2", Background = background, ImageSource = "icon.png" });
			list.Add (new HomeViewModel { Title = "3", Background = background, ImageSource = "icon.png" });
			list.Add (new HomeViewModel { Title = "4", Background = background, ImageSource = "icon.png" });

			Pages = list;

			CurrentPage = Pages.First();
		}

		IEnumerable<HomeViewModel> _pages;
		public IEnumerable<HomeViewModel> Pages {
			get {
				return _pages;
			}
			set {
				SetObservableProperty (ref _pages, value);
				CurrentPage = Pages.FirstOrDefault ();
			}
		}

		HomeViewModel _currentPage;
		public HomeViewModel CurrentPage {
			get {
				return _currentPage;
			}
			set {
				SetObservableProperty (ref _currentPage, value);
			}
		}
	}

	public class HomeViewModel : BaseViewModel, ITabProvider
	{
		public HomeViewModel() {}

		public string Title { get; set; }
		public Color Background { get; set; }
		public string ImageSource { get; set; }
	}
}

