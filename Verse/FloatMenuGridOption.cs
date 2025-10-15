using System;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class FloatMenuGridOption
{
	public Texture2D texture;

	public Color color = Color.white;

	public Action action;

	public TipSignal? tooltip;

	public Rect iconTexCoords = new Rect(0f, 0f, 1f, 1f);

	public Action<Rect> postDrawAction;

	public bool Disabled => action == null;

	public MenuOptionPriority Priority
	{
		get
		{
			if (Disabled)
			{
				return MenuOptionPriority.DisabledOption;
			}
			return MenuOptionPriority.Default;
		}
	}

	public FloatMenuGridOption(Texture2D texture, Action action, Color? color = null, TipSignal? tooltip = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		this.texture = texture;
		this.action = action;
		this.color = (Color)(((_003F?)color) ?? Color.white);
		this.tooltip = tooltip;
	}

	public bool OnGUI(Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !Disabled && Mouse.IsOver(rect);
		if (!Disabled)
		{
			MouseoverSounds.DoRegion(rect);
		}
		if (tooltip.HasValue)
		{
			TooltipHandler.TipRegion(rect, tooltip.Value);
		}
		Color val = GUI.color;
		if (Disabled)
		{
			GUI.color = FloatMenuOption.ColorBGDisabled * val;
		}
		else if (flag)
		{
			GUI.color = FloatMenuOption.ColorBGActiveMouseover * val;
		}
		else
		{
			GUI.color = FloatMenuOption.ColorBGActive * val;
		}
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = ((!Disabled) ? FloatMenuOption.ColorTextActive : FloatMenuOption.ColorTextDisabled) * val;
		Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);
		GUI.color = new Color(color.r, color.g, color.b, color.a * val.a);
		Rect val2 = rect.ContractedBy(2f);
		if (!flag)
		{
			val2 = val2.ContractedBy(2f);
		}
		Material mat = (Disabled ? TexUI.GrayscaleGUI : null);
		Widgets.DrawTextureFitted(val2, (Texture)(object)texture, 1f, new Vector2(1f, 1f), iconTexCoords, 0f, mat);
		GUI.color = val;
		postDrawAction?.Invoke(val2);
		if (Widgets.ButtonInvisible(rect))
		{
			Chosen();
			return true;
		}
		return false;
	}

	public void Chosen()
	{
		if (!Disabled)
		{
			action?.Invoke();
		}
		else
		{
			SoundDefOf.ClickReject.PlayOneShotOnCamera();
		}
	}
}
