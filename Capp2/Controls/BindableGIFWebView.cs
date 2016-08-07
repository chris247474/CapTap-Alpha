using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Capp2
{
	public class BindableGIFWebView:WebView
	{
		public string GIFSource{
			get
			{
				Debug.WriteLine("BindableGIFWebView getting GIFSource");
				return (string)GetValue(GIFSourceProperty);
			}
			set
			{
				Debug.WriteLine("BindableGIFWebView setting GIFSource");
				try
				{
					
					SetValue(GIFSourceProperty, value);
				}
				catch (ArgumentException ex)
				{
					Debug.WriteLine("BindableGIFWebView.GIFSource set error: {0}", ex.Message);
				}
			}
		}
		public static readonly BindableProperty GIFSourceProperty =
			BindableProperty.Create("GIFSource", typeof(string), typeof(BindableGIFWebView), "", BindingMode.TwoWay
			                        ,null, HandleGIFPropertyChanged, HandleGIFPropertyChanging);

		private static void HandleGIFPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var GIFWebView = (BindableGIFWebView)bindable;
			string html = @"<html>
							<body style=""margin: 0; padding: 0"">
								<img style=""position:fixed; height: 423.5; width: 240; top:0;left 0;"" src = """ + 
									(string)GIFWebView.GetValue(GIFSourceProperty) + @"""/>
							</body>
						</html>";
			Debug.WriteLine("BindableGIFWebView HtmlWebViewSource Html: {0}", html);
			var source = new HtmlWebViewSource();
			source.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
			source.Html = html;
			GIFWebView.Source = source;

			Debug.WriteLine("HandleGIFPropertyChanged - Total memory allocated: {0}", GC.GetTotalMemory(true));
		}

		private static void HandleGIFPropertyChanging(BindableObject bindable, object oldValue, object newValue) {
			Debug.WriteLine("Clearing BindableGIFWebView sourceproperty, Memory allocated: {0}", GC.GetTotalMemory(true));
			var GIFWebView = (BindableGIFWebView)bindable;
			GIFWebView.ClearValue(SourceProperty);
			GIFWebView = null;
			GC.Collect();
			Debug.WriteLine("Memory allocated: {0}", GC.GetTotalMemory(true));
		}

		public BindableGIFWebView()
		{
			Debug.WriteLine("BindableGIFWebView started");
			this.HeightRequest = 423.5;
			this.WidthRequest = 240;
			this.HorizontalOptions = LayoutOptions.Center;
			Debug.WriteLine("BindableGIFWebView done");
		}
	}
}

