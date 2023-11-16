using TMPro;
using UnityEngine;

public class TextMeshTranslator : MonoBehaviour
{
    [HideInInspector] public string ID;
    string translatedString;
    Language language;
    

    private void OnEnable()
    {
        GameManager.Instance.languageChanged.AddListener(ChangeText);
    }

    private void OnDisable()
    {
        GameManager.Instance.languageChanged.RemoveListener(ChangeText);
    }

    public void ChangeText()
    {
        if (GameManager.Instance.CurrentSelectedLanguage == null)
        { return; }

        language = GameManager.Instance.CurrentSelectedLanguage;

        for (int i = 0; i < language.LanguageDictionary.Count; i++)
        {
            if (language.LanguageDictionary[i].ID == ID && !string.IsNullOrWhiteSpace(language.LanguageDictionary[i].TranslatedString))
            {
                translatedString = language.LanguageDictionary[i].TranslatedString;
                gameObject.GetComponent<TextMeshProUGUI>().text = translatedString;
                return;
            }
        }

        gameObject.GetComponent<TextMeshProUGUI>().text = $"??{ID}??";
    }

    public void ChangeID(string newID)
    {
        ID = newID;
    }

    
    

}
