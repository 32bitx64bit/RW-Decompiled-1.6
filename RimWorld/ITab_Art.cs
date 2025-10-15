using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Art : ITab
{
	private static string cachedImageDescription;

	private static CompArt cachedImageSource;

	private static TaleReference cachedTaleRef;

	private static readonly Vector2 WinSize = new Vector2(400f, 300f);

	private CompArt SelectedCompArt
	{
		get
		{
			Thing thing = Find.Selector.SingleSelectedThing;
			if (thing is MinifiedThing minifiedThing)
			{
				thing = minifiedThing.InnerThing;
			}
			return thing?.TryGetComp<CompArt>();
		}
	}

	public override bool IsVisible
	{
		get
		{
			if (SelectedCompArt != null)
			{
				return SelectedCompArt.Active;
			}
			return false;
		}
	}

	public ITab_Art()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabArt";
		tutorTag = "Art";
	}

	protected override void FillTab()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Rect rect;
		Rect val = (rect = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f));
		Text.Font = GameFont.Medium;
		Widgets.Label(rect, SelectedCompArt.Title.Truncate(((Rect)(ref rect)).width));
		if (cachedImageSource != SelectedCompArt || cachedTaleRef != SelectedCompArt.TaleRef)
		{
			cachedImageDescription = SelectedCompArt.GenerateImageDescription();
			cachedImageSource = SelectedCompArt;
			cachedTaleRef = SelectedCompArt.TaleRef;
		}
		Rect rect2 = val;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin + 35f;
		Text.Font = GameFont.Small;
		Widgets.Label(rect2, cachedImageDescription);
	}
}
