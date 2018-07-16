using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	internal static class StringParsing
	{
		public static bool ParseBool(this string value)
		{
			bool flag;
			if (!bool.TryParse(value, out flag))
			{
				throw new FormatException(string.Format("Couldn't parse bool: '{0}'", value));
			}
			return flag;
		}

		public static DateTime ParseDateTime(this string value)
		{
			DateTime dateTime;
			if (!DateTime.TryParse(value, out dateTime))
			{
				throw new FormatException(string.Format("Couldn't parse DateTime: '{0}'", value));
			}
			return dateTime;
		}

		public static double ParseDouble(this string value)
		{
			double num;
			if (!double.TryParse(value, out num))
			{
				throw new FormatException(string.Format("Couldn't parse double: '{0}'", value));
			}
			return num;
		}

		public static float ParseFloat(this string value)
		{
			float single;
			if (!float.TryParse(value, out single))
			{
				throw new FormatException(string.Format("Couldn't parse float: '{0}'", value));
			}
			return single;
		}

		public static int ParseInt(this string value)
		{
			int num;
			if (!int.TryParse(value, out num))
			{
				throw new FormatException(string.Format("Couldn't parse int: '{0}'", value));
			}
			return num;
		}

		public static long ParseLong(this string value)
		{
			long num;
			if (!long.TryParse(value, out num))
			{
				throw new FormatException(string.Format("Couldn't parse long: '{0}'", value));
			}
			return num;
		}

		public static short ParseShort(this string value)
		{
			short num;
			if (!short.TryParse(value, out num))
			{
				throw new FormatException(string.Format("Couldn't parse short: '{0}'", value));
			}
			return num;
		}
	}
}