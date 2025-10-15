using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class UI_BackgroundMain : UIMenuBackground
{
	public Texture2D overrideBGImage;

	private Dictionary<ExpansionDef, float> expansionImageFades;

	private static readonly Vector2 BGPlanetSize = new Vector2(2048f, 1280f);

	private static readonly Texture2D BGPlanet = ContentFinder<Texture2D>.Get("UI/HeroArt/BGPlanet");

	private float DeltaAlpha => Time.deltaTime * 2f;

	public void SetupExpansionFadeData()
	{
		expansionImageFades = new Dictionary<ExpansionDef, float>();
		foreach (ExpansionDef allExpansion in ModLister.AllExpansions)
		{
			expansionImageFades.Add(allExpansion, 0f);
		}
	}

	public override void BackgroundOnGUI()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Invalid comparison between Unknown and I4
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = (Vector2)(((Object)(object)overrideBGImage != (Object)null) ? new Vector2((float)((Texture)overrideBGImage).width, (float)((Texture)overrideBGImage).height) : BGPlanetSize);
		bool flag = true;
		if ((float)UI.screenWidth > (float)UI.screenHeight * (val.x / val.y))
		{
			flag = false;
		}
		Rect val2 = default(Rect);
		if (flag)
		{
			float num = UI.screenHeight;
			float num2 = (float)UI.screenHeight * (val.x / val.y);
			((Rect)(ref val2))._002Ector((float)(UI.screenWidth / 2) - num2 / 2f, 0f, num2, num);
		}
		else
		{
			float num3 = UI.screenWidth;
			float num4 = (float)UI.screenWidth * (val.y / val.x);
			((Rect)(ref val2))._002Ector(0f, (float)(UI.screenHeight / 2) - num4 / 2f, num3, num4);
		}
		GUI.DrawTexture(val2, (Texture)(object)(overrideBGImage ?? BGPlanet), (ScaleMode)2);
		if ((int)Event.current.type == 7)
		{
			DoOverlay(val2);
		}
	}

	private void DoOverlay(Rect bgRect)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (expansionImageFades == null)
		{
			return;
		}
		foreach (ExpansionDef allExpansion in ModLister.AllExpansions)
		{
			if (!allExpansion.isCore && !allExpansion.BackgroundImage.NullOrBad() && !(expansionImageFades[allExpansion] <= 0f))
			{
				if ((Object)(object)allExpansion.BackgroundImage != (Object)(object)overrideBGImage)
				{
					GUI.color = new Color(1f, 1f, 1f, expansionImageFades[allExpansion]);
					GUI.DrawTexture(bgRect, (Texture)(object)allExpansion.BackgroundImage, (ScaleMode)1);
					GUI.color = Color.white;
				}
				expansionImageFades[allExpansion] = Mathf.Clamp01(expansionImageFades[allExpansion] - DeltaAlpha / 2f);
			}
		}
	}

	public void Notify_Hovered(ExpansionDef expansionDef)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 7)
		{
			expansionImageFades[expansionDef] = Mathf.Clamp01(expansionImageFades[expansionDef] + DeltaAlpha);
		}
	}
}
