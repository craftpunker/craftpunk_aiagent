
namespace Battle
{
    using System;
#if _CLIENTLOGIC_
    using UnityEngine;
#endif

    //���Կ���
    public struct EntityAttrValue
    {
        public Fix64 Atk;
        public Fix64 AtkSpeed;
        public Fix64 AtkReadyTime;
        public Fix64 AtkBackswing;
        public Fix64 MoveSpeed;

        public EntityAttrValue(Fix64 atk, Fix64 atkSpeed, Fix64 atkReadyTime, Fix64 atkBackswing, Fix64 moveSpeed)
        {
            Atk = atk;
            AtkSpeed = atkSpeed;
            AtkReadyTime = atkReadyTime;
            AtkBackswing = atkBackswing;
            MoveSpeed = moveSpeed;
        }

        public EntityAttrValue Copy()
        {
            return new EntityAttrValue(Atk, AtkSpeed, AtkReadyTime, AtkBackswing, MoveSpeed);
        }
    }

    public class EntityBase : GameObjectBase
    {
        public FsmCompent<EntityBase> Fsm;
        //public SkeletonAnimation Spine;

        public TroopsData TroopsData;

        public EntityAttrValue CurrAttrValue; //��¼��ǰ������ֵ

        //-------------�������Զ���ԭʼ������-----------------
        public int PlayerId;
        public int CfgId; //ʿ����������ID
        public Fix64 Atk;
        public Fix64 MaxHp;
        public Fix64 Hp;
        public Fix64 AtkSpeed;
        public Fix64 MoveSpeed;
        public Fix64 AtkRange;
        public Fix64 AtkRangeSq; //�����뾶
        public Fix64 InAtkRange;
        public Fix64 InAtkRangeSq;
        public Fix64 RadiusSq; //����뾶��С
        public int AtkSkillId;
        public SoldierType SoldierType;
        public FixVector3 AtkSkillShowPos;//�չ���ʾλ��
        public FixVector3 Center; //���ĵ�
        public Fix64 AtkReadyTime;//ǰҡʱ��
        public Fix64 AtkBackswing; //��ҡʱ��
        public int DeadSkillId;
        public int BornSkillId;
        //-------------------------------------------------------

        public int SoldierFlagId; //�ͻ����õĲ���ID����������ID�����������ظ�
        //public EntityStageBag StageBag = new EntityStageBag();
        public BuffBag BuffBag = new BuffBag();
        public SoldierFlagBase SoldierFlag; //����

        public EntityBase LockEntity; //������Ŀ��
        //public bool IsMoveX = false; //0��AI��Ҫ�Ⱥ���
        public int FaceDir = -1; //1:��ߣ�-1:�ұ�

        public Agent Agent;
        public PlayerGroup PlayerGroup;
        public bool IsSoldier; //�����Ƿ�ս��ʿ��
        public bool IsSummon; //�Ƿ��ٻ���

        public FixVector2 TargetPos; //orcaĿ�ĵ� ,���켯�ϵ�͵���λ��
        public FixVector2 FlagOffsetPos;

        public Action AtkAction;
        public Action DeadAction;
        public Action<EntityBase, Fix64> BeAtkAction; //������

        public Fix64 AtkCD;
#if _CLIENTLOGIC_
        public Vector4 ShaderValue; //��¼Shader���� x:����ʱ�� y:���� z��͸��
        public Vector4 BloodShaderSequenceValue; //Ѫ���� xy���У�z����͸��λ��
#endif
        public int CurrentState; //��¼��ǰ״̬,��ӦStageConst
        public SoldierJob Job;

        public SkillEffectBase SkillEffectFsm; //����skefCtorFsm��skef�ű�

#if _CLIENTLOGIC_
        //public MaterialPropertyBlock Block;
        public Renderer Mesh;
        public Material Material;
        public Material BloodMaterial;
        public TextMesh text;
        public Transform GPUBlood;

        private AminInfo aminInfo;
#endif


        public override void Init()
        {
            base.Init();

            PlayerId = 0;
            CfgId = 0;
            Atk = Fix64.Zero;
            MaxHp = Fix64.Zero;
            Hp = Fix64.Zero;
            MoveSpeed = Fix64.Zero;
            AtkRangeSq = Fix64.Zero;
            AtkSpeed = Fix64.Zero;
            RadiusSq = Fix64.Zero;
            LockEntity = null;
            TargetPos = FixVector2.None;
            FlagOffsetPos = FixVector2.None;
            AtkRange = Fix64.Zero;
            IsSoldier = true;
            SoldierType = 0;
            //IsMoveX = false;
            AtkAction = null;
            DeadAction = null;
            BeAtkAction = null;
            TroopsData = null;
            AtkCD = Fix64.Zero;
            FaceDir = -1;
#if _CLIENTLOGIC_
            ShaderValue = new Vector4();
#endif
            CurrentState = 0;
            IsSummon = false;
            BuffBag.Init(this);
            Job = SoldierJob.None;
            SkillEffectFsm = null;
#if _CLIENTLOGIC_
            //Block = null;
            Mesh = null;
            Material = null;
            text = null;
            aminInfo = null;
            GPUBlood = null;
#endif
        }

        //��ʼ�߼�
        public virtual void StartLogic()
        {
            CurrAttrValue = new EntityAttrValue(Atk, AtkSpeed, AtkReadyTime, AtkBackswing, MoveSpeed);
        }

        public virtual void UpdateLogic()
        {
            BuffBag.Update();
            AtkCD += GameData.instance._FixFrameLen;
        }

        //��ȡ����,������
        public EntityAttrValue GetEntityAttrValueCopy()
        {
            EntityAttrValue attr = CurrAttrValue.Copy();

            return attr;
        }

        //��ȡ���ܳ�������
        public FixVector3 GetAtkSkillShowPos()
        {
            return Fixv3LogicPosition + (Fix64)FaceDir * AtkSkillShowPos;
        }

#if _CLIENTLOGIC_

        public void DoAnim(AnimType animType)
        {
            if(aminInfo != null)
                aminInfo.Release();

            aminInfo = AnimMgr.instance.GetAnimInfo(AnimData.Cfgid, animType);
        }

        public void UpdateAnim(float interpolation)
        {
            if (aminInfo == null)
                return;

            ShaderValue.x = aminInfo.CurrTime;
            ShaderValue.y = FaceDir;
            ShaderValue.z = ShaderValue.z > 0 ? ShaderValue.z - interpolation * 3.3f : 0;
            ShaderValue.w = alpha;//BattleMgr.instance.CheckStage(this, StageConst.Stealth) ? 0 : 1;

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

        public void UpdateBloodRander()
        {
            if (!GameData.instance.IsOpenBlood)
                return;

            float hpValue = (float)(Hp / MaxHp);

            if (hpValue >= 0.95f)
            {
                GPUBlood.SetActiveEx(false);
                return;
            }
            else
            {
                GPUBlood.SetActiveEx(true);
            }

            BloodShaderSequenceValue.z = hpValue;
            SetBloodMaterialV4("_Sequence", BloodShaderSequenceValue);
        }

        protected void SetMeshBlock()
        {
            if (PlayerGroup == PlayerGroup.Player)
            {
                GroupRange.gameObject.SetActive(false);
            }
            else
            {
                GroupRange.gameObject.SetActive(true);
            }

            if (GameObj == null)
                return;

            //Block = new MaterialPropertyBlock();
            Mesh = Trans.Find("GPUAnim").GetComponent<Renderer>();
            Material = Mesh.materials[0];

            GPUBlood = Trans.Find("GPUBlood");
            GPUBlood.SetActiveEx(false);
            BloodMaterial = GPUBlood.GetComponent<Renderer>().materials[0];

            if (IsSummon)
                SetAlpha(0);

            BloodShaderSequenceValue = BloodMaterial.GetVector("_Sequence");
            BloodShaderSequenceValue.z = 1;
            SetBloodMaterialV4("_Sequence", BloodShaderSequenceValue);
            SetBloodMaterialV4("_Value", new Vector4(0, 0, 0, 1));
            if (PlayerGroup == PlayerGroup.Player)
            {
                BloodMaterial.SetVector("_Color", new Vector4(1, 0.28f, 0, 0));
            }
            else
            {
                BloodMaterial.SetVector("_Color", new Vector4(0.28f, 1, 1, 0));
            }
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

        public void SetBloodMaterialV4(string name, Vector4 value)
        {
            if (GameObj == null)
                return;

            BloodMaterial.SetVector(name, value);
            //Block.SetVector(name, value);
            //Mesh.SetPropertyBlock(Block);
        }
#endif

        //void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.blue;
        //    //������
        //    Gizmos.DrawLine(Fixv2LogicPosition.ToVector3(), TargetPos.ToVector3());
        //}

        public void SetAgent(Agent agent)
        {
            Agent = agent;
        }

        public void UpdateRenderPosition(float interpolation)
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
                Trans.position = Vector3.Lerp(Fixv3LastPosition.ToVector3(), Fixv3LogicPosition.ToVector3(), interpolation);

                //UpdateRenderRotation(interpolation);
            }
            else
            {
                Trans.position = Fixv3LogicPosition.ToVector3();
            }
#endif
        }

        public void RecordLastPos()
        {
            Fixv3LastPosition = Fixv3LogicPosition;
        }

        //public virtual void LineMove(FixVector2 parentPos)
        //{

        //}

        public virtual void Release()
        {
            Fsm?.ReleaseAllFsmState();
            Fsm = null;
            //Spine = null;
            Agent = null;
            LockEntity = null;
            AtkAction = null;
            DeadAction = null;
            BeAtkAction = null;
            TroopsData = null;
            SoldierFlag = null;
            SkillEffectFsm = null;
            BuffBag.Release();
#if _CLIENTLOGIC_
            //Block = null;
            Mesh = null;
            Material = null;
            text = null;
            GPUBlood = null;
#endif

            base.ReleaseGameObj();
        }
    }
}
