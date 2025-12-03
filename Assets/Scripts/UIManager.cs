using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;  //txtScoreゲームオブジェクトの持つTextコンポーネントをインスペクターからアサインする

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private CanvasGroup canvasGroupInfo;  //CanvasGroupコンポーネントをスクリプトから制御するためのアサイン場所

    [SerializeField]
    private ResultPopUp resultPopUpPrefab;  //今回作成しているResultPopUpスクリプトを制御するためのアサイン場所

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private Button btnInfo;

    [SerializeField]
    private Button btnTitle;

    [SerializeField]
    private Text lblStart;

    [SerializeField]
    private CanvasGroup canvasGroupTitle;

    [SerializeField]
    private CanvasGroup txtInfocanvasGroup; //txtInfo用

    private Tweener tweener;

    ///<summary>
    ///スコア表示を更新する
    ///</summary>
    ///<param name="score"></param>
    public void UpdateDisplayScore(int score)
    {
        txtScore.text = score.ToString();
    }

    ///<summary>
    ///ゲームオーバー表示
    ///</summary>
    public void DisplayGameOverInfo()
    {
        Debug.Log("DisplayGameOverInfoメソッド開始");
        Debug.Log($"canvasGroupInfo.alpha -> {canvasGroupInfo.alpha}");

        //InfoBackGroundゲームオブジェクトの持つCanvansGroupコンポーネントのAlphaの値を、1秒かけて1に変更して、背景と文字が画面に見えるようにする
        canvasGroupInfo.DOFade(1.0f, 1.0f);
        txtInfocanvasGroup.DOFade(1.0f, 1.0f);

        //文字列をアニメーションさせて表示
        txtInfo.DOText("Game Over...", 1.0f);

        canvasGroupInfo.blocksRaycasts = true;
        btnInfo.onClick.AddListener(RestartGame);
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

    ///<summary>
    ///タイトルへ戻る
    ///</summary>
    public void RestartGame()
    {
        Debug.Log("RestartGameメソッドが呼ばれました");

        //ボタンからメソッドを削除(重複クリック防止)
        btnInfo.onClick.RemoveAllListeners();

        //現在のシーンの名前を取得
        string sceneName = SceneManager.GetActiveScene().name;

        canvasGroupInfo.DOFade(0f, 1.0f).OnComplete(()=>
        {
            Debug.Log("Restart");
            SceneManager.LoadScene(sceneName);
        });
    }

    private void Start()
    {

        //タイトル表示
        SwitchDisplayTitle(true, 1.0f);

        //ボタンのOnClickイベントにメソッドを登録
        btnTitle.onClick.AddListener(OnClickTitle);
    }

    ///<summary>
    ///タイトル表示
    ///</summary>
    public void SwitchDisplayTitle(bool isSwitch, float alpha)
    {
        if (isSwitch) canvasGroupTitle.alpha = 0;

        canvasGroupTitle.DOFade(alpha, 1.0f).SetEase(Ease.Linear).OnComplete(() =>
        {
            lblStart.gameObject.SetActive(isSwitch);
        });

        if(tweener == null)
        {
            //Tap Startの文字をゆっくり点滅させる
            tweener = lblStart.gameObject.GetComponent<CanvasGroup>().DOFade(0, 1.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            tweener.Kill();
        }
    }

    ///<summary>
    ///タイトル表示中に画面をクリックした際の処理
    ///</summary>
    private void OnClickTitle()
    {
        //ボタンのメソッドを削除して重複タップ防止
        btnTitle.onClick.RemoveAllListeners();

        //タイトルを徐々に非表示
        SwitchDisplayTitle(false, 0.0f);

        //タイトル表示が消えるのと入れ替わりで、ゲームスタートの文字を表示する
        StartCoroutine(DisplayGameStartInfo());
    }

    ///<summary>
    ///ゲームスタート表示
    ///</summary>
    ///<returns></returns>

    public IEnumerator DisplayGameStartInfo()
    {
        Debug.Log("開始");
        yield return new WaitForSeconds(0.5f);

        canvasGroupInfo.alpha = 0;
        txtInfocanvasGroup.alpha = 0; 

        canvasGroupInfo.DOFade(1.0f, 0.5f);
        txtInfocanvasGroup.DOFade(1.0f, 0.5f); 
        txtInfo.text = "Game Start!";
        Debug.Log("フェードイン開始");

        yield return new WaitForSeconds(1.0f);
        Debug.Log("フェードアウト開始");

        canvasGroupInfo.DOFade(0f, 0.5f);
        txtInfocanvasGroup.DOFade(0f, 0.5f); 

        yield return new WaitForSeconds(0.5f);
        Debug.Log("フェードアウト完了");

        canvasGroupInfo.blocksRaycasts = false;
        //canvasGroupInfo.gameObject.SetActive(false);
        canvasGroupTitle.gameObject.SetActive(false);
        Debug.Log("タイトル非表示");
    }
}
