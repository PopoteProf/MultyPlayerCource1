using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerName:MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldPlayerName;
    [SerializeField] private Button _bpValidate;

    public EventHandler<FixedString32Bytes> OnPlayerValidateName;

    public void Start()
    {
        _bpValidate.onClick.AddListener(ValidatePlayerName);
    }

    private void ValidatePlayerName() {
        if (String.IsNullOrEmpty(_inputFieldPlayerName.text)) {
            OnPlayerValidateName.Invoke(this, "Player");
        }
        else
        {
            OnPlayerValidateName.Invoke(this, _inputFieldPlayerName.text);
        }
        gameObject.SetActive(false);
    }
}