using TMPro;
using UnityEngine;

public class UIDeathCountDown: MonoBehaviour {
    [SerializeField] private TMP_Text _txtCountDown;

    public void SetNewText(string text) => _txtCountDown.text = text;
}