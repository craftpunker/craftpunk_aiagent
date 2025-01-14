
using Battle;
using DG.Tweening;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleView : UiBase
{
    U3dObj CardsScrollView;

    private Slider playerHpSlider;
    private Slider enemyHpSlider;
    //private Slider CostSlider;
    private U3dObj Cost;

    private U3dObj winPanel;
    private U3dObj defeatPanel;
    private U3dObj costValue;
    private U3dObj txtPlayerName;
    private U3dObj imgPlayerIcon;
    private U3dObj txtEnemyName;
    private U3dObj btnSurrender;
    private U3dObj content;
    private U3dObj txtTick;
    private U3dObj txtCountdown;
    private U3dObj imgWaitMask;
    private RectTransform ImgSpeakerLRect;
    private RectTransform ImgSpeakerRRect;

    private U3dObj imgSurrending;

    private U3dObj winMask;
    private U3dObj defeatMask;

    private Dictionary<GameObject, SkillCardItem> _ItemList = new Dictionary<GameObject, SkillCardItem>();

    public float rayLength = 100f;
    int layerMask;
    int uiLayerMask;
    private bool IsDragSkillCard;
    private SpecialEffect skillCircle;

    private JSONNode CardTable;

    private bool isLongPassSurrending; //投降标志位
    private bool isSurrending; //已经投降
    private float maxSurrendingTime;
    private float currSurrendingTime;

    private string checkMsg;

    protected override void OnInit()
    {
        CardsScrollView = Find("CardsScrollView");
        maxSurrendingTime = 1.2f;
        playerHpSlider = transform.Find("PlayerMsg/HeadSlider").GetComponent<Slider>();
        enemyHpSlider = transform.Find("EnemyMsg/HeadSlider").GetComponent<Slider>();
        //CostSlider = transform.Find("CostSlider").GetComponent<Slider>();
        Cost = Find("CardsScrollView/Viewport/CostBg/Cost");
        costValue = Find("CardsScrollView/Viewport/CostBg/CostValue");
        txtPlayerName = Find("PlayerMsg/TxtName");
        imgPlayerIcon = Find("PlayerMsg/ImgHead");
        txtEnemyName = Find("EnemyMsg/TxtName");
        btnSurrender = Find("BtnSurrender");
        imgSurrending = btnSurrender.Find("ImgSurrending");
        txtTick = Find("TxtTick");
        txtCountdown = Find("TxtCountdown");
        imgWaitMask = Find("ImgWaitMask");

        winPanel = Find("WinPanelBG");
        defeatPanel = Find("DefeatPanelBG");
        winMask = winPanel.Find("Mask");
        defeatMask = defeatPanel.Find("Mask");

        ImgSpeakerLRect = winPanel.Find("WinPanel/ImgSpeakerL").RectTransform;
        ImgSpeakerRRect = winPanel.Find("WinPanel/ImgSpeakerR").RectTransform;
        content = Find("WinPanelBG/WinPanel/Scroll View/Viewport/Content");

        SetOnClick(winMask.gameObject, () =>
        {
            EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
            EventDispatcher<string>.instance.TriggerEvent(EventName.Scene_ToSceneMainFsm);
        });

        SetOnClick(defeatMask.gameObject, () =>
        {
            EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
            EventDispatcher<string>.instance.TriggerEvent(EventName.Scene_ToSceneMainFsm, checkMsg);
        });

        SetOnPointerDown(btnSurrender.gameObject, (ped) =>
        {
            if (isSurrending)
                return;

            currSurrendingTime = 0;
            imgSurrending.Image.fillAmount = 0;
            imgSurrending.SetActive(true);
            isLongPassSurrending = true;
        });

        //SetLongPress(btnSurrender.gameObject, OnSurrender, 3);

        SetOnPointerUp(btnSurrender.gameObject, (ped) =>
        {
            imgSurrending.Image.fillAmount = 0;
            imgSurrending.SetActive(false);
            isLongPassSurrending = false;
        });

        CardTable = GameData.instance.TableJsonDict["CardConf"];

        Vector3 endScale = new Vector3(400, 400);
        Vector3 startScale = new Vector3(370, 370);

        ImgSpeakerLRect.sizeDelta = startScale;
        DOTween.Sequence().Insert(0, ImgSpeakerLRect.DOSizeDelta(endScale, 0.3f).SetEase(Ease.OutQuad)).SetLoops(-1, LoopType.Yoyo);
        ImgSpeakerRRect.sizeDelta = startScale;
        DOTween.Sequence().Insert(0, ImgSpeakerRRect.DOSizeDelta(endScale, 0.3f).SetEase(Ease.OutQuad)).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnSurrender()
    {
        BattleMgr.instance.DecideOutcome("fail");
    }

    protected override void OnShow()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            ResMgr.instance.ReleaseGameObject(content.transform.GetChild(i).gameObject);
        }

        var skillCircleData = GameData.instance.TableJsonDict["GlobalConf"]["skillCircle"];
        var animData = GameData.instance.TableJsonDict["AnimConf"][skillCircleData["intValue"].ToString()];
        //0:触发特效
        SpecialEffectFactory.instance.CreateSpecialEffect(skillCircleData["intValue"], FixVector3.Zero, (go) =>
        {
            skillCircle = go;
            go.GameObj.SetActive(false);
            BattleMgr.instance.SpecialEffectList.Remove(skillCircle);
        });

        isLongPassSurrending = false;
        isSurrending = false;
        currSurrendingTime = 0;
        imgSurrending.Image.fillAmount = 0;
        //imgSurrending.SetActive(false);

        winPanel.SetActive(false);
        defeatPanel.SetActive(false);
        txtCountdown.SetActive(false);
        imgWaitMask.SetActive(false);

        playerHpSlider.value = 1;
        enemyHpSlider.value = 1;

        //CostSlider.value = (float)GameData.instance.PlayerBattleData.costInit;
        Cost.Image.fillAmount = (float)GameData.instance.PlayerBattleData.costInit / (float)BattleMgr.instance.CostMax;
        costValue.Text.text = GameData.instance.PlayerBattleData.costInit.ToString();

        IsDragSkillCard = false;
        checkMsg = null;

        EventDispatcher<int>.instance.AddEvent(EventName.UI_EnemyHpChange, (evtName, evt) =>
        {
            enemyHpSlider.value = BattleMgr.instance.CurrEnemyCumHp / BattleMgr.instance.EnemyCumHp;
        });

        EventDispatcher<int>.instance.AddEvent(EventName.UI_PlayerHpChange, (evtName, evt) =>
        {
            playerHpSlider.value = BattleMgr.instance.CurrPlayerCumHp / BattleMgr.instance.PlayerCumHp;
        });

        EventDispatcher<bool>.instance.AddEvent(EventName.UI_ShowBattleWaitImg, (evtName, evt) =>
        {
            var value = evt[0];
            imgWaitMask.SetActive(value);
            var imgWait = imgWaitMask.Find("ImgWait");
            imgWait.transform.DOKill();
            if (value)
            {
                Vector3 doR0tate_vct = new Vector3(0, 0, 360); //设置旋转的角度
                float useTime_r = 5;
                imgWait.transform.DORotate(doR0tate_vct, useTime_r, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
            }
        });

        EventDispatcher<int>.instance.AddEvent(EventName.UI_BattleCountDown, (evtName, evt) =>
        {
            int count = evt[0];

            if (count == 0)
            {
                txtCountdown.Text.text = "Fight";
            }
            else if (count == -1)
            {
                txtCountdown.SetActive(false);
            }
            else
            {
                txtCountdown.SetActive(true);
                txtCountdown.Text.text = count.ToString();
            }
        });


        EventDispatcher<JSONNode>.instance.AddEvent(EventName.UI_ShowBattleResult, (evtName, evt) =>
        {
            var jsonNode = evt[0];
            EventDispatcher<bool>.instance.TriggerEvent(EventName.UI_ShowBattleWaitImg, false);
            if (jsonNode["data"]["result"] == "win")
            {
                //winPanel.Find("WinPanel/DiamondsBG/TxtDiamonds").Text.text = jsonNode["data"]["rewardItems"][GameConfig.Diamond];
                //winPanel.Find("WinPanel/GoldBG/TxtGold").Text.text = jsonNode["data"]["rewardItems"][GameConfig.Gold];
                var rewardItems = jsonNode["data"]["rewardItems"];

                var ItemConf = GameData.instance.TableJsonDict["ItemConf"];

                foreach (var item in rewardItems)
                {
                    var itemData = ItemConf[item.Key];

                    ResMgr.instance.LoadGameObjectAsync("BattleResultItem", (go) =>
                    {
                        go.SetActive(true);
                        go.transform.SetParent(content.transform);
                        var txtCount = go.transform.Find("BG/TxtCount");
                        var bg = go.transform.Find("BG");
                        var rawImg = go.transform.Find("BG/RawImage");

                        int boxCfgId = itemData["boxCfgId"];
 
                        bg.SetActiveEx(!(boxCfgId > 0));

                        txtCount.GetComponent<Text>().text = item.Value;
                        go.transform.localScale = Vector3.one;

                        var box = go.transform.Find("BoxAnim");
                        var boxAnimCtor = box.GetComponent<Animator>();
                        box.SetActiveEx(boxCfgId > 0);

                        ResMgr.instance.LoadSpriteAsync(itemData["bgIconAtlas"], itemData["bgIcon"], (sprite) =>
                        {
                            bg.GetComponent<Image>().sprite = sprite;

                            ResMgr.instance.LoadSpriteAsync(itemData["iconAtlas"], itemData["icon"], (sprite1) =>
                            {                          
                                var image = rawImg.GetComponent<Image>();
                                image.sprite = sprite1;
                                image.SetNativeSize();

                                var animValue = boxCfgId - 300000;
                                boxAnimCtor.SetInteger("value", animValue);
                                MonoTimeMgr.instance.SetTimeAction(0.5f, () =>
                                {
                                    boxAnimCtor.SetInteger("value", animValue + 10);

                                    MonoTimeMgr.instance.SetTimeAction(0.5f, () =>
                                    {
                                        box.SetActiveEx(false);
                                        if (boxCfgId > 0)
                                        {
                                            Vector3 endScale = bg.localScale;
                                            bg.localScale = Vector3.zero;
                                            DOTween.Sequence().Insert(0, bg.DOScale(endScale, 0.4f).SetEase(Ease.OutQuad));
                                        }

                                        bg.SetActiveEx(true);
                                    });
                                });
                            });
                        });
                    });
                }

                var scoreBg = winPanel.Find("WinPanel/ScoreBG");
                if (jsonNode["data"]["battleType"] == "pvp")
                {
                    scoreBg.SetActive(true);
                    scoreBg.Find("Score").Text.text = jsonNode["data"]["pvpInfo"]["newScore"];
                }
                else if (jsonNode["data"]["battleType"] == "pve")
                {
                    scoreBg.SetActive(false);
                }

                winPanel.SetActive(true);
                DoTweenAnimUtil.ScaleAni(winPanel.transform.Find("WinPanel"), 0, 0.3f, DG.Tweening.Ease.OutQuad, Vector3.zero);
                AudioMgr.instance.PlayOneShot("ui_battle_win");
            }
            else
            {

                var scoreBg = defeatPanel.Find("DefeatPanel/ScoreBG");
                if (jsonNode["data"]["battleType"] == "pvp")
                {
                    scoreBg.SetActive(true);
                    scoreBg.Find("Score").Text.text = jsonNode["data"]["pvpInfo"]["newScore"];
                }
                else if (jsonNode["data"]["battleType"] == "pve")
                {
                    scoreBg.SetActive(false);
                }

                if (jsonNode["data"]["checkRet"] != 0)
                {
                    checkMsg = jsonNode["data"]["checkMsg"];

                    EventDispatcher<bool>.instance.TriggerEvent(EventName.Scene_ShowEnemyTroops, false);
                    EventDispatcher<string>.instance.TriggerEvent(EventName.Scene_ToSceneMainFsm, checkMsg);
                }
                else
                {
                    defeatPanel.SetActive(true);
                    DoTweenAnimUtil.ScaleAni(defeatPanel.transform.Find("DefeatPanel"), 0, 0.3f, DG.Tweening.Ease.OutQuad, Vector3.zero);
                    AudioMgr.instance.PlayOneShot("ui_battle_lose");
                }
            }
        });

        EventDispatcher<int>.instance.AddEvent(EventName.UI_UpdateCost, (evtName, evt) =>
        {
            Cost.Image.fillAmount = (float)BattleMgr.instance.CurrCost / (float)BattleMgr.instance.CostMax;
            //CostSlider.value = (float)BattleMgr.instance.CurrCost;
            costValue.Text.text = BattleMgr.instance.CurrCost.ToString();

            CheckCardSkillCost();
        });

        var cardsContent = transform.Find("CardsScrollView/Viewport/Content");

        for (int i = 0; i < cardsContent.childCount; i++)
        {
            cardsContent.GetChild(i).gameObject.SetActive(false);
        }

        var cards = GameData.instance.PlayerBattleData.cards;
        for (int i = 0; i < cards.Count; i++)
        {
            var go = cardsContent.GetChild(i);
            go.gameObject.SetActive(true);
            var kv = GameData.instance.PlayerBattleData.cards.ElementAt(i);
            SkillCardItem item = SkillCardItem.GetItem<SkillCardItem>(go, this);
            item.OnBeginDragSkillCard = OnBeginDragSkillCard;
            item.OnEndDragSkillCard = OnEndDragSkillCard;
            item.Cost = (int)kv.Value.Cost;
            item.SetData(kv.Value);
            _ItemList.Add(go.gameObject, item);
        }

        layerMask = LayerMask.GetMask("RayCast");
        uiLayerMask = LayerMask.GetMask("UI");

        Cost.Image.fillAmount = (float)BattleMgr.instance.CurrCost / (float)BattleMgr.instance.CostMax;
        //CostSlider.value = (float)BattleMgr.instance.CurrCost / (float)BattleMgr.instance.CostMax;
        CheckCardSkillCost();

        if (AppConfig.Platform == "discord")
        {
            txtPlayerName.Text.text = GameData.instance.DiscordPlayerName;
            //if (GameData.instance.DiscordIcon != null)
            //{
            //    imgHead.Image.sprite = GameData.instance.DiscordIcon;
            //}
        }
        else
        {
            txtPlayerName.Text.text = GameData.instance.UserData["name"];
        }

        txtEnemyName.Text.text = GameData.instance.EnemyBattleData.name;

        if (BattleMgr.instance.PvMode == PvMode.Pve)
        {
            int pass_pve = GameData.instance.PermanentData["pass_pve"];
            int showGiveUpLevel = GameData.instance.TableJsonDict["GlobalConf"]["showGiveUpLevel"]["intValue"];
            btnSurrender.SetActive(pass_pve >= showGiveUpLevel);

            int showSkillPanelLevel = GameData.instance.TableJsonDict["GlobalConf"]["showSkillPanelLevel"]["intValue"];
            CardsScrollView.SetActive(pass_pve >= showSkillPanelLevel);
            Find("CardsBg").SetActive(pass_pve >= showSkillPanelLevel);
        }
        else if(BattleMgr.instance.PvMode == PvMode.Pvp)
        {
            Find("CardsBg").SetActive(true);
            Find("CardsScrollView").SetActive(true);
        }
        else
        {
            Find("CardsBg").SetActive(false);
            Find("CardsScrollView").SetActive(false);
        }
    }

    private void CheckCardSkillCost()
    {
        foreach (var item in _ItemList)
        {
            bool value = (float)BattleMgr.instance.CurrCost >= item.Value.Cost ? true : false;
            item.Value.ShowMask(value);
        }
    }

    public void OnBeginDragSkillCard(int skillCardCfgId)
    {
        IsDragSkillCard = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                var hitPos = GameUtils.CheckMapBorderUnity(hit.point);
                skillCircle.Fixv3LogicPosition = FixVector3.ToFixVector3(hitPos);
                skillCircle.Fixv3LastPosition = skillCircle.Fixv3LogicPosition;
            }
        }

        var cardData = CardTable[skillCardCfgId.ToString()];
        float range = cardData["range"].AsFloat / 1000;
        skillCircle.Trans.localScale = new Vector3(range, range, range);
        skillCircle.GameObj.SetActive(true);
    }

    public void OnEndDragSkillCard(int skillCardCfgId)
    {
        IsDragSkillCard = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (EventSystem.current.IsPointerOverGameObject())
        {

        }
        else
        {
            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                var hitPos = GameUtils.CheckMapBorderUnity(hit.point);
                BattleMgr.instance.SaveCardSkillOp(skillCardCfgId, new FixVector3((Fix64)hitPos.x, (Fix64)hitPos.y, Fix64.Zero), PlayerGroup.Player);

                if (GuideMgr.guideNodeList.Count > 0)
                {
                    foreach (var item in GuideMgr.guideNodeList)
                    {
                        if (item.IsUseBattleSkill && item.stage == GuideNode.Stage.Guiding && item.cfg["strArg1"] == skillCardCfgId.ToString())
                        {
                            UiMgr.Close<GuideView>();
                            item.NextStep();
                            skillCircle.GameObj.SetActive(false);
                            return;
                        }
                    }
                }
            }
        }

        skillCircle.GameObj.SetActive(false);

    }

    public void OnDragSkillCard()
    {
        if (!IsDragSkillCard)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (skillCircle.GameObj.activeSelf)
                {
                    skillCircle.GameObj.SetActive(false);
                }

                return;
            }

            if (Physics.Raycast(ray, out hit, rayLength, layerMask))
            {
                if (!skillCircle.GameObj.activeSelf)
                {
                    skillCircle.GameObj.SetActive(true);
                }

                var hitPos = GameUtils.CheckMapBorderUnity(hit.point);
                skillCircle.Fixv3LogicPosition = FixVector3.ToFixVector3(hitPos);
            }
            
        }
    }

    protected override void OnUpdate()
    {
        OnDragSkillCard();

        if (skillCircle?.GameObj != null && skillCircle.GameObj.activeSelf)
        {
            skillCircle.UpdateAnim(0);
            skillCircle.UpdateRenderPosition(Time.deltaTime);
            skillCircle.RecordLastPos();
        }

        var frame = GameData.instance.EndStep - GameData.instance._UGameLogicFrame;
        if (frame > 0)
        {
            var s = frame * GameData.instance._FixFrameLen;
            var dhms = TimeUtils.DHMS((int)s);
            txtTick.Text.text = $"{dhms.Minute}:{dhms.Second}";
        }

        if (isSurrending)
            return;

        if (isLongPassSurrending)
        {
            currSurrendingTime += Time.deltaTime;
            imgSurrending.Image.fillAmount = currSurrendingTime / maxSurrendingTime;
            if (currSurrendingTime >= maxSurrendingTime)
            {
                OnSurrender();
                isSurrending = true;
                isLongPassSurrending = false;
            }
        }
    }

    protected override void OnHide()
    {
        EventDispatcher<int>.instance.RemoveEventByName(EventName.UI_EnemyHpChange);
        EventDispatcher<int>.instance.RemoveEventByName(EventName.UI_PlayerHpChange);
        EventDispatcher<int>.instance.RemoveEventByName(EventName.UI_UpdateCost);
        EventDispatcher<JSONNode>.instance.RemoveEventByName(EventName.UI_ShowBattleResult);
        EventDispatcher<int>.instance.RemoveEventByName(EventName.UI_BattleCountDown);
        EventDispatcher<bool>.instance.RemoveEventByName(EventName.UI_ShowBattleWaitImg);

        if (skillCircle != null)
        {
            skillCircle.BKilled = true;
            if (skillCircle.GameObj != null)
            {
                ResMgr.instance.ReleaseGameObject(skillCircle.GameObj);
            }
            skillCircle.GameObj = null;
            skillCircle = null;
        }

        for (int i = 0; i < content.transform.childCount; i++)
        {
            ResMgr.instance.ReleaseGameObject(content.transform.GetChild(i).gameObject);
        }

        _ItemList.Clear();
    }

    protected override void OnDestroy()
    {

    }

    private void OnStartGame()
    {

    }


    // guide================================
    public override RectTransform GetGuideTrans(GuideNode guideNode)
    {
        if (guideNode.IsUseBattleSkill) {
            foreach (SkillCardItem item in _ItemList.Values)
            {
                if (item.CardData.CfgId.ToString() == guideNode.cfg["strArg1"]) { 
                    return item.transform.GetComponent<RectTransform>();
                }
            }
            return null;
        }

        return base.GetGuideTrans(guideNode);
    }

    public override void TriggerGuide(GuideNode guideNode)
    {


        base.TriggerGuide(guideNode);
    }
}
