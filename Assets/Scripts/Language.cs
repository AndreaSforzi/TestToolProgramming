using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Languages/Language")]
public class Language : ScriptableObject
{
    [SerializeField] List<DictionaryEntry> languageDictionary = new();

    [Serializable]
    public class DictionaryEntry
    {
        [SerializeField] string id;
        [SerializeField] string translatedString;

#if UNITY_EDITOR
        public bool openInEditor;
#endif


        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public string TranslatedString
        {
            get { return translatedString; }
            set { translatedString = value; }
        }
    }

    public List<DictionaryEntry> LanguageDictionary => languageDictionary;

    public DictionaryEntry FindWithID(string idToFind) => languageDictionary.Find(i => i.ID == idToFind);

    public void AddNewEntry(DictionaryEntry entry) => languageDictionary.Insert(languageDictionary.Count, entry);
    
    public void RemoveEntry(int index) => languageDictionary.RemoveAt(index);
}
