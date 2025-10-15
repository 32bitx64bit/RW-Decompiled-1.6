using Verse;

namespace RimWorld;

public class Instruction_SetGrowingZonePlant : Lesson_Instruction
{
	private Zone_Growing GrowZone => (Zone_Growing)base.Map.zoneManager.AllZones.FirstOrDefault((Zone z) => z is Zone_Growing);

	public override void LessonOnGUI()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		TutorUtility.DrawLabelOnGUI(Gen.AveragePosition(GrowZone.cells), def.onMapInstruction);
		base.LessonOnGUI();
	}

	public override void LessonUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		GenDraw.DrawArrowPointingAt(Gen.AveragePosition(GrowZone.cells));
	}
}
