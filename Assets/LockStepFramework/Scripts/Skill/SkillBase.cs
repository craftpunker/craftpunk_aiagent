
using System;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

namespace Battle{
    public class SkillBase : IRelease
    {
        public SkillData SkillData;

        public EntityBase Origin;
        public EntityBase Target;
        public int MainCfgid; //记录第一个cfgid值，表示该技能表现的主体特效
        public PlayerGroup PlayerGroup;

        public FixVector3 Fixv3LogicPosition;
        public FixVector3 Fixv3LastPosition;
        public int FaceDir = -1; //1:左边，-1:右边
#if _CLIENTLOGIC_
        public Vector4 ShaderValue; //记录Shader参数 x:动画时间 y:方向 z：透明

        public GameObject GameObj;
        public Transform Trans;

        //public MaterialPropertyBlock Block;
        public Renderer Mesh;
        public Material Material;

        private AminInfo aminInfo;
        public AnimData AnimData;

        public Transform GpuAnim;
#endif
        public EntityAttrValue OriginAttrValue;

        public bool BKilled = false;
        public bool Is3D;

        public virtual void Init()
        {
            BKilled = false;
            MainCfgid = 0;
#if _CLIENTLOGIC_
            ShaderValue = new Vector4();
            ShaderValue.w = 1;
#endif
            FaceDir = -1;
            Is3D = false;
            PlayerGroup = PlayerGroup.None;
        }

        public virtual void Start()
        {
#if _CLIENTLOGIC_
            AudioMgr.instance.PlayOneShot(AudioConf.Skill, SkillData.cfgId.ToString(), AudioType.BornAudio);
#endif
        }

        public virtual void Update()
        {

        }

        public void RecordLastPos()
        {
            Fixv3LastPosition = Fixv3LogicPosition;
        }

#if _CLIENTLOGIC_

        public void CreateFromPrefab(string path, Action<GameObject> callBack = null)
        {
            if (string.IsNullOrEmpty(path))
                return;

            ResMgr.instance.LoadGameObjectAsync(path, (obj) =>
            {
                GameObj = obj;
                Trans = GameObj.transform;
                Trans.localScale = AnimData.PrefabSize;
                GpuAnim = Trans.Find("GPUAnim");
                if(GpuAnim != null)
                {
                    GpuAnim.rotation = Quaternion.Euler(-45, 0, 0);
                }
                GameObj.SetActive(false);
                callBack?.Invoke(obj);
            });
        }

        public void DoAnim(AnimType animType)
        {
            if (aminInfo != null)
                aminInfo.Release();

            aminInfo = AnimMgr.instance.GetAnimInfo(MainCfgid, animType);
        }

        public virtual void UpdateAnim(float interpolation)
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
                }
                else
                {
                    aminInfo.CurrTime = aminInfo.EndTime;
                }
            }
        }

        protected void SetMeshBlock()
        {
            if (GameObj == null)
                return;

            //Block = new MaterialPropertyBlock();
            Mesh = Trans.Find("GPUAnim").GetComponent<Renderer>();
            Material = Mesh.materials[0];
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
#endif

        public virtual void UpdateRenderPosition(float interpolation)
        {
#if _CLIENTLOGIC_
            if (BKilled || GameObj == null)
            {
                return;
            }

            if (Fixv3LastPosition == FixVector3.None)
                return;

            if (interpolation != 0)
            {
                Trans.localPosition = Vector3.Lerp(Fixv3LastPosition.ToVector3(), Fixv3LogicPosition.ToVector3(), interpolation);

                //UpdateRenderRotation(interpolation);
            }
            else
            {
                Trans.localPosition = Fixv3LogicPosition.ToVector3();
            }
#endif
        }

        public virtual void Release()
        {
#if _CLIENTLOGIC_
            AudioMgr.instance.PlayOneShot(AudioConf.Skill, SkillData.cfgId.ToString(), AudioType.DeadAudio);

            if (GameObj != null)
                ResMgr.instance.ReleaseGameObject(GameObj);

            AnimData = null;
            Material = null;
            //Block = null;
            Mesh = null;
            aminInfo = null;
            GpuAnim = null;
            GameObj = null;
            Trans = null;
#endif

            Origin = null;
            Target = null;
            SkillData = null;
        }
    }
}
