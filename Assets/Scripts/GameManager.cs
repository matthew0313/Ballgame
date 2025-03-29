using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    public GameManager() => Instance = this;


    Player player;

    [SerializeField] Transform bottomPlatform;
    [SerializeField] GameObject platformPrefab;
    [SerializeField] float platformInterval = 5.0f;
    [SerializeField] float platformXMin = -8.0f, platformXMax = 8.0f;
    [SerializeField] float platformXMinDifference = 2.0f;

    [Header("Hooks")]
    [SerializeField] FloatHook heightHook;
    [SerializeField] FloatHook bestHeightHook;

    [Header("Save")]
    [SerializeField] float autoSaveInterval = 60.0f;

    List<ISavable> savables = new();
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        savables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISavable>().ToList();
        savables.Sort((a, b) => b.sortOrder.CompareTo(a.sortOrder));
    }
    private void Update()
    {
        UpdateHeight();
        SpawnPlatforms();
        UpdateAutoSave();
    }
    void UpdateHeight()
    {
        float height = player.transform.position.y - bottomPlatform.position.y;
        heightHook.SetValue(height);
        if (height > bestHeightHook.value) bestHeightHook.SetValue(height);
    }
    List<GameObject> platforms = new();
    void SpawnPlatforms()
    {
        float lastPlatformY = platforms.Count > 0 ? platforms[platforms.Count - 1].transform.position.y : bottomPlatform.position.y;
        float lastPlatformX = platforms.Count > 0 ? platforms[platforms.Count - 1].transform.position.x : 0.0f;
        while (player.transform.position.y + 50.0f > lastPlatformY)
        {
            lastPlatformY += platformInterval;
            float probLeft = ((lastPlatformX - platformXMinDifference / 2.0f) - platformXMin) / (platformXMax - platformXMin - platformXMinDifference);
            if(UnityEngine.Random.Range(0.0f, 1.0f) <= probLeft)
            {
                lastPlatformX = UnityEngine.Random.Range(platformXMin, lastPlatformX - platformXMinDifference / 2.0f);
            }
            else
            {
                lastPlatformX = UnityEngine.Random.Range(lastPlatformX + platformXMinDifference / 2.0f, platformXMax);
            }
            platforms.Add(Instantiate(platformPrefab, new Vector2(lastPlatformX, lastPlatformY), Quaternion.identity));
        }
    }
    float autoSaveCounter = 0.0f;
    void UpdateAutoSave()
    {
        autoSaveCounter += Time.deltaTime;
        if(autoSaveCounter >= autoSaveInterval)
        {
            autoSaveCounter = 0.0f;
            GlobalManager.Instance.Save(SaveGame());
        }
    }
    private void OnEnable()
    {
        GlobalManager.Instance.onSceneWillLoad += OnSceneWillLoad;  
    }
    private void OnDisable()
    {
        GlobalManager.Instance.onSceneWillLoad -= OnSceneWillLoad;
    }
    void OnSceneWillLoad(string sceneName)
    {
        GlobalManager.Instance.Save(SaveGame());
    }
    private void OnApplicationQuit()
    {
        GlobalManager.Instance.Save(SaveGame());
    }
    public SaveData SaveGame()
    {
        SaveData data = new();
        data.bestHeight = bestHeightHook.value;
        foreach(var i in platforms) data.platforms.Add(i.transform.position);
        foreach (var i in savables) i.Save(data);
        return data;
    }
    public void LoadGame(SaveData data)
    {
        bestHeightHook.SetValue(data.bestHeight);
        foreach(var i in data.platforms) platforms.Add(Instantiate(platformPrefab, i, Quaternion.identity));
        foreach (var i in savables) i.Load(data);
    }
}
