using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldFeatureTextMesh_Legacy : WorldFeatureTextMesh
{
	private TextMesh textMesh;

	private const float TextScale = 0.23f;

	private const int MinFontSize = 13;

	private const int MaxFontSize = 40;

	[TweakValue("Interface.World", 0f, 10f)]
	private static float TextScaleFactor = 7.5f;

	public override bool Active => ((Component)textMesh).gameObject.activeInHierarchy;

	public override Vector3 Position => ((Component)textMesh).transform.position;

	public override Color Color
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return textMesh.color;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			textMesh.color = value;
		}
	}

	public override string Text
	{
		get
		{
			return textMesh.text;
		}
		set
		{
			textMesh.text = value;
		}
	}

	public override float Size
	{
		set
		{
			textMesh.fontSize = Mathf.RoundToInt(value * TextScaleFactor);
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)textMesh).transform.rotation;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			((Component)textMesh).transform.rotation = value;
		}
	}

	public override Vector3 LocalPosition
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)textMesh).transform.localPosition;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			((Component)textMesh).transform.localPosition = value;
		}
	}

	private static void TextScaleFactor_Changed()
	{
		Find.WorldFeatures.textsCreated = false;
	}

	public override void SetActive(bool active)
	{
		((Component)textMesh).gameObject.SetActive(active);
	}

	public override void Destroy()
	{
		Object.Destroy((Object)(object)((Component)textMesh).gameObject);
	}

	public override void Init()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("World feature name (legacy)");
		val.layer = WorldCameraManager.WorldLayer;
		Object.DontDestroyOnLoad((Object)(object)val);
		textMesh = val.AddComponent<TextMesh>();
		textMesh.color = new Color(1f, 1f, 1f, 0f);
		textMesh.anchor = (TextAnchor)4;
		textMesh.alignment = (TextAlignment)1;
		((Renderer)((Component)textMesh).GetComponent<MeshRenderer>()).sharedMaterial.renderQueue = 3610;
		Color = new Color(1f, 1f, 1f, 0f);
		((Component)textMesh).transform.localScale = new Vector3(0.23f, 0.23f, 0.23f);
	}

	public override void WrapAroundPlanetSurface(PlanetLayer layer)
	{
	}
}
