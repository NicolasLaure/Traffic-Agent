using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Panel_Manager : Singleton<Panel_Manager>
{

    // This holds all of our instances
    private List<Panel_Instance_Model> _ListInstances = new List<Panel_Instance_Model>();

    // Pool of panels
    private Object_Pool _object_Pool;

    private void Start()
    {
        _object_Pool = Object_Pool.Instance;
    }

    public void ShowPanel(string current_panelID) 
    {
        GameObject panel_Instance = _object_Pool.GetObjectFromPool(current_panelID);

        if (panel_Instance != null) 
        {
            // Add this new Panel to the list
            _ListInstances.Add(new Panel_Instance_Model {panelID = current_panelID,panelInstace = panel_Instance});
        }
        else
        {
            Debug.LogWarning($"Trying to use a panel ID = {current_panelID}, but this is not found in panels");
        }
    }

    public void HideLastPanel() 
    {
        if (AnyPanelShowing())
        {
            // Get the last element added to the list
            var lastPanel = _ListInstances[_ListInstances.Count - 1];
            _ListInstances.Remove(lastPanel);

            _object_Pool.PoolObject(lastPanel.panelInstace);
        }
    }


    // Returns true if any panel is showing
    public bool AnyPanelShowing() 
    {
        return GetAmountPanelsInQueue() > 0;
    }


    // Returns how many panels we have in the list
    public int GetAmountPanelsInQueue() 
    {
        return _ListInstances.Count;
    }
}
