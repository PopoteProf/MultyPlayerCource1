using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectionPanel : MonoBehaviour
{
    [SerializeField] private Button _bpHost;
    [SerializeField] private Button _bpServer;
    [SerializeField] private Button _bpJoin;
    [SerializeField] private Button _bpRelay;
    [SerializeField] private Button _bpJoinRelay;
    [SerializeField] private TMP_InputField _inputFieldRelayCode;
    
    void Start() {
        _bpHost.onClick.AddListener(OnHostClicked);
        _bpServer.onClick.AddListener(OnServerClicked);
        _bpJoin.onClick.AddListener(OnJoinClicked);
        _bpRelay.onClick.AddListener(ManageRelay);
        _bpJoinRelay.onClick.AddListener(JoinRelay);
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

    private async Task<Allocation> AllocationRelay() {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(20);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async void ManageRelay()
    {

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        
        Allocation allocation =await AllocationRelay();
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("Relay join Code = "+ joinCode);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        UIManager.Instance.UIRelayCode.SetRelayCode(joinCode);
        OnHostClicked();
    }

    private async void JoinRelay() {
        if (string.IsNullOrEmpty(_inputFieldRelayCode.text)) return;

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        string joincode = _inputFieldRelayCode.text;
        Debug.Log("Try to join with code"+ joincode);
        

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joincode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log("Connection done !");
            OnJoinClicked();
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}