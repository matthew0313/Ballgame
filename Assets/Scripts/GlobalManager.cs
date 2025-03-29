using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MEC;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<GlobalManager>("GlobalManager"));
        DontDestroyOnLoad(Instance.gameObject);
    }

    [SerializeField] FloatHook sceneLoadingProgressHook;

    [Header("UI")]
    [SerializeField] RectTransform sceneLoadBlack;
    [SerializeField] GameObject sceneLoadingBar;
    public void StartGame()
    {
        LoadScene("Game", () =>
        {
            Load((data) => { if (GameManager.Instance != null && data != null) GameManager.Instance.LoadGame(data); });
        });
    }
    public Action<string> onSceneWillLoad;
    bool sceneLoading = false;
    public void LoadScene(string sceneName, Action onLoad = null)
    {
        if (sceneLoading) return;
        sceneLoading = true;

        sceneLoadBlack.pivot = new Vector2(1.0f, 0.5f);
        sceneLoadBlack.DOKill();
        sceneLoadBlack.DOScaleX(1.0f, 0.5f).SetEase(Ease.OutCirc).SetUpdate(true).OnComplete(() =>
        {
            sceneLoadingBar.SetActive(true);
            Timing.RunCoroutine(LoadingScene(sceneName, onLoad));
        });
    }
    IEnumerator<float> LoadingScene(string sceneName, Action onLoad)
    {
        onSceneWillLoad?.Invoke(sceneName);

        var op = SceneManager.LoadSceneAsync(sceneName);
        UnityAction<Scene, LoadSceneMode> onLoadAction = (scene, loadSceneMode) => { onLoad?.Invoke(); };
        SceneManager.sceneLoaded += onLoadAction;

        sceneLoadingProgressHook.SetValue(0.0f);
        while (!op.isDone)
        {
            sceneLoadingProgressHook.SetValue(op.progress);
            yield return Timing.WaitForOneFrame;
        }
        sceneLoadingProgressHook.SetValue(1.0f);
        sceneLoadingBar.SetActive(false);

        SceneManager.sceneLoaded -= onLoadAction;
        sceneLoading = false;
        sceneLoadBlack.pivot = new Vector2(0.0f, 0.5f);
        sceneLoadBlack.DOScaleX(0.0f, 0.5f).SetEase(Ease.OutCirc).SetUpdate(true);
    }
    const string savePath = "Saves";
    FileHandler handler = new LocalFileHandler();
    public void Save(SaveData data)
    {
        handler.Save(Path.Combine(savePath, "Save.json"), JsonUtility.ToJson(data), null);
    }
    void Load(Action<SaveData> onLoad)
    {
        handler.Load(Path.Combine(savePath, "Save.json"), (content) =>
        {
            onLoad?.Invoke(content != null ? JsonUtility.FromJson<SaveData>(content) : null);
        });
    }
}