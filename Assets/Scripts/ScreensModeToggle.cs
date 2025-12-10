using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public GameObject tableCanvas;
    public GameObject playerCanvas;

    public enum UIMode { Table, Player }
    public UIMode startMode = UIMode.Table;

    void Start()
    {
        SetMode(startMode);
    }

    public void SetMode(UIMode mode)
    {
        tableCanvas.SetActive(mode == UIMode.Table);
        playerCanvas.SetActive(mode == UIMode.Player);
    }
}
