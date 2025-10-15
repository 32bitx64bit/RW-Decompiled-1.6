using UnityEngine;

namespace Verse;

public static class UI
{
	public static int screenWidth;

	public static int screenHeight;

	public static Vector2 MousePositionOnUI => Vector2.op_Implicit(Input.mousePosition / Prefs.UIScale);

	public static Vector2 MousePositionOnUIInverted
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 mousePositionOnUI = MousePositionOnUI;
			mousePositionOnUI.y = (float)screenHeight - mousePositionOnUI.y;
			return mousePositionOnUI;
		}
	}

	public static Vector2 MousePosUIInvertedUseEventIfCan
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (Event.current != null)
			{
				return GUIToScreenPoint(Event.current.mousePosition);
			}
			return MousePositionOnUIInverted;
		}
	}

	public static void ApplyUIScale()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (Prefs.UIScale == 1f)
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			return;
		}
		screenWidth = Mathf.RoundToInt((float)Screen.width / Prefs.UIScale);
		screenHeight = Mathf.RoundToInt((float)Screen.height / Prefs.UIScale);
		float uIScale = Prefs.UIScale;
		float uIScale2 = Prefs.UIScale;
		GUI.matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, new Vector3(uIScale, uIScale2, 1f));
	}

	public static void FocusControl(string controlName, Window window)
	{
		GUI.FocusControl(controlName);
		Find.WindowStack.Notify_ManuallySetFocus(window);
	}

	public static void UnfocusCurrentControl()
	{
		GUI.FocusControl((string)null);
	}

	public static void UnfocusCurrentTextField()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		GUI.SetNextControlName("FOR_UNFOCUS");
		GUI.TextField(default(Rect), "");
		GUI.FocusControl("FOR_UNFOCUS");
	}

	public static Vector2 GUIToScreenPoint(Vector2 guiPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GUIUtility.GUIToScreenPoint(guiPoint / Prefs.UIScale);
	}

	public static Rect GUIToScreenRect(Rect guiRect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Rect result = default(Rect);
		((Rect)(ref result)).min = GUIToScreenPoint(((Rect)(ref guiRect)).min);
		((Rect)(ref result)).max = GUIToScreenPoint(((Rect)(ref guiRect)).max);
		return result;
	}

	public static void RotateAroundPivot(float angle, Vector2 center)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GUIUtility.RotateAroundPivot(angle, center * Prefs.UIScale);
	}

	public static Vector2 MapToUIPosition(this Vector3 v)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Find.Camera.WorldToScreenPoint(v) / Prefs.UIScale;
		return new Vector2(val.x, (float)screenHeight - val.y);
	}

	public static Vector3 UIToMapPosition(float x, float y)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return UIToMapPosition(new Vector2(x, y));
	}

	public static Vector3 UIToMapPosition(Vector2 screenLoc)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Ray val = Find.Camera.ScreenPointToRay(Vector2.op_Implicit(screenLoc * Prefs.UIScale));
		return new Vector3(((Ray)(ref val)).origin.x, 0f, ((Ray)(ref val)).origin.z);
	}

	public static float CurUICellSize()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return (MapToUIPosition(new Vector3(1f, 0f, 0f)) - MapToUIPosition(new Vector3(0f, 0f, 0f))).x;
	}

	public static Vector3 MouseMapPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return UIToMapPosition(MousePositionOnUI);
	}

	public static IntVec3 MouseCell()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return UIToMapPosition(MousePositionOnUI).ToIntVec3();
	}
}
