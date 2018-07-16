using System;
using System.Runtime.Serialization;

namespace SkyTrakWrapper
{
	public class STSWNotIntializedException : Exception
	{
		public STSWNotIntializedException()
		{
		}

		public STSWNotIntializedException(string message) : base(message)
		{
		}

		public STSWNotIntializedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected STSWNotIntializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}