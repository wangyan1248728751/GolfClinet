using System;
using System.Runtime.CompilerServices;

public static class WebServiceRequestTypesExtentions
{
	public static ServerService GetService(this ServerRequest request)
	{
		switch (request)
		{
			case ServerRequest.RegisterSkytrak:
				{
					return ServerService.Registration;
				}
			case ServerRequest.AddShots:
			case ServerRequest.GetSessionObj:
			case ServerRequest.GetRemoteSessionObj:
			case ServerRequest.GetSessionsList:
			case ServerRequest.GetRemoteSessionData:
			case ServerRequest.GetChallengeScores:
			case ServerRequest.UpdateChallengeScores:
			case ServerRequest.UpdateActivity:
			case ServerRequest.UpdateSession:
				{
					return ServerService.Session;
				}
			case ServerRequest.UpdateUser:
			case ServerRequest.GetCustomerEsns:
				{
					return ServerService.User;
				}
			case ServerRequest.GetSessionGuid:
				{
					return ServerService.Authorization;
				}
			case ServerRequest.GetUserInfo:
			case ServerRequest.CreateAccount:
			case ServerRequest.ResetPassword:
				{
					return ServerService.Account;
				}
			case ServerRequest.GetAppVersion:
				{
					return ServerService.General;
				}
		}
		throw new NotSupportedException(string.Format("Request type {0} is not supported", request));
	}

	public static string GetUrlPart(this ServerRequest request)
	{
		switch (request)
		{
			case ServerRequest.RegisterSkytrak:
				{
					return "register_skytrak";
				}
			case ServerRequest.AddShots:
				{
					return "add_shots";
				}
			case ServerRequest.GetSessionObj:
				{
					return "get_session_obj";
				}
			case ServerRequest.GetRemoteSessionObj:
				{
					return "get_remote_session_obj";
				}
			case ServerRequest.GetSessionsList:
				{
					return "get_sessions_list";
				}
			case ServerRequest.GetRemoteSessionData:
				{
					return "get_remote_session_data";
				}
			case ServerRequest.GetChallengeScores:
				{
					return "get_challenge_scores";
				}
			case ServerRequest.UpdateChallengeScores:
				{
					return "update_challenge_scores";
				}
			case ServerRequest.UpdateActivity:
				{
					return "update_activity";
				}
			case ServerRequest.UpdateSession:
				{
					return "update_session";
				}
			case ServerRequest.UpdateUser:
				{
					return "update_user";
				}
			case ServerRequest.GetCustomerEsns:
				{
					return "get_customer_esns";
				}
			case ServerRequest.GetSessionGuid:
				{
					return "get_session_guid";
				}
			case ServerRequest.GetUserInfo:
				{
					return "get_user_info";
				}
			case ServerRequest.CreateAccount:
				{
					return "create_account";
				}
			case ServerRequest.ResetPassword:
				{
					return "email_resetPassword";
				}
			case ServerRequest.GetAppVersion:
				{
					return "get_app_version";
				}
		}
		throw new NotSupportedException(string.Format("Request type {0} is not supported", request));
	}

	public static string GetUrlPart(this ServerService service)
	{
		switch (service)
		{
			case ServerService.Registration:
				{
					return "registration";
				}
			case ServerService.Session:
				{
					return "session";
				}
			case ServerService.User:
				{
					return "user";
				}
			case ServerService.Authorization:
				{
					return "authorization";
				}
			case ServerService.Account:
				{
					return "account";
				}
			case ServerService.General:
				{
					return null;
				}
		}
		throw new NotSupportedException(string.Format("Service type {0} is not supported", service));
	}

	public static bool IsSkyTrakCall(this ServerRequest request)
	{
		switch (request)
		{
			case ServerRequest.RegisterSkytrak:
			case ServerRequest.AddShots:
			case ServerRequest.GetSessionObj:
			case ServerRequest.GetRemoteSessionObj:
			case ServerRequest.GetSessionsList:
			case ServerRequest.GetRemoteSessionData:
			case ServerRequest.GetChallengeScores:
			case ServerRequest.UpdateChallengeScores:
			case ServerRequest.UpdateActivity:
			case ServerRequest.UpdateSession:
			case ServerRequest.UpdateUser:
			case ServerRequest.GetCustomerEsns:
			case ServerRequest.GetAppVersion:
				{
					return true;
				}
			case ServerRequest.GetSessionGuid:
			case ServerRequest.GetUserInfo:
			case ServerRequest.CreateAccount:
			case ServerRequest.ResetPassword:
				{
					return false;
				}
		}
		throw new NotSupportedException(string.Format("Request type {0} is not supported", request));
	}
}