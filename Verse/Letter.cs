using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public abstract class Letter : IArchivable, IExposable, ILoadReferenceable
{
	public int ID;

	public LetterDef def;

	private TaggedString label;

	public LookTargets lookTargets;

	public Faction relatedFaction;

	public int arrivalTick;

	public float arrivalTime;

	public string debugInfo;

	public const float DrawWidth = 38f;

	public const float DrawHeight = 30f;

	private const float FallTime = 1f;

	private const float FallDistance = 200f;

	public virtual bool CanShowInLetterStack => true;

	public virtual bool CanDismissWithRightClick => true;

	public bool ArchivedOnly => !Find.LetterStack.LettersListForReading.Contains(this);

	public IThingHolder ParentHolder => Find.World;

	Texture IArchivable.ArchivedIcon => (Texture)(object)def.Icon;

	Color IArchivable.ArchivedIconColor => def.color;

	string IArchivable.ArchivedLabel => label;

	string IArchivable.ArchivedTooltip => GetMouseoverText();

	int IArchivable.CreatedTicksGame => arrivalTick;

	bool IArchivable.CanCullArchivedNow => !Find.LetterStack.LettersListForReading.Contains(this);

	LookTargets IArchivable.LookTargets => lookTargets;

	public virtual bool ShouldAutomaticallyOpenLetter => false;

	public TaggedString Label
	{
		get
		{
			return label;
		}
		set
		{
			label = value.CapitalizeFirst();
		}
	}

	public virtual void ExposeData()
	{
		Scribe_Values.Look(ref ID, "ID", 0);
		Scribe_Defs.Look(ref def, "def");
		Scribe_Values.Look(ref label, "label");
		Scribe_Deep.Look(ref lookTargets, "lookTargets");
		Scribe_References.Look(ref relatedFaction, "relatedFaction");
		Scribe_Values.Look(ref arrivalTick, "arrivalTick", 0);
		if (Scribe.mode == LoadSaveMode.PostLoadInit && relatedFaction != null && !Find.FactionManager.AllFactionsListForReading.Contains(relatedFaction))
		{
			relatedFaction = null;
		}
	}

	public virtual void DrawButtonAt(float topY)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Invalid comparison between Unknown and I4
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)UI.screenWidth - 38f - 12f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(num, topY, 38f, 30f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(val);
		float num2 = Time.time - arrivalTime;
		Color color = def.color;
		if (num2 < 1f)
		{
			((Rect)(ref val2)).y = ((Rect)(ref val2)).y - (1f - num2) * 200f;
			color.a = num2 / 1f;
		}
		if (!Mouse.IsOver(val) && def.bounce && num2 > 15f && num2 % 5f < 1f)
		{
			float num3 = (float)UI.screenWidth * 0.06f;
			float num4 = 2f * (num2 % 1f) - 1f;
			float num5 = num3 * (1f - num4 * num4);
			((Rect)(ref val2)).x = ((Rect)(ref val2)).x - num5;
		}
		if ((int)Event.current.type == 7)
		{
			if (def.flashInterval > 0f)
			{
				float num6 = Time.time - (arrivalTime + 1f);
				if (num6 > 0f && num6 % def.flashInterval < 1f)
				{
					GenUI.DrawFlash(num, topY, (float)UI.screenWidth * 0.6f, Pulser.PulseBrightness(1f, 1f, num6) * 0.55f, def.flashColor);
				}
			}
			GUI.color = color;
			Widgets.DrawShadowAround(val2);
			GUI.DrawTexture(val2, (Texture)(object)def.Icon);
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)2;
			string text = PostProcessedLabel();
			Vector2 val3 = Text.CalcSize(text);
			float x = val3.x;
			float y = val3.y;
			Vector2 val4 = default(Vector2);
			((Vector2)(ref val4))._002Ector(((Rect)(ref val2)).x + ((Rect)(ref val2)).width / 2f, ((Rect)(ref val2)).center.y - y / 2f + 4f);
			float num7 = val4.x + x / 2f - (float)(UI.screenWidth - 2);
			if (num7 > 0f)
			{
				val4.x -= num7;
			}
			GUI.DrawTexture(new Rect(val4.x - x / 2f - 6f - 1f, val4.y, x + 12f, 16f), (Texture)(object)TexUI.GrayTextBG);
			GUI.color = new Color(1f, 1f, 1f, 0.75f);
			Rect rect = new Rect(val4.x - x / 2f, val4.y - 3f, x, 999f);
			Text.WordWrap = false;
			Widgets.Label(rect, text);
			Text.WordWrap = true;
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
		}
		if (CanDismissWithRightClick && (int)Event.current.type == 0 && Event.current.button == 1 && Mouse.IsOver(val))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			Find.LetterStack.RemoveLetter(this);
			Event.current.Use();
		}
		if (Widgets.ButtonInvisible(val2))
		{
			OpenLetter();
			Event.current.Use();
		}
	}

	public virtual void CheckForMouseOverTextAt(float topY)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)UI.screenWidth - 38f - 12f;
		if (!Mouse.IsOver(new Rect(num, topY, 38f, 30f)))
		{
			return;
		}
		Find.LetterStack.Notify_LetterMouseover(this);
		TaggedString mouseoverText = GetMouseoverText();
		if (!mouseoverText.Resolve().NullOrEmpty())
		{
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)0;
			float num2 = Text.CalcHeight(mouseoverText, 310f);
			num2 += 20f;
			float num3 = num - 330f - 10f;
			float num4 = Mathf.Max(topY - num2 / 2f, 0f);
			Rect infoRect = new Rect(num3, num4, 330f, num2);
			Find.WindowStack.ImmediateWindow(2768333, infoRect, WindowLayer.Super, delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				Text.Font = GameFont.Small;
				Rect rect = infoRect.AtZero().ContractedBy(10f);
				Widgets.BeginGroup(rect);
				Widgets.Label(new Rect(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height), mouseoverText.Resolve());
				Widgets.EndGroup();
			});
		}
	}

	protected abstract string GetMouseoverText();

	public abstract void OpenLetter();

	public virtual void Received()
	{
	}

	public virtual void Removed()
	{
	}

	protected virtual string PostProcessedLabel()
	{
		return label;
	}

	void IArchivable.OpenArchived()
	{
		OpenLetter();
	}

	public string GetUniqueLoadID()
	{
		return "Letter_" + ID;
	}
}
