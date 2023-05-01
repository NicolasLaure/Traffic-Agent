using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdGeneratorController : MonoBehaviour
{
    [SerializeField] List<GameObject> adsList;

    [SerializeField] float spawnCoolDown;
    [SerializeField] int QuantityToSpawn;
    float timeToSpawn;

    int spawnedAds = 0;
    float timePass;

    bool inLevel = false;
    GameObject gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
    }

    void Update()
    {
        if (inLevel)
        {
            timePass += Time.deltaTime;
            if (Time.time >= timeToSpawn && spawnedAds < QuantityToSpawn && timePass >= spawnCoolDown)
            {
                SpawnAd();
                spawnedAds++;
                timePass = 0;
            }
        }
    }
    
    void SpawnAd()
    {
        List<GameObject> spawnPoints = new List<GameObject>();
        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnPoints.Add(spawnPoint);
        }

        int maxRange = spawnPoints.Count;

        if (maxRange == 0)
        {
            EndLevel();
            return;
        }

        int randomNum = Random.Range(0, maxRange);

        GameObject adToSpawn = adsList[Random.Range(0, adsList.Count)].gameObject;
        gameManager.GetComponent<PanelManager>().WindowInstantiate(adToSpawn, spawnPoints[randomNum].transform.position);
    }

    public void StartLevel(float SpawnCd, int QuantityOfAds)
    {
        inLevel = true;
        spawnCoolDown = SpawnCd;
        QuantityToSpawn = QuantityOfAds;
        timeToSpawn = Random.Range(Time.time, Time.time + 10);
        spawnedAds = 0;
    }

    public void EndLevel()
    {
        QuantityToSpawn = 0;
        inLevel = false;
    }
}
