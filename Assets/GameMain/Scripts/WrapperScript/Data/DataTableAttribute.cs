using System;
using System.Runtime.CompilerServices;

namespace Data
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class DataTableAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public DataTableAttribute(string name)
		{
			this.Name = name;
		}
	}
}