using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour
{
    bool _localizationHasFinished = false;

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
        StartCoroutine(LoadDelay(scene));
    }

    IEnumerator SetLocale(int localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        _localizationHasFinished = true;
    }

    IEnumerator LoadDelay(string scene)
    {
        while (_localizationHasFinished == false) yield return null;
        SceneManager.LoadScene(scene);
    }
}
