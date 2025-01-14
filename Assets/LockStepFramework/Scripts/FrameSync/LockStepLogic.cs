//
// @brief: 
// @version: 1.0.0
// @author helin
// @date: 8/20/2018
// 
// 
//


namespace Battle
{
    public class LockStepLogic
    {
        //
        public Fix64 FAccumilatedTime = Fix64.Zero;

        //
        private Fix64 m_fNextGameTime = Fix64.Zero;

        //
        private Fix64 m_fFrameLen;

        //
        private BattleLogic m_CallUnit = null;

        //
        private Fix64 m_fInterpolation = Fix64.Zero;

        public void Init()
        {
            m_fFrameLen = GameData.instance._FixFrameLen;
            GameData.instance._CumTime = Fix64.Zero;
            GameData.instance._UGameLogicFrame = 0;
            if(GameData.instance.EndStep == 0)
                GameData.instance.EndStep = (int)(GameData.instance.PlayerBattleData.maxTick / GameData.instance._FixFrameLen);

            FAccumilatedTime = Fix64.Zero;
            m_fNextGameTime = Fix64.Zero;
            m_fInterpolation = Fix64.Zero;

            m_CallUnit = ClassPool.instance.Pop<BattleLogic>();
            m_CallUnit.Init();
        }

        public void UpdateLogic()
        {
            Fix64 deltaTime = Fix64.Zero;

#if _CLIENTLOGIC_
            float dTime = UnityEngine.Time.deltaTime;
            deltaTime = (Fix64)dTime;
#else
        deltaTime = (Fix64)0.1;
#endif

            /***********************************/
            FAccumilatedTime = FAccumilatedTime + deltaTime;
            //GameData.instance._CumTime += deltaTime;

            //,,
            while (FAccumilatedTime > m_fNextGameTime)
            {

                if (GameData.instance._UGameLogicFrame >= GameData.instance.EndStep && !BattleMgr.instance.IsOutcome)
                {
                    BattleMgr.instance.DecideOutcome("fail");
                }

                //
                m_CallUnit.FrameLockLogic();

                //
                m_fNextGameTime += m_fFrameLen;

                if (!BattleMgr.instance.IsOutcome)
                {
                    //
                    GameData.instance._UGameLogicFrame += 1;
                    GameData.instance._CumTime += m_fFrameLen;
                }
            }

            //,,  =（ +  - ）/ 
            //                                                          - / 
            //
            //
            //1.    (0.02 + 0.06 - 0.06) / 0.06
            //2.    (0.04 + 0.06 - 0.06) / 0.06
            //3.    (0.06 + 0.06 - 0.06) / 0.06
            //4.    (0.08 + 0.06 - 0.12) / 0.06

            m_fInterpolation = (FAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

            //AnimMgr.instance.UnityUpdate();

            if (m_CallUnit != null)
            {
                //
                m_CallUnit.UpdateRenderPosition(m_fInterpolation);
#if _CLIENTLOGIC_
                m_CallUnit.UnityUpdate(dTime);
#endif
            }
            //m_CallUnit.UpdateRenderRotation(m_fInterpolation);
            /***********************************/
        }

        //- 
        // 
        // @param unit 
        // @return none
        //private void SetCallUnit(BattleLogic unit)
        //{
        //    m_CallUnit = unit;
        //}

        public void Release()
        {
            m_CallUnit.Release();
            m_CallUnit = null;
            ClassPool.instance.Push(this);
        }
    }
}
