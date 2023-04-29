using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class ComputerTimer : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    private void Start()
    {
        text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        text.text = (System.DateTime.Now.Minute % 24).ToString("00") + ":" + System.DateTime.Now.Second.ToString("00");
    }
}
