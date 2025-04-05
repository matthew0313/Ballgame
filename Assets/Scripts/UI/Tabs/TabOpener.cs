using UnityEngine;

public class TabOpener : MonoBehaviour
{
    Tab openTab;
    private void OnDisable()
    {
        if (openTab != null) openTab.CloseTab();
        openTab = null;
    }
    public void OpenTab(Tab tab)
    {
        if (openTab != null) openTab.CloseTab();
        openTab = tab;
        if (openTab != null) openTab.OpenTab();
    }
    public void CloseTab()
    {
        OpenTab(null);
    }
 }
