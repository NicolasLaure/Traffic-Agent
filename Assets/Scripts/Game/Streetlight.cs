using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NodeCycle))]
public class Streetlight : MonoBehaviour
{
    public static float streetlightInterval = 5;
    
    public GameObject flipped;

    NodeCycle nodeCycle;

    private void Start()
    {
        nodeCycle = gameObject.GetComponent<NodeCycle>();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        while (DeliveryGame.instance.winCondition < 0)
        {
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(FlipStreetlight());
    }

    public void ChangeStreetLight()
    {
        nodeCycle.ChangeNodeType(flipped);
    }

    IEnumerator FlipStreetlight()
    {
        yield return new WaitForSeconds(streetlightInterval);
        ChangeStreetLight();
    }
}
