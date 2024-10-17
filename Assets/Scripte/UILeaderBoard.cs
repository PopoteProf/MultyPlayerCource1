using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UILeaderBoard : MonoBehaviour
{
    [SerializeField] private UIPlayerLeaderBoardPanel _prefabPlayerLeaderBoardPanel;

    public void ClearLeaderboard() {
        for (int i = transform.childCount-1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void PopulateLeaderboard(SerializablePlayerData[] datas) {
        ClearLeaderboard();
        foreach (var data in datas) {
            UIPlayerLeaderBoardPanel panel = Instantiate(_prefabPlayerLeaderBoardPanel, transform);
            panel.SetPlayerData(data);
        }
    }
}