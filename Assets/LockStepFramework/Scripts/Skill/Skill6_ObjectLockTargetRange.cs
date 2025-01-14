
using System.Collections.Generic;
#if _CLIENTLOGIC_
using UnityEngine;
#endif


namespace Battle
{
    //，【skf1】()

#if _CLIENTLOGIC_
    public class ManyObj
    {
        public Transform Obj;
        public Vector3 Dir;
        public Vector3 Offset;
        public bool IsRun;
    }
#endif

    public class Skill6_ObjectLockTargetRange : SkillBase
    {
        private int skfId1;

        private Fix64 radius;
        private int count;
        private int objCount; //
        private Fix64 speed;

        private PlayerGroup targetGroup;
        private FixVector3 targetPos;

        private FixVector3 skillStartPos;
        private FixVector3 distance;
        private Fix64 maxTime;
        private Fix64 time;

        private Fix64 waitReleaseTime = Fix64.One;
        private bool isWaitRelease;
#if _CLIENTLOGIC_
        private Vector3 forward;
        private List<ManyObj> manyObjs = new List<ManyObj>();
        private SpecialEffect specRange;
#endif

        public override void Start()
        {
            base.Start();
            skfId1 = SkillData.skillEffectCfgIds[0];

            radius = SkillData.fix64Args[0];
            count = (int)SkillData.fix64Args[1];
            speed = SkillData.fix64Args[2];
            objCount = (int)SkillData.fix64Args[3];

            targetGroup = GameUtils.GetTargetGroup(PlayerGroup, SkillData.targetGroup);
            skillStartPos = PlayerGroup == PlayerGroup.Player ? new FixVector3(-25, 18, -20) : new FixVector3(25, 18, -20);
            targetPos = Fixv3LogicPosition;
            Fixv3LogicPosition = skillStartPos;
            distance = targetPos - skillStartPos;
            maxTime = FixVector3.Model(distance) / speed;
            time = Fix64.Zero;
            isWaitRelease = false;

#if _CLIENTLOGIC_
            Is3D = GameUtils.PrefabIs3D(AnimData);

            forward = distance.GetNormalized().ToVector3();

            for (int i = 0; i < objCount; i++)
            {
                if (i == 0)
                {
                    CreateFromPrefab(AnimData.Prefab, (go) =>
                    {
                        if (Is3D)
                        {
                            go.transform.forward = forward;
                        }
                        else
                        {
                            SetMeshBlock();
                            DoAnim(AnimType.Idle);
                            BattleMgr.instance.FaceEnemy(Origin, Target);
                            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                            GpuAnim.localRotation = Quaternion.Euler(-45, 0, angle);
                        }
                        Trans.position = skillStartPos.ToVector3();
                        go.SetActive(true);
                    });
                }
                else
                {
                    ResMgr.instance.LoadGameObjectAsync(AnimData.Prefab, (go) =>
                    {
                        var range = Fix64.One;
                        Fix64 randomX = GameData.instance.sRandom.Range(-range, range);
                        Fix64 randomY = GameData.instance.sRandom.Range(-range, range);
                        Fix64 randomZ = GameData.instance.sRandom.Range(-range, range);

                        FixVector3 randomPos = new FixVector3(randomX, randomY, randomZ);

                        go.transform.forward = forward;
                        //go.transform.position = Trans.position;//.ToVector3(); //+ randomPos.ToVector3();
                        ManyObj obj = ClassPool.instance.Pop<ManyObj>();
                        obj.Dir = forward;
                        obj.Offset = randomPos.ToVector3();
                        obj.Obj = go.transform;
                        obj.IsRun = true;
                        manyObjs.Add(obj);
                    });
                }
            }

            specRange = SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 2, targetPos);
#endif
        }

        public override void Update()
        {
            base.Update();

            var t = time / maxTime;
            time += GameData.instance._FixFrameLen;

            if (!isWaitRelease)
            {
                if (t >= Fix64.One)
                {
                    t = Fix64.One;
                    isWaitRelease = true;
                    DoSkill();
                    if (objCount > 1)
                    {
                        TimeMgr.instance.SetTimeAction(new Fix64(2), () =>
                        {
                            BKilled = true;
                        });
                    }
                    else
                    {
                        BKilled = true;
                    }
                }
                Fixv3LogicPosition = skillStartPos + distance * t;
            }
        }

        private void DoSkill()
        {
            Fix64 rangeSq = radius * radius;
            List<Agent> agents = new List<Agent>();
            Simulator.instance.kdTree_.computeAgentNeighborsByPos(targetPos.ToFixVector2(), ref rangeSq, targetGroup, ref agents, count);

            foreach (Agent agent in agents)
            {
                SkillEffectFactory.instance.CreateSkillEffect(skfId1, this, Origin, agent.entity);
            }

#if _CLIENTLOGIC_
            SpecialEffectFactory.instance.CreateSpecialEffect(SkillData.animCfgIds, 1, targetPos);
#endif
        }

#if _CLIENTLOGIC_
        private void MoveManyObj(float interpolation)
        {
            for (int i = 0; i < manyObjs.Count; i++)
            {
                var item = manyObjs[i];
                if (item.IsRun)
                {
                    if (isWaitRelease)
                    {
                        //
                        item.Obj.position += (float)speed * interpolation * item.Dir;
                    }
                    else
                    {
                        //skill，manyobjskill
                        item.Obj.position = Trans.position + item.Offset;
                    }

                    if (item.Obj.position.z > 0)
                    {
                        item.IsRun = false;
                        item.Obj.position = new Vector3(item.Obj.position.x, item.Obj.position.y, 0);
                    }
                }
            }
        }

        public override void UpdateAnim(float interpolation)
        {
            base.UpdateAnim(interpolation);

            if (objCount > 1 && Trans != null)
            {
                MoveManyObj(interpolation);
            }
        }
#endif

        public override void Release()
        {
#if _CLIENTLOGIC_
            for (int i = manyObjs.Count - 1; i >= 0; i--)
            {
                var item = manyObjs[i];
                ResMgr.instance.ReleaseGameObject(item.Obj.gameObject);
            }

            specRange.BKilled = true;
            specRange = null;

            //ResMgr.instance.ReleaseGameObject(rangeObj);
            manyObjs.Clear();
#endif
            ClassPool.instance.Push(this);
            base.Release();
        }
    }
}
