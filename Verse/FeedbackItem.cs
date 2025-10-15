using UnityEngine;

namespace Verse;

public abstract class FeedbackItem
{
	protected Vector2 FloatPerSecond = new Vector2(20f, -20f);

	private int uniqueID;

	public float TimeLeft = 2f;

	protected Vector2 CurScreenPos;

	private static int freeUniqueID;

	public FeedbackItem(Vector2 ScreenPos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		uniqueID = freeUniqueID++;
		CurScreenPos = ScreenPos;
		CurScreenPos.y -= 15f;
	}

	public void Update()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		TimeLeft -= Time.deltaTime;
		CurScreenPos += FloatPerSecond * Time.deltaTime;
	}

	public abstract void FeedbackOnGUI();

	protected void DrawFloatingText(string str, Color TextColor)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		float x = Text.CalcSize(str).x;
		Rect wordRect = new Rect(CurScreenPos.x - x / 2f, CurScreenPos.y, x, 20f);
		Find.WindowStack.ImmediateWindow(5983 * uniqueID + 495, wordRect, WindowLayer.Super, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			Rect val = wordRect.AtZero();
			Text.Anchor = (TextAnchor)1;
			Text.Font = GameFont.Small;
			GUI.DrawTexture(val, (Texture)(object)TexUI.GrayTextBG);
			GUI.color = TextColor;
			Widgets.Label(val, str);
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
		}, doBackground: false);
	}
}
