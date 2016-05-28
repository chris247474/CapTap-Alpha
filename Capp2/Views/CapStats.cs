using System;
using Xamarin.Forms;
using Syncfusion.SfChart.XForms;
using System.Collections.ObjectModel;

namespace Capp2
{
	public class CapStats:ContentPage
	{
		public CapStats ()
		{
			var piedata = StatsHelper.CreateChartData (App.Database.GetCappStats ());
			StackLayout stack;

			if (piedata.Count > 0) {
				stack = new StackLayout{ 
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Padding = new Thickness(20),
					Children = {
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateEmptyStackSpace(),

						UIBuilder.CreateModalXPopper(new Command(() => {
							Navigation.PopModalAsync();
						}), "Stats"),

						StatsHelper.CreatePieChart (piedata, 
							"CAPP", "Values", "CAPP Ratio"),
						
						UIBuilder.CreateEmptyStackSpace(),
						StatsHelper.CreateSplineChart(StatsHelper.CreateChartData(App.Database.GetDailyYesCalls()),
							"Dates", "Yes Calls", "Productivity"),
					}
				};
			} else {
				stack = new StackLayout{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Padding = new Thickness(20),
					Children = {
						UIBuilder.CreateEmptyStackSpace(),
						UIBuilder.CreateEmptyStackSpace(),

						UIBuilder.CreateModalXPopper(new Command(() => {
							Navigation.PopModalAsync();
						})),

						new StackLayout{
							HorizontalOptions = LayoutOptions.CenterAndExpand,
							VerticalOptions = LayoutOptions.CenterAndExpand,
							Children = {
								new Label{
									Text = "Your CAPP Ratio will show up after a few calls",
									TextColor = Color.Accent,
									HorizontalTextAlignment = TextAlignment.Center,
									HorizontalOptions = LayoutOptions.CenterAndExpand,
									VerticalOptions = LayoutOptions.CenterAndExpand,
									FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
								},
								UIBuilder.CreateEmptyStackSpace(),
								UIBuilder.CreateEmptyStackSpace(),
								UIBuilder.CreateEmptyStackSpace(),
								UIBuilder.CreateEmptyStackSpace(),
							}
						}
					}
				};
			}

			Content = new ScrollView{
				Orientation = ScrollOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = stack
			};
		}

		StackLayout CreateView(){
			return new StackLayout{
				Children = {}
			};
		}
	}
}

