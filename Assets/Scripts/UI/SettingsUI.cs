using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public void SaveSettings()
    {
        GlobalManager.Instance.SaveSettings();
    }
}