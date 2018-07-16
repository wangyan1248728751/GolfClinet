using System;

namespace Data
{
	internal class DataEntryException : Exception
	{
		public DataEntryException(string message) : base(message)
		{
		}
	}
}