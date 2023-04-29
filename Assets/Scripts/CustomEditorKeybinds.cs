using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
class CustomEditorKeybinds : EditorWindow
{
    //When the keybind is pressed, verifies if the circumstances are correct for the keybind to be run
    [MenuItem("Custom/Cycle Node _g", true)]
    static bool ValidateCycleNode()
    {
        GameObject[] selected = Selection.gameObjects;
        return (selected.Length > 0);
    }

    //If the keybind is pressed, and the circumstances have been verified, run the following code
    [MenuItem("Custom/Cycle Node _g")]
    static void CycleNode()
    {
        List<GameObject> newSelection = new List<GameObject>();
        foreach (GameObject selected in Selection.gameObjects)
        {
            NodeCycle nodeCycle = selected.GetComponent<NodeCycle>();
            if (nodeCycle != null)
            {
                newSelection.Add(nodeCycle.Cycle());
            }
        }
        Selection.objects = newSelection.ToArray();
    }
}
#endif