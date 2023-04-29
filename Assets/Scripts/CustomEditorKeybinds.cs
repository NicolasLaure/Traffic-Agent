using UnityEngine;
using UnityEditor;

class CustomEditorKeybinds : EditorWindow
{
    //When the keybind is pressed, verifies if the circumstances are correct for the keybind to be run
    [MenuItem("Custom/Cycle Node _g", true)]
    static bool ValidateCycleNode()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<NodeCycle>() != null;
    }

    //If the keybind is pressed, and the circumstances have been verified, run the following code
    [MenuItem("Custom/Cycle Node _g")]
    static void CycleNode()
    {
        Selection.activeGameObject.GetComponent<NodeCycle>().Cycle();
    }
}