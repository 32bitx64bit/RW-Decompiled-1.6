using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using RimWorld;
using RimWorld.IO;

namespace Verse;

public static class TranslationFilesCleaner
{
	private class PossibleDefInjection
	{
		public string suggestedPath;

		public string normalizedPath;

		public bool isCollection;

		public bool fullListTranslationAllowed;

		public string curValue;

		public IEnumerable<string> curValueCollection;

		public FieldInfo fieldInfo;

		public Def def;
	}

	private const string NewlineTag = "NEWLINE";

	private const string NewlineTagFull = "<!--NEWLINE-->";

	public static void CleanupTranslationFiles()
	{
		LoadedLanguage curLang = LanguageDatabase.activeLanguage;
		IEnumerable<ModMetaData> activeModsInLoadOrder = ModsConfig.ActiveModsInLoadOrder;
		if (!activeModsInLoadOrder.Any((ModMetaData x) => x.IsCoreMod) || activeModsInLoadOrder.Any((ModMetaData x) => !x.Official))
		{
			Messages.Message("MessageDisableModsBeforeCleaningTranslationFiles".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return;
		}
		if (LanguageDatabase.activeLanguage.AllDirectories.Any((Tuple<VirtualDirectory, ModContentPack, string> x) => x.Item1 is TarDirectory))
		{
			Messages.Message("MessageUnpackBeforeCleaningTranslationFiles".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return;
		}
		LongEventHandler.QueueLongEvent(delegate
		{
			if (curLang.anyKeyedReplacementsXmlParseError || curLang.anyDefInjectionsXmlParseError)
			{
				string text = curLang.lastKeyedReplacementsXmlParseErrorInFile ?? curLang.lastDefInjectionsXmlParseErrorInFile;
				Messages.Message("MessageCantCleanupTranslationFilesBeucaseOfXmlError".Translate(text), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				curLang.LoadData();
				Dialog_MessageBox dialog_MessageBox = Dialog_MessageBox.CreateConfirmation("ConfirmCleanupTranslationFiles".Translate(curLang.FriendlyNameNative), delegate
				{
					LongEventHandler.QueueLongEvent(DoCleanupTranslationFiles, "CleaningTranslationFiles".Translate(), doAsynchronously: true, null);
				}, destructive: true);
				dialog_MessageBox.buttonAText = "ConfirmCleanupTranslationFiles_Confirm".Translate();
				Find.WindowStack.Add(dialog_MessageBox);
			}
		}, null, doAsynchronously: false, null);
	}

	private static void DoCleanupTranslationFiles()
	{
		try
		{
			try
			{
				CleanupKeyedTranslations();
			}
			catch (Exception ex)
			{
				Log.Error("Could not cleanup keyed translations: " + ex);
			}
			try
			{
				CleanupDefInjections();
			}
			catch (Exception ex2)
			{
				Log.Error("Could not cleanup def-injections: " + ex2);
			}
			try
			{
				CleanupBackstories();
			}
			catch (Exception ex3)
			{
				Log.Error("Could not cleanup backstories: " + ex3);
			}
			string text = string.Join("\n", ModsConfig.ActiveModsInLoadOrder.Select((ModMetaData x) => GetLanguageFolderPath(LanguageDatabase.activeLanguage, x.RootDir.FullName)).ToArray());
			Messages.Message("MessageTranslationFilesCleanupDone".Translate(text), MessageTypeDefOf.TaskCompletion, historical: false);
		}
		catch (Exception ex4)
		{
			Log.Error("Could not cleanup translation files: " + ex4);
		}
	}

	private static void CleanupKeyedTranslations()
	{
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Expected O, but got Unknown
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Expected O, but got Unknown
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Expected O, but got Unknown
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Expected O, but got Unknown
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Expected O, but got Unknown
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Expected O, but got Unknown
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Expected O, but got Unknown
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Expected O, but got Unknown
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Expected O, but got Unknown
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Expected O, but got Unknown
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Expected O, but got Unknown
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Expected O, but got Unknown
		LoadedLanguage activeLanguage = LanguageDatabase.activeLanguage;
		LoadedLanguage english = LanguageDatabase.defaultLanguage;
		List<LoadedLanguage.KeyedReplacement> list = (from x in activeLanguage.keyedReplacements
			where !x.Value.isPlaceholder && !english.HaveTextForKey(x.Key)
			select x.Value).ToList();
		HashSet<LoadedLanguage.KeyedReplacement> writtenUnusedKeyedTranslations = new HashSet<LoadedLanguage.KeyedReplacement>();
		foreach (ModMetaData item in ModsConfig.ActiveModsInLoadOrder)
		{
			string languageFolderPath = GetLanguageFolderPath(activeLanguage, item.RootDir.FullName);
			string text = Path.Combine(languageFolderPath, "CodeLinked");
			string text2 = Path.Combine(languageFolderPath, "Keyed");
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (directoryInfo.Exists)
			{
				if (!Directory.Exists(text2))
				{
					Directory.Move(text, text2);
					Thread.Sleep(1000);
					directoryInfo = new DirectoryInfo(text2);
				}
			}
			else
			{
				directoryInfo = new DirectoryInfo(text2);
			}
			DirectoryInfo directoryInfo2 = new DirectoryInfo(Path.Combine(GetLanguageFolderPath(english, item.RootDir.FullName), "Keyed"));
			if (!directoryInfo2.Exists)
			{
				if (item.IsCoreMod)
				{
					Log.Error("English keyed translations folder doesn't exist.");
				}
				if (!directoryInfo.Exists)
				{
					continue;
				}
			}
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			FileInfo[] files;
			if (activeLanguage != english)
			{
				files = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
				foreach (FileInfo fileInfo in files)
				{
					try
					{
						fileInfo.Delete();
					}
					catch (Exception ex)
					{
						Log.Error("Could not delete " + fileInfo.Name + ": " + ex);
					}
				}
				files = directoryInfo2.GetFiles("*.xml", SearchOption.AllDirectories);
				foreach (FileInfo fileInfo2 in files)
				{
					try
					{
						string fullName = directoryInfo2.FullName;
						char directorySeparatorChar = Path.DirectorySeparatorChar;
						string path = new Uri(fullName + directorySeparatorChar).MakeRelativeUri(new Uri(fileInfo2.FullName)).ToString();
						string text3 = Path.Combine(directoryInfo.FullName, path);
						Directory.CreateDirectory(Path.GetDirectoryName(text3));
						fileInfo2.CopyTo(text3);
					}
					catch (Exception ex2)
					{
						Log.Error("Could not copy " + fileInfo2.Name + ": " + ex2);
					}
				}
			}
			files = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo3 in files)
			{
				try
				{
					XDocument val = XDocument.Load(fileInfo3.FullName, (LoadOptions)1);
					XElement val2 = ((XContainer)val).DescendantNodes().OfType<XElement>().FirstOrDefault();
					if (val2 == null)
					{
						continue;
					}
					try
					{
						XNode[] array = ((XContainer)val2).DescendantNodes().ToArray();
						foreach (XNode val3 in array)
						{
							XElement val4 = (XElement)(object)((val3 is XElement) ? val3 : null);
							if (val4 == null)
							{
								continue;
							}
							XNode[] array2 = ((XContainer)val4).DescendantNodes().ToArray();
							foreach (XNode val5 in array2)
							{
								try
								{
									XText val6 = (XText)(object)((val5 is XText) ? val5 : null);
									if (val6 != null && !val6.Value.NullOrEmpty())
									{
										string comment = " EN: " + val6.Value + " ";
										val3.AddBeforeSelf((object)new XComment(SanitizeXComment(comment)));
										val3.AddBeforeSelf((object)Environment.NewLine);
										val3.AddBeforeSelf((object)"  ");
									}
								}
								catch (Exception ex3)
								{
									Log.Error("Could not add comment node in " + fileInfo3.Name + ": " + ex3);
								}
								val5.Remove();
							}
							try
							{
								if (activeLanguage.TryGetTextFromKey(((object)val4.Name).ToString(), out var translated))
								{
									if (!translated.NullOrEmpty())
									{
										((XContainer)val4).Add((object)new XText(translated.Replace("\n", "\\n").RawText));
									}
								}
								else
								{
									((XContainer)val4).Add((object)new XText("TODO"));
								}
							}
							catch (Exception ex4)
							{
								Log.Error("Could not add existing translation or placeholder in " + fileInfo3.Name + ": " + ex4);
							}
						}
						bool flag = false;
						foreach (LoadedLanguage.KeyedReplacement item2 in list)
						{
							if (new Uri(fileInfo3.FullName).Equals(new Uri(item2.fileSourceFullPath)))
							{
								if (!flag)
								{
									((XContainer)val2).Add((object)"  ");
									((XContainer)val2).Add((object)new XComment(" UNUSED "));
									((XContainer)val2).Add((object)Environment.NewLine);
									flag = true;
								}
								XElement val7 = new XElement(XName.op_Implicit(item2.key));
								if (item2.isPlaceholder)
								{
									((XContainer)val7).Add((object)new XText("TODO"));
								}
								else if (!item2.value.NullOrEmpty())
								{
									((XContainer)val7).Add((object)new XText(item2.value.Replace("\n", "\\n")));
								}
								((XContainer)val2).Add((object)"  ");
								((XContainer)val2).Add((object)val7);
								((XContainer)val2).Add((object)Environment.NewLine);
								writtenUnusedKeyedTranslations.Add(item2);
							}
						}
						if (flag)
						{
							((XContainer)val2).Add((object)Environment.NewLine);
						}
					}
					finally
					{
						SaveXMLDocumentWithProcessedNewlineTags((XNode)(object)val.Root, fileInfo3.FullName);
					}
				}
				catch (Exception ex5)
				{
					Log.Error("Could not process " + fileInfo3.Name + ": " + ex5);
				}
			}
		}
		foreach (IGrouping<string, LoadedLanguage.KeyedReplacement> item3 in from x in list
			where !writtenUnusedKeyedTranslations.Contains(x)
			group x by x.fileSourceFullPath)
		{
			try
			{
				if (File.Exists(item3.Key))
				{
					Log.Error("Could not save unused keyed translations to " + item3.Key + " because this file already exists.");
					continue;
				}
				SaveXMLDocumentWithProcessedNewlineTags((XNode)new XDocument(new object[1] { (object)new XElement(XName.op_Implicit("LanguageData"), new object[4]
				{
					(object)new XComment("NEWLINE"),
					(object)new XComment(" UNUSED "),
					((IEnumerable<LoadedLanguage.KeyedReplacement>)item3).Select((Func<LoadedLanguage.KeyedReplacement, XElement>)delegate(LoadedLanguage.KeyedReplacement x)
					{
						//IL_0040: Unknown result type (might be due to invalid IL or missing references)
						//IL_004a: Expected O, but got Unknown
						//IL_0045: Unknown result type (might be due to invalid IL or missing references)
						//IL_004b: Expected O, but got Unknown
						string text4 = (x.isPlaceholder ? "TODO" : x.value);
						return new XElement(XName.op_Implicit(x.key), (object)new XText(text4.NullOrEmpty() ? "" : text4.Replace("\n", "\\n")));
					}),
					(object)new XComment("NEWLINE")
				}) }), item3.Key);
			}
			catch (Exception ex6)
			{
				Log.Error("Could not save unused keyed translations to " + item3.Key + ": " + ex6);
			}
		}
	}

	private static void CleanupDefInjections()
	{
		foreach (ModMetaData item in ModsConfig.ActiveModsInLoadOrder)
		{
			string languageFolderPath = GetLanguageFolderPath(LanguageDatabase.activeLanguage, item.RootDir.FullName);
			string text = Path.Combine(languageFolderPath, "DefLinked");
			string text2 = Path.Combine(languageFolderPath, "DefInjected");
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (directoryInfo.Exists)
			{
				if (!Directory.Exists(text2))
				{
					Directory.Move(text, text2);
					Thread.Sleep(1000);
					directoryInfo = new DirectoryInfo(text2);
				}
			}
			else
			{
				directoryInfo = new DirectoryInfo(text2);
			}
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			FileInfo[] files = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				try
				{
					fileInfo.Delete();
				}
				catch (Exception ex)
				{
					Log.Error("Could not delete " + fileInfo.Name + ": " + ex);
				}
			}
			foreach (Type item2 in GenDefDatabase.AllDefTypesWithDatabases())
			{
				try
				{
					CleanupDefInjectionsForDefType(item2, directoryInfo.FullName, item);
				}
				catch (Exception ex2)
				{
					Log.Error("Could not process def-injections for type " + item2.Name + ": " + ex2);
				}
			}
		}
	}

	private static void CleanupDefInjectionsForDefType(Type defType, string defInjectionsFolderPath, ModMetaData mod)
	{
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Expected O, but got Unknown
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Expected O, but got Unknown
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Expected O, but got Unknown
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected O, but got Unknown
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Expected O, but got Unknown
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Expected O, but got Unknown
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Expected O, but got Unknown
		LoadedLanguage activeLanguage = LanguageDatabase.activeLanguage;
		List<KeyValuePair<string, DefInjectionPackage.DefInjection>> list = (from x in activeLanguage.defInjections.Where((DefInjectionPackage x) => x.defType == defType).SelectMany((DefInjectionPackage x) => x.injections)
			where !x.Value.isPlaceholder && x.Value.ModifiesDefFromModOrNullCore(mod, defType)
			select x).ToList();
		Dictionary<string, DefInjectionPackage.DefInjection> dictionary = new Dictionary<string, DefInjectionPackage.DefInjection>();
		foreach (KeyValuePair<string, DefInjectionPackage.DefInjection> item2 in list)
		{
			if (!dictionary.ContainsKey(item2.Value.normalizedPath))
			{
				dictionary.Add(item2.Value.normalizedPath, item2.Value);
			}
		}
		if (defType == typeof(BackstoryDef))
		{
			foreach (DefInjectionPackage.DefInjection legacyBackstoryTranslation in BackstoryTranslationUtility.GetLegacyBackstoryTranslations(activeLanguage.AllDirectories))
			{
				if (!dictionary.ContainsKey(legacyBackstoryTranslation.path))
				{
					dictionary.Add(legacyBackstoryTranslation.path, legacyBackstoryTranslation);
				}
			}
		}
		List<PossibleDefInjection> possibleDefInjections = new List<PossibleDefInjection>();
		DefInjectionUtility.ForEachPossibleDefInjection(defType, delegate(string suggestedPath, string normalizedPath, bool isCollection, string str, IEnumerable<string> collection, bool translationAllowed, bool fullListTranslationAllowed, FieldInfo fieldInfo, Def def)
		{
			if (translationAllowed)
			{
				PossibleDefInjection item = new PossibleDefInjection
				{
					suggestedPath = suggestedPath,
					normalizedPath = normalizedPath,
					isCollection = isCollection,
					fullListTranslationAllowed = fullListTranslationAllowed,
					curValue = str,
					curValueCollection = collection,
					fieldInfo = fieldInfo,
					def = def
				};
				possibleDefInjections.Add(item);
			}
		}, mod);
		if (!possibleDefInjections.Any() && !list.Any())
		{
			return;
		}
		List<KeyValuePair<string, DefInjectionPackage.DefInjection>> source = list.Where((KeyValuePair<string, DefInjectionPackage.DefInjection> x) => !x.Value.injected).ToList();
		foreach (string fileName in possibleDefInjections.Select((PossibleDefInjection x) => GetSourceFile(x.def)).Concat(source.Select((KeyValuePair<string, DefInjectionPackage.DefInjection> x) => x.Value.fileSource)).Distinct())
		{
			try
			{
				XDocument val = new XDocument();
				bool flag = false;
				try
				{
					XElement val2 = new XElement(XName.op_Implicit("LanguageData"));
					((XContainer)val).Add((object)val2);
					((XContainer)val2).Add((object)new XComment("NEWLINE"));
					List<PossibleDefInjection> source2 = possibleDefInjections.Where((PossibleDefInjection x) => GetSourceFile(x.def) == fileName).ToList();
					List<KeyValuePair<string, DefInjectionPackage.DefInjection>> source3 = source.Where((KeyValuePair<string, DefInjectionPackage.DefInjection> x) => x.Value.fileSource == fileName).ToList();
					foreach (string defName in from x in source2.Select((PossibleDefInjection x) => x.def.defName).Concat(source3.Select((KeyValuePair<string, DefInjectionPackage.DefInjection> x) => x.Value.DefName)).Distinct()
						orderby x
						select x)
					{
						try
						{
							IEnumerable<PossibleDefInjection> enumerable = source2.Where((PossibleDefInjection x) => x.def.defName == defName);
							IEnumerable<KeyValuePair<string, DefInjectionPackage.DefInjection>> enumerable2 = source3.Where((KeyValuePair<string, DefInjectionPackage.DefInjection> x) => x.Value.DefName == defName);
							if (enumerable.Any())
							{
								bool flag2 = false;
								foreach (PossibleDefInjection item3 in enumerable)
								{
									if (item3.isCollection)
									{
										IEnumerable<string> englishList = GetEnglishList(item3.normalizedPath, item3.curValueCollection, dictionary);
										bool flag3 = false;
										if (englishList != null)
										{
											int num = 0;
											foreach (string item4 in englishList)
											{
												_ = item4;
												if (dictionary.ContainsKey(item3.normalizedPath + "." + num))
												{
													flag3 = true;
													break;
												}
												num++;
											}
										}
										if (flag3 || !item3.fullListTranslationAllowed)
										{
											if (englishList == null)
											{
												continue;
											}
											int num2 = -1;
											foreach (string item5 in englishList)
											{
												num2++;
												string text = item3.normalizedPath + "." + num2;
												string text2 = item3.suggestedPath + "." + num2;
												if (TKeySystem.TrySuggestTKeyPath(text, out var tKeyPath))
												{
													text2 = tKeyPath;
												}
												if (!dictionary.TryGetValue(text, out var value))
												{
													value = null;
												}
												if (value == null && !DefInjectionUtility.ShouldCheckMissingInjection(item5, item3.fieldInfo, item3.def))
												{
													continue;
												}
												flag2 = true;
												flag = true;
												try
												{
													if (!item5.NullOrEmpty())
													{
														((XContainer)val2).Add((object)new XComment(SanitizeXComment(" EN: " + item5.Replace("\n", "\\n") + " ")));
													}
												}
												catch (Exception ex)
												{
													Log.Error("Could not add comment node in " + fileName + ": " + ex);
												}
												if (LanguageDatabase.activeLanguage == LanguageDatabase.defaultLanguage)
												{
													((XContainer)val2).Add((object)new XElement(XName.op_Implicit(text2), (object)item5));
												}
												else
												{
													((XContainer)val2).Add((object)GetDefInjectableFieldNode(text2, value));
												}
											}
											continue;
										}
										bool flag4 = false;
										if (englishList != null)
										{
											foreach (string item6 in englishList)
											{
												if (DefInjectionUtility.ShouldCheckMissingInjection(item6, item3.fieldInfo, item3.def))
												{
													flag4 = true;
													break;
												}
											}
										}
										if (!dictionary.TryGetValue(item3.normalizedPath, out var value2))
										{
											value2 = null;
										}
										if (value2 == null && !flag4)
										{
											continue;
										}
										flag2 = true;
										flag = true;
										try
										{
											string text3 = ListToLiNodesString(englishList);
											if (!text3.NullOrEmpty())
											{
												((XContainer)val2).Add((object)new XComment(SanitizeXComment(" EN:\n" + text3.Indented() + "\n  ")));
											}
										}
										catch (Exception ex2)
										{
											Log.Error("Could not add comment node in " + fileName + ": " + ex2);
										}
										if (LanguageDatabase.activeLanguage == LanguageDatabase.defaultLanguage)
										{
											((XContainer)val2).Add((object)ListToXElement(englishList, item3.suggestedPath, value2?.fullListInjectionComments));
										}
										else
										{
											((XContainer)val2).Add((object)GetDefInjectableFieldNode(item3.suggestedPath, value2));
										}
										continue;
									}
									if (!dictionary.TryGetValue(item3.normalizedPath, out var value3))
									{
										value3 = null;
									}
									string text4 = ((value3 != null && value3.injected) ? value3.replacedString : item3.curValue);
									if (value3 == null && !DefInjectionUtility.ShouldCheckMissingInjection(text4, item3.fieldInfo, item3.def))
									{
										continue;
									}
									flag2 = true;
									flag = true;
									try
									{
										if (!text4.NullOrEmpty())
										{
											((XContainer)val2).Add((object)new XComment(SanitizeXComment(" EN: " + text4.Replace("\n", "\\n") + " ")));
										}
									}
									catch (Exception ex3)
									{
										Log.Error("Could not add comment node in " + fileName + ": " + ex3);
									}
									if (LanguageDatabase.activeLanguage == LanguageDatabase.defaultLanguage)
									{
										((XContainer)val2).Add((object)new XElement(XName.op_Implicit(item3.suggestedPath), (object)text4));
									}
									else
									{
										((XContainer)val2).Add((object)GetDefInjectableFieldNode(item3.suggestedPath, value3));
									}
								}
								if (flag2)
								{
									((XContainer)val2).Add((object)new XComment("NEWLINE"));
								}
							}
							if (!enumerable2.Any())
							{
								continue;
							}
							flag = true;
							((XContainer)val2).Add((object)new XComment(" UNUSED "));
							foreach (KeyValuePair<string, DefInjectionPackage.DefInjection> item7 in enumerable2)
							{
								((XContainer)val2).Add((object)GetDefInjectableFieldNode(item7.Value.path, item7.Value));
							}
							((XContainer)val2).Add((object)new XComment("NEWLINE"));
						}
						catch (Exception ex4)
						{
							Log.Error("Could not process def-injections for def " + defName + ": " + ex4);
						}
					}
				}
				finally
				{
					if (flag)
					{
						string text5 = Path.Combine(defInjectionsFolderPath, defType.Name);
						Directory.CreateDirectory(text5);
						SaveXMLDocumentWithProcessedNewlineTags((XNode)(object)val, Path.Combine(text5, fileName));
					}
				}
			}
			catch (Exception ex5)
			{
				Log.Error("Could not process def-injections for file " + fileName + ": " + ex5);
			}
		}
	}

	private static void CleanupBackstories()
	{
		string activeLanguageCoreModFolderPath = GetActiveLanguageCoreModFolderPath();
		string text = Path.Combine(activeLanguageCoreModFolderPath, "Backstories");
		if (Directory.Exists(text))
		{
			string text2 = Path.Combine(activeLanguageCoreModFolderPath, "Backstories DELETE_ME");
			Directory.Move(text, text2);
			string text3 = Path.Combine(text, "Backstories.xml");
			string text4 = Path.Combine(text2, "Backstories.xml");
			Find.WindowStack.Add(new Dialog_MessageBox("RestartAfterImportingLegacyBackstoryTanslations".Translate(text3, text4), null, GenCommandLine.Restart));
		}
	}

	private static string GetActiveLanguageCoreModFolderPath()
	{
		ModContentPack modContentPack = LoadedModManager.RunningMods.FirstOrDefault((ModContentPack x) => x.IsCoreMod);
		return GetLanguageFolderPath(LanguageDatabase.activeLanguage, modContentPack.RootDir);
	}

	public static string GetLanguageFolderPath(LoadedLanguage language, string modRootDir)
	{
		return Path.Combine(Path.Combine(modRootDir, "Languages"), language.folderName);
	}

	private static void SaveXMLDocumentWithProcessedNewlineTags(XNode doc, string path)
	{
		File.WriteAllText(path, "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" + ((object)doc).ToString().Replace("<!--NEWLINE-->", "").Replace("&gt;", ">"), Encoding.UTF8);
	}

	private static string ListToLiNodesString(IEnumerable<string> list)
	{
		if (list == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in list)
		{
			stringBuilder.Append("<li>");
			if (!item.NullOrEmpty())
			{
				stringBuilder.Append(item.Replace("\n", "\\n"));
			}
			stringBuilder.Append("</li>");
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString().TrimEndNewlines();
	}

	private static XElement ListToXElement(IEnumerable<string> list, string name, List<Pair<int, string>> comments)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		XElement val = new XElement(XName.op_Implicit(name));
		if (list != null)
		{
			int num = 0;
			foreach (string item in list)
			{
				if (comments != null)
				{
					for (int i = 0; i < comments.Count; i++)
					{
						if (comments[i].First == num)
						{
							((XContainer)val).Add((object)new XComment(comments[i].Second));
						}
					}
				}
				XElement val2 = new XElement(XName.op_Implicit("li"));
				if (!item.NullOrEmpty())
				{
					((XContainer)val2).Add((object)new XText(item.Replace("\n", "\\n")));
				}
				((XContainer)val).Add((object)val2);
				num++;
			}
			if (comments != null)
			{
				for (int j = 0; j < comments.Count; j++)
				{
					if (comments[j].First == num)
					{
						((XContainer)val).Add((object)new XComment(comments[j].Second));
					}
				}
			}
		}
		return val;
	}

	private static string AppendXmlExtensionIfNotAlready(string fileName)
	{
		if (!fileName.ToLower().EndsWith(".xml"))
		{
			return fileName + ".xml";
		}
		return fileName;
	}

	private static string GetSourceFile(Def def)
	{
		if (!def.fileName.NullOrEmpty())
		{
			return AppendXmlExtensionIfNotAlready(def.fileName);
		}
		return "Unknown.xml";
	}

	private static IEnumerable<string> GetEnglishList(string normalizedPath, IEnumerable<string> curValue, Dictionary<string, DefInjectionPackage.DefInjection> injectionsByNormalizedPath)
	{
		if (injectionsByNormalizedPath.TryGetValue(normalizedPath, out var value) && value.injected)
		{
			return value.replacedList;
		}
		if (curValue == null)
		{
			return null;
		}
		List<string> list = curValue.ToList();
		if (LanguageDatabase.activeLanguage == LanguageDatabase.defaultLanguage)
		{
			return list;
		}
		for (int i = 0; i < list.Count; i++)
		{
			string key = normalizedPath + "." + i;
			if (injectionsByNormalizedPath.TryGetValue(key, out var value2) && value2.injected)
			{
				list[i] = value2.replacedString;
			}
		}
		return list;
	}

	private static XElement GetDefInjectableFieldNode(string suggestedPath, DefInjectionPackage.DefInjection existingInjection)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		if (existingInjection == null || existingInjection.isPlaceholder)
		{
			return new XElement(XName.op_Implicit(suggestedPath), (object)new XText("TODO"));
		}
		if (existingInjection.IsFullListInjection)
		{
			return ListToXElement(existingInjection.fullListInjection, suggestedPath, existingInjection.fullListInjectionComments);
		}
		XElement val;
		if (!existingInjection.injection.NullOrEmpty())
		{
			if (existingInjection.suggestedPath.EndsWith(".slateRef") && ConvertHelper.IsXml(existingInjection.injection))
			{
				try
				{
					val = XElement.Parse("<" + suggestedPath + ">" + existingInjection.injection + "</" + suggestedPath + ">");
				}
				catch (Exception ex)
				{
					Log.Warning("Could not parse XML: " + existingInjection.injection + ". Exception: " + ex);
					val = new XElement(XName.op_Implicit(suggestedPath));
					((XContainer)val).Add((object)existingInjection.injection);
				}
			}
			else
			{
				val = new XElement(XName.op_Implicit(suggestedPath));
				((XContainer)val).Add((object)new XText(existingInjection.injection.Replace("\n", "\\n")));
			}
		}
		else
		{
			val = new XElement(XName.op_Implicit(suggestedPath));
		}
		return val;
	}

	private static string SanitizeXComment(string comment)
	{
		while (comment.Contains("-----"))
		{
			comment = comment.Replace("-----", "- - -");
		}
		while (comment.Contains("--"))
		{
			comment = comment.Replace("--", "- -");
		}
		return comment;
	}
}
