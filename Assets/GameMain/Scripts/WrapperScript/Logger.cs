using SkyTrakWrapper.Interfaces;
using System;

public class Logger : ILogger
{
	public Logger()
	{
		AppLog.Init(true);
	}

	public void Debug(string value)
	{
		AppLog.Log(string.Concat("(Wrapper Debug): ", value), true);
	}

	public void Error(string value)
	{
		AppLog.Log(string.Concat("(Wrapper Error): ", value), true);
	}

	public void Info(string value)
	{
		AppLog.Log(string.Concat("(Wrapper Info): ", value), true);
	}

	public void Warning(string value)
	{
		AppLog.Log(string.Concat("(Wrapper Warning): ", value), true);
	}
}

namespace SkyTrakWrapper.Interfaces
{
	public interface ILogger
	{
		void Debug(string value);

		void Error(string value);

		void Info(string value);

		void Warning(string value);
	}
}

public interface IFileWriter
{
	void RemoveFile(string path);

	void WriteToFile(string path, string text);
}

public interface IEmailSender
{
	void SendMail(string toAddress, string subject, string body, string pathToAttach);
}