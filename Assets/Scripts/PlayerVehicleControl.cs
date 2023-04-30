using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(VehicleNavigation))]
public class PlayerVehicleControl : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int destination;
    public float deliveryTime;

    VehicleNavigation vehicleNavigation;
    bool turnLeft = false;
    bool turnRight = false;

    bool returning = false;

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

    public void OnPointerClick(PointerEventData eventData)
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

    void Update()
    {
        if (vehicleNavigation.reachedNode)
        {
            if (vehicleNavigation.curNode == destination)
            {
                if (returning)
                {
                    //Returned back to the start
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
                    //Reached the destination
                    DeliveryGame.instance.winCondition--;
                    returning = true;
                    Vector2Int temp = vehicleNavigation.gridStart;
                    vehicleNavigation.gridStart = destination;
                    destination = temp;
                    vehicleNavigation.movementDir = leftTurnConversions[leftTurnConversions[vehicleNavigation.movementDir]];
                    vehicleNavigation.mapGrid.grid[vehicleNavigation.curNode.x, vehicleNavigation.curNode.y].obj.GetComponent<NodeCycle>().SetWeakOccupied(false, gameObject);
                    vehicleNavigation.mapGrid.RespawnVehicle(gameObject, deliveryTime);
                }
            }

            if (turnLeft)
            {
                if (vehicleNavigation.IsNextNodeDestroyer(leftTurnConversions[vehicleNavigation.movementDir]) == false)
                {
                    vehicleNavigation.movementDir = leftTurnConversions[vehicleNavigation.movementDir];
                    turnLeft = false;
                }
            }
            else if (turnRight)
            {
                if (vehicleNavigation.IsNextNodeDestroyer(rightTurnConversions[vehicleNavigation.movementDir]) == false)
                {
                    vehicleNavigation.movementDir = rightTurnConversions[vehicleNavigation.movementDir];
                    turnRight = false;
                }
            }
        }
    }
}
