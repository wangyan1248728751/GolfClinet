using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SystemIoFileWriter : IFileWriter
{
	private const int BUFFER_SIZE = 2;

	private List<string> _logBuffer = new List<string>(2);

	public SystemIoFileWriter()
	{
	}

	void IFileWriter.RemoveFile(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		if (File.Exists(path))
		{
			try
			{
				File.Delete(path);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				AppLog.Log(string.Concat("Exception while removing file ", exception.Message), true);
				throw;
			}
		}
	}

	void IFileWriter.WriteToFile(string path, string text)
	{
		if (!File.Exists(path))
		{
			string str = (!string.IsNullOrEmpty(SoftwareVersion.PrevVersion) ? string.Format(", Prev Version: {0}", SoftwareVersion.PrevVersion) : string.Empty);
			File.WriteAllText(path, string.Format("Log initialized {0} ({1}) {2}, {3}{4}{5}", new object[] { SoftwareVersion.Version, DateTime.UtcNow, Application.platform, WebService.ServerUrl, str, Environment.NewLine }));
		}
		this._logBuffer.Add(text);
		if (this._logBuffer.Count >= 2)
		{
			try
			{
				string empty = string.Empty;
				foreach (string str1 in this._logBuffer)
				{
					empty = string.Concat(empty, str1, Environment.NewLine);
				}
				File.AppendAllText(path, empty);
				this._logBuffer.Clear();
			}
			catch (Exception exception)
			{
				Debug.LogError(exception.Message);
			}
		}
	}
}