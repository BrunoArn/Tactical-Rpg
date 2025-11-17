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
        if (targetTile == null) return;

        actionController = actor.genericActionController;
        StartCoroutine(AttackCharacterRoutine());

        if (targetTile.OccupyingUnit != null)
        {
            targetTile.OccupyingUnit.health.TakeDamage(actor.stats.attack);
            return;
        }

        if (targetTile.OccupyingObstacle != null && targetTile.OccupyingObstacle.health != null)
        {
            targetTile.OccupyingObstacle.health.TakeDamage(actor.stats.attack);
        }
    }

    private IEnumerator AttackCharacterRoutine()
    {
        myAnimator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(attackAnimation.length);
        myAnimator.SetBool("isAttacking", false);
        actionController.BeforeEndTurn();

    }
}
