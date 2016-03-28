using UnityEngine;
using System.Collections;

public class T_Animation : MonoBehaviour {
    [System.Serializable]
    public class Anim
    {
        public AnimationClip idle;
        public AnimationClip aimForward;
        public AnimationClip aimBackward;
        public AnimationClip aimRight;
        public AnimationClip aimLeft;

        public AnimationClip runForward;

        public AnimationClip sprintForward;

        public AnimationClip normalAttack;
    }
    public Anim anim;
    public Animation _animation;
    public Transform _spine;

    private T_MoveCtrl T_moveCtrl;
    private T_Attack T_attack;

    private T_MoveCtrl.MoveFlag moveFlag;
    private T_MoveCtrl.MoveState moveState;
    void Start () {
        _animation = GetComponentInChildren<Animation>();
        _animation.clip = anim.idle;




        _animation.Play();

        T_moveCtrl = GetComponent<T_MoveCtrl>();
        T_attack = GetComponent<T_Attack>();

        
    }

    void animationBlend(string basic, string additive)
    {
        if (T_attack.isFire())
        {
            _animation[basic].layer = 0;
            _animation[anim.idle.name].layer = 0;
            _animation[additive].layer = 1;
            _animation[additive].blendMode = AnimationBlendMode.Additive;
            _animation[additive].AddMixingTransform(_spine);
            _animation.Play(additive);
        }
    }


    void Update () {
        moveFlag = T_moveCtrl.getMoveFlag();
        moveState = T_moveCtrl.getMoveState();


        //run상태 이동,
        if(moveState == T_MoveCtrl.MoveState.Run)
        {

            if (moveFlag.forward) _animation.CrossFade(anim.aimForward.name, 0.15f);
            else if (moveFlag.backward) _animation.CrossFade(anim.aimBackward.name, 0.15f);
            else if (moveFlag.right) _animation.CrossFade(anim.aimRight.name, 0.15f);
            else if (moveFlag.left) _animation.CrossFade(anim.aimLeft.name, 0.15f);
            else _animation.CrossFade(anim.idle.name, 0.15f);

            if (T_attack.isFire())
            {
                _animation.Play(anim.normalAttack.name);

            }
        }
        //sprint상태 이동,
        else if (moveState == T_MoveCtrl.MoveState.Sprint)
        {

            if (moveFlag.forward) _animation.CrossFade(anim.runForward.name, 0.15f);
            else if (moveFlag.backward) _animation.CrossFade(anim.runForward.name, 0.15f);
            else if (moveFlag.right) _animation.CrossFade(anim.runForward.name, 0.15f);
            else if (moveFlag.left) _animation.CrossFade(anim.runForward.name, 0.15f);
            else _animation.CrossFade(anim.idle.name, 0.15f);
        }
        //이동상태 이외인 경우 ex(스킬 등의 애니메이션들)
        else
        {
            if (T_attack.isFire())
            {
                _animation.CrossFade(anim.normalAttack.name, 0.15f);
            }
            //idle상태
            else
                _animation.CrossFade(anim.idle.name, 0.15f);
        }
    }
}
