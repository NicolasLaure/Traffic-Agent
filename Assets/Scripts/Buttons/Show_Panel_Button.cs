using UnityEngine;

public class Show_Panel_Button : MonoBehaviour
{
    // The ID of the panel we want to show
    public string panelID;
    [SerializeField] private Panel_Manager _panel_Manager;

    private void Start()
    {
        _panel_Manager = Panel_Manager.Instance;
    }
    public void DoShowPanel() 
    {
        _panel_Manager.ShowPanel(panelID);
    }
}
