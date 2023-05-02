using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootController : MonoBehaviour
{

    [SerializeField] float bootTime;
    GameObject cursor;
    private void Awake()
    {
        gameObject.SetActive(true);
    }
    private void Start()
    {
        cursor = GameObject.Find("MousePointer");
        cursor.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(BootTime());

    }


    IEnumerator BootTime()
    {
        yield return new WaitForSeconds(bootTime);
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        cursor.SetActive(true);
    }
}
