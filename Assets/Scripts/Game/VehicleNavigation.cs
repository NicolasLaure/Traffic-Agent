using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleNavigation : MonoBehaviour
{
    [System.Serializable]
    public struct ConditionalBlocker
    {
        public MapGrid.GridState blocker;
        public Direction passCondition;
    }

    public Vector2Int gridStart;
    public float movementSpeed = 10;
    public Direction movementDir;
    public ConditionalBlocker[] blockers;
    public MapGrid.GridState[] destroyers;
    public float deathDist = 0.5f;
    public bool destroysVehicles = true;
    public bool respawns = false;
    public float respawnTime = 1.0f;

    [HideInInspector]
    public MapGrid mapGrid;
    [HideInInspector]
    public bool reachedNode;
    [HideInInspector]
    public Vector2Int curNode;
    [HideInInspector]
    public bool invincibility = false;

    float movementProgress = 0;

    bool willDie = false;
    bool blocked = false;
    bool collisionChecked = false;

    bool offMap = false;
    bool oob = false;

    [HideInInspector]
    public bool destroyed = false;


    [System.Serializable]
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    public static Dictionary<Direction, Vector2Int> directions = new Dictionary<Direction, Vector2Int> 
    {
        {Direction.UP, new Vector2Int(0, -1) },
        {Direction.DOWN, new Vector2Int(0, 1) },
        {Direction.LEFT, new Vector2Int(-1, 0) },
        {Direction.RIGHT, new Vector2Int(1, 0) },
        {Direction.NONE, new Vector2Int(0, 0) }
    };

    public void Start()
    {
        invincibility = false;
        movementProgress = 0;
        offMap = false;
        oob = false;
        destroyed = false;
        willDie = false;
        blocked = false;
        collisionChecked = false;
        mapGrid = gameObject.transform.parent.GetComponent<MapGrid>();
        reachedNode = false;
        curNode = new Vector2Int(gridStart.x, gridStart.y);
        if (gameObject.GetComponent<PlayerVehicleControl>())
        {
            gameObject.GetComponent<PlayerVehicleControl>().Setup();
        }
        gameObject.transform.position = mapGrid.grid[curNode.x, curNode.y].obj.transform.position;

        //Unless this vehicle is spawning from an inaccesible node, mark the spawn node as occupied
        if (IsNextNodeDestroyer(Direction.NONE) == false)
        {
            //If this vehicle is a destroyer, mark the node it's entering as occupied
            if (destroysVehicles == true)
            {
                mapGrid.grid[curNode.x, curNode.y].obj.GetComponent<NodeCycle>().SetOccupied(true);
            }
            else
            {
                //If this vehicle is not a destroyer, mark as weakly occupied
                mapGrid.grid[curNode.x, curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(true, gameObject);
            }
        }
    }

    void Update()
    {
        string state = "";
        if (offMap || destroyed)
            state = "none";
        else if (blocked)
            state = "blocked";
        else if (reachedNode)
            state = "reachedNode";
        else if (movementProgress == 0)
            state = "beginMovement";
        else
            state = "movement";

        switch (state)
        {
            case "none":
                break;

            //If the destination is blocked, then check if it's been unblocked
            case "blocked":
                if (IsNextNodeBlocking(movementDir))
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
                if (IsNextNodeBlocking(movementDir))
                {
                    Blocked();
                }
                else
                {
                    if (IsNextNodeDestroyer(movementDir))
                    {
                        Destroyed();
                    }
                    movementProgress = Mathf.Min(movementSpeed * Time.deltaTime + movementProgress, 100);
                }
                break;

            //If during a movement, move
            case "movement":
                GetNode(movementDir);
                Vector2 prevPos = mapGrid.grid[curNode.x, curNode.y].obj.transform.position;
                Vector2 nextPos = mapGrid.grid[curNode.x + directions[movementDir].x, curNode.y + directions[movementDir].y].obj.transform.position;
                gameObject.transform.position = Vector2.Lerp(prevPos, nextPos, movementProgress * 0.01f);

                if (movementProgress * 0.01f > deathDist)
                {
                    //Unoccupy the previous node
                    if (destroysVehicles == true)
                    {
                        mapGrid.grid[curNode.x, curNode.y].obj.GetComponent<NodeCycle>().SetOccupied(false);
                    }
                    else
                    {
                        mapGrid.grid[curNode.x, curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(false, gameObject);
                    }

                    if (!invincibility)
                    {
                        //If set to die, or the node this vehicle is entering is suddenly blocked by a destroyer, kill this vehicle
                        if (willDie == true || IsNextNodeDestroyer(movementDir))
                        {
                            Destruction();
                        }
                        else
                        {
                            //If this vehicle is a destroyer, mark the node it's entering as occupied
                            if (destroysVehicles == true)
                            {
                                mapGrid.grid[curNode.x + directions[movementDir].x, curNode.y + directions[movementDir].y].obj.GetComponent<NodeCycle>().SetOccupied(true);
                            }
                            else
                            {
                                //If this vehicle is not a destroyer, check once upon entering the space if a player is already there, and if so die, otherwise mark as weakly occupied
                                if (collisionChecked == false)
                                {
                                    if (GetNode(movementDir) == MapGrid.GridState.OCCUPIED_WEAK)
                                    {
                                        willDie = true;
                                        Destruction();
                                    }
                                    else
                                    {
                                        mapGrid.grid[curNode.x + directions[movementDir].x, curNode.y + directions[movementDir].y].obj.GetComponent<NodeCycle>().SetWeakOccupied(true, gameObject);
                                    }
                                    collisionChecked = true;
                                }
                            }
                        }
                    }
                }

                movementProgress = Mathf.Min(movementSpeed * Time.deltaTime + movementProgress, 100);
                if (movementProgress >= 100)
                {
                    movementProgress = 0;
                    curNode = new Vector2Int(curNode.x + directions[movementDir].x, curNode.y + directions[movementDir].y);
                    reachedNode = true;
                    collisionChecked = false;
                }
                break;
        }
    }

    //Checks to see if the desired destination blocks this vehicle
    public bool IsNextNodeBlocking(Direction dir)
    {
        if (invincibility)
            return false;

        MapGrid.GridState nextNodeType = GetNode(dir);

        foreach (ConditionalBlocker t in blockers)
        {
            if (nextNodeType == t.blocker && dir == t.passCondition)
            {
                return true;
            }
        }
        return false;
    }

    //Checks to see if the desired destination is a blocker, ignoring the condition
    public bool IsNextNodeBlocker(Direction dir)
    {
        if (invincibility)
            return false;

        MapGrid.GridState nextNodeType = GetNode(dir);

        foreach (ConditionalBlocker t in blockers)
        {
            if (nextNodeType == t.blocker)
            {
                return true;
            }
        }
        return false;
    }

    //Checks to see if the desired destination will destroy this vehicle
    public bool IsNextNodeDestroyer(Direction dir)
    {
        if (invincibility)
            return false;

        MapGrid.GridState nextNodeType = GetNode(dir);

        if (nextNodeType == MapGrid.GridState.TRAFFICLIGHTHOR && (dir == Direction.LEFT || dir == Direction.RIGHT))
            return false;
        else if (nextNodeType == MapGrid.GridState.TRAFFICLIGHTVER && (dir == Direction.UP || dir == Direction.DOWN))
            return false;

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
        if (GetNode(movementDir) == MapGrid.GridState.OFF_MAP)
        {
            oob = true;
        }
    }

    public void Destruction()
    {
        if (destroyed == false)
        {
            if (respawns == true)
            {
                offMap = true;
                mapGrid.RespawnVehicle(gameObject, respawnTime);
            }
            else
            {
                if (oob == false && gameObject.GetComponent<Animator>() != null)
                {
                    gameObject.GetComponent<PlayerVehicleControl>().SwitchAnim("destruction");
                    destroyed = true;
                }
                else
                {
                    if (gameObject.GetComponent<PlayerVehicleControl>() != null)
                    {
                        DeliveryGame.instance.EndGame();
                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    //Made to be run from an animation event
    public void FinishedDestruction()
    {
        if (gameObject.GetComponent<PlayerVehicleControl>() != null)
        {
            DeliveryGame.instance.EndGame();
        }
        Destroy(gameObject);
    }
}
