using UnityEngine;

namespace Verse;

public static class MeshMakerPlanes
{
	private const float BackLiftAmount = 0.0018292684f;

	private const float TwistAmount = 0.0009146342f;

	public static Mesh NewPlaneMesh(float size)
	{
		return NewPlaneMesh(size, flipped: false);
	}

	public static Mesh NewPlaneMesh(float size, bool flipped)
	{
		return NewPlaneMesh(size, flipped, backLift: false);
	}

	public static Mesh NewPlaneMesh(float size, bool flipped, bool backLift)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return NewPlaneMesh(new Vector2(size, size), flipped, backLift, twist: false);
	}

	public static Mesh NewPlaneMesh(float size, bool flipped, bool backLift, bool twist)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return NewPlaneMesh(new Vector2(size, size), flipped, backLift, twist);
	}

	public static Mesh NewPlaneMesh(Vector2 size, bool flipped, bool backLift, bool twist)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Expected O, but got Unknown
		//IL_023e: Expected O, but got Unknown
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		Vector2[] array2 = (Vector2[])(object)new Vector2[4];
		int[] array3 = new int[6];
		array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
		array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
		array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
		array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);
		if (backLift)
		{
			array[1].y = 0.0018292684f;
			array[2].y = 0.0018292684f;
			array[3].y = 0.0007317074f;
		}
		if (twist)
		{
			array[0].y = 0.0009146342f;
			array[1].y = 0.0004573171f;
			array[2].y = 0f;
			array[3].y = 0.0004573171f;
		}
		if (!flipped)
		{
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(0f, 1f);
			array2[2] = new Vector2(1f, 1f);
			array2[3] = new Vector2(1f, 0f);
		}
		else
		{
			array2[0] = new Vector2(1f, 0f);
			array2[1] = new Vector2(1f, 1f);
			array2[2] = new Vector2(0f, 1f);
			array2[3] = new Vector2(0f, 0f);
		}
		array3[0] = 0;
		array3[1] = 1;
		array3[2] = 2;
		array3[3] = 0;
		array3[4] = 2;
		array3[5] = 3;
		Mesh val = new Mesh
		{
			name = "NewPlaneMesh()",
			vertices = array,
			uv = array2
		};
		val.SetTriangles(array3, 0);
		val.RecalculateNormals();
		val.RecalculateBounds();
		MeshPool.EnsureMetaDataCached(val, size, flipped);
		return val;
	}

	public static Mesh NewWholeMapPlane()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = NewPlaneMesh(2000f, flipped: false, backLift: false);
		Vector2[] array = (Vector2[])(object)new Vector2[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = val.uv[i] * 200f;
		}
		val.uv = array;
		return val;
	}
}
