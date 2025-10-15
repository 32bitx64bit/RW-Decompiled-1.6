using UnityEngine;
using Verse;

namespace RimWorld;

public class Area_Allowed : Area
{
	private string labelInt;

	private Color colorInt = Color.red;

	public override string Label => labelInt;

	public override Color Color => colorInt;

	public override bool Mutable => true;

	public override int ListPriority => 500;

	public override string RenamableLabel
	{
		get
		{
			return labelInt ?? BaseLabel;
		}
		set
		{
			labelInt = value;
		}
	}

	public override string BaseLabel => "Area";

	public Area_Allowed()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public Area_Allowed(AreaManager areaManager, string label = null)
		: base(areaManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		base.areaManager = areaManager;
		if (!label.NullOrEmpty())
		{
			labelInt = label;
		}
		else
		{
			int num = 1;
			while (true)
			{
				labelInt = "AreaDefaultLabel".Translate(num);
				if (areaManager.GetLabeled(labelInt) == null)
				{
					break;
				}
				num++;
			}
		}
		colorInt = new Color(Rand.Value, Rand.Value, Rand.Value);
		colorInt = Color.Lerp(colorInt, Color.gray, 0.5f);
	}

	public override void ExposeData()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.ExposeData();
		Scribe_Values.Look(ref labelInt, "label");
		Scribe_Values.Look(ref colorInt, "color");
	}

	public override bool AssignableAsAllowed()
	{
		return true;
	}

	public void SetColor(Color newColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		colorInt = newColor;
		InvalidateColorTexture();
	}

	public void SetLabel(string newLabel)
	{
		labelInt = newLabel;
	}

	public override string GetUniqueLoadID()
	{
		return "Area_" + ID + "_Named_" + labelInt;
	}

	public override string ToString()
	{
		return labelInt;
	}
}
