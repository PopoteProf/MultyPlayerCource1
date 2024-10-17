using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectionPanel : MonoBehaviour
{
    [SerializeField] private Button _bpHost;
    [SerializeField] private Button _bpServer;
    [SerializeField] private Button _bpJoin;
    void Start()
    {
        _bpHost.onClick.AddListener(OnHostClicked);
        _bpServer.onClick.AddListener(OnServerClicked);
        _bpJoin.onClick.AddListener(OnJoinClicked);
    }

    private void OnJoinClicked()
    {
        NetworkManager.Singleton.StartClient();
        ClosePanel();
    }

    private void OnServerClicked()
    {
        NetworkManager.Singleton.StartServer();
        ClosePanel();
    }

    private void OnHostClicked()
    {
        NetworkManager.Singleton.StartHost();
        ClosePanel();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}