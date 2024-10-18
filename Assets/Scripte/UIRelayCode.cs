using TMPro;
using UnityEngine;

public class UIRelayCode: MonoBehaviour
{
    [SerializeField] private TMP_Text _txtRelayCode;

    public void SetRelayCode(string txt) {
        _txtRelayCode.text = txt;
    }
}