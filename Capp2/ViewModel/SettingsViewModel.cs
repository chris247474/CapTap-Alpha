using System;
using System.ComponentModel;
using Capp2.Helpers;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace Capp2
{
	public class SettingsViewModel : INotifyPropertyChanged { 
		public int Count { 
			get { return Settings.Count; } 
			set { if (Settings.Count == value) 
				return; Settings.Count = value; OnPropertyChanged(); 
			} 
		} 
		private Command increase; 
		public Command IncreaseCommand { 
			get { return increase ?? (increase = new Command(() =>Count++)); } 
		} 
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName]string name = "") { 
			var changed = PropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		} 

		#region Setting BOMTemplateText
		public string BOMTemplateSettings { 
			get { return Settings.BOMTemplateSettings; } 
			set { if (Settings.BOMTemplateSettings == value) 
				return; Settings.BOMTemplateSettings = value; OnBOMTemplatePropertyChanged(); 
			} 
		} 
		public event PropertyChangedEventHandler BOMTemplatePropertyChanged;
		public void OnBOMTemplatePropertyChanged([CallerMemberName]string name = "") { 
			var changed = BOMTemplatePropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region Setting DailyEmailTemplateSettings
		public string DailyEmailTemplateSettings { 
			get { return Settings.DailyEmailTemplateSettings; } 
			set { if (Settings.DailyEmailTemplateSettings == value) 
				return; Settings.DailyEmailTemplateSettings = value; OnDailyEmailTemplateSettingsPropertyChanged(); 
			} 
		} 
		public event PropertyChangedEventHandler DailyEmailTemplateSettingsPropertyChanged;
		public void OnDailyEmailTemplateSettingsPropertyChanged([CallerMemberName]string emailname = "") { 
			var changed = DailyEmailTemplateSettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(emailname)); 
		}
		#endregion

		#region Setting DefaultNamelist
		public string DefaultNamelistSettings { 
			get { return Settings.DefaultNamelistSettings; } 
			set { if (Settings.DefaultNamelistSettings == value) 
				return; Settings.DefaultNamelistSettings = value; OnDefaultNamelistPropertyChanged(); 
			} 
		} 
		public event PropertyChangedEventHandler DefaultNamelistPropertyChanged;
		public void OnDefaultNamelistPropertyChanged([CallerMemberName]string name = "") { 
			var changed = DefaultNamelistPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region Setting BOMLocationSettings
		public string BOMLocationSettings { 
			get { return Settings.LocSettings; } 
			set { if (Settings.LocSettings == value) 
				return; Settings.LocSettings = value; OnBOMLocationPropertyChanged(); 
			} 
		} 
		public event PropertyChangedEventHandler BOMLocationPropertyChanged;
		public void OnBOMLocationPropertyChanged([CallerMemberName]string name = "") { 
			var changed = BOMLocationPropertyChanged; 
			if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region Setting BOMDayTimeSettings
		public string BOMDayTimeSettings { 
			get { return Settings.DayTimeSettings; } 
			set { if (Settings.DayTimeSettings == value) 
				return; Settings.DayTimeSettings = value; OnBOMDayTimePropertyChanged(); 
			} 
		} 
		public event PropertyChangedEventHandler BOMDayTimePropertyChanged;
		public void OnBOMDayTimePropertyChanged([CallerMemberName]string name = "") { 
			var changed = BOMDayTimePropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region UserRemindedForTomorrow
		public string RemindedForTomorrowSettings { 
			get { return Settings.RemindedForTomorrowSettings; } 
			set { if (Settings.RemindedForTomorrowSettings == value) 
				return; Settings.RemindedForTomorrowSettings = value; OnRemindedForTomorrowSettingsPropertyChanged(); 
			} 
		}
		public event PropertyChangedEventHandler RemindedForTomorrowSettingsPropertyChanged;
		public void OnRemindedForTomorrowSettingsPropertyChanged([CallerMemberName]string name = "") { 
			var changed = RemindedForTomorrowSettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region DateReminded
		public string DateRemindedSettings { 
			get { return Settings.DateRemindedSettings; } 
			set { if (Settings.DateRemindedSettings == value) 
				return; Settings.DateRemindedSettings = value; OnDateRemindedSettingsPropertyChanged(); 
			} 
		}
		public event PropertyChangedEventHandler DateRemindedSettingsPropertyChanged;
		public void OnDateRemindedSettingsPropertyChanged([CallerMemberName]string name = "") { 
			var changed = DateRemindedSettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region MeetingConfirmSettings
		public string MeetingConfirmSettings { 
			get { return Settings.MeetingConfirmSettings; } 
			set { if (Settings.MeetingConfirmSettings == value) 
				return; Settings.MeetingConfirmSettings = value; OnMeetingConfirmSettingsPropertyChanged(); 
			} 
		}
		public event PropertyChangedEventHandler MeetingConfirmSettingsPropertyChanged;
		public void OnMeetingConfirmSettingsPropertyChanged([CallerMemberName]string name = "") { 
			var changed = MeetingConfirmSettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion


		#region UserRemindedForToday
		public string RemindedForTodaySettings { 
			get { return Settings.RemindedForTodaySettings; } 
			set { if (Settings.RemindedForTodaySettings == value) 
				return; Settings.RemindedForTodaySettings = value; OnRemindedForTodaySettingsPropertyChanged(); 
			} 
		}
		public event PropertyChangedEventHandler RemindedForTodaySettingsPropertyChanged;
		public void OnRemindedForTodaySettingsPropertyChanged([CallerMemberName]string name = "") { 
			var changed = RemindedForTodaySettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region DateRemindedToday
		public string DateRemindedForTodaySettings { 
			get { return Settings.DateRemindedForTodaySettings; } 
			set { if (Settings.DateRemindedForTodaySettings == value) 
				return; Settings.DateRemindedForTodaySettings = value; OnDateRemindedForTodaySettingsPropertyChanged(); 
			} 
		}
		public event PropertyChangedEventHandler DateRemindedTodaySettingsPropertyChanged;
		public void OnDateRemindedForTodaySettingsPropertyChanged([CallerMemberName]string name = "") { 
			var changed = DateRemindedTodaySettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion

		#region MeetingTodayConfirmSettings
		public string MeetingTodayConfirmSettings { 
			get { return Settings.MeetingTodayConfirmSettings; } 
			set { if (Settings.MeetingTodayConfirmSettings == value) 
				return; Settings.MeetingTodayConfirmSettings = value; OnMeetingConfirmTodaySettingsPropertyChanged(); 
			} 
		}
		public event PropertyChangedEventHandler MeetingConfirmTodaySettingsPropertyChanged;
		public void OnMeetingConfirmTodaySettingsPropertyChanged([CallerMemberName]string name = "") { 
			var changed = MeetingConfirmTodaySettingsPropertyChanged; if (changed == null) return; 
			changed(this, new PropertyChangedEventArgs(name)); 
		}
		#endregion
	} 
}

