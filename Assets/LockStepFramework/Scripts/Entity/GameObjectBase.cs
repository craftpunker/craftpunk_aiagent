

namespace Battle
{

#if _CLIENTLOGIC_
    using UnityEngine;
    using System;
#endif
    public class GameObjectBase : IRelease
    {
        //
        public bool BKilled = false;
        public bool CanRelease = false;

#if _CLIENTLOGIC_
        public GameObject GameObj;
        public Transform Trans;
        //public Transform AnimEntity;
        public AnimData AnimData;

        public Transform BuffBagTrans;
        public Action CreateCallBack; //
        public Transform GroupRange;
        private AnimData GroupRangeAnimData;
        protected float alpha;
#endif

        //
        public FixVector3 Fixv3LogicRotation;

        //
        public FixVector3 Fixv3LogicScale;
        //
        public Fix64 Radius = Fix64.One;

        public FixVector3 Fixv3LogicPosition;
        public FixVector3 Fixv3LastPosition;
#if _CLIENTLOGIC_
        public AminInfo GroupRangeAminInfo;
#endif

        public virtual void Init()
        {
            BKilled = false;
            CanRelease = false;
            //ResPath = null;

            Fixv3LastPosition = FixVector3.Zero;
            Fixv3LogicPosition = FixVector3.Zero;
            Fixv3LogicScale = FixVector3.Zero;
            Radius = Fix64.One;
            //Flip = false;
            Fixv3LogicRotation = FixVector3.Zero;
#if _CLIENTLOGIC_
            //Anim = null;
            GameObj = null;
            Trans = null;
            AnimData = null;
            CreateCallBack = null;
            GroupRange = null;
            GroupRangeAminInfo = null;
            alpha = 1;
            //AnimEntity = null;
            //SpineTrans = null;
            //SpineAnim = null;
#endif
        }

        public virtual void Start()
        {

        }

        public void ReleaseGameObj()
        {
            //ResPath = null;
#if _CLIENTLOGIC_
            if (GameObj != null)
            {
                ResMgr.instance.ReleaseGameObject(GameObj);
                GameObj = null;
                Trans = null;
                AnimData = null;
                BuffBagTrans = null;
                CreateCallBack = null;
                //AnimEntity = null;
                //SpineTrans = null;
                //SpineAnim = null;
                //Anim = null;
                //BuffBag = null;
            }
#endif
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
                BuffBagTrans = Trans.Find("BuffBag");

                GroupRange = Trans.Find("GPUAnimGroupRange");
                GroupRangeAminInfo = AnimMgr.instance.GetAnimInfo(30006, AnimType.Idle);
                //ShadowAminInfo = AnimMgr.instance.GetAnimInfo(30023, AnimType.Idle);

                //var block = new MaterialPropertyBlock();
                var mesh = GroupRange.GetComponent<Renderer>();
                var material = mesh.material;
                material.SetVector("_Value", new Vector4(GroupRangeAminInfo.CurrTime, 0, 0, 1));

                //mesh.GetPropertyBlock(block);
                //block.SetVector("_Value", new Vector4(GroupRangeAminInfo.CurrTime, 0, 0, 1));
                //mesh.SetPropertyBlock(block);
                CreateCallBack?.Invoke();
                callBack?.Invoke(obj);
            });
        }
        public void SetAlpha(float value)
        {
            alpha = value;
        }

        public void UpdateGroupRangeAnim()
        {

        }
#endif
        public virtual void Release()
        {
            
        }
    }
}

