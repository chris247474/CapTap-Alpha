using System;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Capp2
{
	public static class StatsHelper
	{
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
					Font = Font.SystemFontOfSize(20, FontAttributes.Bold),
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
		public static SfChart CreatePieChart(ObservableCollection<ChartDataPoint> data, string XLabel, string YLabel, string LegendTitle){
			
			SfChart chart = new SfChart{
				Legend = CreateLegend(LegendTitle, Color.Maroon),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			chart.Series.Add (
				new PieSeries{
					EnableAnimation = true,
					ItemsSource = data, 
					XBindingPath = XLabel, 
					YBindingPath = YLabel ,
					ExplodeIndex = 1,
					ExplodeOnTouch = true,
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
					//EnableSmartLabels = true,
				}
			);
			return chart;
		}

		public static ObservableCollection<ChartDataPoint> CreatePieChartData (ChartData[] list){
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
		public double Value{ get; set;}
	}
}

