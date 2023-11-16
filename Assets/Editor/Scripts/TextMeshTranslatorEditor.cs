using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using static Unity.VisualScripting.Icons;

[CustomEditor(typeof(TextMeshTranslator))]

public class TextMeshTranslatorEditor : Editor
{
    int selectedElement;
    Language currentLanguageText;
    TextMeshTranslator textComponent;

    public void OnSceneGUI()
    {
        textComponent = (TextMeshTranslator)target;
        if (currentLanguageText != GameManager.Instance.currentSelectedLanguage)
            ChangePreview();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        textComponent = (TextMeshTranslator)target;

        selectedElement = LanguageEditor.instance.IDs.IndexOf(textComponent.ID);
        string[] IDsArray = LanguageEditor.instance.IDs.ToArray();


        EditorGUI.BeginChangeCheck();

        selectedElement = EditorGUILayout.Popup(selectedElement, IDsArray);

        EditorGUILayout.LabelField("Translated string : ",EditorStyles.boldLabel);

        if (IDsArray.Length >= selectedElement &&  selectedElement >= 0)
        { 
            EditorGUILayout.LabelField(GameManager.Instance.currentSelectedLanguage.FindWithID(IDsArray[selectedElement]).TranslatedString, EditorStyles.largeLabel);
        }
            
        
        if (EditorGUI.EndChangeCheck())
        {
            textComponent.ChangeID(IDsArray[selectedElement]);
            ChangePreview();
        }

    }

    public void ChangePreview()
    {
        textComponent.ChangeText();
        currentLanguageText = GameManager.Instance.currentSelectedLanguage;
        EditorUtility.SetDirty(target);
    }
    
}
