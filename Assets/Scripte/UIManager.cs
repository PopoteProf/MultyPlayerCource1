using UnityEngine;

public class UIManager : MonoBehaviour
{
     public UIConnectionPanel UIConnectionPanel;
     public UIPlayerName UIPlayerName;
     public UIDeathCountDown UIDeathCountDown;
    
    public static UIManager Instance;
    

    private void Start()
    {
        Instance = this;
    }
}