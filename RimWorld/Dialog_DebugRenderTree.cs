using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_DebugRenderTree : Window
{
	public Pawn pawn;

	private PawnRenderTree tree;

	private Vector2 scrollPosition;

	private float scrollHeight;

	private PawnRenderNode currentNode;

	private float alpha = 1f;

	private PawnDrawParms drawParms;

	private Dictionary<PawnRenderNode, bool> nodeExpanded = new Dictionary<PawnRenderNode, bool>();

	private bool showAll;

	private AnimationDef currentAnimation;

	private const float NodeHeight = 25f;

	private const float IndentWidth = 12f;

	private const float IndentHeightOffset = 6.5f;

	private static readonly FloatRange AngleRange = new FloatRange(-180f, 180f);

	private static readonly FloatRange ScaleRange = new FloatRange(0.01f, 10f);

	private static readonly FloatRange OffsetRange = new FloatRange(-10f, 10f);

	private static readonly FloatRange LayerRange = new FloatRange(-100f, 100f);

	public override Vector2 InitialSize => new Vector2(600f, 600f);

	protected override float Margin => 10f;

	public Dialog_DebugRenderTree(Pawn pawn)
	{
		doCloseX = true;
		preventCameraMotion = false;
		closeOnAccept = false;
		draggable = true;
		Init(pawn);
	}

	public void Init(Pawn pawn)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		currentNode = null;
		scrollPosition = Vector2.zero;
		scrollHeight = 0f;
		nodeExpanded.Clear();
		this.pawn = pawn;
		tree = pawn.Drawer.renderer.renderTree;
		currentAnimation = tree.currentAnimation;
		optionalTitle = pawn.LabelShortCap + " (" + pawn.RaceProps.renderTree.defName + ")";
		drawParms = new PawnDrawParms
		{
			pawn = pawn,
			rotDrawMode = RotDrawMode.Fresh,
			flags = (PawnRenderFlags.Headgear | PawnRenderFlags.Clothes)
		};
		PawnRenderNode rootNode = tree.rootNode;
		if (rootNode != null)
		{
			AddNode(rootNode, null);
		}
	}

	private void AddNode(PawnRenderNode node, PawnRenderNode parent)
	{
		if (parent != null && !nodeExpanded.ContainsKey(parent))
		{
			nodeExpanded.Add(parent, value: true);
		}
		if (!node.children.NullOrEmpty())
		{
			PawnRenderNode[] children = node.children;
			foreach (PawnRenderNode node2 in children)
			{
				AddNode(node2, node);
			}
		}
	}

	public override void WindowUpdate()
	{
		base.WindowUpdate();
		Pawn pawn = Find.Selector.SelectedPawns.FirstOrDefault();
		if (pawn != null && pawn != this.pawn)
		{
			Init(pawn);
		}
		drawParms.facing = this.pawn.Rotation;
		drawParms.posture = this.pawn.GetPosture();
		drawParms.bed = this.pawn.CurrentBed();
		drawParms.coveredInFoam = this.pawn.Drawer.renderer.FirefoamOverlays.coveredInFoam;
		drawParms.carriedThing = this.pawn.carryTracker?.CarriedThing;
		drawParms.dead = this.pawn.Dead;
		drawParms.rotDrawMode = this.pawn.Drawer.renderer.CurRotDrawMode;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		LeftRect(inRect.LeftHalf());
		RightRect(inRect.RightHalf());
	}

	private void LeftRect(Rect inRect)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Invalid comparison between Unknown and I4
		if (pawn == null || tree == null || !tree.Resolved)
		{
			Close();
			return;
		}
		Widgets.DrawMenuSection(inRect);
		inRect = inRect.ContractedBy(Margin / 2f);
		Widgets.HorizontalSlider(new Rect(((Rect)(ref inRect)).x + Margin, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width - Margin * 2f, 25f), ref alpha, FloatRange.ZeroToOne, "Alpha " + alpha.ToStringPercent(), 0.01f);
		pawn.Drawer.renderer.renderTree.debugTint = Color.white.ToTransparent(alpha);
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + (25f + Margin);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x + Margin, ((Rect)(ref inRect)).y, "Animation".GetWidthCached() + 4f, 25f);
		using (new TextBlock(GameFont.Tiny, (TextAnchor)3))
		{
			Widgets.Label(rect, "Animation");
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).xMax + 4f, ((Rect)(ref rect)).y, ((Rect)(ref inRect)).width - ((Rect)(ref rect)).width - Margin * 2f, 25f);
		Widgets.DrawLightHighlight(val);
		Widgets.DrawHighlightIfMouseover(val);
		using (new TextBlock((TextAnchor)4))
		{
			Widgets.Label(val, (currentAnimation == null) ? "None" : currentAnimation.defName);
		}
		if (Widgets.ButtonInvisible(val))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			list.Add(new FloatMenuOption("None", delegate
			{
				currentAnimation = null;
				pawn.Drawer.renderer.SetAnimation(null);
			}));
			foreach (AnimationDef item in DefDatabase<AnimationDef>.AllDefsListForReading)
			{
				AnimationDef def = item;
				list.Add(new FloatMenuOption(item.defName, delegate
				{
					currentAnimation = def;
					pawn.Drawer.renderer.SetAnimation(def);
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + (25f + Margin);
		using (new TextBlock(GameFont.Tiny))
		{
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref inRect)).x + Margin, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width - Margin * 2f, 25f), "Show all nodes", ref showAll);
		}
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + (25f + Margin);
		Widgets.BeginScrollView(inRect, ref scrollPosition, new Rect(0f, 0f, ((Rect)(ref inRect)).width - 16f, scrollHeight));
		float curY = 0f;
		ListNode(tree.rootNode, 0, ref curY, ((Rect)(ref inRect)).width - 16f);
		if ((int)Event.current.type == 8)
		{
			scrollHeight = curY;
		}
		Widgets.EndScrollView();
	}

	private void RightRect(Rect inRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(inRect);
		inRect = inRect.ContractedBy(Margin / 2f);
		Widgets.BeginGroup(inRect);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width, Text.LineHeight);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(Margin, ((Rect)(ref rect)).height + Margin, ((Rect)(ref inRect)).width - Margin * 2f, 30f);
		Widgets.DrawLightHighlight(rect);
		if (currentNode != null)
		{
			using (new TextBlock((TextAnchor)4))
			{
				Widgets.Label(rect, currentNode.ToString().Truncate(((Rect)(ref inRect)).width));
			}
			Widgets.Label(val, currentNode.Worker.GetType().Name.Truncate(((Rect)(ref inRect)).width));
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			bool checkOn = currentNode.debugEnabled;
			Widgets.CheckboxLabeled(val, "Enabled", ref checkOn);
			if (checkOn != currentNode.debugEnabled)
			{
				currentNode.debugEnabled = checkOn;
				currentNode.requestRecache = true;
			}
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Vector3 pivot;
			Vector3 val2 = currentNode.Worker.OffsetFor(currentNode, drawParms, out pivot);
			val2.y = currentNode.Worker.AltitudeFor(currentNode, drawParms);
			float num = currentNode.Props.baseLayer;
			if (currentNode.Props.drawData != null)
			{
				num = currentNode.Props.drawData.LayerForRot(drawParms.facing, num);
			}
			num += currentNode.debugLayerOffset;
			Widgets.Label(val, "Offset " + ((Vector3)(ref val2)).ToString("F2") + " Layer " + num);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugOffset.x, OffsetRange, "X: " + currentNode.debugOffset.x, 0.05f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugOffset.z, OffsetRange, "Z: " + currentNode.debugOffset.z, 0.05f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugLayerOffset, LayerRange, "Layer: " + currentNode.debugLayerOffset, 1f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			((Rect)(ref val)).y = ((Rect)(ref val)).y + Margin;
			Widgets.Label(val, "Pivot (" + pivot.x.ToStringPercent() + ", " + pivot.z.ToStringPercent() + ")");
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugPivotOffset.x, FloatRange.ZeroToOne, "X: " + currentNode.debugPivotOffset.x.ToStringPercent(), 0.01f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugPivotOffset.y, FloatRange.ZeroToOne, "Y: " + currentNode.debugPivotOffset.y.ToStringPercent(), 0.01f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			((Rect)(ref val)).y = ((Rect)(ref val)).y + Margin;
			Quaternion val3 = currentNode.Worker.RotationFor(currentNode, drawParms);
			float y = ((Quaternion)(ref val3)).eulerAngles.y;
			Widgets.Label(val, "Rotation " + y.ToString("F0") + "°");
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugAngleOffset, AngleRange, "Angle: " + currentNode.debugAngleOffset.ToString("F0") + "°", 1f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Vector3 val4 = currentNode.Worker.ScaleFor(currentNode, drawParms);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + Margin;
			Rect rect2 = val;
			Vector3 val5 = val4;
			Widgets.Label(rect2, "Scale " + ((object)(Vector3)(ref val5)).ToString());
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			Widgets.HorizontalSlider(val, ref currentNode.debugScale, ScaleRange, "Scale: " + currentNode.debugScale.ToStringPercent("F0"), 0.01f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			((Rect)(ref val)).y = ((Rect)(ref val)).y + Margin;
			bool checkOn2 = currentNode.debugFlip;
			bool flag = currentNode.FlipGraphic(drawParms);
			Widgets.CheckboxLabeled(val, $"Flip ({flag})", ref checkOn2);
			if (checkOn2 != currentNode.debugFlip)
			{
				currentNode.debugFlip = checkOn2;
				currentNode.requestRecache = true;
			}
			((Rect)(ref val)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height;
			if ((currentNode.debugAngleOffset != 0f || currentNode.debugScale != 1f || currentNode.debugOffset != Vector3.zero || currentNode.debugLayerOffset != 0f || !currentNode.debugEnabled || currentNode.debugPivotOffset != DrawData.PivotCenter) && Widgets.ButtonText(new Rect(0f, ((Rect)(ref inRect)).height - 25f, ((Rect)(ref inRect)).width, 25f), "Reset"))
			{
				currentNode.debugAngleOffset = 0f;
				currentNode.debugScale = 1f;
				currentNode.debugOffset = Vector3.zero;
				currentNode.debugLayerOffset = 0f;
				currentNode.debugEnabled = true;
				currentNode.requestRecache = true;
				currentNode.debugPivotOffset = DrawData.PivotCenter;
			}
		}
		else
		{
			using (new TextBlock((TextAnchor)4))
			{
				Widgets.Label(rect, "No node selected");
			}
		}
		Widgets.EndGroup();
	}

	private void ListNode(PawnRenderNode node, int indent, ref float curY, float width)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		if (!showAll && !node.Worker.ShouldListOnGraph(node, drawParms))
		{
			return;
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector((float)(indent + 1) * 12f, curY, width, 25f);
		((Rect)(ref rect)).xMax = width;
		Rect val = rect.ContractedBy(2f);
		Widgets.DrawHighlight(val);
		Widgets.DrawHighlightIfMouseover(val);
		if (currentNode == node)
		{
			Widgets.DrawHighlight(val);
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).height, ((Rect)(ref val)).height);
		Widgets.DrawLightHighlight(val2);
		if (node.Props.useGraphic)
		{
			Graphic primaryGraphic = node.PrimaryGraphic;
			object obj;
			if (primaryGraphic == null)
			{
				obj = null;
			}
			else
			{
				Material obj2 = primaryGraphic.MatAt(Rot4.South);
				obj = ((obj2 != null) ? obj2.mainTexture : null);
			}
			Texture val3 = (Texture)obj;
			if ((Object)(object)val3 != (Object)null)
			{
				GUI.DrawTexture(val2, val3);
			}
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val2)).xMax + 4f, ((Rect)(ref val)).y, ((Rect)(ref val)).width - ((Rect)(ref val)).height - 4f, ((Rect)(ref val)).height);
		if (!node.Worker.CanDrawNow(node, drawParms) || (node.Props.useGraphic && node.PrimaryGraphic == null))
		{
			GUI.color = ColoredText.SubtleGrayColor;
		}
		Widgets.Label(rect2, node.ToString().Truncate(((Rect)(ref rect2)).width));
		GUI.color = Color.white;
		if (Widgets.ButtonInvisible(val))
		{
			currentNode = node;
		}
		if (!node.children.NullOrEmpty())
		{
			Rect val4 = new Rect((float)indent * 12f, curY + 6.5f, 12f, 12f);
			Widgets.DrawHighlightIfMouseover(val4);
			if (Widgets.ButtonImage(val4, nodeExpanded[node] ? TexButton.Minus : TexButton.Plus))
			{
				nodeExpanded[node] = !nodeExpanded[node];
			}
		}
		curY += 25f;
		if (!node.children.NullOrEmpty() && nodeExpanded[node])
		{
			PawnRenderNode[] children = node.children;
			foreach (PawnRenderNode node2 in children)
			{
				ListNode(node2, indent + 1, ref curY, width);
			}
		}
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector2 initialSize = InitialSize;
		windowRect = GenUI.Rounded(new Rect(5f, 5f, initialSize.x, initialSize.y));
	}
}
