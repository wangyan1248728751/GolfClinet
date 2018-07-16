using System;

namespace Data
{
	internal class SynchronizationException : Exception
	{
		public SynchronizationException(string message) : base(message)
		{
		}
	}
}