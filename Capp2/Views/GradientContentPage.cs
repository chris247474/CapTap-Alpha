using System;
using Xamarin.Forms;

namespace Capp2
{
	public class GradientContentPage : ContentPage
	{
		public static BindableProperty StartColorProperty = BindableProperty.Create<GradientContentPage, Color>(p => p.StartColor, Color.White);
		public static BindableProperty EndColorProperty = BindableProperty.Create<GradientContentPage, Color>(p => p.EndColor, Color.Gray);

		public Color StartColor
		{
			get { return (Color) GetValue(StartColorProperty); }
			set { SetValue(StartColorProperty, value); }
		}

		public Color EndColor
		{
			get { return (Color) GetValue(EndColorProperty); }
			set { SetValue(EndColorProperty, value); }
		}
	}
}

