using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChecker : MonoBehaviour
{
    private MoveObject moveObject;  //MoveObjectスクリプトを取得した際に代入するための準備
    
    void Start()
    {
        moveObject = GetComponent<MoveObject>();  //このスクリプトがアタッチされているゲームオブジェクトの持つ、MoveObjectスクリプトを探して取得し、moveObject変数に代入
    }

    ///<summary>
    ///空中床に移動速度を与える
    ///</summary>
    public void SetInitialSpeed()
    {
        //アサインしているゲームオブジェクトの持つMoveObjectスクリプトのmoveSpeed変数にアクセスして、右辺の値を代入する
        moveObject.moveSpeed = 0.005f;
    }
   
}
