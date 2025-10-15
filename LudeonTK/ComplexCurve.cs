using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LudeonTK;

public class ComplexCurve : IEquatable<ComplexCurve>
{
	private List<Keyframe> keyframes;

	private WrapMode preWrapMode = (WrapMode)8;

	private WrapMode postWrapMode = (WrapMode)8;

	[Unsaved(false)]
	private AnimationCurve curve;

	public Keyframe[] Keys
	{
		get
		{
			return curve.keys;
		}
		set
		{
			curve.keys = value;
			keyframes = value.ToList();
		}
	}

	public int Length => curve.length;

	public WrapMode PreWrapMode
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return curve.preWrapMode;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			curve.preWrapMode = value;
		}
	}

	public WrapMode PostWrapMode
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return curve.postWrapMode;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			curve.postWrapMode = value;
		}
	}

	public static ComplexCurve LinearNormalized => new ComplexCurve(AnimationCurve.Linear(0f, 0f, 1f, 1f));

	public ComplexCurve()
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0009: Unknown result type (might be due to invalid IL or missing references)


	public ComplexCurve(params Keyframe[] keyframes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		curve = new AnimationCurve(keyframes);
	}

	public ComplexCurve(AnimationCurve curve)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.curve = curve;
	}

	public float Evaluate(float time)
	{
		return curve.Evaluate(time);
	}

	public int AddKey(float time, float value)
	{
		return curve.AddKey(time, value);
	}

	public int AddKey(Keyframe keyframe)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return curve.AddKey(keyframe);
	}

	public int MoveKey(int index, Keyframe keyframe)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return curve.MoveKey(index, keyframe);
	}

	public void RemoveKey(int index)
	{
		curve.RemoveKey(index);
	}

	public void SmoothTangents(int index, float weight)
	{
		curve.SmoothTangents(index, weight);
	}

	public static ComplexCurve Constant(float timeStart, float timeEnd, float value)
	{
		return new ComplexCurve(AnimationCurve.Constant(timeStart, timeEnd, value));
	}

	public static ComplexCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
	{
		return new ComplexCurve(AnimationCurve.Linear(timeStart, valueStart, timeEnd, valueEnd));
	}

	public static ComplexCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
	{
		return new ComplexCurve(AnimationCurve.EaseInOut(timeStart, valueStart, timeEnd, valueEnd));
	}

	public static ComplexCurve Empty()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		return new ComplexCurve(new AnimationCurve());
	}

	public AnimationCurve GetInternalCurveCopy()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		return new AnimationCurve(curve.keys)
		{
			preWrapMode = curve.preWrapMode,
			postWrapMode = curve.postWrapMode
		};
	}

	public void PostLoad()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		curve = new AnimationCurve
		{
			preWrapMode = preWrapMode,
			postWrapMode = postWrapMode,
			keys = keyframes?.ToArray()
		};
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (this == o)
		{
			return true;
		}
		if (o.GetType() == GetType())
		{
			return Equals((ComplexCurve)o);
		}
		return false;
	}

	public bool Equals(ComplexCurve other)
	{
		if (other != null)
		{
			return curve.Equals(other.curve);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)curve).GetHashCode();
	}
}
