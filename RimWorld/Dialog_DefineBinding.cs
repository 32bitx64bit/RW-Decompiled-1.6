using UnityEngine;
using Verse;
using Verse.Steam;

namespace RimWorld;

public class Dialog_DefineBinding : Window
{
	protected Vector2 windowSize = new Vector2(400f, 200f);

	protected KeyPrefsData keyPrefsData;

	protected KeyBindingDef keyDef;

	protected KeyPrefs.BindingSlot slot;

	public override Vector2 InitialSize => windowSize;

	protected override float Margin => 0f;

	public Dialog_DefineBinding(KeyPrefsData keyPrefsData, KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.keyDef = keyDef;
		this.slot = slot;
		this.keyPrefsData = keyPrefsData;
		closeOnAccept = false;
		closeOnCancel = false;
		forcePause = true;
		onlyOneOfTypeAllowed = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I4
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = (TextAnchor)4;
		if (SteamDeck.IsSteamDeckInNonKeyboardMode)
		{
			Widgets.Label(inRect, "PressAnyKeyOrEscController".Translate().Resolve().AdjustedForKeys());
		}
		else
		{
			Widgets.Label(inRect, "PressAnyKeyOrEsc".Translate());
		}
		Text.Anchor = (TextAnchor)0;
		if (!Event.current.isKey || (int)Event.current.type != 4 || (int)Event.current.keyCode == 0)
		{
			return;
		}
		if ((int)Event.current.keyCode != 27)
		{
			keyPrefsData.EraseConflictingBindingsForKeyCode(keyDef, Event.current.keyCode, delegate(KeyBindingDef oldDef)
			{
				Messages.Message("KeyBindingOverwritten".Translate(oldDef.LabelCap), MessageTypeDefOf.TaskCompletion, historical: false);
			});
			keyPrefsData.SetBinding(keyDef, slot, Event.current.keyCode);
		}
		Close();
		Event.current.Use();
	}
}
