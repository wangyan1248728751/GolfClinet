using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums {

	

}

public enum ServerRequest
{
	RegisterSkytrak,
	AddShots,
	GetSessionObj,
	GetRemoteSessionObj,
	GetSessionsList,
	GetRemoteSessionData,
	GetChallengeScores,
	UpdateChallengeScores,
	UpdateActivity,
	UpdateSession,
	UpdateUser,
	GetCustomerEsns,
	GetSessionGuid,
	GetUserInfo,
	CreateAccount,
	ResetPassword,
	GetAppVersion
}
public enum ServerService
{
	Registration,
	Session,
	User,
	Authorization,
	Account,
	General
}
