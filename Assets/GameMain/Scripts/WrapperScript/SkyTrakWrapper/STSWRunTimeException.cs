using System;
using System.Runtime.Serialization;

namespace SkyTrakWrapper
{
	public class STSWRunTimeException : Exception
	{
		public STSWRunTimeException()
		{
		}

		public STSWRunTimeException(RIPEErrType errCode, string message) : base(string.Concat("SkyTrakSW Exception: ", message, ". Code: ", errCode.ToString("D")))
		{
		}

		public STSWRunTimeException(RIPEErrType errCode, string message, Exception inner) : base(string.Concat("SkyTrakSW Exception: ", message, ". Code: ", errCode.ToString("D")), inner)
		{
		}

		protected STSWRunTimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}