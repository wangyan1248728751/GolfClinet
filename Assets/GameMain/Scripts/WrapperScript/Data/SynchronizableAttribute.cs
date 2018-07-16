using System;
using System.Runtime.CompilerServices;

namespace Data
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class SynchronizableAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public bool Pull
		{
			get;
			private set;
		}

		public bool Push
		{
			get;
			private set;
		}

		public SynchronizableAttribute() : this(null, true, true)
		{
		}

		public SynchronizableAttribute(string name, bool push = true, bool pull = true)
		{
			this.Name = name;
			this.Push = push;
			this.Pull = pull;
		}
	}
}