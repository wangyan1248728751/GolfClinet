using System;
using System.Collections.Generic;
using System.Linq;

public class AggregateException : Exception
{
	private Exception[] _exceptions;

	public Exception[] InnerExceptions
	{
		get
		{
			return this._exceptions.Clone() as Exception[];
		}
	}

	public AggregateException(IEnumerable<Exception> exceptions) : this("Multiple exceptions occured", exceptions)
	{
	}

	public AggregateException(string message, IEnumerable<Exception> exceptions) : base(message)
	{
		this._exceptions = exceptions.ToArray<Exception>();
	}
}