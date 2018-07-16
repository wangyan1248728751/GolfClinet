using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ClubPopUpManager : MonoBehaviour
{
	[SerializeField]
	private Text _handednessLabel;

	//[SerializeField]
	//private ClubSelectionButton _clubButton;

	//[SerializeField]
	//private ClubSelectionButton _numericClubButton;

	//[SerializeField]
	//private ClubSelectionPopup mainPopUp;

	//[SerializeField]
	//private ClubSelectionPopup numericPopUp;

	public const int MaxCompareClubs = 5;

	private int _clubCompareIdx;

	private readonly List<string> _compareClubNames = new List<string>();

	public int ClubCompareIdx
	{
		get
		{
			return this._clubCompareIdx;
		}
	}

	public ReadOnlyCollection<string> CompareClubNames
	{
		get
		{
			return this._compareClubNames.AsReadOnly();
		}
	}

	public ClubPopUpManager()
	{
	}

	public int GetClubTracerColorIndex(string name)
	{
		return CGameManager.instance.GetClubColorIndex(name);
	}

	public void OnClubCompareButtonClick(string clubName)
	{
		if (this._clubCompareIdx < 5)
		{
			string str = clubName;
			if (clubName.Contains("#"))
			{
				str = clubName.Substring(0, clubName.IndexOf("#"));
			}
			this._compareClubNames.Add(str);
			this._clubCompareIdx++;
		}
	}

	private void OnClubSelectedEvent(string clubName)
	{
		this.SetClub(clubName);
	}

	private void OnDestroy()
	{
		//CGameManager.instance.UiHolder.GameSettings.OnHandnessChanged -= new Action(this.OnHandnessChanged);
		//CGameManager.instance.UiHolder.ShotHistoryView.OnNewSession -= new Action(this.OnNewSession);
	}

	private void OnHandnessChanged()
	{
		//if (!CGameManager.instance.UiHolder.GameSettings.IsHandnessLefty)
		//{
		//	this._handednessLabel.text = "RIGHT HANDED";
		//}
		//else
		//{
		//	this._handednessLabel.text = "LEFT HANDED";
		//}
	}

	private void OnNewSession()
	{
		this.ResetClubCompare();
		this.SetClub("UNDEFINED");
	}

	public void ResetClubCompare()
	{
		//this._clubCompareIdx = 0;
		//this._compareClubNames.Clear();
		//this.mainPopUp.ResetToNewSession();
		//this.numericPopUp.ResetToNewSession();
	}

	public void SetClub(string clubName)
	{
		//if (CGameManager.instance.SetClub(clubName))
		//{
		//	this._clubButton.SetClub(clubName);
		//	if (this._numericClubButton != null)
		//	{
		//		this._numericClubButton.SetClub(clubName);
		//	}
		//	this.OnClubWasChanged();
		//}
	}

	private void Start()
	{
		//CGameManager.instance.UiHolder.GameSettings.OnHandnessChanged += new Action(this.OnHandnessChanged);
		//CGameManager.instance.UiHolder.ShotHistoryView.OnNewSession += new Action(this.OnNewSession);
		//this.mainPopUp.OnClubSelectedEvent = new Action<string>(this.OnClubSelectedEvent);
		//this.mainPopUp.Init(this);
		//if (this.numericPopUp != null)
		//{
		//	this.numericPopUp.OnClubSelectedEvent = new Action<string>(this.OnClubSelectedEvent);
		//	this.numericPopUp.Init(this);
		//}
	}

	public bool WasClubUsed(string name)
	{
		return CSimulationManager.instance.CheckClubUsed(name);
	}

	public event Action OnClubWasChanged;
}