using System.Collections;

public class Lost: State
{ 
    public Lost(BattleSystem battleSystem): base(battleSystem)
    {
    }
    public override IEnumerator Start()
    {
        BattleSystem.GameUIPresenter.SetEndGame($"Увы, {BattleSystem.PlayerController.PlayerDataController.CharacterName}, но вы проиграли! Но не отчаивайтесь, за старания мы дарим вам 500 валюты!");
        BattleSystem.PlayerController.PlayerDataController.Money += 500;
        yield break;
    }
}