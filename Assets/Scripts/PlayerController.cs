using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";  //キー入力用の文字列指定(InputManagerのHorizontalの入力を判定するための文字列)
    private string jump = "Jump";

    private Rigidbody2D rb;  //コンポーネントの取得用
    private Animator anim;

    private float scale; //向きの設定に利用する

    private float limitPosX = 8.5f;  //横方向の制限値
    private float limitPosY = 4.45f; //縦方向の制限値

    public float moveSpeed;  //移動速度
    public float jumpPower;  //ジャンプ力・浮遊力

    public bool isGrounded;

    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;
    void Start()
    {
        //必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        scale = transform.localScale.x;
    }
    void Update()
    {
        //地面接地 Physics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが接地している距離かどうかを確認し、接地しているならtrue,接地していないならfalseを返す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 1.2f, groundLayer);

        //SceneビューにPhysics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 1.2f, Color.red, 1.0f);

        //ジャンプ
        if (Input.GetButtonDown(jump))  //InputManagerのJumpの項目に登録されているキー入力を判定する
        {
            Jump();
        }

        //接地していない(空中にいる)間で、落下中の場合
        if(isGrounded == false && rb.linearVelocity.y < 0.15f)
        {
            //落下アニメを繰り返す
            anim.SetTrigger("Fall");
        }

        //linearlinearVelocity.yの値が5.0fを超える場合(ジャンプ連続で押した場合)
        if(rb.linearVelocity.y > 5.0)
        {
            //linearVelocity.yの値に制限をかける(落下せずに上空で待機できてしまう現象を防ぐため)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5.0f);
        }
    }

    /// <summary>
    /// ジャンプと空中浮遊
    /// </summary>
    private void Jump()
    {
        //キャラの位置を上方向へ移動させる(ジャンプ・浮遊)
        rb.AddForce(transform.up * jumpPower);

        //Jump(Up+Mid)アニメーションを再生する
        anim.SetTrigger("Jump");
    }

    void FixedUpdate()
    {
        //移動
        Move();
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        //水平(横)方向への入力受付
        float x = Input.GetAxis(horizontal);  //InputManagerのHorizontakに登録されているキーの入力があるかどうか確認を行う

        //xの値が0ではない場合 = キー入力がある場合
        if(x != 0)
        {
            //velocity(速度)に新しい値を代入して移動
            //②Unity6000以降の場合
            rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

            //temp変数に現在のlocalScale値を代入
            Vector3 temp = transform.localScale;

            //現在のキー入力値xをtemp.xに代入
            temp.x = x;

            //向きが変わる時に小数になるとキャラが縮んで見えてしまうので整数値にする
            if(temp.x > 0)
            {
                //数字が0よりも大きければすべて1にする
                temp.x = scale;
            }
            else
            {
                //数字が0よりも小さければすべて-1にする
                temp.x = -scale;
            }

            //キャラの向きを移動方向に合わせる
            transform.localScale = temp;

            //①待機状態のアニメの再生を止めて、走るアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.5f); //☆追加 Runアニメーションに対して、0.5fの値を情報として渡す。遷移条件がgreater 0.1なので、0.1以上の値を渡すと条件が成立してRunアニメーションが再生される
        }
        else
        {
            //②Unity6000以降の場合
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            //②走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.0f); //☆追加 Runアニメーションに対して、0.1fの値を情報として渡す。遷移条件がless 0.1なので、0.1以下の値を渡すと条件が成立してRunアニメーションが停止される
        }

        //現在の位置情報が移動範囲の制限灰を超えていないか確認する。超えていたら、制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        //現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);
    }
}
