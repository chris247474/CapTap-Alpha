using System;
using Newtonsoft.Json;

namespace Capp2
{
	public class PlaylistItemAzure
	{
		string id, playlistname;
		
		[JsonProperty(PropertyName = "id")]
		public string ID 
		{
			get { return id; }
			set { id = value;}
		}

		[JsonProperty(PropertyName = "PlaylistName")]
		public string PlaylistName { get{return playlistname; } set{playlistname = value; } }
	}
}

