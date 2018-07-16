using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class AppLog
{
	private const string PrevFilePathKey = "PrevLogFilePathKey";

	private const string CurrentFilePathKey = "CurrentLogFilePathKey";

	private const string PrevWrapperLogPathKey = "PrevWrapperLogPathKey";

	private const string CurrentWrapperLogPathKey = "CurrentWrapperLogPathKey";

	private const string PrevSdkLogPathKey = "PrevSdkLogPathKey";

	private const string CurrentSdkLogPathKey = "CurrentSdkLogPathKey";

	private static string _currentFilePath;

	private static string _prevFilePath;

	private static string _currentWrapperLogPath;

	private static string _prevWrapperLogPath;

	private static string _currentSdkLogPath;

	private static string _prevSdkLogPath;

	private static bool _isInited;

	private static bool _writeToFile;

	private static IFileWriter _fileWriter;

	private static IEmailSender _emailSender;

	public static string CurrentWrapperLogPath
	{
		get
		{
			return AppLog._currentWrapperLogPath;
		}
	}

	private static string GenerateEncryptedUserDetails(string userName, string hashedPass, string esn, string previousEsn)
	{
		return StringCipher.Encrypt(string.Format("{0}\r{1}\r{2}\r{3}\r", new object[] { userName, hashedPass, esn, previousEsn }), "wW?gv*%@!hJ=QEexEm@FSVy5dd5nb5c#Qz?CZ8Gzd%havSt8RQ$fc&Zd5RZ=4$eD9#4H7k@Uh_WXm?Dw=NAmb8fCb3CQrQynPx=*");
	}

	private static string GenerateNewFileName(string extension)
	{
		return string.Format((extension == null ? "{0}" : "{0}.{1}"), Guid.NewGuid(), extension);
	}

	private static string GenerateNewFilePath(string extension)
	{
		return string.Format("{0}/{1}", Application.persistentDataPath, AppLog.GenerateNewFileName(extension));
	}

	private static string GetSdkLogPath()
	{
		DateTime utcNow = DateTime.UtcNow;
		return string.Format("{0}/{1}-{2}-{3}_{4}-{5}-{6}.log", new object[] { Application.persistentDataPath, utcNow.Day, utcNow.Month, utcNow.Year, utcNow.Hour, utcNow.Minute, utcNow.Second });
	}

	private static string GetUnityLogFilePath()
	{
		return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity\\Editor\\Editor.log");
	}

	public static void Init(bool writeToFile)
	{
		if (AppLog._isInited)
		{
			return;
		}
		Application.logMessageReceivedThreaded += new Application.LogCallback(AppLog.OnLogMessageReceivedThreaded);
		AppLog._isInited = true;
		AppLog._fileWriter = new SystemIoFileWriter();
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			//AppLog._emailSender = new EtceteraMailWriter();
		}
		else
		{
			//AppLog._emailSender = new PCMailWriter();
		}
		AppLog._currentFilePath = PlayerPrefs.GetString("CurrentLogFilePathKey");
		AppLog._prevFilePath = PlayerPrefs.GetString("PrevLogFilePathKey");
		AppLog._currentWrapperLogPath = PlayerPrefs.GetString("CurrentWrapperLogPathKey");
		AppLog._prevWrapperLogPath = PlayerPrefs.GetString("PrevWrapperLogPathKey");
		AppLog._fileWriter.RemoveFile(AppLog._prevFilePath);
		AppLog._fileWriter.RemoveFile(AppLog._prevWrapperLogPath);
		AppLog._prevFilePath = null;
		AppLog._prevWrapperLogPath = null;
		if (!string.IsNullOrEmpty(AppLog._currentFilePath))
		{
			AppLog._prevFilePath = AppLog._currentFilePath;
		}
		if (!string.IsNullOrEmpty(AppLog._currentWrapperLogPath))
		{
			AppLog._prevWrapperLogPath = AppLog._currentWrapperLogPath;
		}
		AppLog._currentFilePath = AppLog.GenerateNewFilePath("txt");
		AppLog._currentWrapperLogPath = AppLog.GenerateNewFilePath("txt");
		AppLog._writeToFile = writeToFile;
		PlayerPrefs.SetString("CurrentLogFilePathKey", AppLog._currentFilePath);
		PlayerPrefs.SetString("PrevLogFilePathKey", AppLog._prevFilePath);
		PlayerPrefs.SetString("CurrentWrapperLogPathKey", AppLog._currentWrapperLogPath);
		PlayerPrefs.SetString("PrevWrapperLogPathKey", AppLog._prevWrapperLogPath);
	}

	public static void Log(string logMessage, bool showLogOutput = true)
	{
		if (!AppLog._isInited)
		{
			AppLog.Init(true);
		}
		if (showLogOutput)
		{
			Debug.Log(logMessage);
		}
		if (AppLog._writeToFile)
		{
			DateTime utcNow = DateTime.UtcNow;
			AppLog._fileWriter.WriteToFile(AppLog._currentFilePath, string.Format("{0}: {1}", utcNow, logMessage));
		}
	}

	public static void LogError(string logMessage, bool showLogOutput = true)
	{
		if (!AppLog._isInited)
		{
			AppLog.Init(true);
		}
		if (showLogOutput)
		{
			Debug.LogError(logMessage);
		}
		if (AppLog._writeToFile)
		{
			AppLog._fileWriter.WriteToFile(AppLog._currentFilePath, string.Format("{0}: ERROR: {1}", DateTime.UtcNow, logMessage));
		}
	}

	public static void LogException(Exception exception, bool showLogOutput = true)
	{
		if (!AppLog._isInited)
		{
			AppLog.Init(true);
		}
		if (showLogOutput)
		{
			Debug.LogException(exception);
		}
		if (AppLog._writeToFile)
		{
			string str = string.Format("{0}: EXCEPTION: {1}. {2}\nSTACK TRACE:\n{3}", new object[] { DateTime.UtcNow, exception.GetType().Name, exception.Message, exception.StackTrace });
			for (Exception i = exception.InnerException; i != null; i = i.InnerException)
			{
				str = string.Concat(str, string.Format("\nINNER: {0}. {1}\nSTACK TRACE: \n{2}", i.GetType().Name, i.Message, i.StackTrace));
			}
			AppLog._fileWriter.WriteToFile(AppLog._currentFilePath, str);
		}
	}

	private static void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Exception)
		{
			return;
		}
		AppLog.LogError(string.Format("UNCAUGHT EXCEPTION: {0}{1}STACK_TRACE: {2}", condition, Environment.NewLine, stackTrace), true);
	}

	private static void PutStreamToZip(string fileNameExtention, ZipOutputStream zipStream, Stream stream)
	{
		ZipEntry zipEntry = new ZipEntry(AppLog.GenerateNewFileName(fileNameExtention))
		{
			DateTime = DateTime.Now
		};
		zipStream.PutNextEntry(zipEntry);
		StreamUtils.Copy(stream, zipStream, new byte[4096]);
		zipStream.CloseEntry();
		stream.Close();
		stream.Dispose();
	}

	public static void SendToEmail(bool currentLog = true)
	{
		//string str = AppLog.ZipData((!currentLog ? AppLog._prevFilePath : AppLog._currentFilePath), (!currentLog ? AppLog._prevWrapperLogPath : AppLog._currentWrapperLogPath), (!currentLog ? AppLog._prevSdkLogPath : AppLog._currentSdkLogPath), DatabaseManager.DatabasePath, LoginManager.Username, LoginManager.HashedPw, ApplicationDataManager.instance.ESN, ApplicationDataManager.instance.PreviousESN);
		//string str1 = string.Format("Log report, {0}, is current session - {1}", DateTime.UtcNow, currentLog);
		//AppLog._emailSender.SendMail("logs@skytrakgolf.com", "LogReport", str1, str);
	}

	public static void SetSdkLogPath()
	{
		AppLog._currentSdkLogPath = PlayerPrefs.GetString("CurrentSdkLogPathKey");
		AppLog._prevSdkLogPath = PlayerPrefs.GetString("PrevSdkLogPathKey");
		AppLog._fileWriter.RemoveFile(AppLog._prevSdkLogPath);
		AppLog._prevSdkLogPath = null;
		if (!string.IsNullOrEmpty(AppLog._currentSdkLogPath))
		{
			AppLog._prevSdkLogPath = AppLog._currentSdkLogPath;
		}
		AppLog._currentSdkLogPath = AppLog.GetSdkLogPath();
		Debug.Log(string.Concat("SDK LOG - ", AppLog._currentSdkLogPath));
		PlayerPrefs.SetString("CurrentSdkLogPathKey", AppLog._currentSdkLogPath);
		PlayerPrefs.SetString("PrevSdkLogPathKey", AppLog._prevSdkLogPath);
	}

	private static string ZipData(string pathToLog, string pathToWrapperLog, string pathToSdkLog, string pathToDb, string userName, string hashedPass, string esn, string previousEsn)
	{
		string str = AppLog.GenerateNewFilePath("package");
		using (FileStream fileStream = File.Create(str))
		{
			ZipOutputStream zipOutputStream = new ZipOutputStream(fileStream)
			{
				Password = "=-p(3S:*tLa&B^!^dDN`"
			};
			zipOutputStream.SetLevel(3);
			AppLog.PutStreamToZip("z", zipOutputStream, new MemoryStream(Encoding.UTF8.GetBytes(AppLog.GenerateEncryptedUserDetails(userName, hashedPass, esn, previousEsn))));
			AppLog.PutStreamToZip("l", zipOutputStream, File.OpenRead(pathToLog));
			AppLog.PutStreamToZip("d", zipOutputStream, File.OpenRead(pathToDb));
			if (File.Exists(pathToWrapperLog))
			{
				AppLog.PutStreamToZip("wl", zipOutputStream, File.Open(pathToWrapperLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			}
			if (File.Exists(pathToSdkLog))
			{
				AppLog.PutStreamToZip("sdkl", zipOutputStream, File.Open(pathToSdkLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			}
			string unityLogFilePath = AppLog.GetUnityLogFilePath();
			if (!string.IsNullOrEmpty(unityLogFilePath) && File.Exists(unityLogFilePath))
			{
				AppLog.PutStreamToZip("log", zipOutputStream, File.Open(unityLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			}
			zipOutputStream.Close();
		}
		return str;
	}
}