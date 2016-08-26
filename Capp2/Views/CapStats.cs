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
			var piedata = StatsHelper.CreateChartData (
				App.Database.GetCappStats ()
				/*new ChartData[]{
				 * //tests
					new ChartData{Name = "Called", Value = 10},
					new ChartData{Name = "Appointed", Value = 5},
					new ChartData{Name = "Presented", Value = 10},
					new ChartData{Name = "Purchased", Value = 5},
				}*/
			);
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
						}), "Call Stats"),

						StatsHelper.CreatePieChart (piedata, 
							"CAPP", "Values", "Call Ratio"),
						
						/*UIBuilder.CreateEmptyStackSpace(),
						StatsHelper.CreateSplineChart(StatsHelper.CreateChartData(
							//App.Database.GetDailyYesCalls()
							new ChartData[]{
								new ChartData{Name = "Date1", Value = 10},
								new ChartData{Name = "Date2", Value = 5},
								new ChartData{Name = "Date3", Value = 10},
								new ChartData{Name = "Date4", Value = 5},
							}
						), "Dates", "Yes Calls", "Productivity"),*/

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
									Text = "Your Call Ratio will show up after a few calls",
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

			AdHelper.AddGreenURLOrangeTitleBannerToStack (stack);
		}

	}
}

