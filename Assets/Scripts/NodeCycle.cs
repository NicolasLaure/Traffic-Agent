using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCycle : MonoBehaviour
{
    [HideInInspector]
    public MapGrid mapGrid;
    [HideInInspector]
    public int gridX;
    [HideInInspector]
    public int gridY;

    public string nodeName;
    public GameObject nextType;

#if UNITY_EDITOR
    public void Cycle()
    {
        MapGrid.Node tempNode = new MapGrid.Node(MapGrid.GridState.ROAD, gridX, gridY, Instantiate(nextType, mapGrid.gridParent.transform), mapGrid);
        tempNode.obj.name = nextType.GetComponent<NodeCycle>().nodeName + " (" + gridX + "; " + gridY + ")";
        //tempNode.obj.transform.localPosition = new Vector2(mapGrid.gridSpacing.x * gridX, mapGrid.gridSpacing.y * gridY * -1) + mapGrid.gridOffset;
        RectTransform rt = tempNode.obj.GetComponent<RectTransform>();
        rt.anchorMin = mapGrid.topLeft + new Vector2(gridX * mapGrid.horizontalSpacing, gridY * mapGrid.verticalSpacing * -1);
        rt.anchorMax = rt.anchorMin;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapGrid.nodeSize);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapGrid.nodeSize);
        mapGrid.grid[gridX, gridY] = tempNode;
        StartCoroutine(Destroy(gameObject));
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
#endif
}
