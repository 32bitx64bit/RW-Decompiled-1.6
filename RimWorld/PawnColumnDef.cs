using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnDef : Def
{
	public Type workerClass = typeof(PawnColumnWorker);

	public bool sortable;

	public bool ignoreWhenCalculatingOptimalTableSize;

	[NoTranslate]
	public string headerIcon;

	public Vector2 headerIconSize;

	[MustTranslate]
	public string headerTip;

	public bool headerAlwaysInteractable;

	public bool paintable;

	public bool groupable;

	public TrainableDef trainable;

	public int gap;

	public WorkTypeDef workType;

	public bool moveWorkTypeLabelDown;

	public bool showIcon;

	public bool useLabelShort;

	public int widthPriority;

	public int width = -1;

	[Unsaved(false)]
	private PawnColumnWorker workerInt;

	[Unsaved(false)]
	private Texture2D headerIconTex;

	private const int IconWidth = 26;

	private static readonly Vector2 IconSize = new Vector2(26f, 26f);

	public PawnColumnWorker Worker
	{
		get
		{
			if (workerInt == null)
			{
				workerInt = (PawnColumnWorker)Activator.CreateInstance(workerClass);
				workerInt.def = this;
			}
			return workerInt;
		}
	}

	public Texture2D HeaderIcon
	{
		get
		{
			if ((Object)(object)headerIconTex == (Object)null && !headerIcon.NullOrEmpty())
			{
				headerIconTex = ContentFinder<Texture2D>.Get(headerIcon);
			}
			return headerIconTex;
		}
	}

	public Vector2 HeaderIconSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (headerIconSize != default(Vector2))
			{
				return headerIconSize;
			}
			if ((Object)(object)HeaderIcon != (Object)null)
			{
				return IconSize;
			}
			return Vector2.zero;
		}
	}

	public bool HeaderInteractable
	{
		get
		{
			if (!sortable && headerTip.NullOrEmpty())
			{
				return headerAlwaysInteractable;
			}
			return true;
		}
	}
}
