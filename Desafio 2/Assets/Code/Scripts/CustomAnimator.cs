using UnityEngine;

public class CustomAnimator : MonoBehaviour {

    public Animator animator;
    public string currentState;
    // estados de animação que vai ser pego no animator do gameObject 

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private bool AnimatorHasState(string stateName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            //Debug.Log($"clip: {clip.name}");
            if (clip.name == stateName) return true;
        }
        return false;
    }
    public void ChangeState(string newState)
    {
        //Debug.Log($"currentState: {this.currentState} = newState: {newState}");

        //if (!AnimatorHasState(newState))
        //{
        //    Debug.LogWarning($"Estado '{newState}' não existe no Animator!");
        //    return;
        //}
        if (currentState.Equals(newState)) return;
        this.currentState = newState;
        animator.Play(newState);
    }
}
