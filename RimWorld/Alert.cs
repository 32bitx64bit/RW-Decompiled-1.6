using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public abstract class Alert
{
	protected AlertPriority defaultPriority;

	protected string defaultLabel;

	protected string defaultExplanation;

	protected bool requireRoyalty;

	protected bool requireIdeology;

	protected bool requireBiotech;

	protected bool requireAnomaly;

	protected float lastBellTime = -1000f;

	private int jumpToTargetCycleIndex = -1;

	private bool cachedActive;

	private string cachedLabel;

	private AlertBounce alertBounce;

	public const float Width = 154f;

	private const float TextWidth = 148f;

	public const float InfoRectWidth = 330f;

	private static readonly Texture2D AlertBGTex = SolidColorMaterials.NewSolidColorTexture(Color.white);

	private static readonly Texture2D AlertBGTexHighlight = TexUI.HighlightTex;

	private static List<GlobalTargetInfo> tmpTargets = new List<GlobalTargetInfo>();

	public virtual AlertPriority Priority => defaultPriority;

	protected virtual Color BGColor => Color.clear;

	public bool Active => cachedActive;

	public string Label
	{
		get
		{
			if (!Active)
			{
				return string.Empty;
			}
			return cachedLabel;
		}
	}

	public float Height
	{
		get
		{
			Text.Font = GameFont.Small;
			return Text.CalcHeight(Label, 148f);
		}
	}

	public bool EnabledWithActiveExpansions
	{
		get
		{
			if (requireRoyalty && !ModsConfig.RoyaltyActive)
			{
				return false;
			}
			if (requireIdeology && !ModsConfig.IdeologyActive)
			{
				return false;
			}
			if (requireBiotech && !ModsConfig.BiotechActive)
			{
				return false;
			}
			if (requireAnomaly && !ModsConfig.AnomalyActive)
			{
				return false;
			}
			return true;
		}
	}

	public virtual string GetJumpToTargetsText => "ClickToJumpToProblem".Translate();

	public abstract AlertReport GetReport();

	public virtual TaggedString GetExplanation()
	{
		return defaultExplanation;
	}

	public virtual string GetLabel()
	{
		return defaultLabel;
	}

	public void Notify_Started()
	{
		if ((int)Priority >= 1)
		{
			if (alertBounce == null)
			{
				alertBounce = new AlertBounce();
			}
			alertBounce.DoAlertStartEffect();
			if (Time.timeSinceLevelLoad > 1f && Time.realtimeSinceStartup > lastBellTime + 0.5f)
			{
				SoundDefOf.TinyBell.PlayOneShotOnCamera();
				lastBellTime = Time.realtimeSinceStartup;
			}
		}
	}

	public void Recalculate()
	{
		AlertReport report = GetReport();
		cachedActive = report.active;
		if (report.active)
		{
			cachedLabel = GetLabel();
		}
	}

	public virtual void AlertActiveUpdate()
	{
	}

	public virtual Rect DrawAt(float topY, bool minimized)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector((float)UI.screenWidth - 154f, topY, 154f, Height);
		if (alertBounce != null)
		{
			((Rect)(ref val)).x = ((Rect)(ref val)).x - alertBounce.CalculateHorizontalOffset();
		}
		GUI.color = BGColor;
		GUI.DrawTexture(val, (Texture)(object)AlertBGTex);
		GUI.color = Color.white;
		Widgets.BeginGroup(val);
		Text.Anchor = (TextAnchor)5;
		Widgets.Label(new Rect(0f, 0f, 148f, Height), Label);
		Widgets.EndGroup();
		if (Mouse.IsOver(val))
		{
			GUI.DrawTexture(val, (Texture)(object)AlertBGTexHighlight);
		}
		if (Widgets.ButtonInvisible(val))
		{
			OnClick();
		}
		Text.Anchor = (TextAnchor)0;
		return val;
	}

	protected virtual void OnClick()
	{
		IEnumerable<GlobalTargetInfo> allCulprits = GetReport().AllCulprits;
		if (allCulprits == null)
		{
			return;
		}
		tmpTargets.Clear();
		foreach (GlobalTargetInfo item in allCulprits)
		{
			if (item.IsValid)
			{
				tmpTargets.Add(item);
			}
		}
		if (tmpTargets.Any())
		{
			if (Event.current.button == 1)
			{
				jumpToTargetCycleIndex--;
			}
			else
			{
				jumpToTargetCycleIndex++;
			}
			CameraJumper.TryJumpAndSelect(tmpTargets[GenMath.PositiveMod(jumpToTargetCycleIndex, tmpTargets.Count)]);
			tmpTargets.Clear();
		}
	}

	public void DrawInfoPane()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 7)
		{
			return;
		}
		Recalculate();
		if (!Active)
		{
			return;
		}
		TaggedString expString = GetExplanation();
		if (!expString.NullOrEmpty())
		{
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)0;
			if (GetReport().AnyCulpritValid)
			{
				expString += "\n\n(" + GetJumpToTargetsText + ")";
			}
			float num = Text.CalcHeight(expString, 310f);
			num += 20f;
			Rect infoRect = new Rect((float)UI.screenWidth - 154f - 330f - 8f, Mathf.Max(Mathf.Min(Event.current.mousePosition.y, (float)UI.screenHeight - num), 0f), 330f, num);
			if (((Rect)(ref infoRect)).yMax > (float)UI.screenHeight)
			{
				ref Rect reference = ref infoRect;
				((Rect)(ref reference)).y = ((Rect)(ref reference)).y - ((float)UI.screenHeight - ((Rect)(ref infoRect)).yMax);
			}
			if (((Rect)(ref infoRect)).y < 0f)
			{
				((Rect)(ref infoRect)).y = 0f;
			}
			Find.WindowStack.ImmediateWindow(138956, infoRect, WindowLayer.Super, delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				Text.Font = GameFont.Small;
				Rect rect = infoRect.AtZero();
				Widgets.DrawWindowBackground(rect);
				Rect rect2 = rect.ContractedBy(10f);
				Widgets.BeginGroup(rect2);
				Widgets.Label(new Rect(0f, 0f, ((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height), expString);
				Widgets.EndGroup();
			}, doBackground: false);
		}
	}
}
