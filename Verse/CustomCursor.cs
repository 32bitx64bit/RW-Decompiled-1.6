using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class CustomCursor
{
	private static readonly Texture2D CursorTex = ContentFinder<Texture2D>.Get("UI/Cursors/CursorCustom");

	private static Vector2 CursorHotspot = new Vector2(3f, 3f);

	public static void Activate()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Cursor.SetCursor(CursorTex, CursorHotspot, (CursorMode)0);
	}

	public static void Deactivate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Cursor.SetCursor((Texture2D)null, Vector2.zero, (CursorMode)0);
	}
}
