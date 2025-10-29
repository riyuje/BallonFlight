using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";  //�L�[���͗p�̕�����w��(InputManager��Horizontal�̓��͂𔻒肷�邽�߂̕�����)
    private string jump = "Jump";

    private Rigidbody2D rb;  //�R���|�[�l���g�̎擾�p
    private Animator anim;

    private float scale; //�����̐ݒ�ɗ��p����

    private float limitPosX = 8.5f;  //�������̐����l
    private float limitPosY = 4.45f; //�c�����̐����l

    public float moveSpeed;  //�ړ����x
    public float jumpPower;  //�W�����v�́E���V��

    public bool isGrounded;

    [SerializeField, Header("Linecast�p �n�ʔ��背�C���[")]
    private LayerMask groundLayer;
    void Start()
    {
        //�K�v�ȃR���|�[�l���g���擾���ėp�ӂ����ϐ��ɑ��
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        scale = transform.localScale.x;
    }
    void Update()
    {
        //�n�ʐڒn Physics2D.Linecast���\�b�h�����s���āAGround Layer�ƃL�����̃R���C�_�[�Ƃ��ڒn���Ă��鋗�����ǂ������m�F���A�ڒn���Ă���Ȃ�true,�ڒn���Ă��Ȃ��Ȃ�false��Ԃ�
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 1.2f, groundLayer);

        //Scene�r���[��Physics2D.Linecast���\�b�h��Line��\������
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 1.2f, Color.red, 1.0f);

        //�W�����v
        if (Input.GetButtonDown(jump))  //InputManager��Jump�̍��ڂɓo�^����Ă���L�[���͂𔻒肷��
        {
            Jump();
        }

        //�ڒn���Ă��Ȃ�(�󒆂ɂ���)�ԂŁA�������̏ꍇ
        if(isGrounded == false && rb.linearVelocity.y < 0.15f)
        {
            //�����A�j�����J��Ԃ�
            anim.SetTrigger("Fall");
        }

        //linearlinearVelocity.y�̒l��5.0f�𒴂���ꍇ(�W�����v�A���ŉ������ꍇ)
        if(rb.linearVelocity.y > 5.0)
        {
            //linearVelocity.y�̒l�ɐ�����������(���������ɏ��őҋ@�ł��Ă��܂����ۂ�h������)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5.0f);
        }
    }

    /// <summary>
    /// �W�����v�Ƌ󒆕��V
    /// </summary>
    private void Jump()
    {
        //�L�����̈ʒu��������ֈړ�������(�W�����v�E���V)
        rb.AddForce(transform.up * jumpPower);

        //Jump(Up+Mid)�A�j���[�V�������Đ�����
        anim.SetTrigger("Jump");
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

        //���݂̈ʒu��񂪈ړ��͈͂̐����D�𒴂��Ă��Ȃ����m�F����B�����Ă�����A�����͈͓��Ɏ��߂�
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        //���݂̈ʒu���X�V(�����͈͂𒴂����ꍇ�A�����ňړ��͈̔͂𐧌�����)
        transform.position = new Vector2(posX, posY);
    }
}
