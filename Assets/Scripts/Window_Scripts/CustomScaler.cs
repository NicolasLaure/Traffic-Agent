using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomScaler : MonoBehaviour
{
    public bool reload = false;
    public RectTransform rectTransform;
    public Vector2 size;
    public Vector2 resolution;

    private void Start()
    {

    }

    public void OnResolutionChange(Vector2 curResolution)
    {
        if (rectTransform != null)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (curResolution.x / resolution.x) * size.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (curResolution.y / resolution.y) * size.y);
            Debug.Log(curResolution);
        }
    }



#if UNITY_EDITOR
    //This weird workaround is to avoid a spam warning
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }

    //Whenever this script is loaded, or a variable is changed, the following function is called
    void _OnValidate()
    {
        if (reload)
        {
            reload = false;
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            OnResolutionChange((Vector2)Res);
            foreach (CustomScaler cs in GetComponentsInChildren<CustomScaler>())
            {
                cs.OnResolutionChange((Vector2)Res);
            }
        }
    }
#endif
}
