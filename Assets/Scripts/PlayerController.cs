using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";  //キー入力用の文字列指定(InputManagerのHorizontalの入力を判定するための文字列)

    private Rigidbody2D rb;  //コンポーネントの取得用

    private Animator anim;

    private float scale; //向きの設定に利用する

    public float moveSpeed;  //移動速度
    void Start()
    {
        //必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        scale = transform.localScale.x;
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
    }

    void Update()
    {
        
    }
}
