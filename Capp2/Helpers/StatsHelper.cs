using System;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Capp2.Helpers;
using System.Diagnostics;

namespace Capp2
{
	public static class StatsHelper
	{

		public static StackLayout CreateSplineChart(ObservableCollection<ChartDataPoint> data, string XLabel, 
			string YLabel, string LegendTitle)
		{
			SplineAreaSeries splineseries = CreateSplineAreaSeries (data, XLabel, YLabel);

			SfChart chart = new SfChart{
				Legend = CreateLegend(LegendTitle, Color.Maroon),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			chart.Series.Add(splineseries);

			return new StackLayout{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					chart,
				}
			};
		}

		public static SplineAreaSeries CreateSplineAreaSeries(ObservableCollection<ChartDataPoint> data, string XLabel, string YLabel){
			return new SplineAreaSeries{
				//EnableTooltip = true,
				//EnableDataPointSelection = true,
				//EnableAnimation = true,
				ItemsSource = data, 
				XBindingPath = XLabel, 
				YBindingPath = YLabel,
				/*StrokeColor = Color.FromHex(Values.YELLOW),
				DataMarker=new ChartDataMarker(){
					LabelStyle = new DataMarkerLabelStyle{
						Font = Font.SystemFontOfSize(18, FontAttributes.Bold)
					}
				},*/
			};
		}

		public static string GetYesCallMessage(bool synergy = false){
			var yescalls = App.Database.GetTodaysYesCalls ();
			string message = string.Empty;

			if (yescalls == 0) {
				if (synergy) {
					message = "has done absolutely nothing today. Nothing. " +
						"I'm telling Daniel Laogan. Better hope I don't find his number in this phone";
				} else {
					message =  "has done nothing today. Absolutely nothing. Sigh...";
				}
			} else if (yescalls < 5 && yescalls > 0) {
				if (synergy) {
					message = string.Format ("got {0} yes call(s) today! Not bad. Maybe I won't tell Sandy", yescalls);
				} else {
					message = string.Format ("got {0} yes call(s) today! Not bad.", yescalls);
				}
			}else if(yescalls > 5 && yescalls < 10){
				if (synergy) {
				} else {
				}
				message = string.Format ("got {0} yes call(s) today! Keep it up!!!", yescalls);
			}else if(yescalls >= 10){
				if (synergy) {
					message = string.Format ("got {0} yes call(s) today! " +
						"Team Elite here we go!", yescalls);
				} else {
					message = string.Format ("got {0} yes call(s) today! " +
						"Beat that!", yescalls);
				}
			}
			message += "\n\n- CapTap";

			return message;
		}

		public static ChartLegend CreateLegend(string title, Color TitleColor, 
			TextAlignment textalign = TextAlignment.Center,
			LegendPlacement legendPosition = LegendPlacement.Top, 
			ChartOrientation legendOrientation = ChartOrientation.Horizontal,
			double legendtitleborderwidth = 3, 
			bool toggleseriesvisibility = true)
		{
			return new ChartLegend{
				Title = new ChartTitle{
					Text = title,
					TextColor = TitleColor,
					TextAlignment = textalign,
					Font = Font.SystemFontOfSize(18, FontAttributes.Bold),
					BorderWidth = legendtitleborderwidth,
					BackgroundColor = Color.Transparent,
				},
				LabelStyle = new ChartLegendLabelStyle{
					Font = Font.SystemFontOfSize(12, FontAttributes.Bold)
				},
				DockPosition = legendPosition,
				Orientation = legendOrientation,
				ToggleSeriesVisibility = toggleseriesvisibility,
			};
		}
		public static bool HasCalledData(ChartData[] data){
			for (int c = 0; c < data.Length; c++) {
				if (string.Equals(data[c].Name, "Called") && data[c].Value > 0) {
					return true;
				}
			}
			return false;
		}

		public static async Task CheckForTips(string warmcold){
			var called = App.Database.GetCalledContacts (Values.ALLPLAYLISTPARAM);
			var appointed = App.Database.GetAppointedContacts (Values.ALLPLAYLISTPARAM);
			double calledCount = called.Count;
			double appointedCount = appointed.Count;
			double ratio = appointedCount / calledCount;

			if (string.Equals (warmcold, Values.WARMCONTACTS)) {
				if (ratio < 0.3) {
					AlertHelper.Alert ("Might wanna check your warm market calling technique. ", string.Format (
						"It's a bit low at around {0} yes call(s) for every {1} call(s)", ratio*10, calledCount));
				}
			}else if(string.Equals (warmcold, Values.COLDCONTACTS)) {
				if (ratio < 0.1) {
					AlertHelper.Alert ("Might wanna check your warm market calling technique. ", string.Format (
						"It's a bit low at {0} yes call(s) for every {1} call(s)", ratio*10, calledCount));
				}
			}
		}

		/*public EventHandler<ToggledEventArgs> OnPercentageOrActual(object sender, ToggledEventArgs e){
			
		}*/

		public static PieSeries CreatePieSeries(ObservableCollection<ChartDataPoint> data, string XLabel, string YLabel){

			return new PieSeries{
				EnableAnimation = true,
				ItemsSource = data, 
				XBindingPath = XLabel, 
				YBindingPath = YLabel ,
				//ExplodeIndex = 3,
				//ExplodeOnTouch = true,
				ExplodeRadius = 20,
				ExplodeAll = false,
				EnableSmartLabels = true,
				DataMarkerPosition = CircularSeriesDataMarkerPosition.OutsideExtended,
				ConnectorLineType= ConnectorLineType.Bezier,
				StartAngle=75,
				EndAngle=435,
				DataMarker=new ChartDataMarker(){
					LabelStyle = new DataMarkerLabelStyle{
						Font = Font.SystemFontOfSize(18, FontAttributes.Bold)
					}
				},
				//EnableTooltip = true,
				CircularCoefficient = 0.9,
			};
		}
		public static StackLayout CreatePieChart(ObservableCollection<ChartDataPoint> data, string XLabel, string YLabel, string LegendTitle){
			PieSeries pieseries = CreatePieSeries (data, XLabel, YLabel);
			SfChart chart = new SfChart{
				Legend = CreateLegend(LegendTitle, Color.Maroon),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			chart.GestureRecognizers.Add (new TapGestureRecognizer{
				NumberOfTapsRequired = 2, 
				Command = new Command(() => 
					{
						//pieseries.DataMarker.LabelContent = LabelContent.Percentage; 
						if(pieseries.DataMarker.LabelContent == LabelContent.Percentage){
							pieseries.DataMarker.LabelContent = LabelContent.YValue; 
						}else if(pieseries.DataMarker.LabelContent == LabelContent.YValue){
							pieseries.DataMarker.LabelContent = LabelContent.Percentage; 
						}
					}
				) });
			chart.Series.Add (pieseries);

			Switch switcher = new Switch ();
			switcher.Toggled += (object sender, ToggledEventArgs e) => {
				Debug.WriteLine("Reversing percentage or actual");
				if(pieseries.DataMarker.LabelContent == LabelContent.Percentage){
					pieseries.DataMarker.LabelContent = LabelContent.YValue; 
				}else if(pieseries.DataMarker.LabelContent == LabelContent.YValue){
					pieseries.DataMarker.LabelContent = LabelContent.Percentage; 
				}

				UIAnimationHelper.ShrinkUnshrinkElement(chart);
			};
			Label label = new Label{ 
				Text = "Show Percentage",  
			};
			Label totalLabel = new Label{ 
				TextColor = Color.FromHex(Values.MaterialDesignOrange),
				Text = string.Format("Total Calls: {0}", 
					App.Database.GetCalledContacts (Values.ALLPLAYLISTPARAM).Count),
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
			};

			return new StackLayout{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						Children = {
							label, switcher
						},
					},
					chart,
					totalLabel
				}
			};
		}

		public static ObservableCollection<ChartDataPoint> CreateChartData (ChartData[] list){
			ObservableCollection<ChartDataPoint> data = new ObservableCollection<ChartDataPoint> ();
			//convert input List to ChartDataPoint ObservableCollection
			for(int c = 0;c < list.Length;c++){
				if (list [c].Value > 0) {
					data.Add (new ChartDataPoint (list[c].Name, list[c].Value));
				}
			}

			return data;
		}
	}

	public class ChartData{
		public string Name{ get; set;}
		public double Value{ get; set;} = 0;
	}
}

