using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_ModSettings : Window
{
	private Mod mod;

	private const float TopAreaHeight = 40f;

	private const float TopButtonHeight = 35f;

	private const float TopButtonWidth = 150f;

	public override Vector2 InitialSize => new Vector2(900f, 700f);

	public Dialog_ModSettings(Mod mod)
	{
		forcePause = true;
		doCloseX = true;
		doCloseButton = true;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
		this.mod = mod;
	}

	public override void PreClose()
	{
		base.PreClose();
		mod.WriteSettings();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref inRect)).width - 150f - 17f, 35f), mod.SettingsCategory());
		Text.Font = GameFont.Small;
		Rect inRect2 = default(Rect);
		((Rect)(ref inRect2))._002Ector(0f, 40f, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - 40f - Window.CloseButSize.y);
		mod.DoSettingsWindowContents(inRect2);
	}
}
