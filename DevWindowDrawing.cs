using LudeonTK;
using UnityEngine;
using Verse;

public class DevWindowDrawing : IWindowDrawing
{
	public GUIStyle EmptyStyle => DevGUI.EmptyStyle;

	public bool DoCloseButton(Rect rect, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return DevGUI.ButtonText(rect, text);
	}

	public bool DoClostButtonSmall(Rect rect)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return DevGUI.ButtonImage(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 22f - 4f, ((Rect)(ref rect)).y + 4f, 22f, 22f), DevGUI.Close);
	}

	public void DoGrayOut(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		DevGUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
	}

	public void DoWindowBackground(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = DevGUI.WindowBGFillColor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = Color.white;
	}

	public void BeginGroup(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		DevGUI.BeginGroup(rect);
	}

	public void EndGroup()
	{
		DevGUI.EndGroup();
	}
}
