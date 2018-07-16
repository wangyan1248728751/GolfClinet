using Data;
using Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

public static class LoginManager
{
	private const string Salt = "D3a4E4sqVG04xe0";

	public const string sES2_REMEMBER_ME = "RememberMe";

	public const string sES2HashedPass = "hashedPassword_";

	private const int InternetCheckInterval = 30;

	private static User _user;

	private static bool _isLoggingIn;

	private static bool _isUserLoggedIn;

	private static bool _isOnlineLogin;

	private static bool _isInternetAvailable;

	private static bool _autoLogInWasFailed;

	private static bool _rememberUser;

	private static Action _onLogInCompleteAction;

	private static Action<LoginManager.AuthErrorCode, string> _onLogInFailAction;

	public static string HashedPw
	{
		get;
		private set;
	}

	public static bool IsUserLoggedIn
	{
		get
		{
			return LoginManager._isUserLoggedIn;
		}
	}

	public static bool IsUserSkippedLogIn
	{
		get;
		set;
	}

	public static bool RememberUser
	{
		get
		{
			return !string.IsNullOrEmpty(PlayerPrefs.GetString("RememberMe", string.Empty));
		}
	}

	public static string Token
	{
		get;
		private set;
	}

	public static User UserData
	{
		get
		{
			return LoginManager._user;
		}
	}

	public static string Username
	{
		get;
		private set;
	}

	static LoginManager()
	{
		LoginManager.IsUserSkippedLogIn = false;
		LoginManager._rememberUser = LoginManager.RememberUser;
		int num = Scheduler.instance.AddDelegate(new Scheduler.delegateMethod(LoginManager.CheckInternetConnection), 30);
		Scheduler.instance.SetSchedulerIndexToImmediate(num);
	}

	private static void AssignAuthCallbacks(Action onFinish, Action<LoginManager.AuthErrorCode, string> onError)
	{
		LoginManager._onLogInCompleteAction = onFinish;
		LoginManager._onLogInFailAction = onError;
		LoginManager.OnUserLogInComplete += LoginManager._onLogInCompleteAction;
		LoginManager.OnUserLogInFail += LoginManager._onLogInFailAction;
	}

	public static void AuthenticateUser(string username, string password, bool rememberUser, Action onFinish, Action<LoginManager.AuthErrorCode, string> onError)
	{
		string str = LoginManager.CreatePasswordHash(password, "D3a4E4sqVG04xe0");
		LoginManager._rememberUser = rememberUser;
		LoginManager.AssignAuthCallbacks(onFinish, onError);
		LoginManager.TryToLogIn(username, str);
	}

	private static void AuthenticateUserHash(string userName, string hashPassword)
	{
		LoginManager.SetLoggingInStatus(true);
		if (LoginManager.OnStartAuthProcess != null)
		{
			LoginManager.OnStartAuthProcess(userName);
		}
		LoginManager.HashedPw = hashPassword;
		LoginManager.Username = userName;
		Dictionary<string, string> strs = new Dictionary<string, string>()
		{
			{ "username", userName },
			{ "hashedpw", hashPassword },
			{ "dev", "E4F68-874E7-C80E5-1E8C3" }
		};
		WebService.CallWebService(ServerRequest.GetSessionGuid, strs, new Action<WebServiceResponse>(LoginManager.HandleAuthResponse), false);
	}

	private static void BuildUserOffline(int customerId)
	{
		LoginManager._user = DataEntry.GetInstance<User>(customerId);
		if (LoginManager._user == null)
		{
			string str = string.Format("User {0} was not found in DB", customerId);
			AppLog.LogError(str, true);
			LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.UserNotInDBError, str);
		}
		else
		{
			LoginManager._isOnlineLogin = false;
			LoginManager._isUserLoggedIn = true;
			LoginManager.FireLogInSuccessEvent();
		}
	}

	private static void BuildUserOnline()
	{
		Dictionary<string, string> strs = new Dictionary<string, string>()
		{
			{ "session", LoginManager.Token },
			{ "dev", "E4F68-874E7-C80E5-1E8C3" }
		};
		WebService.CallWebService(ServerRequest.GetUserInfo, strs, new Action<WebServiceResponse>(LoginManager.ResponseGetUserInfo), false);
	}

	private static void ChangeOnlineStatus(bool isInternetAvailable)
	{
		if (isInternetAvailable == LoginManager._isInternetAvailable)
		{
			return;
		}
		LoginManager._isInternetAvailable = isInternetAvailable;
		if (!LoginManager._isInternetAvailable)
		{
			LoginManager.Token = null;
			if (LoginManager._isUserLoggedIn)
			{
				LoginManager._isOnlineLogin = false;
				AppLog.Log("*** LoginManager: switched to offline mode ***", true);
			}
		}
	}

	private static void CheckInternetConnection()
	{
		LoginManager.ChangeOnlineStatus(NetworkInternetVerification.instance.VerifyInternetAvailability() == NetworkInternetVerification.NET_STATES.VERIFIED_ONLINE);
		AppLog.Log(string.Format("*** LoginManager: check internet connection - available = {0} ***", LoginManager._isInternetAvailable), true);
		if (LoginManager._isLoggingIn)
		{
			return;
		}
		if (LoginManager.RememberUser && !LoginManager._autoLogInWasFailed && !LoginManager._isUserLoggedIn)
		{
			string str = PlayerPrefs.GetString("RememberMe", string.Empty);
			string str1 = PlayerPrefs.GetString(string.Concat("hashedPassword_", str), string.Empty);
			if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str1))
			{
				LoginManager._autoLogInWasFailed = true;
				return;
			}
			LoginManager.TryAutoLogin(str, str1);
		}
		else if (LoginManager._isUserLoggedIn && !LoginManager._isOnlineLogin && LoginManager._isInternetAvailable)
		{
			LoginManager.TryToLogIn(LoginManager.Username, LoginManager.HashedPw);
		}
	}

	public static string ConvertPasswordToHash(string password)
	{
		return LoginManager.CreatePasswordHash(password, "D3a4E4sqVG04xe0");
	}

	public static void CreateAccount(Dictionary<string, string> parameters, Action onFinish, Action<LoginManager.AuthErrorCode, string> onError)
	{
		LoginManager.HashedPw = parameters["hashed_pw"];
		LoginManager.Username = parameters["username"];
		Dictionary<string, string> strs = new Dictionary<string, string>()
		{
			{ "username", parameters["username"] },
			{ "hashed_pw", parameters["hashed_pw"] },
			{ "email", parameters["email"] },
			{ "gender", parameters["gender"] },
			{ "firstname", parameters["firstname"] },
			{ "lastname", parameters["lastname"] },
			{ "dev", "E4F68-874E7-C80E5-1E8C3" },
			{ "dexterity", parameters["dexterity"] }
		};
		Dictionary<string, string> strs1 = strs;
		if (parameters.ContainsKey("country"))
		{
			strs1.Add("country", parameters["country"]);
		}
		if (parameters.ContainsKey("phone_number"))
		{
			strs1.Add("phone_number", parameters["phone_number"]);
		}
		if (parameters.ContainsKey("lead_source"))
		{
			strs1.Add("lead_source", parameters["lead_source"]);
		}
		if (parameters.ContainsKey("shipping_address1") || parameters.ContainsKey("shipping_address2") || parameters.ContainsKey("shipping_city") || parameters.ContainsKey("shipping_state") || parameters.ContainsKey("shipping_postal_code"))
		{
			strs1.Add("shipping_address1", (!parameters.ContainsKey("shipping_address1") ? string.Empty : parameters["shipping_address1"]));
			strs1.Add("shipping_address2", (!parameters.ContainsKey("shipping_address2") ? string.Empty : parameters["shipping_address2"]));
			strs1.Add("shipping_city", (!parameters.ContainsKey("shipping_city") ? string.Empty : parameters["shipping_city"]));
			strs1.Add("shipping_state", (!parameters.ContainsKey("shipping_state") ? string.Empty : parameters["shipping_state"]));
			strs1.Add("shipping_postal_code", (!parameters.ContainsKey("shipping_postal_code") ? string.Empty : parameters["shipping_postal_code"]));
		}
		LoginManager.AssignAuthCallbacks(onFinish, onError);
		LoginManager._rememberUser = true;
		WebService.CallWebService(ServerRequest.CreateAccount, strs1, new Action<WebServiceResponse>(LoginManager.HandleAuthResponse), false);
	}

	private static string CreatePasswordHash(string password, string salt)
	{
		string str = string.Concat(password, salt);
		SHA512 sHA512Managed = new SHA512Managed();
		string str1 = BitConverter.ToString(sHA512Managed.ComputeHash(Encoding.ASCII.GetBytes(str)));
		return str1.Replace("-", string.Empty);
	}

	private static void DisableAutoLogin()
	{
		LoginManager._rememberUser = false;
		PlayerPrefs.SetString("RememberMe", string.Empty);
	}

	private static void FireLogInFailedEvent(LoginManager.AuthErrorCode errorCode, string errorMessage)
	{
		if (LoginManager.OnUserLogInFail != null)
		{
			LoginManager.OnUserLogInFail(errorCode, errorMessage);
			LoginManager.OnUserLogInFail -= LoginManager._onLogInFailAction;
			LoginManager._onLogInFailAction = null;
		}
		switch (errorCode)
		{
			case LoginManager.AuthErrorCode.UnknownError:
			case LoginManager.AuthErrorCode.AuthError:
			case LoginManager.AuthErrorCode.GetUserInfoError:
			case LoginManager.AuthErrorCode.GetCustomerEsnsError:
			case LoginManager.AuthErrorCode.UserNotInDBError:
				{
					LoginManager.DisableAutoLogin();
					break;
				}
		}
	}

	private static void FireLogInSuccessEvent()
	{
		if (LoginManager.OnUserLogInComplete != null)
		{
			LoginManager.OnUserLogInComplete();
			LoginManager.OnUserLogInComplete -= LoginManager._onLogInCompleteAction;
			LoginManager._onLogInCompleteAction = null;
		}
		PlayerPrefs.SetString("RememberMe", (!LoginManager._rememberUser ? string.Empty : LoginManager.Username));
		PlayerPrefs.SetString(string.Concat("hashedPassword_", LoginManager.Username), LoginManager.HashedPw);
		ApplicationDataManager.instance.StartSession();
	}

	private static void GetUserESNs(int customerId)
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("customerid", customerId.ToString());
		JSONObject jSONObject1 = new JSONObject();
		jSONObject1.AddField("data", jSONObject);
		Dictionary<string, string> strs = new Dictionary<string, string>()
		{
			{ "dev", "E4F68-874E7-C80E5-1E8C3" },
			{ "data", jSONObject1.ToString() },
			{ "session", LoginManager.Token }
		};
		WebService.CallWebService(ServerRequest.GetCustomerEsns, strs, new Action<WebServiceResponse>(LoginManager.ResponseGetCustomerEsns), false);
	}

	private static void HandleAuthResponse(WebServiceResponse response)
	{
		if (!response.Success)
		{
			LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.AuthError, response.Message);
			LoginManager.SetLoggingInStatus(false);
		}
		else
		{
			LoginManager.Token = response.Data["session-guid"].str;
			if (!string.IsNullOrEmpty(LoginManager.Token))
			{
				AppLog.Log("Auth was success.", true);
				LoginManager.BuildUserOnline();
			}
			else
			{
				string str = "ERROR: Unknown error - Account was not created";
				AppLog.LogError(str, true);
				LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.AuthError, str);
				LoginManager.SetLoggingInStatus(false);
			}
		}
	}

	private static void HandleResetPasswordResponse(WebServiceResponse response, Action onFinish, Action<LoginManager.AuthErrorCode, string> onError)
	{
		if (!response.Success)
		{
			AppLog.LogError(string.Concat("ResetWassword was failed. Reason: ", response.Message), true);
			if (onError != null)
			{
				onError((AuthErrorCode)5, response.Message);
			}
		}
		else if (onFinish != null)
		{
			onFinish();
		}
	}

	public static void LogOut()
	{
		LoginManager.Token = null;
		LoginManager._user = null;
		LoginManager._isUserLoggedIn = false;
		LoginManager._isOnlineLogin = false;
		LoginManager.DisableAutoLogin();
		if (LoginManager.OnUserLoggedOut != null)
		{
			LoginManager.OnUserLoggedOut();
		}
	}

	public static void ResetPassword(string username, Action onFinish, Action<LoginManager.AuthErrorCode, string> onError)
	{
		if (!LoginManager._isInternetAvailable)
		{
			if (onError != null)
			{
				onError((AuthErrorCode)5, "Internet is not available.");
			}
			return;
		}
		Dictionary<string, string> strs = new Dictionary<string, string>()
		{
			{ "username", username },
			{ "dev", "E4F68-874E7-C80E5-1E8C3" }
		};
		WebService.CallWebService(ServerRequest.ResetPassword, strs, (WebServiceResponse response) => LoginManager.HandleResetPasswordResponse(response, onFinish, onError), false);
	}

	private static void ResponseGetCustomerEsns(WebServiceResponse response)
	{
		if (!response.Success)
		{
			AppLog.LogError(string.Concat("ERROR: ", response.Message), true);
			LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.GetCustomerEsnsError, response.Message);
			LoginManager.SetLoggingInStatus(false);
			return;
		}
		JSONObject item = response.Data["data"]["DATA"];
		List<string> list = (
			from e in item.list
			select e[1].str.Trim(new char[] { '\"' })).ToList<string>();
		ApplicationDataManager.instance.SetPreviousUsedESNList(list);
		foreach (string str in list)
		{
			SecurityWrapperService.Instance.AddEsnToCache(str);
		}
		LoginManager._isOnlineLogin = true;
		LoginManager._isUserLoggedIn = true;
		LoginManager.FireLogInSuccessEvent();
		LoginManager.SetLoggingInStatus(false);
	}

	private static void ResponseGetUserInfo(WebServiceResponse response)
	{
		if (!response.Success)
		{
			AppLog.LogError(string.Concat("ERROR: ", response.Message), true);
			LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.GetUserInfoError, response.Message);
			LoginManager.SetLoggingInStatus(false);
			return;
		}
		string empty = string.Empty;
		if (response.Data.HasField("customerid"))
		{
			response.Data.GetField(ref empty, "customerid", null);
		}
		string userDexterity = "";
		//string userDexterity = DatabaseManager.GetUserDexterity(int.Parse(empty));
		LoginManager._user = DataEntry.DeserializeFromData<User>(response.Data);
		if (LoginManager._user == null)
		{
			string str = "Error during parsing User from server.";
			AppLog.LogError(str, true);
			LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.GetUserInfoError, str);
			LoginManager.SetLoggingInStatus(false);
		}
		else
		{
			if (LoginManager._user.Status == 0)
			{
				LoginManager._user.SetUserStatus(User.UserStatus.Guest);
			}
			if (!string.IsNullOrEmpty(userDexterity))
			{
				LoginManager._user.Dexterity = userDexterity;
			}
			if (string.IsNullOrEmpty(LoginManager._user.Dexterity))
			{
				LoginManager._user.Dexterity = "R";
			}
			//DatabaseManager.InsertOrUpdate<User>(LoginManager._user);
			LoginManager.GetUserESNs(LoginManager._user.Id);
		}
	}

	private static void ResponseUpdateUser(WebServiceResponse response)
	{
		if (!response.Success)
		{
			return;
		}
		if (string.IsNullOrEmpty(response.Message))
		{
			return;
		}
		if (response.Message.Contains("already exist"))
		{
			LoginManager._user.Action = "update";
			LoginManager.UpdateUser();
		}
		else if (response.Message.Contains("does not exist"))
		{
			LoginManager._user.Action = "add";
			LoginManager.UpdateUser();
		}
	}

	private static void SetLoggingInStatus(bool status)
	{
		LoginManager._isLoggingIn = status;
	}

	private static void TryAutoLogin(string username, string hashedPassw)
	{
		if (LoginManager.OnStartAuthProcess != null)
		{
			LoginManager.OnStartAuthProcess(username);
		}
		LoginManager._rememberUser = true;
		LoginManager.TryToLogIn(username, hashedPassw);
	}

	private static void TryToLogIn(string username, string hashPass)
	{
		if (!LoginManager._isInternetAvailable)
		{
			UserLoginData userLoginDatum = new UserLoginData();

			//UserLoginData userLoginDatum = DatabaseManager.SearchForUserBySkyGolfUserName(username);
			string str = PlayerPrefs.GetString(string.Concat("hashedPassword_", username), string.Empty);
			if (userLoginDatum == null)
			{
				LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.UserNotInDBError, "User was not found in DB");
				return;
			}
			if (!string.IsNullOrEmpty(str))
			{
				LoginManager.Username = username;
				LoginManager.HashedPw = str;
			}
			else
			{
				str = PlayerPrefs.GetString(string.Concat("hashedPassword_", userLoginDatum.Email));
				LoginManager.Username = userLoginDatum.Email;
				LoginManager.HashedPw = str;
			}
			if (string.IsNullOrEmpty(str) || !str.Equals(hashPass))
			{
				LoginManager.FireLogInFailedEvent(LoginManager.AuthErrorCode.AuthError, "Saved user password and current password are not equals");
				return;
			}
			LoginManager.BuildUserOffline(userLoginDatum.CustomerId);
		}
		else
		{
			LoginManager.AuthenticateUserHash(username, hashPass);
		}
	}

	public static void UpdateUser()
	{
		if (!LoginManager._isUserLoggedIn || LoginManager._user == null)
		{
			return;
		}
		//DatabaseManager.InsertOrUpdate<User>(LoginManager._user);
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("data", DataEntry.SerializeToJson<User>(LoginManager._user));
		if (LoginManager._isInternetAvailable)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "session", LoginManager.Token },
				{ "dev", "E4F68-874E7-C80E5-1E8C3" },
				{ "data", jSONObject.ToString() },
				{ "status", LoginManager._user.Status.ToString() }
			};
			WebService.CallWebService(ServerRequest.UpdateUser, strs, new Action<WebServiceResponse>(LoginManager.ResponseUpdateUser), false);
		}
	}

	public static event Action<string> OnStartAuthProcess;

	public static event Action OnUserLoggedOut;

	public static event Action OnUserLogInComplete;

	public static event Action<LoginManager.AuthErrorCode, string> OnUserLogInFail;

	public enum AuthErrorCode
	{
		UnknownError,
		AuthError,
		GetUserInfoError,
		GetCustomerEsnsError,
		UserNotInDBError,
		ResetPasswordError
	}
}