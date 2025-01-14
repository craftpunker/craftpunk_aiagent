
namespace Battle
{
    public class SoldierFlagFactory : Singleton<SoldierFlagFactory>
    {
        public SoldierFlagBase CreateFlag(TroopsData troopData, PlayerGroup group)
        {
            SoldierFlagBase flag;
            flag = ClassPool.instance.Pop<NormalSoldierFlag>();

            flag.Init();
            flag.Id = troopData.id;
            flag.SoldierFlagId = troopData.SoldierFlagId;
            flag.PlayerGroup = group;
            flag.SoldierType = (SoldierType)troopData.soldierType;
            flag.MoveSpeed = troopData.moveSpeed * 0.5;
            flag.Pos = troopData.pos;//group == PlayerGroup.Player ? troopData.pos : new FixVector3(-troopData.pos.x, troopData.pos.y, Fix64.Zero);
            flag.ActionDistance = troopData.atkRange;
            flag.FreeSeekDis = group == PlayerGroup.Player ? troopData.freeSeekDis : troopData.freeSeekDis * -Fix64.One;
            flag.RowForward = group == PlayerGroup.Player ? GameData.instance._Right : GameData.instance._Right * -Fix64.One;
            flag.Start();

            return flag;
        }
    }
}