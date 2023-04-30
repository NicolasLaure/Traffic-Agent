using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] float spawnCoolDown;
    [SerializeField] int quantityToSpawn;
    GameObject adGenerator;
    private void Start()
    {
        adGenerator = GameObject.FindGameObjectWithTag("AdGenerator");
    }

    public void SetAdsValues()
    {
        adGenerator.GetComponent<AdGeneratorController>().StartLevel(spawnCoolDown, quantityToSpawn);
    }
}
