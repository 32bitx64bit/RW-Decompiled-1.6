using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_ReformIdeo : Window
{
	public const float HeaderHeight = 40f;

	public const float DescriptionHeight = 50f;

	private static readonly Vector2 MemeBoxSize = IdeoUIUtility.MemeBoxSize;

	private const int StyleBoxSize = 50;

	private const int MemeBoxGap = 10;

	private const int ClearChangesBtnHeight = 30;

	private const int Width = 750;

	private static readonly Color DisabledColor = Color32.op_Implicit(new Color32((byte)55, (byte)55, (byte)55, (byte)200));

	private const int MaxMemesPerRow = 5;

	private Ideo newIdeo;

	private Ideo ideo;

	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private IdeoReformStage stage;

	private List<MemeDef> tmpNormalMemes = new List<MemeDef>();

	private List<MemeDef> tmpPreSelectedMemes = new List<MemeDef>();

	public override Vector2 InitialSize
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (stage == IdeoReformStage.MemesAndStyles)
			{
				return new Vector2(750f, 700f);
			}
			return new Vector2(750f, Mathf.Min(1000f, (float)UI.screenHeight));
		}
	}

	public bool StructureMemeChanged => newIdeo.StructureMeme != ideo.StructureMeme;

	public bool NormalMemesChanged
	{
		get
		{
			int num = 0;
			for (int i = 0; i < ideo.memes.Count; i++)
			{
				if (ideo.memes[i].category == MemeCategory.Normal)
				{
					if (!newIdeo.memes.Contains(ideo.memes[i]))
					{
						return true;
					}
					num++;
				}
			}
			int num2 = 0;
			for (int j = 0; j < newIdeo.memes.Count; j++)
			{
				if (newIdeo.memes[j].category == MemeCategory.Normal)
				{
					num2++;
				}
			}
			return num != num2;
		}
	}

	public bool StylesChanged
	{
		get
		{
			if (!newIdeo.thingStyleCategories.SetsEqual(ideo.thingStyleCategories) && !StructureMemeChanged)
			{
				return !NormalMemesChanged;
			}
			return false;
		}
	}

	public bool AnyChooseOneChanges
	{
		get
		{
			if (!StructureMemeChanged && !NormalMemesChanged)
			{
				return StylesChanged;
			}
			return true;
		}
	}

	public Dialog_ReformIdeo(Ideo ideo)
	{
		if (ModLister.CheckIdeology("Reform ideo dialog"))
		{
			forcePause = true;
			doCloseX = false;
			doCloseButton = false;
			absorbInputAroundWindow = true;
			forceCatchAcceptAndCancelEventEvenIfUnfocused = true;
			this.ideo = ideo;
			newIdeo = IdeoGenerator.MakeIdeo(ideo.foundation.def);
			ideo.CopyTo(newIdeo);
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Invalid comparison between Unknown and I4
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 40f), "ReformIdeoligion".Translate());
		Text.Font = GameFont.Small;
		Widgets.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y + 40f, ((Rect)(ref inRect)).width, 50f), "ReformIdeoligionDesc".Translate());
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y + 40f + 50f, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - 55f - 40f - 50f);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, scrollViewHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float num = 0f;
		if (stage == IdeoReformStage.MemesAndStyles)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(0f, num, ((Rect)(ref viewRect)).width - 16f, Text.LineHeight);
			Widgets.Label(rect, "ReformIdeoChooseOneChange".Translate());
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect)).y + Text.LineHeight, ((Rect)(ref viewRect)).width - 16f);
			GUI.color = Color.white;
			num += 2f * Text.LineHeight;
			Widgets.Label(new Rect(((Rect)(ref viewRect)).x, num, ((Rect)(ref viewRect)).width, Text.LineHeight), "ReformIdeoChangeStructure".Translate());
			num += Text.LineHeight + 10f;
			bool flag = AnyChooseOneChanges && !StructureMemeChanged;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref viewRect)).x + ((Rect)(ref viewRect)).width / 2f - MemeBoxSize.x / 2f, num, MemeBoxSize.x, MemeBoxSize.y);
			IdeoUIUtility.DoMeme(val, newIdeo.StructureMeme, newIdeo, (!flag) ? IdeoEditMode.Reform : IdeoEditMode.None);
			if (flag)
			{
				Widgets.DrawRectFast(val, DisabledColor);
				if (Widgets.ButtonInvisible(val))
				{
					Messages.Message("MessageFluidIdeoOneChangeAllowed".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			num += MemeBoxSize.y + 17f;
			tmpNormalMemes.Clear();
			for (int i = 0; i < newIdeo.memes.Count; i++)
			{
				if (newIdeo.memes[i].category == MemeCategory.Normal)
				{
					tmpNormalMemes.Add(newIdeo.memes[i]);
				}
			}
			Widgets.Label(new Rect(((Rect)(ref viewRect)).x, num, ((Rect)(ref viewRect)).width, Text.LineHeight), (ideo.memes.Count((MemeDef m) => m.category == MemeCategory.Normal) <= 1) ? "ReformIdeoAddMeme".Translate() : "ReformIdeoAddOrRemoveMeme".Translate());
			num += Text.LineHeight + 10f;
			int num2 = Mathf.CeilToInt((float)tmpNormalMemes.Count / 5f);
			bool flag2 = AnyChooseOneChanges && !NormalMemesChanged;
			Rect val2 = default(Rect);
			for (int j = 0; j < num2; j++)
			{
				num += (float)j * MemeBoxSize.y + (float)((j > 0) ? 10 : 0);
				int num3 = j * 5;
				int num4 = Mathf.Min(5, tmpNormalMemes.Count - j * 5);
				float num5 = (float)num4 * (MemeBoxSize.x + 10f);
				float num6 = (((Rect)(ref viewRect)).width - num5) / 2f;
				for (int k = num3; k < num3 + num4; k++)
				{
					((Rect)(ref val2))._002Ector(num6, num, MemeBoxSize.x, MemeBoxSize.y);
					IdeoUIUtility.DoMeme(val2, tmpNormalMemes[k], newIdeo, (!flag2) ? IdeoEditMode.Reform : IdeoEditMode.None, drawHighlight: true, delegate
					{
						tmpPreSelectedMemes.Clear();
						tmpPreSelectedMemes.AddRange(newIdeo.memes.Where((MemeDef m) => !ideo.memes.Contains(m)));
						ideo.CopyTo(newIdeo);
						Find.WindowStack.Add(new Dialog_ChooseMemes(newIdeo, MemeCategory.Normal, initialSelection: false, null, tmpPreSelectedMemes, reformingIdeo: true));
					});
					if (flag2)
					{
						Widgets.DrawRectFast(val2, DisabledColor);
						if (Widgets.ButtonInvisible(val2))
						{
							Messages.Message("MessageFluidIdeoOneChangeAllowed".Translate(), MessageTypeDefOf.RejectInput, historical: false);
						}
					}
					num6 += MemeBoxSize.x + 10f;
				}
			}
			tmpNormalMemes.Clear();
			num += MemeBoxSize.y + 17f;
			Widgets.Label(new Rect(((Rect)(ref viewRect)).x, num, ((Rect)(ref viewRect)).width, Text.LineHeight), "ReformIdeoChangeStyles".Translate());
			num += Text.LineHeight + 10f;
			float curX = ((Rect)(ref viewRect)).x;
			Rect position = default(Rect);
			((Rect)(ref position))._002Ector(curX, num, 0f, 0f);
			bool flag3 = AnyChooseOneChanges && !StylesChanged;
			IdeoUIUtility.DoStyles(ref num, ref curX, ((Rect)(ref viewRect)).width, newIdeo, (!flag3) ? IdeoEditMode.Reform : IdeoEditMode.None, 50);
			if (flag3)
			{
				((Rect)(ref position)).width = curX - ((Rect)(ref position)).x;
				((Rect)(ref position)).height = num - ((Rect)(ref position)).y;
				Widgets.DrawRectFast(position, DisabledColor);
			}
			num += 67f;
		}
		else
		{
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(0f, num, ((Rect)(ref viewRect)).width - 16f, Text.LineHeight);
			Widgets.Label(rect2, "ReformIdeoChangeAny".Translate());
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect2)).y + Text.LineHeight, ((Rect)(ref viewRect)).width - 16f);
			GUI.color = Color.white;
			num += 2f * Text.LineHeight;
			float width = ((Rect)(ref viewRect)).width - 16f;
			IdeoUIUtility.DoNameAndSymbol(ref num, width, newIdeo, IdeoEditMode.Reform);
			num += 10f;
			IdeoUIUtility.DoDescription(ref num, width, newIdeo, IdeoEditMode.Reform);
			num += 10f;
			if (newIdeo.foundation != null)
			{
				IdeoUIUtility.DoFoundationInfo(ref num, width, newIdeo, IdeoEditMode.Reform);
				num += 10f;
			}
			IdeoUIUtility.DoPrecepts(ref num, width, newIdeo, IdeoEditMode.Reform);
			num += 10f;
			IdeoUIUtility.DoAppearanceItems(newIdeo, IdeoEditMode.Reform, ref num, width);
		}
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = num;
		}
		Widgets.EndScrollView();
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref inRect)).xMax - Window.CloseButSize.x, ((Rect)(ref inRect)).height - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y);
		if (stage == IdeoReformStage.MemesAndStyles)
		{
			Rect rect3 = val3;
			((Rect)(ref rect3)).x = ((Rect)(ref inRect)).x;
			if (Widgets.ButtonText(rect3, "Cancel".Translate()))
			{
				Close();
			}
			if (AnyChooseOneChanges)
			{
				if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x + (((Rect)(ref inRect)).width - Window.CloseButSize.x) / 2f, ((Rect)(ref inRect)).height - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y), "ReformIdeoResetChanges".Translate()))
				{
					SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					ResetAllChooseOneChanges();
				}
				num += 47f;
			}
			if (Widgets.ButtonText(val3, "Next".Translate()))
			{
				stage = IdeoReformStage.PreceptsNarrativeAndDeities;
			}
			return;
		}
		Rect rect4 = val3;
		((Rect)(ref rect4)).x = ((Rect)(ref inRect)).x;
		Rect rect5 = inRect;
		((Rect)(ref rect5)).xMin = ((Rect)(ref rect5)).xMax - ((Rect)(ref val3)).width * 3.1f;
		((Rect)(ref rect5)).width = ((Rect)(ref val3)).width * 2f;
		((Rect)(ref rect5)).yMin = ((Rect)(ref rect5)).yMax - ((Rect)(ref val3)).height;
		Pair<Precept, Precept> pair = newIdeo.FirstIncompatiblePreceptPair();
		if (pair != default(Pair<Precept, Precept>))
		{
			GUI.color = Color.red;
			Text.Font = GameFont.Tiny;
			Text.Anchor = (TextAnchor)2;
			string text = pair.First.TipLabel;
			string text2 = pair.Second.TipLabel;
			if (text == text2)
			{
				text = pair.First.UIInfoSecondLine;
				text2 = pair.Second.UIInfoSecondLine;
			}
			Widgets.Label(rect5, "MessageIdeoIncompatiblePrecepts".Translate(text.Named("PRECEPT1"), text2.Named("PRECEPT2")).CapitalizeFirst());
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
		if (Widgets.ButtonText(rect4, "Back".Translate()))
		{
			stage = IdeoReformStage.MemesAndStyles;
		}
		Rect rect6 = val3;
		((Rect)(ref rect6)).x = ((Rect)(ref inRect)).x + (((Rect)(ref inRect)).width - ((Rect)(ref rect6)).width) / 2f - ((pair != default(Pair<Precept, Precept>)) ? 65f : 0f);
		if (Widgets.ButtonText(rect6, "Randomize".Translate()))
		{
			RandomizeNewIdeo();
		}
		if (Widgets.ButtonText(val3, "DoneButton".Translate()))
		{
			IdeoDevelopmentUtility.ConfirmChangesToIdeo(ideo, newIdeo, delegate
			{
				IdeoDevelopmentUtility.ApplyChangesToIdeo(ideo, newIdeo);
				Close();
			});
		}
	}

	private void RandomizeNewIdeo()
	{
		IdeoGenerationParms parms = new IdeoGenerationParms(Faction.OfPlayer.def);
		newIdeo.foundation.RandomizeCulture(parms);
		newIdeo.foundation.RandomizePlace();
		if (newIdeo.foundation is IdeoFoundation_Deity ideoFoundation_Deity)
		{
			ideoFoundation_Deity.GenerateDeities();
		}
		newIdeo.foundation.GenerateTextSymbols();
		newIdeo.foundation.GenerateLeaderTitle();
		newIdeo.foundation.RandomizeIcon();
		newIdeo.foundation.RandomizePrecepts(init: true, parms);
		newIdeo.RegenerateDescription(force: true);
	}

	private void ResetAllChooseOneChanges()
	{
		newIdeo.memes.Clear();
		newIdeo.memes.AddRange(ideo.memes);
		newIdeo.thingStyleCategories.Clear();
		newIdeo.thingStyleCategories.AddRange(ideo.thingStyleCategories);
	}
}
