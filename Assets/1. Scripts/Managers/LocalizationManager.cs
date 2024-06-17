using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeLocale(int localeID)
    {
        StartCoroutine(SetLocale(localeID));
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    IEnumerator SetLocale(int localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
    }
}
