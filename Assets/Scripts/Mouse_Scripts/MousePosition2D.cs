using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition2D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }
}
