using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoalChecker : MonoBehaviour
{
    public float moveSpeed = 0.01f;  //移動速度

    private float stopPos = 6.5f;  //停止地点。画面の右端でストップさせる

    private bool isGoal;  //ゴールの重複判定防止用。一度ゴール判定したらtrueにして、ゴールの判定は1回だけしか行わないようにする

    private GameDirector gameDirector;
    void Update()
    {
        //停止地点に到達するまで移動する
        if(transform.position.x > stopPos)
        {
            transform.position += new Vector3(-moveSpeed, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //接触した(ゴールした)際に1回だけ判定する
        if(col.gameObject.tag == "Player" && isGoal == false)
        {
            //2回目以降はゴール判定を行わないようにするために、trueに変更する
            isGoal = true;

            Debug.Log("ゲームクリア");

            //ゴール地点に侵入したプレイヤーのゲームオブジェクトより。PlayControllerの情報を取得(ここにプログラムが到達する、ということは、Tag の判定により col.gameObject はプレイヤーであると確定しているため取得できる)
            PlayerController playerController = col.gameObject.GetComponent<PlayerController>();

            //PlayerControllerの持つ、UIManagerの変数を利用して、GenerateResultPopUpメソッドを呼び出す。引数にはPlayerControllerのcoinCountを渡す
            playerController.uiManager.GenerateResultPopUp(playerController.coinPoint);

            //ゴール到着
            gameDirector.GoalClear();
        }
    }

    ///<summary>
    ///ゴール地点の初期設定
    ///</summary>
    public void SetUpGoalHouse(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;

        //TODO他に初期設定が必要な場合はここに追加する
    }
}
