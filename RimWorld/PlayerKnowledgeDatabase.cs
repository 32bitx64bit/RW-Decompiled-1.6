using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class PlayerKnowledgeDatabase
{
	private class ConceptKnowledge
	{
		public Dictionary<string, float> knowledge = new Dictionary<string, float>();

		public ConceptKnowledge()
		{
			foreach (ConceptDef allDef in DefDatabase<ConceptDef>.AllDefs)
			{
				knowledge.Add(allDef.defName, 0f);
			}
		}
	}

	private static ConceptKnowledge data;

	static PlayerKnowledgeDatabase()
	{
		ReloadAndRebind();
	}

	public static void ReloadAndRebind()
	{
		data = DirectXmlLoader.ItemFromXmlFile<ConceptKnowledge>(GenFilePaths.ConceptKnowledgeFilePath);
		foreach (ConceptDef allDef in DefDatabase<ConceptDef>.AllDefs)
		{
			if (!data.knowledge.ContainsKey(allDef.defName))
			{
				if (allDef != ConceptDefOf.SteamDeckControlsGame && allDef != ConceptDefOf.SteamDeckControlsMainMenu)
				{
					Log.Warning("Knowledge data was missing key " + allDef?.ToString() + ". Adding it...");
				}
				data.knowledge.Add(allDef.defName, 0f);
			}
		}
	}

	public static void ResetPersistent()
	{
		FileInfo fileInfo = new FileInfo(GenFilePaths.ConceptKnowledgeFilePath);
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
		}
		data = new ConceptKnowledge();
	}

	public static void Save()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			XDocument val = new XDocument();
			XElement val2 = DirectXmlSaver.XElementFromObject(data, typeof(ConceptKnowledge));
			((XContainer)val).Add((object)val2);
			val.Save(GenFilePaths.ConceptKnowledgeFilePath);
		}
		catch (Exception ex)
		{
			GenUI.ErrorDialog("ProblemSavingFile".Translate(GenFilePaths.ConceptKnowledgeFilePath, ex.ToString()));
			Log.Error("Exception saving knowledge database: " + ex);
		}
	}

	public static float GetKnowledge(ConceptDef def)
	{
		return data.knowledge[def.defName];
	}

	public static void SetKnowledge(ConceptDef def, float value)
	{
		float num = data.knowledge[def.defName];
		float num2 = Mathf.Clamp01(value);
		data.knowledge[def.defName] = num2;
		if (num < 0.999f && num2 >= 0.999f)
		{
			NewlyLearned(def);
		}
	}

	public static bool IsComplete(ConceptDef conc)
	{
		return data.knowledge[conc.defName] > 0.999f;
	}

	private static void NewlyLearned(ConceptDef conc)
	{
		TutorSystem.Notify_Event("ConceptLearned-" + conc.defName);
		if (Find.Tutor != null)
		{
			Find.Tutor.learningReadout.Notify_ConceptNewlyLearned(conc);
		}
	}

	public static void KnowledgeDemonstrated(ConceptDef conc, KnowledgeAmount know)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		float num = know switch
		{
			KnowledgeAmount.FrameDisplayed => ((int)Event.current.type == 7) ? 0.004f : 0f, 
			KnowledgeAmount.FrameInteraction => 0.008f, 
			KnowledgeAmount.TinyInteraction => 0.03f, 
			KnowledgeAmount.SmallInteraction => 0.1f, 
			KnowledgeAmount.SpecificInteraction => 0.4f, 
			KnowledgeAmount.Total => 1f, 
			KnowledgeAmount.NoteClosed => 0.5f, 
			KnowledgeAmount.NoteTaught => 1f, 
			_ => throw new NotImplementedException(), 
		};
		if (!(num <= 0f))
		{
			SetKnowledge(conc, GetKnowledge(conc) + num);
			LessonAutoActivator.Notify_KnowledgeDemonstrated(conc);
			if (Find.ActiveLesson != null)
			{
				Find.ActiveLesson.Notify_KnowledgeDemonstrated(conc);
			}
		}
	}
}
