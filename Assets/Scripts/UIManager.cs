using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;  //txtScoreゲームオブジェクトの持つTextコンポーネントをインスペクターからアサインする

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private CanvasGroup canvasGroupInfo;  //CanvasGroupコンポーネントをスクリプトから制御するためのアサイン用

    ///<summary>
    ///スコア表示を更新する
    ///</summary>
    public void UpdateDisplayScore(int score)
    {
        txtScore.text = score.ToString();
    }

    ///<summary>
    ///ゲームオーバー表示
    ///</summary>
    public void DisplayGameOverInfo()
    {
        //InfoBackGroundゲームオブジェクトの持つCanvansGroupコンポーネントのAlphaの値を、1秒かけて1に変更して、背景と文字が画面に見えるようにする
        canvasGroupInfo.DOFade(1.0f, 1.0f);

        //文字列をアニメーションさせて表示
        txtInfo.DOText("Game Over...", 1.0f);
    }
}
