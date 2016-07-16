using System;
namespace Capp2
{
	public interface IFileHelper
	{
		void SaveText(string filename, string text);
		string LoadText(string filename);
	}
}

