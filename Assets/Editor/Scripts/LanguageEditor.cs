using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LanguageEditor : EditorWindow
{
    public static LanguageEditor instance;

    [MenuItem("Tools/Language Editor")]
    private static void OpenWindow()
    {
        GetWindow<LanguageEditor>();
    }

    Vector2 scrollView;

    private int selectedElement;

    string newLanguageName;
    string newID;

    string fileName = "";

    Language[] languages;
    public List<string> IDs;

    private void OnValidate()
    {
        if (instance == null)
            instance = this;
    }

    private void OnGUI()
    {
        string[] languagesAssets = AssetDatabase.FindAssets("t:Language");
        string[] languagesString = new string[languagesAssets.Length];
        string[] languageNames = new string[languagesAssets.Length];
        languages = new Language[languagesAssets.Length];
        IDs = new();

        for (int i = 0; i < languagesAssets.Length; i++)
        {
            languagesString[i] = AssetDatabase.GUIDToAssetPath(languagesAssets[i]);

            languageNames[i] = Path.GetFileNameWithoutExtension(languagesString[i]);
            languages[i] = AssetDatabase.LoadAssetAtPath<Language>(languagesString[i]);
        }

        //Aggiorna lista id
        for (int i = 0; i < languages.Length; i++)
        {
            for (int j = 0; j < languages[i].LanguageDictionary.Count; j++)
            {
                if (!IDs.Contains(languages[i].LanguageDictionary[j].ID))
                    IDs.Add(languages[i].LanguageDictionary[j].ID);
            }
        }

        //Aggiorna lingue
        for (int i = 0; i < languages.Length; i++)
        {
            for (int j = 0; j < IDs.Count; j++)
            {
                if (languages[i].FindWithID(IDs[j]) == null)
                {
                    Language.DictionaryEntry entry = new Language.DictionaryEntry();
                    entry.ID = IDs[j];
                    languages[i].AddNewEntry(entry);
                }
            }
        }


        if (GUILayout.Button("Create CSV file"))
        {
            fileName = Application.dataPath + "/languages.csv";
            WriteCSV();
        }

        if (GUILayout.Button("Read CSV file"))
        {
            fileName = Application.dataPath + "/languages.csv";
            ReadCSV();
        }

        GUILayout.Space(10);

        if (languagesString.Length <= 0)
            return;

        if (selectedElement >= languagesAssets.Length)
            selectedElement = languagesAssets.Length - 1;


        Language selectedLanguage = AssetDatabase.LoadAssetAtPath<Language>(languagesString[selectedElement]);

        if (selectedLanguage == null)
            return;

        NewLanguageGUI();

        NewIDGUI();

        GUILayout.Space(5);
        selectedElement = EditorGUILayout.Popup(selectedElement, languageNames);

        ScrollviewGUI(selectedLanguage);
    }

    private void ScrollviewGUI(Language selectedLanguage)
    {
        scrollView = EditorGUILayout.BeginScrollView(scrollView);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        for (int i = 0; i < IDs.Count; i++)
        {

            GUILayout.Space(5);
            Language.DictionaryEntry entry = selectedLanguage.LanguageDictionary[i];


            string Id = !string.IsNullOrWhiteSpace(entry.ID) ? entry.ID : "??ID??";

            entry.openInEditor = EditorGUILayout.Foldout(entry.openInEditor, $"{Id}", EditorStyles.foldoutHeader);


            if (entry.openInEditor)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;

                List<Language.DictionaryEntry> entries = new List<Language.DictionaryEntry>();

                for (int l = 0; l < languages.Length; l++)
                {
                    entries.Add(languages[l].FindWithID(entry.ID));
                }

                EditorGUI.BeginChangeCheck();

                entry.ID = EditorGUILayout.TextField("ID", entry.ID);

                if (EditorGUI.EndChangeCheck())
                {
                    for (int e = 0; e < entries.Count; e++)
                    {
                        entries[e].ID = entry.ID;
                    }

                    for (int l = 0; l < languages.Length; l++)
                        EditorUtility.SetDirty(languages[l]);

                }

                entry.TranslatedString = EditorGUILayout.TextField("Translated string", entry.TranslatedString);


                EditorGUILayout.EndVertical();

                GUILayout.Space(5);
                if (GUILayout.Button("RemoveEntry"))
                {
                    foreach (Language l in languages)
                    {
                        l.RemoveEntry(i);
                    }
                    for (int l = 0; l < languages.Length; l++)
                        EditorUtility.SetDirty(languages[l]);


                    break;
                }


                EditorGUI.indentLevel--;
            }
        }

        for (int l = 0; l < languages.Length; l++)
        {
            AssetDatabase.SaveAssetIfDirty(languages[l]);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

        GameManager.Instance.gameLanguages = languages;

    }

    private void NewIDGUI()
    {
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();

        newID = EditorGUILayout.TextField(newID);

        if (GUILayout.Button("Add new ID to all languagues"))
        {
            CreateID(newID);
            newID = "";
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
    }

    private void NewLanguageGUI()
    {
        EditorGUILayout.BeginHorizontal();

        newLanguageName = EditorGUILayout.TextField(newLanguageName);

        if (GUILayout.Button("Create new Language", GUILayout.MaxWidth(150)))
        {
            CreateLanguage(newLanguageName);
            newLanguageName = "";
        }

        EditorGUILayout.EndHorizontal();
    }

    private void CreateID(string ID)
    {
        if (string.IsNullOrWhiteSpace(ID))
        {
            EditorUtility.DisplayDialog("ID not selected", "You must assign a string ID first", "OK");
            return;
        }

        if (IDs.Find(s => ID == s) != null)
        {
            EditorUtility.DisplayDialog("This ID already exist", "Chose another ID", "OK");
            return;
        }

        IDs.Add(ID);


        //Aggiorna lingue
        for (int i = 0; i < languages.Length; i++)
        {
            for (int j = 0; j < IDs.Count; j++)
            {
                if (languages[i].FindWithID(IDs[j]) == null)
                {
                    Language.DictionaryEntry entry = new Language.DictionaryEntry();
                    entry.ID = IDs[j];
                    languages[i].AddNewEntry(entry);
                }
            }
        }

        for (int l = 0; l < languages.Length; l++)
            EditorUtility.SetDirty(languages[l]);
    }

    private Language CreateLanguage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            EditorUtility.DisplayDialog("Name not selected", "You must assign a name to the new language first", "OK");
            return null;
        }

        Language newLanguage = ScriptableObject.CreateInstance<Language>();
        AssetDatabase.CreateAsset(newLanguage, $"Assets/Language/{name}.asset");
        AssetDatabase.SaveAssets();
        return newLanguage;
    }



    public void WriteCSV()
    {
        using (StreamWriter textWriter = new StreamWriter(fileName))
        {
            textWriter.Write("ID");

            for (int i = 0; i < languages.Length; i++)
            {
                textWriter.Write("," + languages[i].name);
            }

            textWriter.Write("\n");

            for (int i = 0; i < languages[0].LanguageDictionary.Count; i++)
            {
                textWriter.Write(languages[0].LanguageDictionary[i].ID);

                for (int l = 0; l < languages.Length; l++)
                {
                    textWriter.Write(",");


                    if (languages[l].FindWithID(languages[0].LanguageDictionary[i].ID) != null)
                    {
                        textWriter.Write(languages[l].FindWithID(languages[0].LanguageDictionary[i].ID).TranslatedString);
                    }
                }

                textWriter.Write("\n");
            }
        }


    }


    public void ReadCSV()
    {
        List<Language> readedLanguages = new List<Language>();
        string[] allLines = File.ReadAllLines(fileName);

        for (int l = 0; l < allLines.Length; l++)
        {
            string[] splitData = allLines[l].Split(",");

            if (l == 0)
            {
                for (int c = 1; c < splitData.Length; c++)
                    readedLanguages.Add(CreateLanguage(splitData[c]));
            }

            if (l >= 1)
            {

                for (int c = 1; c < splitData.Length; c++)
                {
                    Language.DictionaryEntry entry = new Language.DictionaryEntry();


                    entry.ID = splitData[0];

                    entry.TranslatedString = splitData[c];

                    readedLanguages[c - 1].AddNewEntry(entry);
                }
            }
        }
    }


}
