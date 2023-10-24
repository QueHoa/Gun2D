using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UltimateJoystick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerUpHandler
{
	public enum ScalingAxis
	{
		Width = 0,
		Height = 1
	}

	public enum Anchor
	{
		Left = 0,
		Right = 1
	}

	public enum JoystickTouchSize
	{
		Default = 0,
		Medium = 1,
		Large = 2,
		Custom = 3
	}

	public enum Axis
	{
		Both = 0,
		X = 1,
		Y = 2
	}

	public enum Boundary
	{
		Circular = 0,
		Square = 1
	}

	public enum TapCountOption
	{
		NoCount = 0,
		Accumulate = 1,
		TouchRelease = 2
	}

	public RectTransform joystick;

	public RectTransform joystickSizeFolder;

	public RectTransform joystickBase;

	private RectTransform baseTrans;

	private Vector2 textureCenter;

	private Vector2 defaultPos;

	private Vector3 joystickCenter;

	public Image highlightBase;

	public Image highlightJoystick;

	public Image tensionAccentUp;

	public Image tensionAccentDown;

	public Image tensionAccentLeft;

	public Image tensionAccentRight;

	public ScalingAxis scalingAxis;

	public Anchor anchor;

	public JoystickTouchSize joystickTouchSize;

	public float joystickSize;

	public float radiusModifier;

	private float radius;

	public bool dynamicPositioning;

	public float customTouchSize_X;

	public float customTouchSize_Y;

	public float customTouchSizePos_X;

	public float customTouchSizePos_Y;

	public float customSpacing_X;

	public float customSpacing_Y;

	public float gravity;

	private bool gravityActive;

	public bool extendRadius;

	public Axis axis;

	public Boundary boundary;

	public TapCountOption tapCountOption;

	public float tapCountDuration;

	public int targetTapCount;

	private float currentTapTime;

	private int tapCount;

	public float deadZone;

	public bool disableVisuals;

	public bool useFade;

	private CanvasGroup joystickGroup;

	public float fadeUntouched;

	public float fadeTouched;

	public float fadeInDuration;

	public float fadeOutDuration;

	private float fadeInSpeed;

	private float fadeOutSpeed;

	public bool useAnimation;

	public Animator joystickAnimator;

	private int animationID;

	public bool showHighlight;

	public Color highlightColor;

	public bool showTension;

	public Color tensionColorNone;

	public Color tensionColorFull;

	private static Dictionary<string, UltimateJoystick> UltimateJoysticks;

	public string joystickName;

	private bool joystickState;

	private bool tapCountAchieved;

	private bool updateHighlightPosition;

	private int _pointerId;

	public float HorizontalAxis
	{
		[CompilerGenerated]
		get
		{
			return 0f;
		}
		[CompilerGenerated]
		private set
		{
		}
	}

	public float VerticalAxis
	{
		[CompilerGenerated]
		get
		{
			return 0f;
		}
		[CompilerGenerated]
		private set
		{
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void OnPointerDown(PointerEventData touchInfo)
	{
	}

	public void OnDrag(PointerEventData touchInfo)
	{
	}

	public void OnPointerUp(PointerEventData touchInfo)
	{
	}

	private void UpdateJoystick(PointerEventData touchInfo)
	{
	}

	private Vector2 ConfigureImagePosition(Vector2 textureSize, Vector2 customSpacing)
	{
		return default(Vector2);
	}

	private void TensionAccentDisplay()
	{
	}

	private void TensionAccentReset()
	{
	}

	private IEnumerator GravityHandler()
	{
		return null;
	}

	private Canvas GetParentCanvas()
	{
		return null;
	}

	private CanvasGroup GetCanvasGroup()
	{
		return null;
	}

	private IEnumerator FadeLogic()
	{
		return null;
	}

	private IEnumerator TapCountdown()
	{
		return null;
	}

	private IEnumerator TapCountDelay()
	{
		return null;
	}

	private void CheckJoystickHighlightForUse()
	{
	}

	private void UpdatePositionValues()
	{
	}

	private static bool JoystickConfirmed(string joystickName)
	{
		return false;
	}

	private void ResetJoystick()
	{
	}

	private void UpdateSizeAndPlacement()
	{
	}

	public void UpdatePositioning()
	{
	}

	public float GetHorizontalAxis()
	{
		return 0f;
	}

	public float GetVerticalAxis()
	{
		return 0f;
	}

	public float GetHorizontalAxisRaw()
	{
		return 0f;
	}

	public float GetVerticalAxisRaw()
	{
		return 0f;
	}

	public float GetDistance()
	{
		return 0f;
	}

	public void UpdateHighlightColor(Color targetColor)
	{
	}

	public void UpdateTensionColors(Color targetTensionNone, Color targetTensionFull)
	{
	}

	public bool GetJoystickState()
	{
		return false;
	}

	public bool GetTapCount()
	{
		return false;
	}

	public void DisableJoystick()
	{
	}

	public void EnableJoystick()
	{
	}

	public static UltimateJoystick GetUltimateJoystick(string joystickName)
	{
		return null;
	}

	public static float GetHorizontalAxis(string joystickName)
	{
		return 0f;
	}

	public static float GetVerticalAxis(string joystickName)
	{
		return 0f;
	}

	public static float GetHorizontalAxisRaw(string joystickName)
	{
		return 0f;
	}

	public static float GetVerticalAxisRaw(string joystickName)
	{
		return 0f;
	}

	public static float GetDistance(string joystickName)
	{
		return 0f;
	}

	public static bool GetJoystickState(string joystickName)
	{
		return false;
	}

	public static bool GetTapCount(string joystickName)
	{
		return false;
	}

	public static void DisableJoystick(string joystickName)
	{
	}

	public static void EnableJoystick(string joystickName)
	{
	}
}
