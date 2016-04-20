using UnityEngine;
using System.Collections;

public class T_Animation : MonoBehaviour {
    #region<레거시 애니메이션>
    //[System.Serializable]
    //public class Anim
    //{
    //    public AnimationClip idle;
    //    public AnimationClip aimForward;
    //    public AnimationClip aimBackward;
    //    public AnimationClip aimRight;
    //    public AnimationClip aimLeft;

    //    public AnimationClip runForward;

    //    public AnimationClip sprintForward;

    //    public AnimationClip normalAttack;
    //}
    //public Anim anim;
    //public Animation _animation;
    //public Transform _spine;
    #endregion



    private Animator animator;

    private T_MoveCtrl t_MoveCtrl;
    private T_Attack t_Attack;
    private T_MoveCtrl t_Movectrl;
    private Transform trPlayerModel;
    private T_Mgr t_Mgr;
    
    private T_MoveCtrl.MoveFlag moveFlag;
    private T_MoveCtrl.MoveState moveState;
    private float h, v;
    private float fSpeed;


    GameObject model;


    void Start () {
        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        t_Attack = GetComponent<T_Attack>();
        t_MoveCtrl = GetComponent<T_MoveCtrl>();
        animator = GetComponentInChildren<Animator>();
        t_Mgr = GetComponent<T_Mgr>();
        trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;


        fSpeed = t_MoveCtrl.GetMoveSpeed();
    }

    bool freeze = false;
    float freezeTime = 0.0f;

    void Update() {
        moveFlag = t_MoveCtrl.GetMoveFlag();
        moveState = t_MoveCtrl.GetMoveState();
        fSpeed = t_MoveCtrl.GetMoveSpeed();

        if (t_Mgr.GetState() != T_Mgr.State.Skill)
        {
            if (moveState == T_MoveCtrl.MoveState.Stop)
            {
                animator.SetBool("bIdle", true);
            }
            else if (moveFlag.forward || moveFlag.backward || moveFlag.right || moveFlag.left)
            {
                animator.SetBool("bIdle", false);
                animator.SetFloat("fSpeed", fSpeed);
            }
        }
        else
        {
            animator.SetBool("bIdle", false);
            animator.SetFloat("fSpeed", fSpeed);
        }


        #region<레거시 애니메이션>
        ////run상태 이동,
        //if(moveState == T_MoveCtrl.MoveState.Run)
        //{

        //    if (moveFlag.forward) _animation.CrossFade(anim.aimForward.name, 0.15f);
        //    else if (moveFlag.backward) _animation.CrossFade(anim.aimBackward.name, 0.15f);
        //    else if (moveFlag.right) _animation.CrossFade(anim.aimRight.name, 0.15f);
        //    else if (moveFlag.left) _animation.CrossFade(anim.aimLeft.name, 0.15f);
        //    else _animation.CrossFade(anim.idle.name, 0.15f);

        //    if (t_Attack.isFire())
        //    {
        //        _animation.Play(anim.normalAttack.name);

        //    }
        //}
        ////sprint상태 이동,
        //else if (moveState == T_MoveCtrl.MoveState.Sprint)
        //{

        //    if (moveFlag.forward) _animation.CrossFade(anim.runForward.name, 0.15f);
        //    else if (moveFlag.backward) _animation.CrossFade(anim.runForward.name, 0.15f);
        //    else if (moveFlag.right) _animation.CrossFade(anim.runForward.name, 0.15f);
        //    else if (moveFlag.left) _animation.CrossFade(anim.runForward.name, 0.15f);
        //    else _animation.CrossFade(anim.idle.name, 0.15f);
        //}
        ////이동상태 이외인 경우 ex(스킬 등의 애니메이션들)
        //else
        //{
        //    if (t_Attack.isFire())
        //    {
        //        _animation.CrossFade(anim.normalAttack.name, 0.15f);
        //    }
        //    //idle상태
        //    else
        //        _animation.CrossFade(anim.idle.name, 0.15f);
        //}
        #endregion
    }

    void Freeze()
    {
        freezeTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    void UnFreeze()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(stateInfo.nameHash, 0, freezeTime);
    }

}
