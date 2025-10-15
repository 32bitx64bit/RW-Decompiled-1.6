using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class SpecificApparelRequirement
{
	public struct TagChance
	{
		public string tag;

		public float chance;
	}

	private BodyPartGroupDef bodyPartGroup;

	private ApparelLayerDef apparelLayer;

	private ThingDef apparelDef;

	private string requiredTag;

	private List<TagChance> alternateTagChoices;

	private ThingDef stuff;

	private ThingStyleDef styleDef;

	private Color color;

	private ColorGenerator colorGenerator;

	private bool locked;

	private bool biocode;

	private QualityCategory? quality;

	private bool useRandomStyleDef;

	private bool ignoreNaked;

	public BodyPartGroupDef BodyPartGroup => bodyPartGroup;

	public ApparelLayerDef ApparelLayer => apparelLayer;

	public ThingDef ApparelDef => apparelDef;

	public string RequiredTag => requiredTag;

	public List<TagChance> AlternateTagChoices => alternateTagChoices;

	public ThingDef Stuff => stuff;

	public Color Color => color;

	public ColorGenerator ColorGenerator => colorGenerator;

	public bool Locked => locked;

	public bool Biocode => biocode;

	public ThingStyleDef StyleDef => styleDef;

	public QualityCategory? Quality => quality;

	public bool UseRandomStyleDef => useRandomStyleDef;

	public bool IgnoreNaked => ignoreNaked;

	public Color GetColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (color != default(Color))
		{
			return color;
		}
		if (colorGenerator != null)
		{
			return colorGenerator.NewRandomizedColor();
		}
		return default(Color);
	}
}
