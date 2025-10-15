using UnityEngine;

namespace Verse;

public class CompColorable : ThingComp
{
	private Color? desiredColor;

	private Color color = Color.white;

	private bool active;

	public Color? DesiredColor
	{
		get
		{
			return desiredColor;
		}
		set
		{
			desiredColor = value;
		}
	}

	public Color Color
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!active)
			{
				return parent.def.graphicData.color;
			}
			return color;
		}
	}

	public bool Active => active;

	public override void Initialize(CompProperties props)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize(props);
		if (parent.def.colorGenerator != null && (parent.Stuff == null || parent.Stuff.stuffProps.allowColorGenerators))
		{
			SetColor(parent.def.colorGenerator.NewRandomizedColor());
		}
	}

	public override void PostExposeData()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		base.PostExposeData();
		Scribe_Values.Look(ref desiredColor, "desiredColor");
		if (Scribe.mode != LoadSaveMode.Saving || active)
		{
			Scribe_Values.Look(ref color, "color");
			Scribe_Values.Look(ref active, "colorActive", defaultValue: false);
		}
	}

	public override void PostSplitOff(Thing piece)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.PostSplitOff(piece);
		if (active)
		{
			piece.SetColor(color);
		}
	}

	public void Recolor()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!desiredColor.HasValue)
		{
			Log.Error($"Tried recoloring apparel {parent} which does not have a desired color set!");
		}
		else
		{
			SetColor(DesiredColor.Value);
		}
	}

	public void Disable()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		active = false;
		color = Color.white;
		desiredColor = null;
		parent.Notify_ColorChanged();
	}

	public void SetColor(Color value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!(value == color))
		{
			active = true;
			color = value;
			desiredColor = null;
			parent.Notify_ColorChanged();
		}
	}
}
