using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

[StaticConstructorOnStartup]
public abstract class PawnRoleSelectionWidgetBase<RoleType> : IPawnRoleSelectionWidget where RoleType : class, ILordJobRole
{
	protected ILordJobCandidatePool candidatePool;

	protected ILordJobAssignmentsManager<RoleType> assignments;

	public bool showIdeoIcon = true;

	private int dragAndDropGroup;

	private RoleType highlightedRole;

	private static readonly object DropContextSpectator = new object();

	private static readonly object DropContextNotParticipating = new object();

	private static List<Action> tmpDelayedGuiCalls = new List<Action>();

	private Rect? lastHoveredDropArea;

	protected float listScrollViewHeight;

	protected Vector2 scrollPositionPawns;

	private int pawnsListEdgeScrollDirection;

	private static List<IGrouping<string, RoleType>> rolesGroupedTmp = new List<IGrouping<string, RoleType>>();

	private static readonly Texture2D WarningIcon = Resources.Load<Texture2D>("Textures/UI/Widgets/Warning");

	public const int PawnsListPadding = 4;

	public const int PawnsListHorizontalGap = 26;

	public const int PawnPortraitHeight = 50;

	public const int PawnPortraitWidth = 50;

	public const int PawnPortraitLabelHeight = 20;

	public const int PawnPortraitHeightTotal = 70;

	public const int PawnPortraitMargin = 4;

	public const int PawnPortraitIconSize = 20;

	public const int HeadlineIconSize = 20;

	public const int EdgeScrollSpeedWhileDragging = 1000;

	private static List<Pawn> nonParticipatingPawnCandidatesTmp = new List<Pawn>();

	private static readonly List<Pawn> tmpSelectedPawns = new List<Pawn>();

	private StringBuilder tipSB = new StringBuilder();

	private static List<Pawn> tmpAssignedPawns = new List<Pawn>();

	public virtual string SpectatorsLabel()
	{
		return "Spectators".Translate();
	}

	public virtual string NotParticipatingLabel()
	{
		return "NotParticipating".Translate();
	}

	public virtual bool ShouldDrawHighlight(RoleType role, Pawn pawn)
	{
		return false;
	}

	public virtual string SpectatorFilterReason(Pawn pawn)
	{
		return null;
	}

	public virtual string ExtraInfoForRole(RoleType role, Pawn pawnToBeAssigned, IEnumerable<Pawn> currentlyAssigned)
	{
		return null;
	}

	public virtual bool ShouldGrayOut(Pawn pawn, out TaggedString reason)
	{
		return !assignments.CanParticipate(pawn, out reason);
	}

	public PawnRoleSelectionWidgetBase(ILordJobCandidatePool candidatePool, ILordJobAssignmentsManager<RoleType> assignments)
	{
		this.candidatePool = candidatePool;
		this.assignments = assignments;
	}

	public void DrawPawnList(Rect listRectPawns)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		float num = ((listScrollViewHeight > ((Rect)(ref listRectPawns)).height) ? 16f : 0f);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref listRectPawns)).width - num, listScrollViewHeight);
		Widgets.BeginScrollView(listRectPawns, ref scrollPositionPawns, viewRect);
		try
		{
			DrawPawnListInternal(viewRect, listRectPawns);
		}
		finally
		{
			Widgets.EndScrollView();
		}
	}

	public void WindowUpdate()
	{
		scrollPositionPawns.y += (float)(pawnsListEdgeScrollDirection * 1000) * Time.deltaTime;
	}

	public virtual void Notify_AssignmentsChanged()
	{
	}

	private void DrawPawnListInternal(Rect viewRect, Rect listRect)
	{
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Invalid comparison between Unknown and I4
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		rolesGroupedTmp.Clear();
		rolesGroupedTmp.AddRange(assignments.RoleGroups());
		try
		{
			int num = DragAndDropWidget.NewGroup();
			dragAndDropGroup = ((num == -1) ? dragAndDropGroup : num);
			int maxPawnsPerRow = Mathf.FloorToInt((((Rect)(ref viewRect)).width - 8f) / 54f);
			float rowHeight = 0f;
			float curY = 0f;
			float curX = 0f;
			int num2 = 0;
			foreach (IGrouping<string, RoleType> item in rolesGroupedTmp)
			{
				IGrouping<string, RoleType> localRoleGroup = item;
				RoleType val = item.First();
				int num3 = 0;
				foreach (RoleType item2 in item)
				{
					num3 += item2.MaxCount;
					num2 += item2.MaxCount;
				}
				string extraInfo = ExtraPawnAssignmentInfo(localRoleGroup);
				IEnumerable<Pawn> enumerable = item.SelectMany((RoleType r) => assignments.AssignedPawns(r));
				TaggedString taggedString = Find.ActiveLanguageWorker.Pluralize(val.CategoryLabelCap, num3);
				Vector2 mp = Event.current.mousePosition;
				DrawRoleGroup(viewRect, enumerable, taggedString, num3, maxPawnsPerRow, ref curX, ref curY, ref rowHeight, delegate(Pawn p, Vector2 dropPos)
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					//IL_0067: Unknown result type (might be due to invalid IL or missing references)
					Pawn pawn2 = (Pawn)DragAndDropWidget.DraggableAt(dragAndDropGroup, Vector2.op_Implicit(mp));
					if (pawn2 != null)
					{
						TryAssignReplace(p, localRoleGroup, pawn2);
					}
					else
					{
						TryAssign(p, localRoleGroup, sendMessage: true, (Pawn)DragAndDropWidget.GetDraggableAfter(dragAndDropGroup, Vector2.op_Implicit(dropPos)), doSound: true, insertLast: true);
					}
				}, item, extraInfo, enumerable.Any() && enumerable.All((Pawn p) => assignments.Required(p)), WarningIcon, null, delegate(Pawn p)
				{
					if (!assignments.Required(p))
					{
						if (!assignments.TryAssignSpectate(p))
						{
							assignments.RemoveParticipant(p);
						}
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
				}, ShouldGrayOut);
			}
			List<Pawn> allCandidatePawns = candidatePool.AllCandidatePawns;
			string spectatorLabel = SpectatorsLabel();
			if (assignments.SpectatorsAllowed)
			{
				List<Pawn> spectatorsForReading = assignments.SpectatorsForReading;
				DrawRoleGroup(viewRect, spectatorsForReading, spectatorLabel, allCandidatePawns.Count, maxPawnsPerRow, ref curX, ref curY, ref rowHeight, delegate(Pawn p, Vector2 dropPos)
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					if (!SendTryAssignMessages(p, null))
					{
						assignments.TryAssignSpectate(p, (Pawn)DragAndDropWidget.GetDraggableAfter(dragAndDropGroup, Vector2.op_Implicit(dropPos)));
						SoundDefOf.DropElement.PlayOneShotOnCamera();
					}
				}, DropContextSpectator, null, locked: false, null, delegate(Pawn p)
				{
					TryAssignAnyRole(p);
				}, delegate(Pawn p)
				{
					if (!assignments.Required(p))
					{
						assignments.RemoveParticipant(p);
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
				}, ShouldGrayOut);
			}
			nonParticipatingPawnCandidatesTmp.Clear();
			nonParticipatingPawnCandidatesTmp.AddRange(allCandidatePawns);
			nonParticipatingPawnCandidatesTmp.AddRange(candidatePool.NonAssignablePawns);
			nonParticipatingPawnCandidatesTmp.RemoveDuplicates();
			IEnumerable<Pawn> selectedPawns = nonParticipatingPawnCandidatesTmp.Where((Pawn p) => !assignments.PawnParticipating(p));
			DrawRoleGroup(viewRect, selectedPawns, NotParticipatingLabel(), nonParticipatingPawnCandidatesTmp.Count, maxPawnsPerRow, ref curX, ref curY, ref rowHeight, delegate(Pawn p, Vector2 dropPos)
			{
				assignments.RemoveParticipant(p);
				SoundDefOf.DropElement.PlayOneShotOnCamera();
			}, DropContextNotParticipating, null, locked: false, null, delegate(Pawn p)
			{
				RoleType firstRole3;
				bool mustReplace4;
				string text5 = CannotAssignReason(p, null, out firstRole3, out mustReplace4);
				if (text5 != null)
				{
					if (!TryAssignAnyRole(p))
					{
						Messages.Message(text5, LookTargets.Invalid, MessageTypeDefOf.RejectInput, historical: false);
					}
				}
				else
				{
					assignments.TryAssignSpectate(p);
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
			}, delegate(Pawn p)
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				string text4 = CannotAssignReason(p, null, out var firstRole2, out var _);
				if (assignments.SpectatorsAllowed)
				{
					Action action = ((text4 != null) ? null : ((Action)delegate
					{
						assignments.TryAssignSpectate(p);
					}));
					list.Add(new FloatMenuOption(PostProcessFloatLabel(spectatorLabel, text4), action));
				}
				foreach (IGrouping<string, RoleType> item3 in rolesGroupedTmp)
				{
					IGrouping<string, RoleType> localRoleGroup2 = item3;
					RoleType val6 = item3.First();
					text4 = CannotAssignReason(p, localRoleGroup2, out firstRole2, out var mustReplace2, isReplacing: true);
					Pawn replacing = (mustReplace2 ? localRoleGroup2.SelectMany((RoleType role) => assignments.AssignedPawns(role)).Last() : null);
					Action action2 = ((text4 != null) ? null : ((Action)delegate
					{
						if (mustReplace2)
						{
							TryAssignReplace(p, localRoleGroup2, replacing);
						}
						else
						{
							TryAssign(p, localRoleGroup2);
						}
					}));
					list.Add(new FloatMenuOption(PostProcessFloatLabel(val6.LabelCap, text4, replacing), action2));
				}
				Find.WindowStack.Add(new FloatMenu(list));
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}, ShouldGrayOut);
			highlightedRole = null;
			curY += rowHeight + 4f;
			using (new TextBlock(GameFont.Tiny, (TextAnchor)3, ColorLibrary.Grey))
			{
				Widgets.Label(new Rect(((Rect)(ref viewRect)).x, curY, ((Rect)(ref viewRect)).width, 24f), SteamDeck.IsSteamDeckInNonKeyboardMode ? "DragPawnsToRolesInfoController".Translate() : "DragPawnsToRolesInfo".Translate());
			}
			curY += 20f;
			if ((int)Event.current.type == 8)
			{
				listScrollViewHeight = curY;
			}
			foreach (Action tmpDelayedGuiCall in tmpDelayedGuiCalls)
			{
				tmpDelayedGuiCall();
			}
			object obj = DragAndDropWidget.CurrentlyDraggedDraggable();
			Pawn pawn = (Pawn)DragAndDropWidget.DraggableAt(dragAndDropGroup, Vector2.op_Implicit(Event.current.mousePosition));
			if (obj != null)
			{
				object obj2 = DragAndDropWidget.HoveringDropArea(dragAndDropGroup);
				if (obj2 != null)
				{
					Rect? val2 = DragAndDropWidget.HoveringDropAreaRect(dragAndDropGroup);
					if (lastHoveredDropArea.HasValue && val2.HasValue)
					{
						Rect? val3 = val2;
						Rect? val4 = lastHoveredDropArea;
						if (val3.HasValue != val4.HasValue || (val3.HasValue && val3.GetValueOrDefault() != val4.GetValueOrDefault()))
						{
							SoundDefOf.DragSlider.PlayOneShotOnCamera();
						}
					}
					lastHoveredDropArea = val2;
				}
				if (obj2 != null && obj2 != DropContextNotParticipating)
				{
					IGrouping<string, RoleType> grouping = obj2 as IGrouping<string, RoleType>;
					RoleType firstRole;
					bool mustReplace;
					string text2 = CannotAssignReason((Pawn)obj, grouping, out firstRole, out mustReplace, grouping != null && pawn != null);
					string text3 = ((firstRole == null) ? null : ExtraPawnAssignmentInfo(grouping, (Pawn)obj));
					if (!string.IsNullOrWhiteSpace(text2) || !string.IsNullOrWhiteSpace(text3))
					{
						string text = (string.IsNullOrWhiteSpace(text2) ? text3 : text2);
						Color color = (string.IsNullOrWhiteSpace(text2) ? ColorLibrary.Yellow : ColorLibrary.RedReadable);
						Text.Font = GameFont.Small;
						Vector2 val5 = Text.CalcSize(text);
						Rect r2 = GenUI.ExpandedBy(new Rect(UI.MousePositionOnUI.x - val5.x / 2f, (float)UI.screenHeight - UI.MousePositionOnUI.y - val5.y - 10f, val5.x, val5.y), 5f);
						Find.WindowStack.ImmediateWindow(47839543, r2, WindowLayer.Super, delegate
						{
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							//IL_0012: Unknown result type (might be due to invalid IL or missing references)
							//IL_0017: Unknown result type (might be due to invalid IL or missing references)
							//IL_0021: Unknown result type (might be due to invalid IL or missing references)
							//IL_0031: Unknown result type (might be due to invalid IL or missing references)
							Text.Font = GameFont.Small;
							GUI.color = color;
							Widgets.Label(r2.AtZero().ContractedBy(5f), text);
							GUI.color = Color.white;
						});
					}
				}
			}
		}
		finally
		{
			tmpDelayedGuiCalls.Clear();
		}
		pawnsListEdgeScrollDirection = 0;
		if (DragAndDropWidget.CurrentlyDraggedDraggable() != null)
		{
			Rect rect = new Rect(((Rect)(ref viewRect)).x, scrollPositionPawns.y, ((Rect)(ref viewRect)).width, 30f);
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref viewRect)).x, scrollPositionPawns.y + (((Rect)(ref listRect)).height - 30f), ((Rect)(ref viewRect)).width, 30f);
			if (Mouse.IsOver(rect))
			{
				pawnsListEdgeScrollDirection = -1;
			}
			else if (Mouse.IsOver(rect2))
			{
				pawnsListEdgeScrollDirection = 1;
			}
		}
	}

	private void DrawRoleGroup(Rect viewRect, IEnumerable<Pawn> selectedPawns, string headline, int maxPawns, int maxPawnsPerRow, ref float curX, ref float curY, ref float rowHeight, Action<Pawn, Vector2> assignAction, object dropAreaContext, string extraInfo = null, bool locked = false, Texture2D extraInfoIcon = null, Action<Pawn> clickHandler = null, Action<Pawn> rightClickHandler = null, TryGetPredicate<Pawn, TaggedString> isGrayedOutPredicate = null)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("DrawRoleGroup"))
		{
			tmpSelectedPawns.AddRange(selectedPawns);
			try
			{
				int num = Mathf.Min(maxPawns, tmpSelectedPawns.Count + 1);
				if (num == 0)
				{
					num = 1;
				}
				int num2 = Mathf.CeilToInt((float)num / (float)maxPawnsPerRow);
				int num3 = Mathf.Min(maxPawnsPerRow, num);
				int num4 = num3 * 50 + (num3 - 1) * 4;
				int num5 = num2 * 70 + (num2 - 1) * 4;
				Vector2 val = Text.CalcSize(headline);
				int num6 = 60;
				int num7 = Mathf.Max(num4, (int)val.x + num6 + ((num6 > 0) ? 10 : 0));
				int num8 = Mathf.FloorToInt((((Rect)(ref viewRect)).width - (curX + 26f)) / 50f);
				bool flag = (num8 > 0 && num8 < maxPawns) || maxPawns + 2 >= maxPawnsPerRow;
				if (flag)
				{
					num4 = maxPawnsPerRow * 50 + (maxPawnsPerRow - 1) * 4;
				}
				if (curX + (float)num7 + 26f > ((Rect)(ref viewRect)).width || flag)
				{
					curY += rowHeight;
					rowHeight = 0f;
					curX = 0f;
				}
				float num9 = 0f;
				Rect rect = default(Rect);
				((Rect)(ref rect))._002Ector(((Rect)(ref viewRect)).x + curX, ((Rect)(ref viewRect)).y + num9 + curY, val.x, val.y);
				Rect val2 = default(Rect);
				((Rect)(ref val2))._002Ector(((Rect)(ref rect)).xMax + 10f, ((Rect)(ref rect)).y + (val.y - 20f) / 2f, (float)num6, 20f);
				num9 += val.y + 4f;
				Rect val3 = default(Rect);
				((Rect)(ref val3))._002Ector(((Rect)(ref viewRect)).x + curX, ((Rect)(ref viewRect)).y + num9 + curY, (float)(num4 + 8), (float)(num5 + 8));
				num9 += ((Rect)(ref val3)).height + 10f;
				rowHeight = Mathf.Max(rowHeight, num9);
				curX += num7 + 26;
				GUI.color = (locked ? ColorLibrary.Grey : Color.white);
				Widgets.Label(rect, headline);
				GUI.color = Color.white;
				Widgets.DrawRectFast(val3, Widgets.MenuSectionBGFillColor);
				if (dropAreaContext is IGrouping<string, RoleType> source && Mouse.IsOver(val3))
				{
					highlightedRole = source.First();
				}
				if (locked)
				{
					Rect val4 = default(Rect);
					((Rect)(ref val4))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, 20f, 20f);
					((Rect)(ref val2)).x = ((Rect)(ref val2)).x + ((Rect)(ref val4)).width;
					((Rect)(ref val2)).width = ((Rect)(ref val2)).width - ((Rect)(ref val4)).height;
					Widgets.DrawTextureFitted(val4, (Texture)(object)IdeoUIUtility.LockedTex, 1f);
					TooltipHandler.TipRegion(val4, () => "Required".Translate(), 93457856);
				}
				if (extraInfo != null)
				{
					Rect val5 = default(Rect);
					((Rect)(ref val5))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, 20f, 20f);
					((Rect)(ref val2)).x = ((Rect)(ref val2)).x + ((Rect)(ref val5)).width;
					((Rect)(ref val2)).width = ((Rect)(ref val2)).width - ((Rect)(ref val5)).height;
					GUI.color = (Color)(Mouse.IsOver(val5) ? Color.white : new Color(0.8f, 0.8f, 0.8f, 1f));
					Widgets.DrawTextureFitted(val5, (Texture)(object)(extraInfoIcon ?? WarningIcon), 1f);
					GUI.color = Color.white;
					TooltipHandler.TipRegion(val5, () => extraInfo, 34899345);
				}
				Rect rect2 = val3.ContractedBy(4f);
				DragAndDropWidget.DropArea(dragAndDropGroup, rect2, delegate(object pawn)
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					assignAction((Pawn)pawn, Event.current.mousePosition);
				}, dropAreaContext);
				if (Mouse.IsOver(rect2))
				{
					Widgets.DrawBoxSolidWithOutline(val3, new Color(0.3f, 0.3f, 0.3f, 1f), new Color(0.5f, 0.5f, 0.5f, 1f), 3);
				}
				GenUI.DrawElementStack(rect2, 70f, tmpSelectedPawns, delegate(Rect r, Pawn p)
				{
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					DrawPawnPortrait(r, p, isGrayedOutPredicate, delegate
					{
						clickHandler?.Invoke(p);
					}, delegate
					{
						rightClickHandler?.Invoke(p);
					});
				}, (Pawn p) => 50f, 4f, 4f, allowOrderOptimization: false);
			}
			finally
			{
				tmpSelectedPawns.Clear();
			}
		}
	}

	private void DrawPawnPortraitInternal(Rect r, Pawn pawn, bool dragging, float scale, TryGetPredicate<Pawn, TaggedString> isGrayedOutPredicate = null)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("DrawPawnPortraitInternal"))
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width * scale, 50f * scale);
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y + 50f * scale, ((Rect)(ref r)).width * scale, 20f * scale);
			TaggedString result = TaggedString.Empty;
			bool flag = isGrayedOutPredicate?.Invoke(pawn, out result) ?? false;
			Material material = (flag ? TexUI.GrayscaleGUI : null);
			GenUI.DrawTextureWithMaterial(val, (Texture)(object)ColonistBar.BGTex, material);
			if (ShouldDrawHighlight(highlightedRole, pawn))
			{
				Widgets.DrawHighlight(val.ContractedBy(3f));
			}
			RenderTexture texture = PortraitsCache.Get(pawn, new Vector2(100f, 100f), Rot4.South, new Vector3(0f, 0f, 0.1f), 1.5f);
			GenUI.DrawTextureWithMaterial(val, (Texture)(object)texture, material);
			Widgets.DrawRectFast(val2, Widgets.WindowBGFillColor);
			float curY = ((Rect)(ref val)).yMax;
			float curX = ((Rect)(ref val)).xMax;
			float iconSize = 20f * scale;
			PawnPortraitIconsDrawer.DrawPawnPortraitIcons(val, pawn, assignments.Required(pawn), flag, ref curX, ref curY, iconSize, showIdeoIcon, out var tooltipActive);
			using (new TextBlock(GameFont.Tiny, (TextAnchor)4, flag ? Color.gray : Color.white))
			{
				Widgets.LabelFit(val2, pawn.LabelShortCap);
				if (!tooltipActive && !dragging)
				{
					TooltipHandler.TipRegion(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width * scale, 70f * scale), PawnTooltip(pawn, result));
				}
			}
		}
	}

	private string PawnTooltip(Pawn pawn, TaggedString cannotAssignReason)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		tipSB.Clear();
		tipSB.AppendLineTagged(pawn.LabelShortCap.AsTipTitle());
		string text = ExtraTipContents(pawn);
		if (!text.NullOrEmpty())
		{
			tipSB.AppendLine(text);
		}
		if (!cannotAssignReason.NullOrEmpty())
		{
			tipSB.AppendLine();
			tipSB.AppendLineTagged(cannotAssignReason.Colorize(ColorLibrary.RedReadable));
		}
		return tipSB.ToString();
	}

	protected virtual string ExtraTipContents(Pawn pawn)
	{
		return null;
	}

	private void DrawPawnPortrait(Rect rect, Pawn pawn, TryGetPredicate<Pawn, TaggedString> isGrayedOutPredicate = null, Action clickHandler = null, Action rightClickHandler = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect) && (int)Event.current.type == 0 && Event.current.button == 1)
		{
			rightClickHandler?.Invoke();
		}
		Vector2 mp = Event.current.mousePosition;
		if (!assignments.Required(pawn) && DragAndDropWidget.Draggable(dragAndDropGroup, rect, pawn, clickHandler, delegate
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			lastHoveredDropArea = DragAndDropWidget.HoveringDropAreaRect(dragAndDropGroup, Vector2.op_Implicit(mp));
		}))
		{
			((Rect)(ref rect)).position = Event.current.mousePosition;
			tmpDelayedGuiCalls.Add(delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				DrawPawnPortraitInternal(rect, pawn, dragging: true, 0.9f, isGrayedOutPredicate);
			});
		}
		else
		{
			DrawPawnPortraitInternal(rect, pawn, dragging: false, 1f, isGrayedOutPredicate);
			Widgets.DrawHighlightIfMouseover(rect);
		}
	}

	private bool SendTryAssignMessages(Pawn pawn, IEnumerable<RoleType> roleGroup, bool isReplacing = false)
	{
		RoleType firstRole;
		bool mustReplace;
		string text = CannotAssignReason(pawn, roleGroup, out firstRole, out mustReplace, isReplacing);
		if (text != null)
		{
			Messages.Message(text, LookTargets.Invalid, MessageTypeDefOf.RejectInput, historical: false);
			return true;
		}
		return false;
	}

	private bool DoTryAssign(Pawn pawn, IEnumerable<RoleType> roleGroup, bool sendMessage = true, Pawn insertBefore = null, bool doSound = true)
	{
		if (sendMessage && SendTryAssignMessages(pawn, roleGroup))
		{
			return false;
		}
		if (!sendMessage && CannotAssignReason(pawn, roleGroup, out var _, out var _) != null)
		{
			return false;
		}
		foreach (RoleType item in roleGroup)
		{
			if (assignments.TryAssign(pawn, item, out var _, PsychicRitualRoleDef.Context.Dialog_BeginPsychicRitual, insertBefore))
			{
				Notify_AssignmentsChanged();
				if (doSound)
				{
					SoundDefOf.DropElement.PlayOneShotOnCamera();
				}
				return true;
			}
		}
		return false;
	}

	private bool TryAssign(Pawn pawn, IEnumerable<RoleType> roleGroup, bool sendMessage = true, Pawn insertBefore = null, bool doSound = true, bool insertLast = false)
	{
		try
		{
			int num = 0;
			foreach (RoleType item in roleGroup)
			{
				if (item.MaxCount == 0)
				{
					num = -1;
				}
				else if (item.MaxCount > 0)
				{
					num += item.MaxCount;
				}
				tmpAssignedPawns.AddRange(assignments.AssignedPawns(item));
			}
			if ((num > 0 && tmpAssignedPawns.Count == num) || (insertBefore != null && !tmpAssignedPawns.Contains(insertBefore)))
			{
				return DoTryAssign(pawn, roleGroup, sendMessage, null, doSound);
			}
			foreach (Pawn tmpAssignedPawn in tmpAssignedPawns)
			{
				assignments.TryUnassignAnyRole(tmpAssignedPawn);
			}
			if (insertBefore == null)
			{
				if (insertLast)
				{
					tmpAssignedPawns.Add(pawn);
				}
				else
				{
					tmpAssignedPawns.Insert(0, pawn);
				}
			}
			else
			{
				tmpAssignedPawns.Insert(tmpAssignedPawns.IndexOf(insertBefore), pawn);
			}
			bool result = false;
			foreach (Pawn tmpAssignedPawn2 in tmpAssignedPawns)
			{
				bool flag = DoTryAssign(tmpAssignedPawn2, roleGroup, sendMessage && tmpAssignedPawn2 == pawn, null, tmpAssignedPawn2 == pawn);
				if (tmpAssignedPawn2 == pawn)
				{
					result = flag;
				}
			}
			return result;
		}
		finally
		{
			tmpAssignedPawns.Clear();
		}
	}

	private bool TryAssignReplace(Pawn pawn, IEnumerable<RoleType> roleGroup, Pawn replacing)
	{
		if (!SendTryAssignMessages(pawn, roleGroup, isReplacing: true))
		{
			bool num = assignments.PawnSpectating(pawn);
			RoleType val = assignments.RoleForPawn(pawn);
			Pawn insertBefore = roleGroup.SelectMany((RoleType r) => assignments.AssignedPawns(r)).SkipWhile((Pawn p) => p != replacing).FirstOrDefault();
			assignments.RemoveParticipant(replacing);
			TryAssign(pawn, roleGroup, sendMessage: true, insertBefore, doSound: true, insertLast: true);
			PsychicRitualRoleDef.Reason reason;
			if (num)
			{
				assignments.TryAssignSpectate(replacing);
			}
			else if (val != null && assignments.TryAssign(replacing, val, out reason))
			{
				Notify_AssignmentsChanged();
			}
		}
		return roleGroup.Contains(assignments.RoleForPawn(pawn));
	}

	private bool TryAssignAnyRole(Pawn p)
	{
		string text = null;
		bool flag = rolesGroupedTmp.Count == 1;
		RoleType firstRole;
		foreach (IGrouping<string, RoleType> item in rolesGroupedTmp)
		{
			text = CannotAssignReason(p, item, out firstRole, out var _, isReplacing: true);
			if (text == null && TryAssign(p, item, sendMessage: false))
			{
				return true;
			}
		}
		foreach (IGrouping<string, RoleType> item2 in rolesGroupedTmp)
		{
			text = CannotAssignReason(p, item2, out firstRole, out var mustReplace2, isReplacing: true);
			if (text == null)
			{
				Pawn replacing = (mustReplace2 ? item2.SelectMany((RoleType role) => assignments.AssignedPawns(role)).Last() : null);
				if (TryAssignReplace(p, item2, replacing))
				{
					return true;
				}
			}
		}
		if (flag && text != null)
		{
			Messages.Message(text, LookTargets.Invalid, MessageTypeDefOf.RejectInput, historical: false);
		}
		SoundDefOf.ClickReject.PlayOneShotOnCamera();
		return false;
	}

	private string ExtraPawnAssignmentInfo(IEnumerable<RoleType> roleGroup, Pawn pawnToBeAssigned = null)
	{
		RoleType val = ((roleGroup != null) ? roleGroup.First() : null);
		IEnumerable<Pawn> enumerable = assignments.AssignedPawns(val);
		if (pawnToBeAssigned != null)
		{
			enumerable = enumerable.Concat(new Pawn[1] { pawnToBeAssigned }).Distinct();
		}
		string text = ExtraInfoForRole(val, pawnToBeAssigned, enumerable);
		if (val.MinCount > 0 && pawnToBeAssigned == null && assignments.FirstAssignedPawn(val) == null)
		{
			int num = 0;
			foreach (RoleType item in roleGroup)
			{
				num += item.MinCount;
			}
			text = ((num <= 1) ? ((string)(text + "MessageLordJobNeedsAtLeastOneRolePawn".Translate(val.Label.Resolve()))) : ((string)(text + "MessageLordJobNeedsAtLeastNumRolePawn".Translate(Find.ActiveLanguageWorker.Pluralize(val.Label), num))));
		}
		return text;
	}

	private static string PostProcessFloatLabel(string label, string unavailableReason, Pawn replacing = null)
	{
		string text = label;
		if (unavailableReason != null)
		{
			text += " (" + "DisabledLower".Translate().CapitalizeFirst() + ": " + unavailableReason.CapitalizeFirst() + ")";
		}
		if (replacing != null)
		{
			text += " (" + "RitualRoleReplaces".Translate(replacing.Named("PAWN")) + ")";
		}
		return "AssignToRole".Translate(text);
	}

	private string CannotAssignReason(Pawn draggable, IEnumerable<RoleType> roles, out RoleType firstRole, out bool mustReplace, bool isReplacing = false)
	{
		int num = 0;
		int num2 = 0;
		firstRole = ((roles != null) ? roles.FirstOrDefault() : null);
		mustReplace = false;
		string text = assignments.PawnNotAssignableReason(draggable, firstRole).CapitalizeFirst();
		if (text == null && firstRole == null)
		{
			text = SpectatorFilterReason(draggable);
		}
		if (text == null && firstRole != null)
		{
			bool flag = true;
			foreach (RoleType role in roles)
			{
				if (!assignments.AssignedPawns(role).Any())
				{
					flag = false;
					break;
				}
				foreach (Pawn item in assignments.AssignedPawns(role))
				{
					if (assignments.ForcedRole(item) != role)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				text = "RoleIsLocked".Translate(firstRole.Label);
			}
		}
		if (text == null && roles != null && !roles.Any((RoleType r) => assignments.RoleForPawn(draggable) == r))
		{
			foreach (RoleType role2 in roles)
			{
				if (role2.MaxCount <= 0)
				{
					num = -1;
				}
				if (num != -1)
				{
					num += role2.MaxCount;
				}
				num2 += assignments.AssignedPawns(role2).Count();
			}
			if (num >= 0 && num <= num2)
			{
				mustReplace = true;
				if (!isReplacing)
				{
					text = "MaxPawnsPerRole".Translate(firstRole.Label, num);
				}
			}
		}
		return text;
	}
}
