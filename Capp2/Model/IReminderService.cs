using System;

namespace Capp2
{
	public interface IReminderService
	{
		void Remind(DateTime dateTime, string title, string message);
	}
}

