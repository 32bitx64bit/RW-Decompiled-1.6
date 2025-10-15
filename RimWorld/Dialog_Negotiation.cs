using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_Negotiation : Dialog_NodeTree
{
	protected Pawn negotiator;

	protected ICommunicable commTarget;

	private const float TitleHeight = 70f;

	private const float InfoHeight = 60f;

	public override Vector2 InitialSize => new Vector2(720f, 600f);

	public Dialog_Negotiation(Pawn negotiator, ICommunicable commTarget, DiaNode startNode, bool radioMode)
		: base(startNode, radioMode)
	{
		this.negotiator = negotiator;
		this.commTarget = commTarget;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(inRect);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width / 2f, 70f);
		Rect rect2 = new Rect(0f, ((Rect)(ref rect)).yMax, ((Rect)(ref rect)).width, 60f);
		Rect rect3 = new Rect(((Rect)(ref inRect)).width / 2f, 0f, ((Rect)(ref inRect)).width / 2f, 70f);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref inRect)).width / 2f, ((Rect)(ref rect)).yMax, ((Rect)(ref rect)).width, 60f);
		Text.Font = GameFont.Medium;
		Widgets.Label(rect, negotiator.LabelCap);
		Text.Anchor = (TextAnchor)2;
		Widgets.Label(rect3, commTarget.GetCallLabel());
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		GUI.color = new Color(1f, 1f, 1f, 0.7f);
		Widgets.Label(rect2, "SocialSkillIs".Translate(negotiator.skills.GetSkill(SkillDefOf.Social).Level));
		Text.Anchor = (TextAnchor)2;
		Widgets.Label(rect4, commTarget.GetInfoText());
		Faction faction = commTarget.GetFaction();
		if (faction != null)
		{
			FactionRelationKind playerRelationKind = faction.PlayerRelationKind;
			GUI.color = playerRelationKind.GetColor();
			Widgets.Label(new Rect(((Rect)(ref rect4)).x, ((Rect)(ref rect4)).y + Text.CalcHeight(commTarget.GetInfoText(), ((Rect)(ref rect4)).width) + Text.SpaceBetweenLines, ((Rect)(ref rect4)).width, 30f), playerRelationKind.GetLabelCap());
		}
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
		Widgets.EndGroup();
		float num = 147f;
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(0f, num, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - num);
		DrawNode(rect5);
	}
}
