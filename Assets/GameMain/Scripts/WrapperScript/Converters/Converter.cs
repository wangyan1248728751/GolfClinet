using System;

namespace Converters
{
	internal abstract class Converter<U, V>
	{
		protected Converter()
		{
		}

		public abstract U Convert(V source);

		public abstract V Convert(U source);
	}
}