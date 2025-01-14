using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SwitchToggle : MonoBehaviour
{
    public RectTransform handleRect;
    public float duration = 0.5f;
    private Vector2 handlePos;
    public Toggle Toggle;

    public Sprite ImgOnBg;
    public Sprite ImgOffBg;

    Image ImgBg;
    Text TxtOn;

    private void Awake()
    {
        Toggle = GetComponent<Toggle>();
        handlePos = handleRect.anchoredPosition;

        ImgBg = transform.Find("Background").GetComponent<Image>();
        TxtOn = transform.Find("Background/TxtOn").GetComponent<Text>();

    }

    public void OnSwitch(bool on)
    {
        handleRect.DOAnchorPos(on ? handlePos : -handlePos, duration).SetEase(Ease.InOutBack);
        ImgBg.sprite = on ? ImgOnBg : ImgOffBg;
        TxtOn.text = on ? "on" : "off";
        TxtOn.alignment = on ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
        Toggle.isOn = on;
    }
}
