using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleNavigation : MonoBehaviour
{
    public Vector2Int gridStart;
    public float movementSpeed = 10;
    public Direction movementDir;
    public MapGrid.GridState[] blockers;
    public MapGrid.GridState[] destroyers;
    public float deathDist = 0.5f;

    [HideInInspector]
    public MapGrid mapGrid;
    [HideInInspector]
    public bool reachedNode;
    [HideInInspector]
    public Vector2Int curNode;

    float movementProgress = 0;

    bool willDie = false;
    bool blocked = false;


    [System.Serializable]
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    Dictionary<Direction, Vector2Int> directions = new Dictionary<Direction, Vector2Int> {
        {Direction.UP, new Vector2Int(0, -1) },
        {Direction.DOWN, new Vector2Int(0, 1) },
        {Direction.LEFT, new Vector2Int(-1, 0) },
        {Direction.RIGHT, new Vector2Int(1, 0) }
    };

    private void Start()
    {
        mapGrid = gameObject.transform.parent.GetComponent<MapGrid>();
        reachedNode = true;
        curNode = new Vector2Int(gridStart.x, gridStart.y);
        gameObject.transform.position = mapGrid.grid[curNode.x, curNode.y].obj.transform.position;
    }

    void Update()
    {
        string state = "";
        if (blocked)
            state = "blocked";
        else if (reachedNode)
            state = "reachedNode";
        else if (movementProgress == 0)
            state = "beginMovement";
        else
            state = "movement";

        switch (state)
        {
            //If the destination is blocked, then check if it's been unblocked
            case "blocked":
                if (IsNextNodeBlocker(movementDir))
                {
                    Blocked();
                }
                else
                {
                    if (IsNextNodeDestroyer(movementDir))
                    {
                        Destroyed();
                    }
                    blocked = false;
                }
                break;

            //If a new movement is about to commence
            case "reachedNode":
                reachedNode = false;
                break;

            //If a new movement is beginning, check ahead to see if it's allowed
            case "beginMovement":
                if (IsNextNodeBlocker(movementDir))
                {
                    Blocked();
                }
                else
                {
                    if (IsNextNodeDestroyer(movementDir))
                    {
                        Destroyed();
                    }
                    movementProgress = Mathf.Min(movementSpeed + movementProgress, 100);
                }
                break;

            //If during a movement, move
            case "movement":
                GetNode(movementDir);
                Vector2 prevPos = mapGrid.grid[curNode.x, curNode.y].obj.transform.position;
                Vector2 nextPos = mapGrid.grid[curNode.x + directions[movementDir].x, curNode.y + directions[movementDir].y].obj.transform.position;
                gameObject.transform.position = Vector2.Lerp(prevPos, nextPos, movementProgress * 0.01f);

                if (willDie == true && movementProgress * 0.01f > deathDist)
                {
                    //Death code
                    Destroy(gameObject);
                }

                movementProgress = Mathf.Min(movementSpeed + movementProgress, 100);
                if (movementProgress >= 100)
                {
                    movementProgress = 0;
                    curNode = new Vector2Int(curNode.x + directions[movementDir].x, curNode.y + directions[movementDir].y);
                    reachedNode = true;
                }
                break;
        }
    }

    //Checks to see if the desired destination blocks this vehicle
    public bool IsNextNodeBlocker(Direction dir)
    {
        MapGrid.GridState nextNodeType = GetNode(dir);
        foreach (MapGrid.GridState t in blockers)
        {
            if (nextNodeType == t)
            {
                return true;
            }
        }
        return false;
    }

    //Checks to see if the desired destination will destroy this vehicle
    public bool IsNextNodeDestroyer(Direction dir)
    {
        MapGrid.GridState nextNodeType = GetNode(dir);
        foreach (MapGrid.GridState t in destroyers)
        {
            if (nextNodeType == t)
            {
                return true;
            }
        }
        return false;
    }

    //Gets the next node in the movement direction
    public MapGrid.GridState GetNode(Direction dir)
    {
        int x = curNode.x + directions[dir].x;
        int y = curNode.y + directions[dir].y;
        if (x >= mapGrid.grid.GetLength(0) || y >= mapGrid.grid.GetLength(1))
        {
            return MapGrid.GridState.OFF_MAP;
        }
        return mapGrid.grid[x, y].gridState;
    }

    //If aiming at a blocked node, stop movement
    public void Blocked()
    {
        reachedNode = true;
        blocked = true;
    }

    //If aiming at a node that'll destroy this vehicle, flag this vehicle to be destroyed after advancing a bit more (deathDist)
    public void Destroyed()
    {
        willDie = true;
    }
}
