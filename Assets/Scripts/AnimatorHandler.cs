using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{

    [SerializeField] Animator myAnimator;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!myAnimator) return;

        AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo(0);
        myAnimator.Play(state.fullPathHash, -1, Random.Range(0, 1f));
    }
}
