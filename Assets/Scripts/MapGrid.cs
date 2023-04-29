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
        BUILDING
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
    public Vector2 gridSpacing = new Vector2(100, 100);
    public Vector2 gridOffset = new Vector2(0, 0);
    public GameObject defaultNode;

    //Previous values of the public variables, used to detect when one has changed
    [SerializeField]
    int prevColumns = 3;
    [SerializeField]
    int prevRows = 3;
    [SerializeField]
    Vector2 prevGridSpacing = new Vector2(100, 100);
    [SerializeField]
    Vector2 prevGridOffset = new Vector2(0, 0);

    //Public variables hidden from the Inspector
    [HideInInspector]
    public GameObject gridParent;
    [HideInInspector] 
    public Node[,] grid;

    //A 1D array used for serialization purposes
    [SerializeField] 
    Node[] serializableGrid;


#if UNITY_EDITOR
    //This weird workaround is to avoid a spam warning
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }

    //Whenever this script is loaded, or a variable is changed, the following function is called
    void _OnValidate()
    {
        if (this == null) return;
        //Raise an error if the default node hasn't been assigned
        if (defaultNode == null)
        {
            Debug.LogWarning("ERROR: No default node prefab specified");
            return;
        }

        //Raise an error if the grid is ever null for some reason
        if (grid == null)
        {
            Debug.LogError("ERROR: Grid was null");
            clearGrid = true;
        }

        Vector2 prevPos = new Vector3(0, 0);

        //If the grid is set to be cleared, clear it and reset everything
        if (clearGrid == true)
        {
            //If a grid parent exists, store its location in order to assign it to the new one that'll be created
            if (gridParent != null)
            {
                prevPos = gridParent.transform.localPosition;
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
            gridParent = new GameObject("Grid");
            gridParent.transform.parent = gameObject.transform;
            gridParent.transform.localPosition = prevPos;
        }

        //If the amount of rows or columns in the grid have changed, update the grid accordingly
        if (prevColumns != columns || prevRows != rows)
        {
            Node[,] tempGrid = new Node[columns, rows];

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (i < grid.GetLength(0) && j < grid.GetLength(1))
                    {
                        grid[i, j].obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                        tempGrid[i, j] = grid[i, j];
                    }
                    else
                    {
                        Node tempNode = new Node(GridState.ROAD, i, j, Instantiate(defaultNode, gridParent.transform), this);
                        tempNode.obj.name = tempNode.obj.GetComponent<NodeCycle>().nodeName + " (" + i + "; " + j + ")";
                        if (j > 0)
                        {
                            tempNode.obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                        }
                        else
                        {
                            if (i > 0)
                            {
                                tempNode.obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                            }
                            else
                            {
                                tempNode.obj.transform.localPosition = new Vector2(0, 0) + gridOffset;
                            }
                        }
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
            if (gridSpacing != prevGridSpacing || gridOffset != prevGridOffset)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        grid[i, j].obj.transform.localPosition = new Vector2(gridSpacing.x * i, gridSpacing.y * j * -1) + gridOffset;
                    }
                }
                prevGridSpacing = gridSpacing;
                prevGridOffset = gridOffset;
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
        Debug.Log(prevColumns + " ; " + prevRows);
        grid = new Node[prevColumns, prevRows];
        for (int i = 0; i < serializableGrid.Length; i++)
        {
            grid[(i / prevRows), (i % prevRows)] = serializableGrid[i];
        }
    }
}
