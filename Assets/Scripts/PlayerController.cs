using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";  //�L�[���͗p�̕�����w��(InputManager��Horizontal�̓��͂𔻒肷�邽�߂̕�����)

    private Rigidbody2D rb;  //�R���|�[�l���g�̎擾�p

    private Animator anim;

    private float scale; //�����̐ݒ�ɗ��p����

    public float moveSpeed;  //�ړ����x
    void Start()
    {
        //�K�v�ȃR���|�[�l���g���擾���ėp�ӂ����ϐ��ɑ��
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        scale = transform.localScale.x;
    }

    void FixedUpdate()
    {
        //�ړ�
        Move();
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    private void Move()
    {
        //����(��)�����ւ̓��͎�t
        float x = Input.GetAxis(horizontal);  //InputManager��Horizontak�ɓo�^����Ă���L�[�̓��͂����邩�ǂ����m�F���s��

        //x�̒l��0�ł͂Ȃ��ꍇ = �L�[���͂�����ꍇ
        if(x != 0)
        {
            //velocity(���x)�ɐV�����l�������Ĉړ�
            //�AUnity6000�ȍ~�̏ꍇ
            rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

            //temp�ϐ��Ɍ��݂�localScale�l����
            Vector3 temp = transform.localScale;

            //���݂̃L�[���͒lx��temp.x�ɑ��
            temp.x = x;

            //�������ς�鎞�ɏ����ɂȂ�ƃL�������k��Ō����Ă��܂��̂Ő����l�ɂ���
            if(temp.x > 0)
            {
                //������0�����傫����΂��ׂ�1�ɂ���
                temp.x = scale;
            }
            else
            {
                //������0������������΂��ׂ�-1�ɂ���
                temp.x = -scale;
            }

            //�L�����̌������ړ������ɍ��킹��
            transform.localScale = temp;

            //�@�ҋ@��Ԃ̃A�j���̍Đ����~�߂āA����A�j���̍Đ��ւ̑J�ڂ��s��
            anim.SetFloat("Run", 0.5f); //���ǉ� Run�A�j���[�V�����ɑ΂��āA0.5f�̒l�����Ƃ��ēn���B�J�ڏ�����greater 0.1�Ȃ̂ŁA0.1�ȏ�̒l��n���Ə�������������Run�A�j���[�V�������Đ������
        }
        else
        {
            //�AUnity6000�ȍ~�̏ꍇ
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            //�A����A�j���̍Đ����~�߂āA�ҋ@��Ԃ̃A�j���̍Đ��ւ̑J�ڂ��s��
            anim.SetFloat("Run", 0.0f); //���ǉ� Run�A�j���[�V�����ɑ΂��āA0.1f�̒l�����Ƃ��ēn���B�J�ڏ�����less 0.1�Ȃ̂ŁA0.1�ȉ��̒l��n���Ə�������������Run�A�j���[�V��������~�����
        }
    }

    void Update()
    {
        
    }
}
