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

    public bool isGameOver = false;  //GameOver状態の判定用。trueならゲームオーバー
    public bool isFirstGenerateBallon;  //初めてバルーンを生成したかを判定するための変数(後程外部スクリプトでも利用するためpublicで宣言する)

    public float moveSpeed;  //移動速度
    public float jumpPower;  //ジャンプ力・浮遊力

    public bool isGrounded;

    public GameObject[] ballons;  //GameObject型の配列。インスペクターからヒエラルキーにあるBallonゲームオブジェクトを2つアサインする
    public int maxBallonCount;  //バルーンを生成する最大数
    public Transform[] ballonTrans; //バルーンの生成位置の配列
    public GameObject ballonPrefab; //バルーンのプレファブ

    public float generateTime; //バルーンを生成する時間
    public bool isGenerating; //バルーンを生成中かどうかを判定する。falseなら生成していない状態。trueは生成中の状態。

    public float knockbackPower;
    public int coinPoint;

    public UIManager uiManager;

    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;

    [SerializeField]
    private StartChecker startChecker;

    [SerializeField]
    private AudioClip knockBackSE;  //敵と接触した際にならすSE用のオーディオファイルをアサインする

    [SerializeField]
    private GameObject knockbackEffectPrefab;  //敵と接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする
    void Start()
    {
        //必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        scale = transform.localScale.x;

        //配列の初期化(バルーンの最大生成数だけ配列の要素数を用意する)
        ballons = new GameObject[maxBallonCount];
    }
    void Update()
    {
        //地面接地 Physics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが接地している距離かどうかを確認し、接地しているならtrue,接地していないならfalseを返す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 1.2f, groundLayer);

        //SceneビューにPhysics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 1.2f, Color.red, 1.0f);

        //ballons変数の最初の要素の値が空ではないなら = バルーンが1ツ生成されるとこの要素に値が代入される = バルーンが1つあるなら
        if (ballons[0] != null)
        {
            //ジャンプ
            if (Input.GetButtonDown(jump))  //InputManagerのJumpの項目に登録されているキー入力を判定する
            {
                Jump();
            }

            //接地していない(空中にいる)間で、落下中の場合
            if (isGrounded == false && rb.linearVelocity.y < 0.15f)
            {
                //落下アニメを繰り返す
                anim.SetTrigger("Fall");
            }

        }
        else
        {
            Debug.Log("バルーンがない。ジャンプ不可");
        }

        //linearlinearVelocity.yの値が5.0fを超える場合(ジャンプ連続で押した場合)
        if (rb.linearVelocity.y > 5.0)
        {
            //linearVelocity.yの値に制限をかける(落下せずに上空で待機できてしまう現象を防ぐため)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5.0f);
        }

        //地面に接地していち、バルーンが生成中ではない場合
        if(isGrounded == true && isGenerating == false)
        {
            //Qボタンを押したら
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //バルーンを1つ作成する
                StartCoroutine(GenerateBallon());
            }
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
        if (isGameOver == true)
        {
            return;
        }

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
            anim.SetBool("Idle", false);
        }
        else
        {
            //②Unity6000以降の場合
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            //②走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.0f); //☆追加 Runアニメーションに対して、0.1fの値を情報として渡す。遷移条件がless 0.1なので、0.1以下の値を渡すと条件が成立してRunアニメーションが停止される
            anim.SetBool("Idle", true);
        }

        //現在の位置情報が移動範囲の制限灰を超えていないか確認する。超えていたら、制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        //現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);
    }

    ///<summary>
    ///バルーン生成
    ///</summary>
    ///<returns></returns>
    private IEnumerator GenerateBallon()
    {
        //全ての配列の要素にバルーンが存在している場合には、バルーンを生成しない
        if (ballons[1] != null)
        {
            yield break;
        }

        //生成中状態にする
        isGenerating = true;

        //isFirstGenerateBallon変数の値がfalse,つまり、ゲームを開始してから、まだバルーンを1回も生成していないなら
        if (isFirstGenerateBallon == false)
        {
            //初回バルーン生成を行ったと判断し、trueに変更する = 次回以降はバルーンを生成しても、if文内の条件を満たさなくなり、この処理には入らない
            isFirstGenerateBallon = true;

            Debug.Log("初回のバルーン生成");

            //startChecker変数に代入されているStartCheckerスクリプトにアクセスして、SetInitiSpeedメソッドを実行する
            startChecker.SetInitialSpeed();
        }

        //1つめの配列の要素が空なら
        if (ballons[0] == null)
        {
            //1つ目のバルーン生成を生成して、1番目の配列へ代入
            ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);

            ballons[0].GetComponent<Ballon>().SetUpBallon(this);
        }
        else
        {
            //2つ目のバルーン生成を生成して、2番目の配列へ代入
            ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);

            ballons[1].GetComponent<Ballon>().SetUpBallon(this);
        }

        //生成時間分待機
        yield return new WaitForSeconds(generateTime);

        //生成中状態終了。再度生成できるようにする
        isGenerating = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //接触したコライダーを持つゲームオブジェクトのTagがEnemyなら
        if(col.gameObject.tag == "Enemy")
        {
            //キャラと敵の位置から距離と方向を計算して、正規化処理を行い、direction変数へ代入
            Vector3 direction = (transform.position - col.transform.position).normalized;

            //敵の反対側にキャラを吹き飛ばす
            transform.position += direction * knockbackPower;

            //敵との接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(knockBackSE, transform.position);

            //接触した際のエフェクトを、敵の位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);
        }
    }

    ///<summary>
    ///バルーン破壊
    ///</summary>
    public void DestroyBallon()
    {
        //TODO後程、バルーンが破壊される際に「割れた」ように見えるアニメ演出を追加する
        if (ballons[1] != null)
        {
            Destroy(ballons[1]);
        }
        else if (ballons[0] != null)
        {
            Destroy(ballons[0]);
        }
    }

    //IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    private void OnTriggerEnter2D(Collider2D col)
    {
        //通過したコライダーを持つゲームオブジェクトのTagがCoinの場合
        if (col.gameObject.tag == "Coin")
        {
            //通過したコライダーを持つゲームオブジェクトの持つCoinスクリプトを取得し、point変数の値をキャラの持つcoinPoint変数に加算
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            uiManager.UpdateDisplayScore(coinPoint);

            //通過したコインのゲームオブジェクトを破壊する
            Destroy(col.gameObject);
        }
    }

    ///<summary>
    ///ゲームオーバー
    ///</summary>
    public void GameOver()
    {
        isGameOver = true;

        //ConsoleビューにisGameOver変数の値を表示する。ここが実行されるとtrueと表示される
        Debug.Log(isGameOver);

        //画面にゲームオーバー表示を行う
        uiManager.DisplayGameOverInfo();
    }
}
