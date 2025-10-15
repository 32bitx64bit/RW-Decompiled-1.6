using UnityEngine;
using Verse;

namespace RimWorld;

public static class RenameUIUtility
{
	public static void DrawRenameButton(Rect rect, Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		TooltipHandler.TipRegionByKey(rect, pawn.RaceProps.IsMechanoid ? "RenameMech" : "RenameAnimal");
		if (Widgets.ButtonImage(rect, TexButton.Rename))
		{
			Find.WindowStack.Add(pawn.NamePawnDialog());
		}
	}

	public static void DrawRenameButton(Rect rect, IRenameable renamable)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		TooltipHandler.TipRegionByKey(rect, "Rename");
		if (Widgets.ButtonImage(rect, TexButton.Rename))
		{
			Find.WindowStack.Add(new Dialog_RenameBuildingStorage(renamable));
		}
	}

	public static void DrawRenameButton(Rect rect, IStorageGroupMember building)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		TooltipHandler.TipRegionByKey(rect, "Rename");
		if (Widgets.ButtonImage(rect, TexButton.Rename))
		{
			Find.WindowStack.Add(new Dialog_RenameBuildingStorage_CreateNew(building));
		}
	}

	public static void DrawRenameButton(Rect rect, CompAnimalPenMarker marker)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		TooltipHandler.TipRegionByKey(rect, "Rename");
		if (Widgets.ButtonImage(rect, TexButton.Rename))
		{
			Find.WindowStack.Add(new Dialog_RenameAnimalPen(marker.parent.Map, marker));
		}
	}

	public static void DrawRenameButton(Rect rect, Building_GravEngine engine)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		TooltipHandler.TipRegionByKey(rect, "Rename");
		if (Widgets.ButtonImage(rect, TexButton.Rename))
		{
			Find.WindowStack.Add(new Dialog_RenameGravship(engine));
		}
	}
}
