using System;
using System.Collections.Generic;
using UnityEngine;

public class PLAYER
{
	public string playerID;

	public string playerName;

	private bool _isLHanded;

	public int[] scores;

	public IList<TShotData> shotData;

	public IList<Vector3> ballFinalPosition;

	public bool isLHanded
	{
		get
		{
			return this._isLHanded;
		}
	}

	public PLAYER(string playerID, string playerName, bool pointsBasedGame, uint roundsToBePlayed, uint turnsPerRound, bool isLeftHanded)
	{
		unsafe
		{
			this.playerID = playerID;
			this.playerName = playerName;
			this._isLHanded = isLeftHanded;
			if (pointsBasedGame)
			{
				this.scores = new int[roundsToBePlayed * turnsPerRound];
			}
			this.shotData = new List<TShotData>();
			this.ballFinalPosition = new List<Vector3>();
		}
	}
}
public struct TShotData
{
	public float m_ballSpeed;

	public float m_launchAngleDegrees;

	public float m_backSpin;

	public float m_sideSpin;

	public float m_sideAngle;

	public float m_clubSpeed;
}