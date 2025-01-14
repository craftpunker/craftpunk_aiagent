
#if _CLIENTLOGIC_
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    //buff 
    public class SpecialEffect : IRelease
    {
        public AnimData AnimData; //idle
        public bool BKilled;
        private float time;

        public Action<SpecialEffect> CreatePrefabCallBack;
        public int FaceDir = -1; //1:，-1:
        public Vector4 ShaderValue; //Shader x: y: z：

        public FixVector3 Fixv3LogicPosition;
        public FixVector3 Fixv3LastPosition;
        public GameObject GameObj;
        public Transform Trans;
        //public Renderer Mesh;
        //public MaterialPropertyBlock Block;
        public Material Material;
        public AminInfo aminInfo;

        //public bool IsUI;

        public void Init()
        {
            BKilled = false;
            time = 0;
            ShaderValue = new Vector4();
            ShaderValue.w = 1;
            FaceDir = -1;
            //IsUI = false;
        }

        public void Start()
        {
            CreateFromPrefab(AnimData.Prefab);
        }


        public void CreateFromPrefab(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            ResMgr.instance.LoadGameObjectAsync(path, (obj) =>
            {
                GameObj = obj;
                GameObj.SetActive(false);
                Trans = GameObj.transform;
                Trans.position = Fixv3LogicPosition.ToVector3();
                Trans.Find("GPUAnim").rotation = Quaternion.Euler(-45, 0, 0);
                Trans.localScale = AnimData.PrefabSize;
                //else
                //{
                //    Trans.localScale = Vector3.one;
                //    var rect = Trans.GetComponent<RectTransform>();
                //    rect.sizeDelta = AnimData.PrefabSize;
                //    rect.GetComponent<Image>().raycastTarget = false;
                //}
                SetMeshBlock();
                DoAnim(AnimType.Idle);
                GameObj.SetActive(true);
                CreatePrefabCallBack?.Invoke(this);
            });
        }

        public void UpdateRenderPosition(float interpolation)
        {
            if (BKilled || GameObj == null)
            {
                return;
            }

            if (Fixv3LastPosition == FixVector3.None)
                return;

            if (interpolation != 0)
            {
                Trans.position = Vector3.Lerp(Fixv3LastPosition.ToVector3(), Fixv3LogicPosition.ToVector3(), interpolation);
                //UpdateRenderRotation(interpolation);
            }
            else
            {
                Trans.position = Fixv3LogicPosition.ToVector3();
            }
        }

        public void RecordLastPos()
        {
            Fixv3LastPosition = Fixv3LogicPosition;
        }

        public void DoAnim(AnimType animType, Action playCompleteCallBack = null)
        {
            if (aminInfo != null)
                aminInfo.Release();

            aminInfo = AnimMgr.instance.GetAnimInfo(AnimData.Cfgid, animType);
            aminInfo.PlayCompleteCallBack = playCompleteCallBack;
            time = 0;
        }

        public void UpdateAnim(float interpolation)
        {
            if (aminInfo == null)
                return;

            ShaderValue.x = aminInfo.CurrTime;
            ShaderValue.y = FaceDir;
            SetMaterialV4("_Value", ShaderValue);
            aminInfo.CurrTime += interpolation * aminInfo.Speed;

            if (aminInfo.CurrTime > aminInfo.EndTime)
            {
                if (aminInfo.Loop)
                {
                    aminInfo.CurrTime = aminInfo.StartTime;

                    if (aminInfo.LifeTime > 0)
                    {
                        if (time >= aminInfo.LifeTime)
                        {
                            if (aminInfo.PlayCompleteCallBack != null)
                            {
                                aminInfo.PlayCompleteCallBack.Invoke();
                            }
                            else
                                BKilled = true;
                        }
                    }
                }
                else
                {
                    aminInfo.CurrTime = aminInfo.EndTime;

                    if (aminInfo.LifeTime > 0)
                    {
                        if (time >= aminInfo.LifeTime)
                        {
                            if (aminInfo.PlayCompleteCallBack != null)
                            {
                                aminInfo.PlayCompleteCallBack.Invoke();
                                aminInfo = null;
                            }
                            else
                                BKilled = true;
                        }
                    }
                    else
                    {
                        BKilled = true;
                    }
                }
            }

            time += interpolation;
        }

        protected void SetMeshBlock()
        {
            if (GameObj == null)
                return;

            Material = Trans.Find("GPUAnim").GetComponent<Renderer>().materials[0];
            //if (IsUI)
            //{
            //    Material = GameObj.GetComponent<Image>().material;
            //}
            //else
            //{
            //    Material = Trans.Find("GPUAnim").GetComponent<Renderer>().materials[0];
            //}
            //Block = new MaterialPropertyBlock();
            //Mesh.GetPropertyBlock(Block);
        }

        public void SetMaterialFloat(string name, float value)
        {
            if (GameObj == null)
                return;

            //Block.SetFloat(name, value);
            //Mesh.SetPropertyBlock(Block);
        }

        public void SetMaterialV4(string name, Vector4 value)
        {
            if (GameObj == null)
                return;

            Material.SetVector(name, value);
            //Block.SetVector(name, value);
            //Mesh.SetPropertyBlock(Block);
        }

        public void Release()
        {
            AnimData = null;
            CreatePrefabCallBack = null;

            if (GameObj != null)
            {
                ResMgr.instance.ReleaseGameObject(GameObj);
                GameObj = null;
                Trans = null;
                //Mesh = null;
                Material = null;
                //Block = null;
                aminInfo = null;
            }

            ClassPool.instance.Push(this);
        }
    }
}
#endif