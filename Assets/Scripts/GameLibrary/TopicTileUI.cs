using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TopicTileUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text mainTitleText;
    [SerializeField] private TMP_Text subTitleText;

    private string _themeId;

    public void Bind(string themeId, string mainTitle, string subTitle, Sprite image, Action<string> onClick)
    {
        _themeId = themeId;

        mainTitleText.text = mainTitle;
        subTitleText.text = subTitle;

        if (backgroundImage != null)
        {
            backgroundImage.sprite = image;
            backgroundImage.enabled = image != null;
            backgroundImage.preserveAspect = true;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(_themeId));
    }
}
