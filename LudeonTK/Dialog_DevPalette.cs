using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LudeonTK;

[StaticConstructorOnStartup]
public class Dialog_DevPalette : Window_Dev
{
	private Vector2 windowPosition;

	private static List<DebugActionNode> cachedNodes;

	private int reorderableGroupID = -1;

	private Dictionary<string, string> nameCache = new Dictionary<string, string>();

	private const string Title = "Dev palette";

	private const float ButtonSize = 24f;

	private const float ButtonSize_Small = 22f;

	private const string NoActionDesc = "<i>To add commands here, open the debug actions menu and click the pin icons.</i>";

	public override bool IsDebug => true;

	protected override float Margin => 4f;

	private List<DebugActionNode> Nodes
	{
		get
		{
			if (cachedNodes == null)
			{
				cachedNodes = new List<DebugActionNode>();
				for (int i = 0; i < Prefs.DebugActionsPalette.Count; i++)
				{
					DebugActionNode node = Dialog_Debug.GetNode(Prefs.DebugActionsPalette[i]);
					if (node != null)
					{
						cachedNodes.Add(node);
					}
				}
			}
			return cachedNodes;
		}
	}

	public Dialog_DevPalette()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		draggable = true;
		focusWhenOpened = false;
		drawShadow = false;
		closeOnAccept = false;
		closeOnCancel = false;
		preventCameraMotion = false;
		drawInScreenshotMode = false;
		windowPosition = Prefs.DevPalettePosition;
		onlyDrawInDevMode = true;
		doCloseX = true;
		EnsureAllNodesValid();
	}

	private void EnsureAllNodesValid()
	{
		cachedNodes = null;
		for (int num = Prefs.DebugActionsPalette.Count - 1; num >= 0; num--)
		{
			string text = Prefs.DebugActionsPalette[num];
			if (Dialog_Debug.GetNode(text) == null)
			{
				Log.Warning("Could not find node from path '" + text + "'. Removing.");
				Prefs.DebugActionsPalette.RemoveAt(num);
				Prefs.Save();
			}
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		DevGUI.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 24f), "Dev palette");
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + 26f;
		if (Prefs.DebugActionsPalette.Count == 0)
		{
			GUI.color = ColoredText.SubtleGrayColor;
			DevGUI.Label(inRect, "<i>To add commands here, open the debug actions menu and click the pin icons.</i>");
			GUI.color = Color.white;
		}
		else
		{
			if ((int)Event.current.type == 7)
			{
				reorderableGroupID = ReorderableWidget.NewGroup(delegate(int from, int to)
				{
					string item = Prefs.DebugActionsPalette[from];
					Prefs.DebugActionsPalette.Insert(to, item);
					Prefs.DebugActionsPalette.RemoveAt((from < to) ? from : (from + 1));
					cachedNodes = null;
					Prefs.Save();
				}, ReorderableDirection.Vertical, inRect, 2f, null, playSoundOnStartReorder: false);
			}
			GUI.BeginGroup(inRect);
			float num = 0f;
			Text.Font = GameFont.Tiny;
			Rect val = default(Rect);
			Rect rect2 = default(Rect);
			Rect val2 = default(Rect);
			Rect butRect = default(Rect);
			for (int i = 0; i < Nodes.Count; i++)
			{
				DebugActionNode debugActionNode = Nodes[i];
				float num2 = 0f;
				num2 += 22f;
				((Rect)(ref val))._002Ector(num2, num, ((Rect)(ref inRect)).width - 44f, 22f);
				if (debugActionNode.ActiveNow)
				{
					if (debugActionNode.settingsField != null)
					{
						Rect rect = val;
						((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - (((Rect)(ref rect)).height + 4f);
						DevGUI.Label(rect, "  " + PrettifyNodeName(debugActionNode));
						GUI.DrawTexture(new Rect(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height), (Texture)(object)(debugActionNode.On ? DevGUI.CheckOn : DevGUI.CheckOff));
						DevGUI.DrawHighlightIfMouseover(val);
						if (DevGUI.ButtonInvisible(val))
						{
							debugActionNode.Enter(null);
						}
					}
					else if (DevGUI.ButtonText(val, "  " + PrettifyNodeName(debugActionNode), (TextAnchor)3))
					{
						debugActionNode.Enter(Find.WindowStack.WindowOfType<Dialog_Debug>());
					}
				}
				else
				{
					DevGUI.Label(val, "  " + PrettifyNodeName(debugActionNode));
				}
				num2 += ((Rect)(ref val)).width;
				((Rect)(ref rect2))._002Ector(0f, num, 22f, 22f);
				((Rect)(ref val2))._002Ector(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, ((Rect)(ref inRect)).width - 22f, 22f);
				if (((int)Event.current.type != 0 || Mouse.IsOver(rect2)) && ReorderableWidget.Reorderable(reorderableGroupID, val2))
				{
					DevGUI.DrawRect(val2, DevGUI.WindowBGFillColor * new Color(1f, 1f, 1f, 0.5f));
				}
				DevGUI.ButtonImage(rect2.ContractedBy(1f), TexButton.DragHash);
				((Rect)(ref butRect))._002Ector(num2, num, 22f, 22f);
				if (Widgets.ButtonImage(butRect, TexButton.Delete))
				{
					Prefs.DebugActionsPalette.RemoveAt(i);
					Prefs.Save();
					cachedNodes = null;
					SetInitialSizeAndPosition();
				}
				num2 += ((Rect)(ref butRect)).width;
				num += 24f;
			}
			Text.Font = GameFont.Small;
			GUI.EndGroup();
		}
		if (!Mathf.Approximately(((Rect)(ref windowRect)).x, windowPosition.x) || !Mathf.Approximately(((Rect)(ref windowRect)).y, windowPosition.y))
		{
			windowPosition = new Vector2(((Rect)(ref windowRect)).x, ((Rect)(ref windowRect)).y);
			Prefs.DevPalettePosition = windowPosition;
		}
	}

	public static void ToggleAction(string actionLabel)
	{
		if (Prefs.DebugActionsPalette.Contains(actionLabel))
		{
			Prefs.DebugActionsPalette.Remove(actionLabel);
		}
		else
		{
			Prefs.DebugActionsPalette.Add(actionLabel);
		}
		Prefs.Save();
		cachedNodes = null;
		Find.WindowStack.WindowOfType<Dialog_DevPalette>()?.SetInitialSizeAndPosition();
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		GameFont font = Text.Font;
		Text.Font = GameFont.Small;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(Text.CalcSize("Dev palette").x + 48f + 10f, 28f);
		if (!Nodes.Any())
		{
			val.x = Mathf.Max(val.x, 200f);
			val.y += Text.CalcHeight("<i>To add commands here, open the debug actions menu and click the pin icons.</i>", val.x) + Margin * 2f;
		}
		else
		{
			Text.Font = GameFont.Tiny;
			for (int i = 0; i < Nodes.Count; i++)
			{
				val.x = Mathf.Max(val.x, Text.CalcSize("  " + PrettifyNodeName(Nodes[i]) + "  ").x + 48f);
			}
			val.y += (float)Nodes.Count * 22f + (float)((Nodes.Count + 1) * 2) + Margin;
		}
		windowPosition.x = Mathf.Clamp(windowPosition.x, 0f, (float)UI.screenWidth - val.x);
		windowPosition.y = Mathf.Clamp(windowPosition.y, 0f, (float)UI.screenHeight - val.y);
		windowRect = new Rect(windowPosition.x, windowPosition.y, val.x, val.y);
		windowRect = windowRect.Rounded();
		Text.Font = font;
	}

	private string PrettifyNodeName(DebugActionNode node)
	{
		string path = node.Path;
		if (nameCache.TryGetValue(path, out var value))
		{
			return value;
		}
		DebugActionNode debugActionNode = node;
		value = debugActionNode.LabelNow.Replace("...", "");
		while (debugActionNode.parent != null && !debugActionNode.parent.IsRoot && (debugActionNode.parent.parent == null || !debugActionNode.parent.parent.IsRoot))
		{
			value = debugActionNode.parent.LabelNow.Replace("...", "") + "\\" + value;
			debugActionNode = debugActionNode.parent;
		}
		nameCache[path] = value;
		return value;
	}

	public override void PostClose()
	{
		base.PostOpen();
		DebugSettings.devPalette = false;
	}
}
