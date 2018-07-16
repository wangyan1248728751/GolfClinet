using System;
using UnityEngine;

public class GameUIHolder : MonoBehaviour
{
	public const int LeaderboardMenuSiblingId = 13;

	public const int RoundsAndTurnsPanelSiblingId = 3;

	public const int MiniScoreBoardPanelSiblingId = 4;

	public GameObject UiDialsParent;

	public GameObject PauseMenu;

	public GameSettings GameSettings;

	public TracerManager TracerManager;

	public CGolfCamera GolfCamera;

	//public GameExitPanelView GameExitPanel;

	//public PreAndPostShotPillView PrePostPill;

	//public NumericDisplays NumericDisplays;

	//public NumericDisplayStatic NumericDisplaysStatic;

	//public ShotHistoryView ShotHistoryView;

	//public PanelFade PanelFade;

	//public DispersionCirclesOverheadView DispCircleOffline;

	//public GameFieldEllipses GameFieldEllipses;

	//public DispersionCirclesClubView DispersionCirclesClubView;

	//public ClubPopUpManager ClubPopUpManager;

	//public HitMissView HitMissView;

	//public OfflinePanelTargetView OfflinePanelTargetView;

	//public PracticeGreensSettingsView PracticeGreensSettingsView;

	//public PracticeGreensHudView PracticeGreenHudView;

	//public GreenTargetView GreenTargetView;

	//public SkillsAssessmentHudView SkillsAssessmentHudView;

	//public BagMappingHudView BagMappingHudView;

	//public LeaderboardMenuView ChallengesGameView;

	public void CleanAll()
	{
		//this.NumericDisplays.Clean();
	}
}