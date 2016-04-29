// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using Xamarin.Forms;

namespace Capp2.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
    private static ISettings AppSettings
    {
      get
      {
        return CrossSettings.Current;
      }
    }
	
	#region DefaultNamelist
	private const string DefaultNamelist = Values.ALLPLAYLISTPARAM;
	private static readonly string DefaultNamelistDefault = Values.ALLPLAYLISTPARAM;
	#endregion

	public static string DefaultNamelistSettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(DefaultNamelist, DefaultNamelistDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(DefaultNamelist, value);
		}
	}

	#region Setting RemindedForToday
		private const string RemindedForToday = "";
		private static readonly string RemindedForTodayDefault = Values.MEETINGSNOTYETREMINDED;
	#endregion

	public static string RemindedForTodaySettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(RemindedForToday, RemindedForTodayDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(RemindedForToday, value);
		}
	}
	#region Setting MeetingTodayConfirmation
	private const string MeetingTodayConfirm = "";
		private static readonly string MeetingTodayConfirmDefault = ",see you later";
	#endregion

	public static string MeetingTodayConfirmSettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(MeetingTodayConfirm, MeetingTodayConfirmDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(MeetingTodayConfirm, value);
		}
	}

	#region Setting DateRemindedForToday
		private const string DateRemindedForToday = "";
		private static readonly string DateRemindedForTodayDefault = DateTime.MinValue.Day.ToString ();
	#endregion

	public static string DateRemindedForTodaySettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(DateRemindedForToday, DateRemindedForTodayDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(DateRemindedForToday, value);
		}
	}
	
	

	#region Setting RemindedForTomorrow
		private const string RemindedForTomorrow = "";
		private static readonly string RemindedForTomorrowDefault = Values.MEETINGSNOTYETREMINDED;
	#endregion

	public static string RemindedForTomorrowSettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(RemindedForTomorrow, RemindedForTomorrowDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(RemindedForTomorrow, value);
		}
	}

	#region Setting MeetingConfirmation
		private const string MeetingConfirm = "";
		private static readonly string MeetingConfirmDefault = ",see you tomorrow";
	#endregion

	public static string MeetingConfirmSettings
	{
		get
		{
				return AppSettings.GetValueOrDefault<string>(MeetingConfirm, MeetingConfirmDefault);
		}
		set
		{
				AppSettings.AddOrUpdateValue<string>(MeetingConfirm, value);
		}
	}

	#region Setting DateReminded
	private const string DateReminded = "";
	private static readonly string DateRemindedDefault = DateTime.MinValue.Day.ToString ();
	#endregion

	public static string DateRemindedSettings
	{
		get
		{
				return AppSettings.GetValueOrDefault<string>(DateReminded, DateRemindedDefault);
		}
		set
		{
				AppSettings.AddOrUpdateValue<string>(DateReminded, value);
		}
	}
	
	#region Setting Location Constants

	private const string LocKey = "name_key";
	private static readonly string LocDefault = "<meetup here>";
	#endregion
	
	public static string LocSettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(LocKey, LocDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(LocKey, value);
		}
	}

	#region Setting meeting Date and Time

	private const string MeetDateTimeKey = "datetime_key";
	private static readonly string MeetDateTimeDefault = "";
		#endregion
	
	public static string DayTimeSettings
	{
		get
		{
			return AppSettings.GetValueOrDefault<string>(MeetDateTimeKey, MeetDateTimeDefault);
		}
		set
		{
			AppSettings.AddOrUpdateValue<string>(MeetDateTimeKey, value);
		}
	}

    #region Setting BOMTemplateText

    private const string BOMKey = "bom_key";
		private static readonly string BOMDefault = "we can meet at <meeting here>. We introduce our guests to the speaker " +
			"then all go up together. " +
			"See you <date here> :). Please reply if recieved ";

		#endregion
    public static string BOMTemplateSettings
    {
      get
      {
        return AppSettings.GetValueOrDefault<string>(BOMKey, BOMDefault);
      }
      set
      {
        AppSettings.AddOrUpdateValue<string>(BOMKey, value);
      }
    }


	
	#region Setting CountKey Constants
	const string CountKey = "count"; 
	private static readonly int CountDefault = 0; 
	#endregion

	public static int Count { 
		get { return AppSettings.GetValueOrDefault<int>(CountKey, CountDefault); } 
		set { AppSettings.AddOrUpdateValue<int>(CountKey, value); } 
	}

	
	
  }
}