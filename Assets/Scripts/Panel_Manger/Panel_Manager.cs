using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Panel_Manager : Singleton<Panel_Manager>
{
    [SerializeField] List<Panel_Model> panels;

    // This holds all of our instances
    private List<Panel_Instance_Model> _ListInstances = new List<Panel_Instance_Model>();
    public void ShowPanel(string current_panelID) 
    {
        Panel_Model panel_Model = panels.FirstOrDefault(panel => panel.panelID == current_panelID);

        if (panel_Model != null) 
        {
            var newInstancePanel = Instantiate(panel_Model.panelPrefab,transform);

            newInstancePanel.transform.localPosition = Vector3.zero;

            // Add this new Panel to the queue
            _ListInstances.Add(new Panel_Instance_Model {panelID = current_panelID,panelInstace = newInstancePanel});
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

            Destroy(lastPanel.panelInstace);
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
