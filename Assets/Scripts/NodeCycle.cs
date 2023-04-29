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
    public MapGrid.GridState nodeType;
    public GameObject nextType;

    public GameObject[] childs;
    
#if UNITY_EDITOR
    public GameObject Cycle()
    {
        MapGrid.Node tempNode = new MapGrid.Node(nextType.GetComponent<NodeCycle>().nodeType, gridX, gridY, Instantiate(nextType, mapGrid.gridParent.transform), mapGrid);
        tempNode.obj.name = nextType.GetComponent<NodeCycle>().nodeName + " (" + gridX + "; " + gridY + ")";
        RectTransform rt = tempNode.obj.GetComponent<RectTransform>();
        rt.anchorMin = mapGrid.topLeft + new Vector2(gridX * mapGrid.horizontalSpacing, gridY * mapGrid.verticalSpacing * -1);
        rt.anchorMax = rt.anchorMin;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapGrid.nodeSize);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapGrid.nodeSize);
        tempNode.obj.GetComponent<NodeCycle>().ChangeChildsSize(mapGrid.nodeSize);
        mapGrid.grid[gridX, gridY] = tempNode;
        StartCoroutine(Destroy(gameObject));
        return tempNode.obj;
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
#endif

    public void ChangeStreetLight(GameObject obj)
    {
         MapGrid.Node tempNode = new MapGrid.Node(obj.GetComponent<NodeCycle>().nodeType, gridX, gridY, Instantiate(obj, mapGrid.gridParent.transform), mapGrid);
        tempNode.obj.name = nextType.GetComponent<NodeCycle>().nodeName + " (" + gridX + "; " + gridY + ")";
        RectTransform rt = tempNode.obj.GetComponent<RectTransform>();
        rt.anchorMin = mapGrid.topLeft + new Vector2(gridX * mapGrid.horizontalSpacing, gridY * mapGrid.verticalSpacing * -1);
        rt.anchorMax = rt.anchorMin;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mapGrid.nodeSize);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mapGrid.nodeSize);
        tempNode.obj.GetComponent<NodeCycle>().ChangeChildsSize(mapGrid.nodeSize);
        mapGrid.grid[gridX, gridY] = tempNode;
        Destroy(this.gameObject);
    }

    public void ChangeChildsSize(float size)
    {        
        foreach (var item in childs)
        {
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            item.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }
    }
}
