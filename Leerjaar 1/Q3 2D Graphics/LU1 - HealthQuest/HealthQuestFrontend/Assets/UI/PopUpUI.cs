using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PopUpUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button doneButton;
    public Button closeButton;

    public void Setup(string title, string description, Action onDone, Action onClose)
    {
        titleText.text = title;
        descriptionText.text = description;
        doneButton.onClick.AddListener(() => onDone?.Invoke());
        closeButton.onClick.AddListener(() => onClose?.Invoke());
    }
}
