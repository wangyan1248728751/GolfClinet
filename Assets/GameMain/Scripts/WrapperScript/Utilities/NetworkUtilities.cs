using System;
using System.IO;
using System.Net;

namespace Utilities
{
	internal static class NetworkUtilities
	{
		private const int InternetAvailabilityCheckAttemptsCount = 3;

		public static string GetHtmlFromUri(string uri)
		{
			string empty;
			string str = string.Empty;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			try
			{
				using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					if (((int)response.StatusCode >= 299 ? false : response.StatusCode >= HttpStatusCode.OK))
					{
						using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
						{
							char[] chrArray = new char[80];
							streamReader.Read(chrArray, 0, (int)chrArray.Length);
							char[] chrArray1 = chrArray;
							for (int i = 0; i < (int)chrArray1.Length; i++)
							{
								str = string.Concat(str, chrArray1[i]);
							}
						}
					}
				}
				return str;
			}
			catch
			{
				empty = string.Empty;
			}
			return empty;
		}

		public static bool IsInternetAvailable()
		{
			int num = 0;
			do
			{
				int num1 = num;
				num = num1 + 1;
				if (num1 < 3)
				{
					continue;
				}
				return false;
			}
			while (!NetworkUtilities.GetHtmlFromUri("http://google.com").Contains("schema.org/WebPage"));
			return true;
		}
	}
}