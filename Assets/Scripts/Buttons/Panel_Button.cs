using UnityEngine;

public class Panel_Button : MonoBehaviour
{
    // The ID of the panel we want to show
    public string panelID;
    private Panel_Manager _panel_Manager;
    public Panel_Behaviour _Behaviour;

    private void Start()
    {
        _panel_Manager = Panel_Manager.Instance;
    }
    public void DoShowPanel() 
    {
        _panel_Manager.ShowPanel(panelID, _Behaviour);
    }

    public void DoHidePanel() 
    {
        _panel_Manager.HideLastPanel();
    }
}
