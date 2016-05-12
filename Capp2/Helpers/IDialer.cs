using System;
using System.Threading.Tasks;

namespace Capp2
{
	public interface IDialer
	{
		Task<bool> Dial(string number);
	}
}

