

namespace Battle
{
    public class BattleEndFsm : FsmState<BattleLogic>
    {
        public override void OnEnter(BattleLogic owner)
        {
            base.OnEnter(owner);
            GameData.instance.BattleScene = BattleScene.End;

        }
    }
}
