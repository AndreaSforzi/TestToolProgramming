using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Language currentSelectedLanguage;
    public Language[] gameLanguages;

    [HideInInspector] public UnityEvent languageChanged;

    public Language CurrentSelectedLanguage
    {
        get 
        {
            if (currentSelectedLanguage == null && gameLanguages.Length>0)
                currentSelectedLanguage = gameLanguages[0];

            return currentSelectedLanguage;
        }
        set
        { 
            currentSelectedLanguage = value;
            languageChanged.Invoke();
        }
    }


    private void OnValidate()
    {
        Instance = this;
    }

    public void ChangeLanguage(Language language)
    {
        CurrentSelectedLanguage = language;
    }
}
