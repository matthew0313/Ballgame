using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        introPlayed = false;
    }
    [SerializeField] Animator anim;

    [SerializeField] Transform bottomPoint;
    [SerializeField] GameObject platformPrefab;
    [SerializeField] float platformInterval = 5.0f;
    [SerializeField] float platformXMin = -8.0f, platformXMax = 8.0f;
    [SerializeField] float platformXMinDifference = 2.0f;

    [SerializeField] float cameraScrollSpeed = 3.0f;

    static bool introPlayed;
    private void Awake()
    {
        if (!introPlayed)
        {
            introPlayed = true;
            anim.SetTrigger("Intro");
        }
    }
    List<GameObject> platforms = new();
    private void Update()
    {
        SpawnPlatforms();
        Camera.main.transform.position += Vector3.up * cameraScrollSpeed * Time.deltaTime;
    }
    void SpawnPlatforms()
    {
        float lastPlatformY = platforms.Count > 0 ? platforms[platforms.Count - 1].transform.position.y : bottomPoint.position.y;
        float lastPlatformX = platforms.Count > 0 ? platforms[platforms.Count - 1].transform.position.x : 0.0f;
        while (Camera.main.transform.position.y + 50.0f > lastPlatformY)
        {
            lastPlatformY += platformInterval;
            float probLeft = ((lastPlatformX - platformXMinDifference / 2.0f) - platformXMin) / (platformXMax - platformXMin - platformXMinDifference);
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= probLeft)
            {
                lastPlatformX = UnityEngine.Random.Range(platformXMin, lastPlatformX - platformXMinDifference / 2.0f);
            }
            else
            {
                lastPlatformX = UnityEngine.Random.Range(lastPlatformX + platformXMinDifference / 2.0f, platformXMax);
            }
            if(platforms.Count <= 0 || platforms[0].transform.position.y > Camera.main.transform.position.y - 50.0f) platforms.Add(Instantiate(platformPrefab, new Vector2(lastPlatformX, lastPlatformY), Quaternion.identity));
            else
            {
                GameObject tmp = platforms[0];
                platforms.RemoveAt(0);
                tmp.transform.position = new Vector2(lastPlatformX, lastPlatformY);
                platforms.Add(tmp);
            }
        }
    }
    public void StartGame()
    {
        GlobalManager.Instance.StartGame();
    }
}
