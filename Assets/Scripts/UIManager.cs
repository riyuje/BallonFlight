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
    private CanvasGroup canvasGroupInfo;  //CanvasGroupコンポーネントをスクリプトから制御するためのアサイン場所S

    [SerializeField]
    private ResultPopUp resultPopUpPrefab;  //今回作成しているResultPopUpスクリプトを制御するためのアサイン場所

    [SerializeField]
    private Transform canvasTran;

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

    ///<summary>
    ///ResultPopUpの生成
    ///</summary>
    public void GenerateResultPopUp(int score)
    {
        //ResultPopUpを生成
        ResultPopUp resultPopUp = Instantiate(resultPopUpPrefab, canvasTran, false);

        //ResultPopUpの設定を行う
        resultPopUp.SetUpResultPopUp(score);
    }
}
