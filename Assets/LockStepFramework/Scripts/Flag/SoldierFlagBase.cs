
using System.Collections.Generic;
using System.Linq;
#if _CLIENTLOGIC_
using UnityEngine;
#endif

//public enum SoldierFlagType
//{
//    CloseCombat = 0, //
//    RemoteAttack = 1, //
//}
namespace Battle
{

    //
    public class SoldierFlagBase : IRelease
    {
        public int Id;//ID，
        public int SoldierFlagId;
        public List<EntityBase> Entitys = new List<EntityBase>();
        public SortedDictionary<int, SoldierFlagBase> FlagLockMeDict = new SortedDictionary<int, SoldierFlagBase>(); //
        public FixVector3 TargetPos; //
        public FixVector3 Pos;
        public Fix64 MoveSpeed;
        public bool BKilled = false;
        public PlayerGroup PlayerGroup;
        public SoldierFlagBase LockTarget;
        public FixVector3 Forward; //，
                                   //public FixVector3 Right; //
        public Fix64 ActionDistance;
        public SoldierType SoldierType;//
        public Fix64 FreeSeekDis;//，
        public FixVector3 RowForward;
        public bool IsFinishMoveX = false; //X
                                           //public Fix64 SoldierActionRange; //
        public bool IsCtorEntitys = true; //

        public FsmCompent<SoldierFlagBase> Fsm;

        public IList<KeyValuePair<Fix64, SoldierFlagBase>> agentNeighbors_ = new List<KeyValuePair<Fix64, SoldierFlagBase>>();

        //public Transform obj;

        public virtual void Init()
        {
            BKilled = false;
            TargetPos = FixVector3.Zero;
            Pos = FixVector3.Zero;
            MoveSpeed = Fix64.Zero;
            PlayerGroup = PlayerGroup.None;
            Forward = FixVector3.Zero;
            //Right = FixVector3.Zero;
            RowForward = FixVector3.Zero;
            //SoldierFlagType = SoldierFlagType.CloseCombat;
            FreeSeekDis = Fix64.Zero;
            IsFinishMoveX = false;
            IsCtorEntitys = true;
        }

        public virtual void Start()
        {

        }

        public virtual void StartLogic()
        {

        }

        public virtual void Update()
        {
            Fsm.OnUpdate(this);
        }

        public void SetSoldier(EntityBase entity)
        {
            Entitys.Add(entity);
            entity.SoldierFlag = this;
        }

        public void RemoveSoldier(EntityBase entity)
        {
            Entitys.Remove(entity);
        }

        public void SoldierBKill(EntityBase entity)
        {
            Entitys.Remove(entity);
            if (Entitys.Count == 0)
            {
                BKilled = true;
                for (int i = FlagLockMeDict.Count - 1; i >= 0; i--)
                {
                    var kv = FlagLockMeDict.ElementAt(i);
                    var lockMeFlag = kv.Value;
                    BattleMgr.instance.UnLockFlag(this, lockMeFlag);
                }
            }
        }

        public void AddFlagLockMe(SoldierFlagBase flag)
        {
            if (FlagLockMeDict.ContainsKey(flag.SoldierFlagId))
                return;

            FlagLockMeDict.Add(flag.SoldierFlagId, flag);
        }

        public void RemoveFlagLockMe(SoldierFlagBase flag)
        {
            if (FlagLockMeDict.ContainsKey(flag.SoldierFlagId))
            {
                FlagLockMeDict.Remove(flag.SoldierFlagId);
            }
        }

        public void insertAgentNeighbor(SoldierFlagBase agent, ref Fix64 rangeSq, bool isFindEnemy)
        {
            if (this != agent)
            {
                Fix64 distSq = FixVector2.SqrMagnitude((Pos - agent.Pos).ToFixVector2());

                if (distSq < rangeSq)
                {
                    if (agentNeighbors_.Count < GameData.instance.MaxNeighbors)
                    {
                        agentNeighbors_.Add(new KeyValuePair<Fix64, SoldierFlagBase>(distSq, agent));
                    }

                    int i = agentNeighbors_.Count - 1;

                    while (i != 0 && distSq < agentNeighbors_[i - 1].Key)
                    {
                        agentNeighbors_[i] = agentNeighbors_[i - 1];
                        --i;
                    }

                    agentNeighbors_[i] = new KeyValuePair<Fix64, SoldierFlagBase>(distSq, agent);

                    rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
                    //if (agentNeighbors_.Count == maxNeighbors_)
                    //{
                    //    rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
                    //}
                }
            }
        }

        public virtual void Release()
        {
            Fsm?.ReleaseAllFsmState();
            Fsm = null;
            LockTarget = null;
            Entitys.Clear();
            FlagLockMeDict.Clear();
            agentNeighbors_.Clear();
        }
    }
}