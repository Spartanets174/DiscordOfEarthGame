using System.Collections;

public class Won : State
{
    public Won(BattleSystem battleSystem) : base(battleSystem)
    {
    }
    public override IEnumerator Start()
    {
        /*Логика при победе*/
        BattleSystem.GameUIPresenter.SetEndGame($"Поздравляем с победой, {BattleSystem.PlayerController.PlayerDataController.CharacterName}! В награду вы получаете 3000 валюты!");
        BattleSystem.PlayerController.PlayerDataController.Money += 3000;
        yield break;
    }
}