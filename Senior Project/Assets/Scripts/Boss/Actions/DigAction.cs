using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dig", menuName = "Boss/Actions/Dig")]
public class Dig : BossAction
{

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(DigCoroutine(boss));  
    }

    private IEnumerator DigCoroutine(BossScript boss)
    {
        yield return null;
    }
}