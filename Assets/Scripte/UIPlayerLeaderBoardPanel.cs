using TMPro;
using UnityEngine;

public class UIPlayerLeaderBoardPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _txtPlayerName;
    [SerializeField] private TMP_Text _txtKills;
    [SerializeField] private TMP_Text _txtDeath;
    
    public void SetPlayerData(SerializablePlayerData data) {
        _txtPlayerName.text = data.PlayerName.ToString();
        _txtKills.text = data.Kills.ToString();
        _txtDeath.text = data.Deaths.ToString();
    }
}