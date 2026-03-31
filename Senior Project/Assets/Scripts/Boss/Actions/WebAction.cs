using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Web", menuName = "Boss/Actions/Web")]

public class WebAction : BossAction
{
    public override void ExecuteAction(BossScript boss)
    {
        if (boss.phaseTwoActivated) { }
    }
}
