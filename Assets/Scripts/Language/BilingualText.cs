using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TMP_Text))]
public class BilingualText : MonoBehaviour
{
    [SerializeField] LangText text;
    TMP_Text body;
    private void OnEnable()
    {
        if(body == null) body = GetComponent<TMP_Text>();
        Settings.onLanguageChange += OnLanguageChange;
        OnLanguageChange();
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= OnLanguageChange;
    }
    void OnLanguageChange()
    {
        body.text = text.text;
    }
}