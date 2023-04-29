using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Panel_Manager : Singleton<Panel_Manager>
{
    [SerializeField] List<Panel_Model> panels;

    // This holds all of our instances
    private Queue<Panel_Instance_Model> _queue = new Queue<Panel_Instance_Model>();
    public void ShowPanel(string current_panelID) 
    {
        Panel_Model panel_Model = panels.FirstOrDefault(panel => panel.panelID == current_panelID);

        if (panel_Model != null) 
        {
            var newInstancePanel = Instantiate(panel_Model.panelPrefab,transform);

            newInstancePanel.transform.localPosition = Vector3.zero;

            // Add this new Panel to the queue
            _queue.Enqueue(new Panel_Instance_Model {panelID = current_panelID,panelInstace = newInstancePanel});
        }
        else
        {
            Debug.LogWarning($"Trying to use a panel ID = {current_panelID}, but this is not found in panels");
        }
    }
}
