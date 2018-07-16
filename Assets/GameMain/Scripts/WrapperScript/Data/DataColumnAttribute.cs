using System;
using System.Runtime.CompilerServices;

namespace Data
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	internal class DataColumnAttribute : Attribute
	{
		public object DefaultValue
		{
			get;
			private set;
		}

		public bool IsAutoincrement
		{
			get;
			private set;
		}

		public bool IsPrimaryKey
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public DataColumnAttribute(string name, bool isPrimaryKey = false, bool isAutoincrement = false, object defaultValue = null)
		{
			this.Name = name;
			this.IsPrimaryKey = isPrimaryKey;
			this.IsAutoincrement = isAutoincrement;
			this.DefaultValue = defaultValue;
		}
	}
}