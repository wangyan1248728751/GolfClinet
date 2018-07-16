using System;
using System.Runtime.CompilerServices;

public static class CGameSelectManager
{
	public static CGameManager.GAME_RULES_TYPE GameSelected
	{
		get;
		set;
	}

	static CGameSelectManager()
	{
		CGameSelectManager.GameSelected = CGameManager.GAME_RULES_TYPE.None;
	}
}