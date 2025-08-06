using System.Collections;
using UnityEngine;

public class EnemyAttackAction : MonoBehaviour, IUnitAction
{
    [Header("Animação")]
    [SerializeField] Animator myAnimator;
    [SerializeField] AnimationClip attackAnimation;
     private ICombatUnit actionController;

    public void ExecuteAction(TileData targetTile, GridUnit actor)
    {
        if (targetTile.OccupyingUnit == null) return;

        actionController = actor.genericActionController;
        StartCoroutine(AttackCharacterRoutine());
        targetTile.OccupyingUnit.health.TakeDamage(actor.stats.attack);
    }

    private IEnumerator AttackCharacterRoutine()
    {
        myAnimator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(attackAnimation.length);
        myAnimator.SetBool("isAttacking", false);
        actionController.BeforeEndTurn();

    }
}
