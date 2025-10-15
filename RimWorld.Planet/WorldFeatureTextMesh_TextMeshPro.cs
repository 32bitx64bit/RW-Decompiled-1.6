using System;
using LudeonTK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public class WorldFeatureTextMesh_TextMeshPro : WorldFeatureTextMesh
{
	private TextMeshPro textMesh;

	public static readonly GameObject WorldTextPrefab = Resources.Load<GameObject>("Prefabs/WorldText");

	[TweakValue("Interface.World", 0f, 5f)]
	private static float TextScale = 1f;

	public override bool Active => ((Component)textMesh).gameObject.activeInHierarchy;

	public override Vector3 Position => textMesh.transform.position;

	public override Color Color
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Graphic)textMesh).color;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Graphic)textMesh).color = value;
		}
	}

	public override string Text
	{
		get
		{
			return ((TMP_Text)textMesh).text;
		}
		set
		{
			((TMP_Text)textMesh).text = value;
		}
	}

	public override float Size
	{
		set
		{
			((TMP_Text)textMesh).fontSize = value * TextScale;
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return textMesh.transform.rotation;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			textMesh.transform.rotation = value;
		}
	}

	public override Vector3 LocalPosition
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return textMesh.transform.localPosition;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			textMesh.transform.localPosition = value;
		}
	}

	private static void TextScale_Changed()
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(WorldTextPrefab);
		Object.DontDestroyOnLoad((Object)(object)val);
		textMesh = val.GetComponent<TextMeshPro>();
		Color = new Color(1f, 1f, 1f, 0f);
		Material[] sharedMaterials = ((Renderer)((Component)textMesh).GetComponent<MeshRenderer>()).sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			sharedMaterials[i].renderQueue = 3610;
		}
	}

	public override void WrapAroundPlanetSurface(PlanetLayer layer)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)textMesh).ForceMeshUpdate(false, false);
		TMP_TextInfo textInfo = ((TMP_Text)textMesh).textInfo;
		int characterCount = textInfo.characterCount;
		if (characterCount == 0)
		{
			return;
		}
		Bounds bounds = ((TMP_Text)textMesh).bounds;
		float num = ((Bounds)(ref bounds)).extents.x * 2f;
		float num2 = layer.DistOnSurfaceToAngle(num);
		Matrix4x4 localToWorldMatrix = textMesh.transform.localToWorldMatrix;
		Matrix4x4 worldToLocalMatrix = textMesh.transform.worldToLocalMatrix;
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < characterCount; i++)
		{
			TMP_CharacterInfo val = textInfo.characterInfo[i];
			if (val.isVisible)
			{
				int materialReferenceIndex = ((TMP_Text)textMesh).textInfo.characterInfo[i].materialReferenceIndex;
				int vertexIndex = val.vertexIndex;
				Vector3 val2 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3];
				val2 /= 4f;
				float num3 = val2.x / (num / 2f);
				bool flag = num3 >= 0f;
				num3 = Mathf.Abs(num3);
				float num4 = num2 / 2f * num3;
				float num5 = (180f - num4) / 2f;
				float num6 = 200f * Mathf.Tan(num4 / 2f * (MathF.PI / 180f));
				((Vector3)(ref val3))._002Ector(Mathf.Sin(num5 * (MathF.PI / 180f)) * num6 * (flag ? 1f : (-1f)), val2.y, Mathf.Cos(num5 * (MathF.PI / 180f)) * num6);
				Vector3 val4 = val3 - val2;
				Vector3 val5 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] + val4;
				Vector3 val6 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] + val4;
				Vector3 val7 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] + val4;
				Vector3 val8 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3] + val4;
				Quaternion val9 = Quaternion.Euler(0f, num4 * (flag ? (-1f) : 1f), 0f);
				val5 = val9 * (val5 - val3) + val3;
				val6 = val9 * (val6 - val3) + val3;
				val7 = val9 * (val7 - val3) + val3;
				val8 = val9 * (val8 - val3) + val3;
				Vector3 val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val5);
				val5 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (layer.Radius + 0.4f));
				val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val6);
				val6 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (layer.Radius + 0.4f));
				val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val7);
				val7 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (layer.Radius + 0.4f));
				val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val8);
				val8 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (layer.Radius + 0.4f));
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] = val5;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] = val6;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] = val7;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3] = val8;
			}
		}
		((TMP_Text)textMesh).UpdateVertexData((TMP_VertexDataUpdateFlags)255);
	}
}
