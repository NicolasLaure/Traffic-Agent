using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour, ISerializationCallbackReceiver
{
    //Custom types
    [System.Serializable]
    public enum GridState
    {
        ROAD,
        BUILDING,
        TRAFFICLIGHTHOR,
        TRAFFICLIGHTVER,
        OFF_MAP,
        OCCUPIED,
        OCCUPIED_WEAK
    }

    [System.Serializable]
    public struct Node
    {
        public GridState gridState;
        public GameObject obj;
        public int gridX;
        public int gridY;

        public Node(GridState gridState, int x, int y, GameObject obj, MapGrid mapGrid)
        {
            this.gridState = gridState;
            this.obj = obj;
            this.gridX = x;
            this.gridY = y;
            NodeCycle nodeCycle = this.obj.GetComponent<NodeCycle>();
            nodeCycle.gridX = x;
            nodeCycle.gridY = y;
            nodeCycle.mapGrid = mapGrid;
        }
    }

    //Public variables in the Inspector
    public bool clearGrid = true;
    public int columns = 3;
    public int rows = 3;
    //public Vector2 gridSpacing = new Vector2(100, 100);
    //public Vector2 gridOffset = new Vector2(0, 0);
    public Vector2 borderSpacing = new Vector2(0, 0);
    public float nodeSize = 10;
    public GameObject defaultNode;

    //Previous values of the public variables, used to detect when one has changed
    [SerializeField, HideInInspector]
    private int prevColumns = 3;
    [SerializeField, HideInInspector]
    private int prevRows = 3;
    //[SerializeField, HideInInspector]
    //Vector2 prevGridSpacing = new Vector2(100, 100);
    //[SerializeField, HideInInspector]
    //Vector2 prevGridOffset = new Vector2(0, 0);
    [SerializeField, HideInInspector]
    private Vector2 prevBorderSpacing = new Vector2(0, 0);
    [SerializeField, HideInInspector]
    private float prevNodeSize = 10;

    //Public variables hidden from the Inspector
    [HideInInspector]
    public GameObject gridParent;
    [HideInInspector] 
    public Node[,] grid;

    //A 1D array used for serialization purposes
    [SerializeField, HideInInspector] 
    Node[] serializableGrid;


    [HideInInspector]
    public Vector2 topLeft;
    [HideInInspector]
    public float horizontalSpacing;
    [HideInInspector]
    public float verticalSpacing;

    public void CalculateSpacing()
    {
        Vector2 workingArea = new Vector2((1 - 2 * borderSpacing.x / 100.0f), (1 - 2 * borderSpacing.y / 100.0f));

        Rect rect = gridParent.GetComponent<RectTransform>().rect;
        horizontalSpacing = workingArea.x / ((columns - 1) * 1.0f);
        verticalSpacing = workingArea.y / ((rows - 1) * 1.0f);

        if (horizontalSpacing * rect.width < verticalSpacing * rect.height)
        {
            verticalSpacing = horizontalSpacing * rect.width / rect.height;
        }
        else
        {
            horizontalSpacing = verticalSpacing * rect.height / rect.width;
        }

        workingArea = new Vector2(horizontalSpacing * (columns - 1), verticalSpacing * (rows - 1));
        topLeft = new Vector2((1 - workingArea.x) * 0.5f, 1 - (1 - workingArea.y) * 0.5f);
    }

#if UNITY_EDITOR
    //This weird workaround is to avoid a spam warning
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }

    //Whenever this script is loaded, or a variable is changed, the following function is called
    void _OnValidate()
    {
        if (this == null) return;
        if (Mathf.Approximately((float)(gameObject.GetComponent<RectTransform>().rect.width / 1.252), (float)(gameObject.GetComponent<RectTransform>().rect.height / 0.794)) == false)
        {
            Debug.LogError("Error: Game panel does not have the ratio of 1252 x 794");
            Debug.Log((float)(gameObject.GetComponent<RectTransform>().rect.width / 1.252));
            Debug.Log((float)(gameObject.GetComponent<RectTransform>().rect.height / 0.794));
        }
        else
        {
            Debug.Log("Congratulations, the game panel has the correct ratio of 1252 x 794");
        }

        //Raise an error if the default node hasn't been assigned
        if (defaultNode == null)
        {
            Debug.LogWarning("Warning: No default node prefab specified");
            return;
        }

        //Raise an error if the grid is ever null for some reason
        if (grid == null)
        {
            Debug.LogError("ERROR: Grid was null");
            clearGrid = true;
        }

        Vector2[] prevRect = new Vector2[] { Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero };

        //If the grid is set to be cleared, clear it and reset everything
        if (clearGrid == true)
        {
            //If a grid parent exists, store its location in order to assign it to the new one that'll be created
            if (gridParent != null)
            {
                prevRect[0] = gridParent.GetComponent<RectTransform>().anchorMin;
                prevRect[1] = gridParent.GetComponent<RectTransform>().anchorMax;
                prevRect[2] = gridParent.GetComponent<RectTransform>().offsetMin;
                prevRect[3] = gridParent.GetComponent<RectTransform>().offsetMax;
            }
            StartCoroutine(Destroy(gridParent));
            gridParent = null;
            grid = new Node[0, 0];
            prevColumns = 0;
            prevRows = 0;
            clearGrid = false;
        }

        //If the grid parent doesn't exist, create a new one
        if (gridParent == null)
        {
            gridParent = new GameObject("Grid", typeof(RectTransform));
            gridParent.transform.SetParent(gameObject.transform);
            gridParent.GetComponent<RectTransform>().anchorMin = prevRect[0];
            gridParent.GetComponent<RectTransform>().anchorMax = prevRect[1];
            gridParent.GetComponent<RectTransform>().offsetMin = prevRect[2];
            gridParent.GetComponent<RectTransform>().offsetMax = prevRect[3];
        }

        //If the amount of rows or columns in the grid have changed, update the grid accordingly
        if (prevColumns != columns || prevRows != rows)
        {
            CalculateSpacing();
            Node[,] tempGrid = new Node[columns, rows];

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (i < grid.GetLength(0) && j < grid.GetLength(1))
                    {
                        //grid[i, j].obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                        RectTransform rt = grid[i, j].obj.GetComponent<RectTransform>();
                        rt.anchorMin = topLeft + new Vector2(i * horizontalSpacing, j * verticalSpacing * -1);
                        rt.anchorMax = rt.anchorMin;
                        rt.offsetMin = Vector2.zero;
                        rt.offsetMax = Vector2.zero;
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeSize);
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, nodeSize);
                        grid[i, j].obj.GetComponent<NodeCycle>().ChangeChildsSize(nodeSize);
                        tempGrid[i, j] = grid[i, j];
                    }
                    else
                    {
                        Node tempNode = new Node(defaultNode.GetComponent<NodeCycle>().nodeType, i, j, Instantiate(defaultNode, gridParent.transform), this);
                        tempNode.obj.name = tempNode.obj.GetComponent<NodeCycle>().nodeName + " (" + i + "; " + j + ")";
                        //tempNode.obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                        RectTransform rt = tempNode.obj.GetComponent<RectTransform>();
                        rt.anchorMin = topLeft + new Vector2(i * horizontalSpacing, j * verticalSpacing * -1);
                        rt.anchorMax = rt.anchorMin;
                        rt.offsetMin = Vector2.zero;
                        rt.offsetMax = Vector2.zero;
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeSize);
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, nodeSize);
                        tempNode.obj.GetComponent<NodeCycle>().ChangeChildsSize(nodeSize);
                        tempGrid[i, j] = tempNode;
                    }
                }
            }

            if (columns < grid.GetLength(0))
            {
                for (int i = columns; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        StartCoroutine(Destroy(grid[i, j].obj));
                    }
                }
            }

            if (rows < grid.GetLength(1))
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = rows; j < grid.GetLength(1); j++)
                    {
                        StartCoroutine(Destroy(grid[i, j].obj));
                    }
                }
            }

            prevColumns = columns;
            prevRows = rows;
            grid = tempGrid;
        }
        else
        {
            //If only the grid spacing or offset have changed, update the grid accordingly
            //if (gridSpacing != prevGridSpacing || gridOffset != prevGridOffset)
            if (borderSpacing != prevBorderSpacing)
            {
                CalculateSpacing();
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        //grid[i, j].obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                        RectTransform rt = grid[i, j].obj.GetComponent<RectTransform>();
                        rt.anchorMin = topLeft + new Vector2(i * horizontalSpacing, j * verticalSpacing * -1);
                        rt.anchorMax = rt.anchorMin;
                        rt.offsetMin = Vector2.zero;
                        rt.offsetMax = Vector2.zero;
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeSize);
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, nodeSize);
                        grid[i, j].obj.GetComponent<NodeCycle>().ChangeChildsSize(nodeSize);
                    }
                }
                //prevGridSpacing = gridSpacing;
                //prevGridOffset = gridOffset;
                prevBorderSpacing = borderSpacing;
            }
            else
            {
                if (nodeSize != prevNodeSize)
                {
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < grid.GetLength(1); j++)
                        {
                            RectTransform rt = grid[i, j].obj.GetComponent<RectTransform>();
                            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nodeSize);
                            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, nodeSize);
                            grid[i, j].obj.GetComponent<NodeCycle>().ChangeChildsSize(nodeSize);
                        }
                    }
                }
            }
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
#endif
    

    //This custom serialization is necessary because Unity doesn't serialize multidimensional arrays for some reason
    public void OnBeforeSerialize()
    {
        // Convert our unserializable array into a serializable list
        serializableGrid = new Node[grid.GetLength(0) * grid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                serializableGrid[i * grid.GetLength(1) + j] = grid[i, j];
            }
        }
    }
    public void OnAfterDeserialize()
    {
        // Convert the serializable list into our unserializable array
        grid = new Node[prevColumns, prevRows];
        for (int i = 0; i < serializableGrid.Length; i++)
        {
            grid[(i / prevRows), (i % prevRows)] = serializableGrid[i];
        }
    }


    public void RespawnVehicle(GameObject go, float respawnTime)
    {
        StartCoroutine(RespawnVehicleCoroutine(go, respawnTime));
    }

    IEnumerator RespawnVehicleCoroutine(GameObject go, float respawnTime)
    {
        go.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        go.SetActive(true);
        go.GetComponent<VehicleNavigation>().Start();
    }
}
