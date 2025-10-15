using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Building_SupportedDoor : Building_Door
{
	private bool openedBefore;

	[Unsaved(false)]
	private Graphic suppportGraphic;

	[Unsaved(false)]
	private Graphic topGraphic;

	private Graphic SupportGraphic
	{
		get
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (suppportGraphic == null)
			{
				Graphic graphic = def.building.doorSupportGraphic?.Graphic;
				if (graphic == null)
				{
					return null;
				}
				suppportGraphic = graphic.GetColoredVersion(graphic.Shader, DrawColor, Graphic.ColorTwo);
			}
			return suppportGraphic;
		}
	}

	private Graphic TopGraphic
	{
		get
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (topGraphic == null)
			{
				Graphic graphic = def.building.doorTopGraphic?.Graphic;
				if (graphic == null)
				{
					return null;
				}
				topGraphic = graphic.GetColoredVersion(graphic.Shader, DrawColor, Graphic.ColorTwo);
			}
			return topGraphic;
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		DoorPreDraw();
		Vector3 drawPos = DrawPos;
		bool flag = base.Rotation == Rot4.North || base.Rotation == Rot4.South;
		drawPos += (flag ? def.building.doorTopHorizontalOffset : def.building.doorTopVerticalOffset);
		drawPos.y = (flag ? AltitudeLayer.BuildingOnTop.AltitudeFor() : AltitudeLayer.Blueprint.AltitudeFor());
		SupportGraphic?.Draw(drawPos + def.building.doorSupportGraphicOffset, base.Rotation, this);
		drawPos.y = AltitudeLayer.Blueprint.AltitudeFor();
		TopGraphic?.Draw(drawPos + def.building.doorTopGraphicOffset, base.Rotation, this);
		base.DrawAt(drawLoc, flip);
	}

	protected override void Tick()
	{
		base.Tick();
		if (!openInt && OpenPct <= 0f && openedBefore)
		{
			openedBefore = false;
			if (!def.building.soundDoorCloseEnd.NullOrUndefined())
			{
				def.building.soundDoorCloseEnd.PlayOneShot(this);
			}
		}
	}

	protected override void DoorOpen(int ticksToClose = 110)
	{
		base.DoorOpen(ticksToClose);
		openedBefore = true;
	}

	public override void Notify_ColorChanged()
	{
		suppportGraphic = null;
		topGraphic = null;
		base.Notify_ColorChanged();
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref openedBefore, "openedBefore", defaultValue: false);
	}
}
