using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdGeneratorController : MonoBehaviour
{
    [SerializeField] List<GameObject> adsList;

    [SerializeField] float spawnCoolDown;
    [SerializeField] int QuantityToSpawn;
    float timeToSpawn;
    // Update is called once per frame
    int spawnedAds = 0;
    float timePass;
    void Update()
    {
        timePass += Time.deltaTime;
        if (Time.time >= timeToSpawn && spawnedAds < QuantityToSpawn && timePass >= spawnCoolDown)
        {
            SpawnAd();
            spawnedAds++;
            timePass = 0;
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
        int randomNum = Random.Range(0, maxRange);

        GameObject adToSpawn = adsList[Random.Range(0, adsList.Count)].gameObject;
        Instantiate(adToSpawn, spawnPoints[randomNum].transform.position,  Quaternion.identity, GameObject.Find("Canvas").transform);
    }

    public void StartLevel(float SpawnCd, int QuantityOfAds)
    {
        spawnCoolDown = SpawnCd;
        QuantityToSpawn = QuantityOfAds;
        timeToSpawn = Random.Range(Time.time, Time.time + 10);
        spawnedAds = 0;
    }
}
