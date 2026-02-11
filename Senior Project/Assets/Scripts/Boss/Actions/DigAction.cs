using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dig", menuName = "Boss/Actions/Dig")]
public class Dig : BossAction
{

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(Dig(boss));  
    }

    private IEnumerator Dig(BossScript boss)
    {
        yield return null;
    }
}