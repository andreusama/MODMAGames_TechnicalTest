using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using System.Linq;
using TMPro;

namespace PetoonsStudio.PSEngine.Credits
{
    [ExecuteInEditMode]
    public class CreditsImporter : MonoBehaviour
    {
#if UNITY_EDITOR

        protected enum DialogResult { CONTINUE, STOP, CONTINUE_ALL };

        [Header("ASSIGNABLE THINGS")]
        [Tooltip("Transform where all objects will be instantiated.")]
        public Transform CanvasTransform;
        [Tooltip("Path of all the json that forms the credits.")]
        public string[] JsonCreditsPath;
        [Tooltip("Scroller of the credits.")]
        public CreditsView creditsScroller;

        [Header("PREFABS")]
        public GameObject ChapterPrefab;
        public GameObject PagePrefab;
        public GameObject SectionPrefab;
        public GameObject SectionColumnsPrefab;
        public GameObject MentionPrefab;
        public GameObject ColumnPrefab;

        [Header("IMPORT OPTIONS")]
        public bool DisableJobsAndNamesWhenEmpty = true;
        public bool CenterNameWhenJobEmpty = true;


        [Header("DEBUG VARIABLES")]
        [SerializeField]
        protected Chapter[] m_CreditsChapter;
        [SerializeField]
        protected List<CreditsChapterUI> m_CreditsChapterUI = new List<CreditsChapterUI>();
        protected bool m_DisplayDialogs = true;

        [ContextMenu("Import Credits")]
        public virtual void ImportCredits()
        {
            ImportCreditsInternal();
        }

        protected virtual void ImportCreditsInternal()
        {
            m_CreditsChapter = new Chapter[JsonCreditsPath.Length];
            ImportJsons();

            //Check if we need to remove objects
            if (m_CreditsChapter.Length < m_CreditsChapterUI.Count)
            {
                int index = m_CreditsChapter.Length - 1;
                int count = m_CreditsChapterUI.Count - m_CreditsChapter.Length - 1;

                for (int i = m_CreditsChapterUI.Count - 1; i > m_CreditsChapter.Length - 1; i--)
                {
                    GameObject.DestroyImmediate(m_CreditsChapterUI[i].gameObject);
                    m_CreditsChapterUI.RemoveAt(i);
                }
            }

            DialogResult dialogResult = DialogResult.CONTINUE;

            //Update all already instancied credits
            for (int i = 0; i < m_CreditsChapterUI.Count; i++)
            {
                dialogResult = UpdateChapter(m_CreditsChapter[i], m_CreditsChapterUI[i], GetCreditsName(JsonCreditsPath[i]), ref dialogResult);
                if (!CheckDialogResult(ref dialogResult, () => { }))
                {
                    break;
                }
            }

            if (dialogResult != DialogResult.STOP)
            {
                for (int i = m_CreditsChapterUI.Count; i < JsonCreditsPath.Length; i++)
                {
                    InstantiateCredits(m_CreditsChapter[i], CanvasTransform, GetCreditsName(JsonCreditsPath[i]));
                }

                m_CreditsChapterUI[0].gameObject.SetActive(true);
                m_CreditsChapterUI[0].Pages[0].gameObject.SetActive(true);

                creditsScroller.UpdateChapters(m_CreditsChapterUI.ToArray());
            }
        }

        [ContextMenu("Clear Credits")]
        public virtual void ClearCredits()
        {
            for (int i = 0; i < m_CreditsChapterUI.Count; i++)
            {
                GameObject.DestroyImmediate(m_CreditsChapterUI[i].gameObject);
            }
            m_CreditsChapterUI.Clear();
            m_CreditsChapter = new Chapter[0];
            creditsScroller.ClearChapters();
        }

        protected void ImportJsons()
        {
            string json;
            for (int i = 0; i < JsonCreditsPath.Length; i++)
            {
                using (StreamReader stream = new StreamReader(JsonCreditsPath[i]))
                {
                    json = stream.ReadToEnd();
                    m_CreditsChapter[i] = JsonUtility.FromJson<Chapter>(json);
                }
            }
        }

        protected string GetCreditsName(string jsonPath)
        {
            char[] separator = { '/' };
            string[] paths = jsonPath.Split(separator);
            separator[0] = '.';
            paths = paths[paths.Length - 1].Split(separator);
            string creditsName = paths[0];
            return creditsName;
        }

        protected DialogResult UpdateChapter(Chapter chapter, CreditsChapterUI chapterUI, string creditsName, ref DialogResult dialogResult)
        {
            if (chapterUI.gameObject.name != creditsName)
            {
                //Dialog box
                dialogResult =
                (DialogResult)EditorUtility.DisplayDialogComplex("Credits Importer", $" {chapterUI.gameObject.name} will change it name to {creditsName}.", "Continue", "Stop", "ContinueAll");
                if (dialogResult == DialogResult.STOP) return dialogResult;

                chapterUI.gameObject.name = creditsName == string.Empty ? chapterUI.gameObject.name : creditsName;
            }

            RemoveUnusedDialog(creditsName, chapter.Pages.Length, chapterUI.Pages.Count, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => RemoveUnused(chapterUI.Pages.Cast<Component>().ToList(), chapter.Pages.Length)))
            {
                return dialogResult;
            }

            for (int i = 0; i < chapterUI.Pages.Count; i++)
            {
                dialogResult = UpdatePage(chapter.Pages[i], chapterUI.Pages[i], ref dialogResult);
                if (!CheckDialogResult(ref dialogResult, () => { }))
                {
                    return dialogResult;
                }
            }

            return dialogResult;
        }

        protected DialogResult UpdatePage(Page page, CreditsPageUI pageUI, ref DialogResult dialogResult)
        {
            UpdateTextDialog(pageUI.Title.text, page.Title, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => UpdateText(pageUI.Title.gameObject, pageUI.Title, page.Title)))
            {
                return dialogResult;
            }

            RemoveUnusedDialog(pageUI.Title.name, page.Sections.Length, pageUI.Sections.Count, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => RemoveUnused(pageUI.Sections.Cast<Component>().ToList(), page.Sections.Length)))
            {
                return dialogResult;
            }

            for (int i = 0; i < pageUI.Sections.Count; i++)
            {
                dialogResult = (DialogResult)UpdateSection(page.Sections[i], pageUI.Sections[i], ref dialogResult);
                if (!CheckDialogResult(ref dialogResult, () => { }))
                {
                    return dialogResult;
                }
            }

            pageUI.gameObject.SetActive(false);
            return dialogResult;
        }

        protected DialogResult UpdateSection(Section section, CreditsSectionUI sectionUI, ref DialogResult dialogResult)
        {
            UpdateTextDialog(sectionUI.Title.text, section.Title, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => UpdateText(sectionUI.Title.gameObject, sectionUI.Title, section.Title)))
            {
                return dialogResult;
            }

            RemoveUnusedDialog(sectionUI.Title.name, section.Mentions.Length, sectionUI.Mentions.Count, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => RemoveUnused(sectionUI.Mentions.Cast<Component>().ToList(), section.Mentions.Length)))
            {
                return dialogResult;
            }


            for (int i = 0; i < sectionUI.Mentions.Count; i++)
            {
                dialogResult = UpdateMention(section.Mentions[i], sectionUI.Mentions[i], ref dialogResult);
                if (!CheckDialogResult(ref dialogResult, () => { }))
                {
                    return dialogResult;
                }
            }
            return dialogResult;
        }

        protected DialogResult UpdateMention(Mention mention, CreditsMentionUI mentionUI, ref DialogResult dialogResult)
        {
            UpdateTextDialog(mentionUI.Name.text, mention.Name, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => UpdateText(mentionUI.Name.gameObject, mentionUI.Name, mention.Name)))
            {
                return dialogResult;
            }

            UpdateTextDialog(mentionUI.Job.text, mention.Job, ref dialogResult);
            if (!CheckDialogResult(ref dialogResult, () => UpdateText(mentionUI.Job.gameObject, mentionUI.Job, mention.Job)))
            {
                return dialogResult;
            }

            return dialogResult;
        }

        protected virtual void InstantiateCredits(Chapter chapter, Transform parent, string creditsName)
        {
            CreditsChapterUI newChapter = ((GameObject)PrefabUtility.InstantiatePrefab(ChapterPrefab, parent)).gameObject.GetComponent<CreditsChapterUI>();
            m_CreditsChapterUI.Add(newChapter);

            newChapter.gameObject.name = creditsName;

            for (int i = 0; i < chapter.Pages.Length; i++)
            {
                newChapter.Pages.Add(InstantiatePage(chapter.Pages[i], newChapter.transform));
            }

            newChapter.transform.position = new Vector3(newChapter.transform.position.x, 0, 0);
        }

        protected virtual CreditsPageUI InstantiatePage(Page page, Transform parent)
        {
            CreditsPageUI newPage = ((GameObject)PrefabUtility.InstantiatePrefab(PagePrefab, parent)).gameObject.GetComponent<CreditsPageUI>();
            if (page.Title == string.Empty)
            {
                newPage.Title.gameObject.SetActive(false);
            }
            else
            {
                newPage.Title.text = page.Title;
                newPage.gameObject.name = page.Title;
            }

            List<CreditsSectionUI> sections = new List<CreditsSectionUI>();

            for (int i = 0; i < page.Sections.Length; i++)
            {
                sections.Add(InstantiateSection(page.Sections[i], newPage.transform));
            }

            newPage.Sections = sections;

            return newPage;
        }

        protected virtual CreditsSectionUI InstantiateSection(Section section, Transform parent)
        {
            if (section.Columns <= 0) section.Columns = 1;

            CreditsSectionUI newSection = ((GameObject)PrefabUtility.InstantiatePrefab(SectionPrefab, parent)).gameObject.GetComponent<CreditsSectionUI>();

            if (section.Title == string.Empty)
            {
                newSection.Title.gameObject.SetActive(false);
            }
            else
            {
                newSection.Title.text = section.Title;
                newSection.gameObject.name = section.Title;
            }

            List<CreditsMentionUI> mentions = new List<CreditsMentionUI>();

            if (section.Columns > 1)
            {
                int columnsSize = section.Mentions.Length / section.Columns;
                if (columnsSize * section.Columns < section.Mentions.Length) columnsSize++;

                GameObject ColumnParent = Instantiate(SectionColumnsPrefab, newSection.transform);

                for (int i = 0; i < section.Columns; ++i)
                {
                    GameObject mentionParent = Instantiate(ColumnPrefab, ColumnParent.transform);

                    for (int j = columnsSize * i; j < (columnsSize * i) + columnsSize; j++)
                    {
                        if (j >= section.Mentions.Length) break;
                        CreditsMentionUI mention = InstantiateMention(section.Mentions[j], mentionParent.transform);
                        mentions.Add(mention);
                    }
                }
            }
            else
            {
                for (int i = 0; i < section.Mentions.Length; ++i)
                {
                    CreditsMentionUI mention = InstantiateMention(section.Mentions[i], newSection.transform);
                    mentions.Add(mention);
                }
            }

            newSection.Mentions = mentions;

            return newSection;
        }

        protected CreditsMentionUI InstantiateMention(Mention mention, Transform parent)
        {
            CreditsMentionUI newMention = ((GameObject)PrefabUtility.InstantiatePrefab(MentionPrefab, parent)).gameObject.GetComponent<CreditsMentionUI>();

            if (DisableJobsAndNamesWhenEmpty && mention.Name == string.Empty)
            {
                newMention.Name.gameObject.SetActive(false);
                Debug.LogWarning($"{newMention.gameObject.name} has no Name assigned");
            }
            else
            {
                newMention.Name.text = mention.Name;
                newMention.gameObject.name = mention.Name;

                if (CenterNameWhenJobEmpty && mention.Job == string.Empty)
                {
                    newMention.Name.alignment = TextAlignmentOptions.Center;
                }
            }

            if (DisableJobsAndNamesWhenEmpty && mention.Job == string.Empty)
            {
                newMention.Job.gameObject.SetActive(false);
                Debug.LogWarning($"{newMention.gameObject.name} has no Job assigned");
            }
            else
            {
                newMention.Job.text = mention.Job;
            }

            return newMention;
        }

        protected bool CheckDialogResult(ref DialogResult dialogResult, Action action)
        {
            switch (dialogResult)
            {
                case DialogResult.CONTINUE_ALL:
                    {
                        m_DisplayDialogs = false;
                    }
                    goto case DialogResult.CONTINUE;
                case DialogResult.CONTINUE:
                    action.Invoke();
                    return true;
                case DialogResult.STOP:
                    {
                        return false;
                    }
                default:
                    dialogResult = DialogResult.STOP;
                    return false;
            }
        }
        protected void RemoveUnused(List<Component> component, int index)
        {
            for (int i = component.Count - 1; i > index - 1; i--)
            {
                DestroyImmediate(component[i].gameObject);
                component.RemoveAt(i);
            }
        }

        protected void RemoveUnusedDialog(string objectName, int elementsCount, int uiElementsCount, ref DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.CONTINUE)
            {
                if (elementsCount < uiElementsCount)
                {
                    dialogResult = (DialogResult)EditorUtility
                    .DisplayDialogComplex("Credits Importer", $"{uiElementsCount - elementsCount} Elements of {objectName} will be deleted. Do you wanna to execute this action.", "Continue", "ContinueAll", "Stop");
                }
            }
        }
        protected void UpdateText(GameObject @object, TextMeshProUGUI titleUI, string newTitle)
        {
            if (newTitle == null || newTitle == string.Empty)
            {
                @object.SetActive(false);
                return;
            }

            if (!@object.activeInHierarchy) @object.SetActive(true);

            if (newTitle != string.Empty)
            {
                titleUI.text = newTitle;
                @object.name = newTitle;
            }
        }
        protected void UpdateTextDialog(string titleUI, string title, ref DialogResult dialogResult)
        {
            if (dialogResult == DialogResult.CONTINUE)
            {
                if (titleUI != title)
                {
                    dialogResult = (DialogResult)EditorUtility
                    .DisplayDialogComplex("Credits Importer", $"Text of {titleUI} will change to {title}.", "Continue", "Stop", "Continue All");
                }
            }
        }
#endif
    }
}