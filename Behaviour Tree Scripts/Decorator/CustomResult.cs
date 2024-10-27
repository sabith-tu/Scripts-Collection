using System;
using RenownedGames.AITree;

[NodeContent("CustomResult", "Custom/CustomResult")]
public class CustomResult : ObserverDecorator
{
    public override event Action OnValueChange;

    public bool result;

    public override bool CalculateResult()
    {
        return result;
    }
}
