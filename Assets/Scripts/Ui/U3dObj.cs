using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class U3dObj
{
    public GameObject gameObject;
    public Transform transform;

    public U3dObj(GameObject gameObject)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
    }
    public U3dObj(Transform transform)
    {
        this.transform = transform;
        gameObject = transform.gameObject;
    }

    public RectTransform RectTransform => transform.GetComponent<RectTransform>();

    public LoopScrollView LoopScrollView => transform.GetComponentInChildren<LoopScrollView>(true);

    public ItemView ItemView => transform.GetComponentInChildren<ItemView>(true);

    public Image Image => transform.GetComponentInChildren<Image>(true);

    public Text Text => transform.GetComponentInChildren<Text>(true);

    //public TextMeshProUGUI TextMeshPro => transform.GetComponentInChildren<TextMeshProUGUI>();

    public Slider Slider => transform.GetComponentInChildren<Slider>(true);

    public Animator Animator => transform.GetComponentInChildren<Animator>(true);

    public Button Button => transform.GetComponentInChildren<Button>(true);

    public Toggle Toggle => transform.GetComponent<Toggle>();
    public InputField InputField => transform.GetComponentInChildren<InputField>(true);
    //-------------------------------

    public void SetParent(Transform parent, bool worldPositionStays = false)
    {
        transform.SetParent(parent, worldPositionStays);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public Transform FindTrans(string name)
    {
        return transform.Find(name);
    }

    public GameObject FindGo(string name)
    {
        return transform.Find(name).gameObject;
    }

    public U3dObj Find(string path) {
        return new U3dObj(transform.Find(path));

    }

    public void SetSprite(string atlasPath, string name, bool isSetNativeSize = false, Action callback = null) {

        if (Image == null) {
            return;
        }

        ResMgr.instance.LoadSpriteAsync(atlasPath, name, (sprite) =>
        {
            Image.sprite = sprite;

            if (isSetNativeSize) {
                Image.SetNativeSize();
            }

            callback?.Invoke();
        });
    }

    public void SetSpriteRectBaseW(float w) {
        Image img = Image;

        if (img == null || img.sprite == null) {
            return;
        }

        float ratio = img.sprite.textureRect.width / w;
        float h = img.sprite.textureRect.height / ratio;

        img.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
    }

    public static Material UiGray = null;
    public void SetImgGray(bool isGray, bool isDeep = true) {
        if (UiGray == null) {
            ResMgr.instance.LoadMaterialAsync("MatImgGray", (mat) => {
                if (UiGray == null) {
                    UiGray = mat;
                }
                SetImgGray(isGray, isDeep);
            });
            return;
        }

        Image[] childs = null;

        if (isDeep)
        {
            childs = transform.GetComponentsInChildren<Image>(true);
        }
        else {
            childs = new Image[] { transform.GetComponent<Image>() };
        }

        foreach (var item in childs)
        {
            if (isGray)
            {
                item.material = UiGray;
            }
            else
            {
                item.material = null;
            }
        }
    }

    public static GameObject ImgRedPoint;

    public Dictionary<Transform, bool> DictRedPointStatus;

    public void SetRedPoint(bool isRed, Transform parent = null) {

        parent = parent == null ? transform : parent;
        DictRedPointStatus = DictRedPointStatus == null ? new Dictionary<Transform, bool>() : DictRedPointStatus;

        if (!DictRedPointStatus.TryAdd(parent, isRed)) {
            DictRedPointStatus[parent] = isRed;
        }

        Transform imgRed = transform.Find("ImgRedPoint");

        if (imgRed != null)
        {
            imgRed.SetActiveEx(isRed);
        }
        else {
            if (!isRed) {
                return;
            }

            if (ImgRedPoint != null)
            {
                imgRed = GameObject.Instantiate(ImgRedPoint).transform;

                imgRed.name = "ImgRedPoint";

                imgRed.SetParent(parent, false);
                imgRed.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                SetRedPoint(DictRedPointStatus[parent], parent);
            }
            else {
                ResMgr.instance.LoadAssetAsync<GameObject>("ImgRedPoint", (go) =>
                {
                    if (ImgRedPoint == null)
                    {
                        ImgRedPoint = go;
                    }
                    SetRedPoint(DictRedPointStatus[parent], parent);
                });

            }
        }
    }
}
