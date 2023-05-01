using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(VehicleNavigation))]
public class PlayerVehicleControl : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public struct Destination
    {
        public Vector2Int node;
        public VehicleNavigation.Direction destinationDir;
        public GameObject highlighted;
        public GameObject normal;
    }

    public List<Destination> destinations = new List<Destination>();
    public float deliveryTime;
    public VehicleNavigation.Direction startDir;
    VehicleNavigation.Direction savedDir;
    string currentAnim = "default";
    Destination curDestination;

    VehicleNavigation vehicleNavigation;
    bool turnLeft = false;
    bool turnRight = false;

    bool delivering = true;

    Dictionary<VehicleNavigation.Direction, VehicleNavigation.Direction> leftTurnConversions = new Dictionary<VehicleNavigation.Direction, VehicleNavigation.Direction> {
        {VehicleNavigation.Direction.UP, VehicleNavigation.Direction.LEFT },
        {VehicleNavigation.Direction.DOWN, VehicleNavigation.Direction.RIGHT },
        {VehicleNavigation.Direction.LEFT, VehicleNavigation.Direction.DOWN },
        {VehicleNavigation.Direction.RIGHT, VehicleNavigation.Direction.UP }
    };

    Dictionary<VehicleNavigation.Direction, VehicleNavigation.Direction> rightTurnConversions = new Dictionary<VehicleNavigation.Direction, VehicleNavigation.Direction> {
        {VehicleNavigation.Direction.UP, VehicleNavigation.Direction.RIGHT },
        {VehicleNavigation.Direction.DOWN, VehicleNavigation.Direction.LEFT },
        {VehicleNavigation.Direction.LEFT, VehicleNavigation.Direction.UP },
        {VehicleNavigation.Direction.RIGHT, VehicleNavigation.Direction.DOWN }
    };

    void Start()
    {
        vehicleNavigation = gameObject.GetComponent<VehicleNavigation>();
    }

    public void Setup()
    {
        DeliveryGame.instance.AddEngine();
        //In order to have the player vehicle leave from a building, this controls the vehicle for the player for 1 node
        delivering = true;
        if (vehicleNavigation.movementDir == leftTurnConversions[startDir])
            turnLeft = true;
        else if (vehicleNavigation.movementDir == rightTurnConversions[startDir])
            turnRight = true;

        vehicleNavigation.curNode = vehicleNavigation.curNode + VehicleNavigation.directions[leftTurnConversions[leftTurnConversions[startDir]]];
        vehicleNavigation.movementDir = startDir;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //If not currently delivering (auto-controlled movement), clicks determine whether this vehicle turns or not
        if (!delivering)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                turnLeft = true;
                turnRight = false;
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                turnLeft = false;
                turnRight = false;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                turnLeft = false;
                turnRight = true;
            }
        }
    }

    void Update()
    {
        if (vehicleNavigation.reachedNode)
        {
            if (delivering)
            {
                delivering = false;
            }

            //If the destination is reached, set invincibility and the stuff needed to have the vehicle be auto controlled into the building
            if (IsDestination(vehicleNavigation.curNode) >= 0)
            {
                delivering = true;
                vehicleNavigation.invincibility = true;
                savedDir = vehicleNavigation.movementDir;
                curDestination = destinations[IsDestination(vehicleNavigation.curNode)];
                vehicleNavigation.movementDir = curDestination.destinationDir;
                turnLeft = false;
                turnRight = false;
            }
            else
            {
                //If the vehicle has finished being auto-controlled into the building, then do stuff depending on if this is the return trip or not
                if (vehicleNavigation.curNode == curDestination.node + VehicleNavigation.directions[curDestination.destinationDir])
                {
                    //If this is the final destination, remove the player vehicle from the game
                    if (destinations.Count <= 1)
                    {
                        DeliveryGame.instance.winCondition--;
                        if (DeliveryGame.instance.winCondition == 0)
                        {
                            DeliveryGame.instance.EndGame();
                        }

                        vehicleNavigation.mapGrid.grid[vehicleNavigation.curNode.x, vehicleNavigation.curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(false, gameObject);
                        Destroy(gameObject);
                    }
                    else
                    {
                        //If not, set everything up to mimic spawning in at the start, but from the destination this time, and remove this destination from the list
                        DeliveryGame.instance.winCondition--;
                        Vector2Int temp = vehicleNavigation.gridStart;
                        vehicleNavigation.gridStart = curDestination.node;
                        startDir = leftTurnConversions[leftTurnConversions[vehicleNavigation.movementDir]];
                        vehicleNavigation.movementDir = savedDir;
                        vehicleNavigation.mapGrid.grid[vehicleNavigation.curNode.x, vehicleNavigation.curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(false, gameObject);
                        delivering = true;
                        DeliveryGame.instance.RemoveEngine();
                        for (int i = 0; i < destinations.Count; i++)
                        {
                            if (destinations[i].node == curDestination.node)
                            {
                                destinations.RemoveAt(i);
                                break;
                            }
                        }
                        gameObject.GetComponent<VehicleNavigation>().mapGrid.Deliver(curDestination, deliveryTime);
                        curDestination = new Destination();

                        vehicleNavigation.mapGrid.RespawnVehicle(gameObject, deliveryTime);
                    }
                }
            }

            /*
            //============ RETURN TRIP VERSION ============

            //If the destination is reached, set invincibility and the stuff needed to have the vehicle be auto controlled into the building
            if (IsDestination(vehicleNavigation.curNode) == true)
            {
                delivering = true;
                vehicleNavigation.invincibility = true;
                savedDir = vehicleNavigation.movementDir;
                vehicleNavigation.movementDir = destinationDir;
                turnLeft = false;
                turnRight = false;
            }
            else
            {
                //If the vehicle has finished being auto-controlled into the building, then do stuff depending on if this is the return trip or not
                if (vehicleNavigation.curNode == destination + VehicleNavigation.directions[destinationDir])
                {
                    //If this is the return trip, remove the player vehicle from the game
                    if (returning)
                    {
                        DeliveryGame.instance.winCondition--;
                        if (DeliveryGame.instance.winCondition == 0)
                        {
                            DeliveryGame.instance.EndGame();
                        }

                        vehicleNavigation.mapGrid.grid[vehicleNavigation.curNode.x, vehicleNavigation.curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(false, gameObject);
                        Destroy(gameObject);
                    }
                    else
                    {
                        //If not, set everything up to mimic spawning in at the start, but from the destination this time, and set the original start as the new destination
                        DeliveryGame.instance.winCondition--;
                        Vector2Int temp = vehicleNavigation.gridStart;
                        vehicleNavigation.gridStart = destination;
                        destination = temp;
                        destinationDir = leftTurnConversions[leftTurnConversions[startDir]];
                        startDir = leftTurnConversions[leftTurnConversions[vehicleNavigation.movementDir]];
                        vehicleNavigation.movementDir = leftTurnConversions[leftTurnConversions[savedDir]];
                        vehicleNavigation.mapGrid.grid[vehicleNavigation.curNode.x, vehicleNavigation.curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(false, gameObject);
                        returning = true;
                        delivering = true;
                        DeliveryGame.instance.RemoveEngine();

                        vehicleNavigation.mapGrid.RespawnVehicle(gameObject, deliveryTime);
                    }
                }
            }
            */

            //If this vehicle was set to turn left, turn left, unless there's a destroyer node to the left, in which case wait until there isn't one
            if (turnLeft)
            {
                if (vehicleNavigation.IsNextNodeDestroyer(leftTurnConversions[vehicleNavigation.movementDir]) == false)
                {
                    vehicleNavigation.movementDir = leftTurnConversions[vehicleNavigation.movementDir];
                    turnLeft = false;
                }
            }
            //Same as left, but right this time
            else if (turnRight)
            {
                if (vehicleNavigation.IsNextNodeDestroyer(rightTurnConversions[vehicleNavigation.movementDir]) == false)
                {
                    vehicleNavigation.movementDir = rightTurnConversions[vehicleNavigation.movementDir];
                    turnRight = false;
                }
            }
        }

        SwitchAnim(vehicleNavigation.movementDir, turnLeft, turnRight);
    }

    public int IsDestination(Vector2Int node)
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            if (destinations[i].node == node)
            {
                return i;
            }
        }
        return -1;
    }

    public void SwitchAnim(VehicleNavigation.Direction dir, bool left, bool right)
    {
        string clipName = "";
        switch (dir)
        {
            case VehicleNavigation.Direction.UP:
                clipName = "up";
                break;
            case VehicleNavigation.Direction.DOWN:
                clipName = "down";
                break;
            case VehicleNavigation.Direction.LEFT:
                clipName = "left";
                break;
            case VehicleNavigation.Direction.RIGHT:
                clipName = "right";
                break;
            default:
                SwitchAnim("default");
                return;
        }

        if (turnLeft)
            clipName += "Left";
        else if (turnRight)
            clipName += "Right";

        SwitchAnim(clipName);
    }

    public void SwitchAnim(string clipName)
    {
        if (clipName != currentAnim && gameObject.activeInHierarchy == true)
        {
            Animator anim = gameObject.GetComponent<Animator>();
            anim.Play(clipName, 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            currentAnim = clipName;
        }
    }
}
