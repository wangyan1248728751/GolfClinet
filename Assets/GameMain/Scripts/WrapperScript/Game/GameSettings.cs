using Data;
using Security;
using SkyTrakWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
	private readonly List<string> _cameraAngles = new List<string>()
	{
		"FIRST PERSON",
		"DYNAMIC",
		"DOWNRANGE",
		"FOLLOW BALL",
		"45 DEGREE"
	};

	private readonly List<string> _tracers = new List<string>()
	{
		"SINGLE SHOT",
		"5 SHOTS",
		"50 SHOTS"
	};

	private readonly List<string> _groundConditions = new List<string>()
	{
		"NORMAL",
		"SOFT",
		"FIRM"
	};

	private readonly List<string> _windDirections = new List<string>()
	{
		"NONE",
		"RANDOM",
		"NORTH",
		"NORTH-EAST",
		"EAST",
		"SOUTH-EAST",
		"SOUTH",
		"SOUTH-WEST",
		"WEST",
		"NORTH-WEST"
	};

	private readonly List<string> _windSpeeds = new List<string>()
	{
		"NONE",
		"0-5",
		"5-10",
		"10-15",
		"15-20",
		"20-25"
	};

	private readonly List<string> _humidity = new List<string>()
	{
		"10%",
		"20%",
		"30%",
		"40%",
		"50%",
		"60%",
		"70%",
		"80%",
		"90%",
		"100%"
	};

	private const float DisabledUIAlpha = 0.7f;

	private const string CAltitudeDefaultValue = "500";

	private const string CTemperatureDefaultValue = "70";

	private const float CameraOffsetDefaultValue = 0.5f;

	private const string PlayerPrefsSessionSettingsSuffix = "|JsonSessionSettings";

	private const string NoLoggedInUser = "NoLoggedInUser";

	[SerializeField]
	private GameObject _bgObject;

	[SerializeField]
	private GameObject _pin;

	[SerializeField]
	private GameObject _centerline;

	[SerializeField]
	private Text _rangeMarkersHeader;

	[SerializeField]
	private CGolfCamera _golfCamera;

	[SerializeField]
	private Button _resetButton;

	[SerializeField]
	private CameraAnglePopup _cameraAngleDropdown;

	[SerializeField]
	private Dropdown _tracersDropdown;

	[SerializeField]
	private Dropdown _conditionsDropdown;

	[SerializeField]
	private Dropdown _windSpeedDropdown;

	[SerializeField]
	private Dropdown _windDirectionDropdown;

	[SerializeField]
	private Dropdown _humidityDropdown;

	[SerializeField]
	private Slider _rangeMarkersSlider;

	[SerializeField]
	private Slider _centerlineSlider;

	[SerializeField]
	private Slider _dispersionCirclesSlider;

	[SerializeField]
	private Slider _mainDistanceSlider;

	[SerializeField]
	private Slider _shotDispersionSlider;

	[SerializeField]
	private Slider _handnessSlider;

	[SerializeField]
	private Slider _shotSpinSlider;

	[SerializeField]
	private InputField _altitudeInput;

	[SerializeField]
	private InputField _temperatureInput;

	[SerializeField]
	private CanvasGroup _cameraAngleGroup;

	[SerializeField]
	private CanvasGroup _tracersGroup;

	[SerializeField]
	private CanvasGroup _mainDistanceGroup;

	[SerializeField]
	private CanvasGroup _dispersionCirclesGroup;

	[SerializeField]
	private CanvasGroup _rangeMarkersGroup;

	[SerializeField]
	private CanvasGroup _centerlineGroup;

	[SerializeField]
	private CanvasGroup _dominantHandGroup;

	[SerializeField]
	private CanvasGroup _ballSpinGroup;

	[SerializeField]
	private CanvasGroup _windDirectionGroup;

	[SerializeField]
	private CanvasGroup _windSpeedGroup;

	[SerializeField]
	private CanvasGroup _humidityGroup;

	[SerializeField]
	private CanvasGroup _temperatureGroup;

	[SerializeField]
	private CanvasGroup _altitudeGroup;

	[SerializeField]
	private CanvasGroup _dispersionDistanceGroup;

	[SerializeField]
	private CameraOffsetView _cameraOffsetView;

	private bool _initialized;

	private bool bProfileLeftHandFlag;

	private bool _saveSettingsGlobal;

	private JSONObject JsonSettings = new JSONObject();

	private CGameManager _gameManager;

	private Color _greenSliderColor = new Color32(123, 193, 68, 255);

	private Color _redSliderColor = new Color32(174, 52, 0, 255);

	public Action OnDispersionDistanceSettingChanged;

	public Action<bool> OnMainDistanceSettingChanged;

	public Action<bool> OnBallSpinSettingChanged;

	public Action OnHandnessChanged;

	public bool DispersionCirclesEnabled
	{
		get
		{
			return this._dispersionCirclesSlider.@value >= 1f;
		}
	}

	private bool IsActive
	{
		get
		{
			return this._bgObject.activeInHierarchy;
		}
	}

	public bool IsDispersionDistanceTotal
	{
		get
		{
			return this._shotDispersionSlider.@value >= 1f;
		}
	}

	public bool IsHandnessLefty
	{
		get;
		private set;
	}

	private bool isPracticeMode
	{
		get
		{
			return CGameManager.instance.currentGameType == CGameManager.GAME_RULES_TYPE.Practice;
		}
	}

	private bool isSAorBMMode
	{
		get
		{
			return (CGameManager.instance.currentGameType == CGameManager.GAME_RULES_TYPE.SkillsAssessment ? true : CGameManager.instance.currentGameType == CGameManager.GAME_RULES_TYPE.BagMapping);
		}
	}

	public bool IsSpinAxisActive
	{
		get
		{
			return this._shotSpinSlider.@value <= 0f;
		}
	}

	public TracerManager.TRACER_AMOUNT TracerAmount
	{
		get;
		private set;
	}

	public GameSettings()
	{
	}

	private void ApplyCameraOffset(float offsetVal)
	{
		this.ApplyCameraOffsetInternal(offsetVal, true);
	}

	private void ApplyCameraOffsetInternal(float offsetVal, bool saveSetting)
	{
		this._cameraAngleDropdown.SetBottomButtonMark((offsetVal < 0.5f ? true : offsetVal > 0.5f));
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.CameraOffsetValue, offsetVal.ToString("F3"));
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(true);
		}
	}

	private void BlockCertainItemsDependingOnGameMode()
	{
		if (CGameManager.instance.currentGameType == CGameManager.GAME_RULES_TYPE.Practice)
		{
			return;
		}
		if (CGameManager.instance.currentGameType == CGameManager.GAME_RULES_TYPE.SkillsAssessment || CGameManager.instance.currentGameType == CGameManager.GAME_RULES_TYPE.BagMapping)
		{
			this._tracersDropdown.@value = 2;
			this._tracersDropdown.interactable = false;
			this._tracersGroup.alpha = 0.7f;
			this.SetTracersInternal(false);
			this._handnessSlider.interactable = false;
			this._handnessSlider.GetComponent<EventTrigger>().enabled = false;
			this._dominantHandGroup.alpha = 0.7f;
			this.SetHandnessInternal(false);
			this._dispersionCirclesSlider.@value = 0f;
			this._dispersionCirclesSlider.interactable = false;
			this._dispersionCirclesSlider.GetComponent<EventTrigger>().enabled = false;
			this._dispersionCirclesGroup.alpha = 0.7f;
			this.SetDispersionCirclesInternal(false);
			this._shotDispersionSlider.interactable = false;
			this._shotDispersionSlider.GetComponent<EventTrigger>().enabled = false;
			this._dispersionDistanceGroup.alpha = 0.7f;
			this.SetDispersionDistanceInternal(false);
			return;
		}
		this._cameraAngleDropdown.interactable = false;
		this._cameraAngleGroup.alpha = 0.7f;
		this.SetCameraInternal(false);
		this._tracersDropdown.@value = 0;
		this._tracersDropdown.interactable = false;
		this._tracersGroup.alpha = 0.7f;
		this.SetTracersInternal(false);
		if (CGameManager.instance.currentGameType != CGameManager.GAME_RULES_TYPE.LongDrive)
		{
			this.SetRangeMarkersInternal(false);
		}
		else
		{
			this._rangeMarkersSlider.@value = 0f;
			this._rangeMarkersSlider.interactable = false;
			this._rangeMarkersSlider.GetComponent<EventTrigger>().enabled = false;
			this._rangeMarkersGroup.alpha = 0.7f;
			this.SetRangeMarkersInternal(false);
		}
		this._centerlineSlider.@value = 0f;
		this._centerlineSlider.interactable = false;
		this._centerlineSlider.GetComponent<EventTrigger>().enabled = false;
		this._centerlineGroup.alpha = 0.7f;
		this.SetCenterlineInternal(false);
		this._dispersionCirclesSlider.@value = 0f;
		this._dispersionCirclesSlider.interactable = false;
		this._dispersionCirclesSlider.GetComponent<EventTrigger>().enabled = false;
		this._dispersionCirclesGroup.alpha = 0.7f;
		this.SetDispersionCirclesInternal(false);
		this._shotDispersionSlider.interactable = false;
		this._shotDispersionSlider.GetComponent<EventTrigger>().enabled = false;
		this._dispersionDistanceGroup.alpha = 0.7f;
		this.SetDispersionDistanceInternal(false);
		this._handnessSlider.interactable = false;
		this._handnessSlider.GetComponent<EventTrigger>().enabled = false;
		this._dominantHandGroup.alpha = 0.7f;
	}

	private void ChangeSliderToggleValue(string stringValue, Slider slider)
	{
		bool flag = slider.@value >= 1f;
		if (bool.TryParse(stringValue, out flag))
		{
			slider.@value = (!flag ? 0f : 1f);
		}
	}

	private void ChangeToggleSetting(string stringValue, Toggle toggleLeft, Toggle toggleRight)
	{
		bool flag = toggleRight.isOn;
		if (bool.TryParse(stringValue, out flag))
		{
			toggleLeft.isOn = !flag;
			toggleRight.isOn = flag;
		}
	}

	private string GetSettingFromJson(GameSettings.GameSetting setting)
	{
		if (!this.JsonSettings.HasField(setting.ToString()))
		{
			return string.Empty;
		}
		return this.JsonSettings[setting.ToString()].str;
	}

	public void Initialize()
	{
		this.bProfileLeftHandFlag = ApplicationDataManager.instance.GetPlayerHandednessIsLefty();
		this._rangeMarkersSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._rangeMarkersSlider.@value = (this._rangeMarkersSlider.@value < 1f ? 1f : 0f));
		this._centerlineSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._centerlineSlider.@value = (this._centerlineSlider.@value < 1f ? 1f : 0f));
		this._dispersionCirclesSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._dispersionCirclesSlider.@value = (this._dispersionCirclesSlider.@value < 1f ? 1f : 0f));
		this._mainDistanceSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._mainDistanceSlider.@value = (this._mainDistanceSlider.@value < 1f ? 1f : 0f));
		this._shotDispersionSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._shotDispersionSlider.@value = (this._shotDispersionSlider.@value < 1f ? 1f : 0f));
		this._handnessSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._handnessSlider.@value = (this._handnessSlider.@value < 1f ? 1f : 0f));
		this._shotSpinSlider.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => this._shotSpinSlider.@value = (this._shotSpinSlider.@value < 1f ? 1f : 0f));
		this._gameManager = CGameManager.instance;
		this._cameraAngleDropdown.options = (
			from option in this._cameraAngles
			select new Dropdown.OptionData(option)).ToList<Dropdown.OptionData>();
		this._tracersDropdown.options = (
			from option in this._tracers
			select new Dropdown.OptionData(option)).ToList<Dropdown.OptionData>();
		this._conditionsDropdown.options = (
			from option in this._groundConditions
			select new Dropdown.OptionData(option)).ToList<Dropdown.OptionData>();
		this._windDirectionDropdown.options = (
			from option in this._windDirections
			select new Dropdown.OptionData(option)).ToList<Dropdown.OptionData>();
		this._windSpeedDropdown.options = (
			from option in this._windSpeeds
			select new Dropdown.OptionData(option)).ToList<Dropdown.OptionData>();
		this._humidityDropdown.options = (
			from option in this._humidity
			select new Dropdown.OptionData(option)).ToList<Dropdown.OptionData>();
		this.LoadDefaults();
		this.TryLoadSessionSettingsFromPlayerPrefs();
		this.SetFromCurrentSettings();
		this.BlockCertainItemsDependingOnGameMode();
		this.SetMembershipFlags();
		this.SetCameraInternal(false);
		this._initialized = true;
	}

	private void LoadDefaults()
	{
		this.SaveSettingToJson(GameSettings.GameSetting.CameraValue, this._cameraAngles[0]);
		this.SaveSettingToJson(GameSettings.GameSetting.CameraOffsetValue, 0.5f.ToString());
		this.SaveSettingToJson(GameSettings.GameSetting.TracesValue, this._tracers[0]);
		this.SaveSettingToJson(GameSettings.GameSetting.GroundConditionsValue, this._groundConditions[2]);
		this.SaveSettingToJson(GameSettings.GameSetting.WindValue, this._windDirections[0]);
		this.SaveSettingToJson(GameSettings.GameSetting.WindSpeedValue, this._windSpeeds[0]);
		this.SaveSettingToJson(GameSettings.GameSetting.HumidityValue, this._humidity[4]);
		this.SaveSettingToJson(GameSettings.GameSetting.AltidudeValue, "500");
		this.SaveSettingToJson(GameSettings.GameSetting.TemperatureValue, "70");
		this.SaveSettingToJson(GameSettings.GameSetting.RangeMarkersValue, "true");
		this.SaveSettingToJson(GameSettings.GameSetting.CenterLineValue, "true");
		this.SaveSettingToJson(GameSettings.GameSetting.DispersionCircles, "false");
		this.SaveSettingToJson(GameSettings.GameSetting.DispersionDistance, "true");
		this.SaveSettingToJson(GameSettings.GameSetting.DistanseToggleValue, "true");
		this.SaveSettingToJson(GameSettings.GameSetting.BallSpinToggleValue, "true");
		this.LoadHandnessFromProfile();
	}

	private void LoadHandnessFromProfile()
	{
		string str;
		if (!LoginManager.IsUserLoggedIn)
		{
			str = "false";
		}
		else
		{
			AppLog.Log(string.Concat("PauseSettings: persistent profile hand setting: ", this.bProfileLeftHandFlag), true);
			str = this.bProfileLeftHandFlag.ToString();
		}
		this.SaveSettingToJson(GameSettings.GameSetting.HandToggleValue, str);
	}

	private void OnAdjustCameraOffsetClick()
	{
		//SlideInAndOutPanel.HideAll();
		this._cameraOffsetView.Show(this._golfCamera.CameraOffset);
	}

	private void OnAltitudeChanged(string alt)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetAltitudeInternal(true);
	}

	private void OnBallSpinChanged(float value)
	{
		if (!this._initialized)
		{
			return;
		}
		this.ToggleSpinObjectsInternal(true);
	}

	private void OnCameraAngleChanged(int idx)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetCameraInternal(true);
	}

	private void OnCenterlineChanged(float val)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetCenterlineInternal(true);
	}

	private void OnDisable()
	{
		this._resetButton.onClick.RemoveListener(new UnityAction(this.Reset));
		this._cameraAngleDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnCameraAngleChanged));
		this._tracersDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnTracersChanged));
		this._conditionsDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnGroundConditionChanged));
		this._windSpeedDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnWindSpeedChanged));
		this._windDirectionDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnWindDirectionChanged));
		this._humidityDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnHumidityChanged));
		this._rangeMarkersSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnRangeMarkersChanged));
		this._centerlineSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnCenterlineChanged));
		this._dispersionCirclesSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnDispersionCirclesChanged));
		this._mainDistanceSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnMainDistanceChanged));
		this._shotDispersionSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnDispersionDistanceChanged));
		this._handnessSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnMainHandChanged));
		this._shotSpinSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnBallSpinChanged));
		this._altitudeInput.onEndEdit.RemoveListener(new UnityAction<string>(this.OnAltitudeChanged));
		this._temperatureInput.onEndEdit.RemoveListener(new UnityAction<string>(this.OnTemperatureChanged));
	}

	private void OnDispersionCirclesChanged(float val)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetDispersionCirclesInternal(true);
	}

	private void OnDispersionDistanceChanged(float value)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetDispersionDistanceInternal(true);
	}

	private void OnEnable()
	{
		this._resetButton.onClick.AddListener(new UnityAction(this.Reset));
		this._cameraAngleDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnCameraAngleChanged));
		this._cameraAngleDropdown.OnAdjustOffsetButton = new Action(this.OnAdjustCameraOffsetClick);
		this._tracersDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnTracersChanged));
		this._conditionsDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnGroundConditionChanged));
		this._windSpeedDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnWindSpeedChanged));
		this._windDirectionDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnWindDirectionChanged));
		this._humidityDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnHumidityChanged));
		this._rangeMarkersSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnRangeMarkersChanged));
		this._centerlineSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnCenterlineChanged));
		this._dispersionCirclesSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnDispersionCirclesChanged));
		this._mainDistanceSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnMainDistanceChanged));
		this._shotDispersionSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnDispersionDistanceChanged));
		this._handnessSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnMainHandChanged));
		this._shotSpinSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnBallSpinChanged));
		this._altitudeInput.onEndEdit.AddListener(new UnityAction<string>(this.OnAltitudeChanged));
		this._temperatureInput.onEndEdit.AddListener(new UnityAction<string>(this.OnTemperatureChanged));
		this._cameraOffsetView.OnOffsetChanged = new Action<float>(this.SetCameraOffset);
		this._cameraOffsetView.OnApplyOffset = new Action<float>(this.ApplyCameraOffset);
	}

	public void OnGameShutDown()
	{
		if (LoginManager.IsUserLoggedIn && CSimulationManager.instance != null)
		{
			CSimulationManager.instance.UpdateHMPlayerHandedness(this.bProfileLeftHandFlag);
		}
	}

	private void OnGroundConditionChanged(int idx)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetCourseConditionInternal(true);
	}

	private void OnHumidityChanged(int idx)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetHumidityInternal(true);
	}

	private void OnMainDistanceChanged(float value)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetMainDistDisplayInternal(true);
	}

	private void OnMainHandChanged(float value)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetHandnessInternal(true);
	}

	private void OnRangeMarkersChanged(float val)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetRangeMarkersInternal(true);
	}

	private void OnTemperatureChanged(string temp)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetTempInternal(true);
	}

	private void OnTracersChanged(int idx)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetTracersInternal(true);
	}

	private void OnWindDirectionChanged(int idx)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetWindDirectionInternal(true);
	}

	private void OnWindSpeedChanged(int idx)
	{
		if (!this._initialized)
		{
			return;
		}
		this.SetWindSpeedInternal(true);
	}

	private void RemoveItemFromPopup(List<Dropdown.OptionData> dropdownOptions, STSWMMSFeatureFlagType flag, string cmpString)
	{
		if (!MembershipManager.GetAccess(flag))
		{
			int num = dropdownOptions.FindIndex(0, (Dropdown.OptionData s) => s.text.Equals(cmpString));
			if (dropdownOptions.Count > num && num >= 0)
			{
				dropdownOptions.RemoveAt(num);
			}
		}
	}

	private void Reset()
	{
		this._saveSettingsGlobal = true;
		this.LoadDefaults();
		this.SetFromCurrentSettings();
		this.SetMembershipFlags();
	}

	private void SaveSessionSettingsToPlayerPrefsIfEnabled(bool ignoreIsActive = false)
	{
		string str;
		if (!this._initialized || !this.IsActive && !ignoreIsActive)
		{
			return;
		}
		str = (!LoginManager.IsUserLoggedIn ? "NoLoggedInUser|JsonSessionSettings" : string.Concat(LoginManager.UserData.Email, "|JsonSessionSettings"));
		PlayerPrefs.SetString(str, this.JsonSettings.ToString());
	}

	private void SaveSettingToJson(GameSettings.GameSetting setting, string value)
	{
		if (!this.JsonSettings.HasField(setting.ToString()))
		{
			this.JsonSettings.AddField(setting.ToString(), value);
		}
		else
		{
			this.JsonSettings[setting.ToString()] = JSONObject.CreateStringObject(value);
		}
	}

	private void SetAltitudeInternal(bool saveSetting)
	{
		float single;
		try
		{
			single = float.Parse(this._altitudeInput.text);
		}
		catch (Exception exception)
		{
			single = 0f;
		}
		WeatherManager.instance.SetAltitude(single);
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.AltidudeValue, single.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
	}

	private void SetCameraInternal(bool saveSetting)
	{
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson((!this.isSAorBMMode ? GameSettings.GameSetting.CameraValue : GameSettings.GameSetting.SACameraValue), this._cameraAngleDropdown.options[this._cameraAngleDropdown.@value].text);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		this._golfCamera.RemoveOverride();
		switch (this._cameraAngleDropdown.@value)
		{
			case 0:
				{
					this._golfCamera.SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE);
					this._gameManager.SetupPlayCamera(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE, true);
					break;
				}
			case 1:
				{
					this._golfCamera.SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE);
					this._gameManager.SetupPlayCamera(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_LAND_DYNAMIC, true);
					break;
				}
			case 2:
				{
					this._golfCamera.SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE);
					this._gameManager.SetupPlayCamera(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_DOWNRANGE, false);
					break;
				}
			case 3:
				{
					this._golfCamera.SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE);
					this._gameManager.SetupPlayCamera(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_FOLLOW_BEHIND_BALL, false);
					break;
				}
			case 4:
				{
					this._golfCamera.UserOverride(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES);
					this._gameManager.SetupPlayCamera(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES, false);
					break;
				}
			default:
				{
					this._golfCamera.RemoveOverride();
					break;
				}
		}
		this._cameraAngleDropdown.ChangeAvailabilityAdjustOffsetButton((!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_CameraAngleOffset) || this._golfCamera.GetCurrentBehavior() == CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES ? false : this._golfCamera.GetCurrentBehavior() != CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES_ZOOM));
	}

	private void SetCameraOffset(float offsetVal)
	{
		this._golfCamera.SetOffset(offsetVal);
	}

	private void SetCenterlineInternal(bool saveSetting)
	{
		bool flag = this._centerlineSlider.@value >= 1f;
		Image component = this._centerlineSlider.transform.GetChild(0).GetComponent<Image>();
		component.color = (!flag ? this._redSliderColor : this._greenSliderColor);
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.CenterLineValue, flag.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		if (this._centerline != null)
		{
			this._centerline.SetActive(flag);
		}
	}

	private void SetCourseConditionInternal(bool saveSetting)
	{
		string item = this._conditionsDropdown.options[this._conditionsDropdown.@value].text;
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.GroundConditionsValue, item);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		GrassManager.instance.ChangeDefaultMaterial(item);
	}

	private void SetDispersionCirclesInternal(bool saveSetting)
	{
		bool flag = this._dispersionCirclesSlider.@value >= 1f;
		Image component = this._dispersionCirclesSlider.transform.GetChild(0).GetComponent<Image>();
		component.color = (!flag ? this._redSliderColor : this._greenSliderColor);
		//CGameManager.instance.UiHolder.DispersionCirclesClubView.Show(flag);
		//CGameManager.instance.UiHolder.DispCircleOffline.Show(flag);
		//CGameManager.instance.UiHolder.GameFieldEllipses.Show(flag);
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.DispersionCircles, flag.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
	}

	private void SetDispersionDistanceInternal(bool saveSetting)
	{
		bool flag = this._shotDispersionSlider.@value >= 1f;
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.DispersionDistance, flag.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		if (this.OnDispersionDistanceSettingChanged != null)
		{
			this.OnDispersionDistanceSettingChanged();
		}
	}

	private void SetFromCurrentSettings()
	{
		float single;
		if (this.isPracticeMode)
		{
			this._cameraAngleDropdown.@value = this._cameraAngles.IndexOf(this.GetSettingFromJson(GameSettings.GameSetting.CameraValue));
			this.SetCameraInternal(false);
			this._tracersDropdown.@value = this._tracers.IndexOf(this.GetSettingFromJson(GameSettings.GameSetting.TracesValue));
			this.SetTracersInternal(false);
			this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.RangeMarkersValue), this._rangeMarkersSlider);
			this.SetRangeMarkersInternal(false);
			this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.CenterLineValue), this._centerlineSlider);
			this.SetCenterlineInternal(false);
			this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.DispersionCircles), this._dispersionCirclesSlider);
			this.SetDispersionCirclesInternal(false);
			this.UpdateHandnessValue(this.bProfileLeftHandFlag);
		}
		else if (this.isSAorBMMode)
		{
			string settingFromJson = this.GetSettingFromJson(GameSettings.GameSetting.SACameraValue);
			if (string.IsNullOrEmpty(settingFromJson))
			{
				settingFromJson = this._cameraAngles[3];
			}
			this._cameraAngleDropdown.@value = this._cameraAngles.IndexOf(settingFromJson);
			this.SetCameraInternal(false);
			this.UpdateHandnessValue(this.bProfileLeftHandFlag);
		}
		if (!float.TryParse(this.GetSettingFromJson(GameSettings.GameSetting.CameraOffsetValue), out single))
		{
			single = 0.5f;
		}
		this.SetCameraOffset(single);
		this.ApplyCameraOffsetInternal(single, false);
		this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.RangeMarkersValue), this._rangeMarkersSlider);
		this.SetRangeMarkersInternal(false);
		this._conditionsDropdown.@value = this._groundConditions.IndexOf(this.GetSettingFromJson(GameSettings.GameSetting.GroundConditionsValue));
		this.SetCourseConditionInternal(false);
		this._windDirectionDropdown.@value = this._windDirectionDropdown.options.FindIndex((Dropdown.OptionData data) => data.text.Equals(this.GetSettingFromJson(GameSettings.GameSetting.WindValue)));
		this.SetWindDirectionInternal(false);
		this._windSpeedDropdown.@value = this._windSpeeds.IndexOf(this.GetSettingFromJson(GameSettings.GameSetting.WindSpeedValue));
		this.SetWindSpeedInternal(false);
		this._humidityDropdown.@value = this._humidity.IndexOf(this.GetSettingFromJson(GameSettings.GameSetting.HumidityValue));
		this.SetHumidityInternal(false);
		this._altitudeInput.text = this.GetSettingFromJson(GameSettings.GameSetting.AltidudeValue);
		this.SetAltitudeInternal(false);
		this._temperatureInput.text = this.GetSettingFromJson(GameSettings.GameSetting.TemperatureValue);
		this.SetTempInternal(false);
		this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.DistanseToggleValue), this._mainDistanceSlider);
		this.SetMainDistDisplayInternal(false);
		this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.BallSpinToggleValue), this._shotSpinSlider);
		this.ToggleSpinObjectsInternal(false);
		this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.DispersionDistance), this._shotDispersionSlider);
		this.SetDispersionDistanceInternal(false);
		this._saveSettingsGlobal = false;
	}

	private void SetHandnessInternal(bool saveSetting)
	{
		this.IsHandnessLefty = this._handnessSlider.@value <= 0f;
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.HandToggleValue, this.IsHandnessLefty.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		if (CSimulationManager.instance != null)
		{
			CSimulationManager.instance.UpdateHMPlayerHandedness(this.IsHandnessLefty);
		}
		if (this.OnHandnessChanged != null)
		{
			this.OnHandnessChanged();
		}
	}

	private void SetHumidityInternal(bool saveSetting)
	{
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.HumidityValue, this._humidityDropdown.options[this._humidityDropdown.@value].text);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		WeatherManager.instance.SetHumidity((WeatherManager.Humidity)this._humidityDropdown.@value);
	}

	private void SetMainDistDisplayInternal(bool saveSetting)
	{
		bool flag = this._mainDistanceSlider.@value >= 1f;
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.DistanseToggleValue, flag.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		if (this.OnMainDistanceSettingChanged != null)
		{
			this.OnMainDistanceSettingChanged(flag);
		}
	}

	private void SetMembershipAltitude()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_Environmental_Altitude))
		{
			this._altitudeInput.text = "500";
			this._altitudeInput.interactable = false;
			this._altitudeGroup.alpha = 0.7f;
			this.SetAltitudeInternal(false);
		}
	}

	private void SetMembershipCameraOffset()
	{
		if (MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_CameraAngleOffset))
		{
			this._cameraAngleDropdown.ChangeAvailabilityAdjustOffsetButton(true);
		}
		else
		{
			this.SetCameraOffset(0.5f);
			this.ApplyCameraOffsetInternal(0.5f, false);
			this._cameraAngleDropdown.ChangeAvailabilityAdjustOffsetButton(false);
		}
	}

	private void SetMembershipDispersionCircles()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_DispersionCircles))
		{
			this._dispersionCirclesSlider.@value = 0f;
			this._dispersionCirclesSlider.interactable = false;
			this._dispersionCirclesSlider.GetComponent<EventTrigger>().enabled = false;
			this._dispersionCirclesGroup.alpha = 0.7f;
			this._shotDispersionSlider.interactable = false;
			this._shotDispersionSlider.GetComponent<EventTrigger>().enabled = false;
			this._dispersionDistanceGroup.alpha = 0.7f;
		}
	}

	private void SetMembershipFlags()
	{
		this.SetMembershipFlagsCamera(this._cameraAngleDropdown);
		this.SetMembershipFlagsTracers(this._tracersDropdown);
		this.SetMembershipFlagsGroundConditions(this._conditionsDropdown);
		this.SetMembershipDispersionCircles();
		this.SetMembershipSideSpin();
		this.SetMembershipWindSpeed();
		this.SetMembershipHumidity();
		this.SetMembershipAltitude();
		this.SetMembershipHandness();
		this.SetMembershipCameraOffset();
		this.SetMembershipTemperature();
		if (SecurityWrapperService.Instance.MMSData.License.ValidationResult == STSWMMSValidationResultType.STSW_VALIDATION_LICENSE_EXPIRED)
		{
			this.LoadDefaults();
			this.SetFromCurrentSettings();
			if (this.TryLoadSessionSettingsFromPlayerPrefs())
			{
				this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.RangeMarkersValue), this._rangeMarkersSlider);
				this.SetRangeMarkersInternal(false);
				this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.CenterLineValue), this._centerlineSlider);
				this.SetCenterlineInternal(false);
				this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.DistanseToggleValue), this._mainDistanceSlider);
				this.SetMainDistDisplayInternal(false);
				this.ChangeSliderToggleValue(this.GetSettingFromJson(GameSettings.GameSetting.BallSpinToggleValue), this._shotSpinSlider);
				this.ToggleSpinObjectsInternal(false);
				this._conditionsDropdown.@value = this._groundConditions.IndexOf(this.GetSettingFromJson(GameSettings.GameSetting.GroundConditionsValue));
				this.SetCourseConditionInternal(false);
				this._altitudeInput.text = this.GetSettingFromJson(GameSettings.GameSetting.AltidudeValue);
				this.SetAltitudeInternal(false);
			}
			this._windDirectionDropdown.interactable = false;
			this._windDirectionGroup.alpha = 0.7f;
			this._windSpeedDropdown.interactable = false;
			this._windSpeedGroup.alpha = 0.7f;
			this._humidityDropdown.interactable = false;
			this._humidityGroup.alpha = 0.7f;
			this._temperatureInput.interactable = false;
			this._temperatureGroup.alpha = 0.7f;
		}
	}

	private void SetMembershipFlagsCamera(Dropdown popUp)
	{
		List<Dropdown.OptionData> optionDatas = popUp.options;
		this.RemoveItemFromPopup(optionDatas, STSWMMSFeatureFlagType.Settings_CameraAngle_Dynamic, this._cameraAngles[1]);
		this.RemoveItemFromPopup(optionDatas, STSWMMSFeatureFlagType.Settings_CameraAngle_FirstPerson, this._cameraAngles[0]);
		this.RemoveItemFromPopup(optionDatas, STSWMMSFeatureFlagType.Settings_CameraAngle_DownRange, this._cameraAngles[2]);
		this.RemoveItemFromPopup(optionDatas, STSWMMSFeatureFlagType.Settings_CameraAngle_FollowTheBall, this._cameraAngles[3]);
		this.RemoveItemFromPopup(optionDatas, STSWMMSFeatureFlagType.Settings_CameraAngle_45Degree, this._cameraAngles[4]);
		if (popUp.@value > popUp.options.Count - 1)
		{
			int num = 0;
			while (num < optionDatas.Count)
			{
				if (!this.GetSettingFromJson(GameSettings.GameSetting.CameraValue).Equals(optionDatas[num].text))
				{
					num++;
				}
				else
				{
					popUp.@value = num;
					break;
				}
			}
		}
		if (popUp.options.Count <= popUp.@value)
		{
			popUp.@value = 0;
		}
		this.SetCameraInternal(false);
	}

	private void SetMembershipFlagsGroundConditions(Dropdown popUp)
	{
		this.RemoveItemFromPopup(popUp.options, STSWMMSFeatureFlagType.Settings_GroundConditions_Normal, this._groundConditions[0]);
		this.RemoveItemFromPopup(popUp.options, STSWMMSFeatureFlagType.Settings_GroundConditions_Soft, this._groundConditions[1]);
		this.RemoveItemFromPopup(popUp.options, STSWMMSFeatureFlagType.Settings_GroundConditions_Firm, this._groundConditions[2]);
		if (popUp.@value > popUp.options.Count - 1)
		{
			int num = 0;
			while (num < popUp.options.Count)
			{
				if (!this.GetSettingFromJson(GameSettings.GameSetting.GroundConditionsValue).Equals(popUp.options[num].text))
				{
					num++;
				}
				else
				{
					popUp.@value = num;
					break;
				}
			}
		}
		if (popUp.options.Count <= popUp.@value)
		{
			popUp.@value = 0;
		}
		this.SetCourseConditionInternal(false);
	}

	private void SetMembershipFlagsTracers(Dropdown popUp)
	{
		this.RemoveItemFromPopup(popUp.options, STSWMMSFeatureFlagType.Settings_Tracers_AllShots, this._tracers[2]);
		this.RemoveItemFromPopup(popUp.options, STSWMMSFeatureFlagType.Settings_Tracers_Last5, this._tracers[1]);
		this.RemoveItemFromPopup(popUp.options, STSWMMSFeatureFlagType.Settings_Tracers_SingleShot, this._tracers[0]);
		if (popUp.@value > popUp.options.Count - 1)
		{
			int num = 0;
			while (num < popUp.options.Count)
			{
				if (!this.GetSettingFromJson(GameSettings.GameSetting.TracesValue).Equals(popUp.options[num].text))
				{
					num++;
				}
				else
				{
					popUp.@value = num;
					break;
				}
			}
		}
		if (popUp.options.Count <= popUp.@value)
		{
			popUp.@value = 0;
		}
		this.SetTracersInternal(false);
	}

	private void SetMembershipHandness()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_GolferOrientation))
		{
			this._handnessSlider.interactable = false;
			this._handnessSlider.GetComponent<EventTrigger>().enabled = false;
			this._dominantHandGroup.alpha = 0.7f;
		}
	}

	private void SetMembershipHumidity()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_Environmental_Humidity))
		{
			this._humidityDropdown.@value = 4;
			this._humidityDropdown.interactable = false;
			this._humidityGroup.alpha = 0.7f;
			this.SetHumidityInternal(false);
		}
	}

	private void SetMembershipSideSpin()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_SideSpinSideAxis))
		{
			this._shotSpinSlider.interactable = false;
			this._shotSpinSlider.GetComponent<EventTrigger>().enabled = false;
			this._ballSpinGroup.alpha = 0.7f;
		}
	}

	private void SetMembershipTemperature()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_Temperature))
		{
			this._temperatureInput.text = "70";
			this._temperatureInput.interactable = false;
			this._temperatureGroup.alpha = 0.7f;
			this.SetTempInternal(false);
		}
	}

	private void SetMembershipWindSpeed()
	{
		if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_Environmental_Wind))
		{
			this._windSpeedDropdown.@value = 0;
			this._windSpeedDropdown.interactable = false;
			this._windSpeedGroup.alpha = 0.7f;
			this.SetWindSpeedInternal(false);
			this._windDirectionDropdown.@value = 0;
			this._windDirectionDropdown.interactable = false;
			this._windDirectionGroup.alpha = 0.7f;
			this.SetWindDirectionInternal(false);
		}
	}

	private void SetRangeMarkersInternal(bool saveSetting)
	{
		bool flag = this._rangeMarkersSlider.@value >= 1f;
		Image component = this._rangeMarkersSlider.transform.GetChild(0).GetComponent<Image>();
		component.color = (!flag ? this._redSliderColor : this._greenSliderColor);
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.RangeMarkersValue, flag.ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		if (this._pin != null)
		{
			this._pin.SetActive(flag);
		}
	}

	public void SetRangeMarkersToggle(GameObject pinObject, string header)
	{
		this._pin = pinObject;
		this._rangeMarkersHeader.text = header;
	}

	private void SetTempInternal(bool saveSetting)
	{
		float single;
		if (!float.TryParse(this._temperatureInput.text, out single))
		{
			single = 70f;
			this._temperatureInput.text = single.ToString("F0");
		}
		WeatherManager.instance.SetTemp(single);
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.TemperatureValue, this._temperatureInput.text);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
	}

	private void SetTracersInternal(bool saveSetting)
	{
		string item = this._tracersDropdown.options[this._tracersDropdown.@value].text;
		switch (this._tracersDropdown.@value)
		{
			case 0:
				{
					this.TracerAmount = TracerManager.TRACER_AMOUNT.ONE;
					break;
				}
			case 1:
				{
					this.TracerAmount = TracerManager.TRACER_AMOUNT.FIVE;
					break;
				}
			case 2:
				{
					this.TracerAmount = TracerManager.TRACER_AMOUNT.FIFTY;
					break;
				}
			default:
				{
					this.TracerAmount = TracerManager.TRACER_AMOUNT.ONE;
					break;
				}
		}
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.TracesValue, item);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		CGameManager.instance.UiHolder.TracerManager.ChangeTracerDisplay(this.TracerAmount);
	}

	private void SetWindDirectionInternal(bool saveSetting)
	{
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.WindValue, this._windDirectionDropdown.options[this._windDirectionDropdown.@value].text);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		WeatherManager.instance.SetWindDirection((WeatherManager.WindDirection)this._windDirectionDropdown.@value);
	}

	private void SetWindSpeedInternal(bool saveSetting)
	{
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.WindSpeedValue, this._windSpeedDropdown.options[this._windSpeedDropdown.@value].text);
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		WeatherManager.instance.SetWindSpeed((WeatherManager.WindSpeed)this._windSpeedDropdown.@value);
	}

	private void ToggleSpinObjectsInternal(bool saveSetting)
	{
		bool flag = this._shotSpinSlider.@value <= 0f;
		if (saveSetting || this._saveSettingsGlobal)
		{
			this.SaveSettingToJson(GameSettings.GameSetting.BallSpinToggleValue, (!flag).ToString());
			this.SaveSessionSettingsToPlayerPrefsIfEnabled(false);
		}
		if (this.OnBallSpinSettingChanged != null)
		{
			this.OnBallSpinSettingChanged(flag);
		}
	}

	private bool TryLoadSessionSettingsFromPlayerPrefs()
	{
		string str;
		bool flag;
		try
		{
			bool isUserLoggedIn = LoginManager.IsUserLoggedIn;
			str = (!isUserLoggedIn ? "NoLoggedInUser|JsonSessionSettings" : string.Concat(LoginManager.UserData.Email, "|JsonSessionSettings"));
			if (!PlayerPrefs.HasKey(str))
			{
				flag = false;
			}
			else
			{
				string str1 = PlayerPrefs.GetString(str);
				this.JsonSettings = new JSONObject(str1, -2, false, false);
				if (isUserLoggedIn)
				{
					this.SaveSettingToJson(GameSettings.GameSetting.HandToggleValue, this.bProfileLeftHandFlag.ToString());
				}
				flag = true;
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			AppLog.Log(string.Concat("Exception: in LoadSettingsFromPlayerPrefs - ", exception.Message), true);
			flag = false;
		}
		return flag;
	}

	public void UpdateHandnessValue(bool isLefty)
	{
		this.ChangeSliderToggleValue((!isLefty).ToString(), this._handnessSlider);
		this.SetHandnessInternal(false);
	}

	private enum CameraAngles
	{
		FirstPerson,
		Dynamic,
		Downrange,
		FollowBall,
		Degree45
	}

	private enum GameSetting
	{
		CameraValue,
		TracesValue,
		AlignmentValue,
		RangeMarkersValue,
		GroundConditionsValue,
		WindValue,
		WindSpeedValue,
		HumidityValue,
		AltidudeValue,
		TemperatureValue,
		CenterLineValue,
		HandToggleValue,
		BallSpinToggleValue,
		DistanseToggleValue,
		DistanseUnitValue,
		SACameraValue,
		CameraOffsetValue,
		DispersionCircles,
		DispersionDistance
	}

	private enum GroundConditions
	{
		Normal,
		Soft,
		Firm
	}

	private enum Tracers
	{
		SingleShot,
		Shots5,
		Shots50
	}
}