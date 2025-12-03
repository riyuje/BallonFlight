using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    private GoalChecker goalHousePrefab;  //ゴール地点のプレファブをアサイン

    [SerializeField]
    private PlayerController playerController;  //ヒエラルキーにあるYuko_Playerゲームオブジェクトをアサイン

    [SerializeField]
    private FloorGenerator[] floorGenerators;  //floorGeneratorスクリプトのアタッチされているゲームオブジェクトをアサイン

    [SerializeField]
    private RandomObjectGenerator[] randomObjectGenerators;  //RandomObjectGeneratorスクリプトのアタッチされているゲームオブジェクトをアサイン

    [SerializeField]
    private AudioManager audioManager;  //ヒエラルキーにあるAudioManagerスクリプトのアタッチされているゲームオブジェクトをアサイン

    private bool isSetUp;  //ゲームの準部判定用。trueになるとゲーム開始

    private bool isGameUp;  //ゲーム終了判定用。trueになるとゲーム終了

    private int generateCount;  //空中床の生成回数

    //generateCount変数のプロパティ
    public int GenerateCount
    {
        set
        {
            generateCount = value;

            Debug.Log("生成数 / クリア目標数　: " + generateCount + " / " + clearCount);

            if (generateCount >= clearCount)
            {
                //ゴール地点を生成
                GenerateGoal();

                //ゲーム終了
                GameUp();
            }
        }
        get
        {
            return generateCount;
        }
    }

    public int clearCount;  //ゴールを生成するまでに必要な空中床の生成回数

    void Start()
    {
        //タイトル曲再生
        StartCoroutine(audioManager.PlayBGM(0));
        //ゲーム開始状態にセット
        isGameUp = false;
        isSetUp = false;

        //FloorGeneratorの準備
        SetUpFloorGenerators();

        //TODO各ジェネレータを停止
        StopGenerators();
    }
    
    /// <summary>
    /// FloorGeneratorの準備
    /// </summary>
    private void SetUpFloorGenerators()
    {
        for (int i = 0; i < floorGenerators.Length; i++)
        {
            //FloorGeneratorの準備・初期設定を行う
            floorGenerators[i].SetUpGenerator(this); 
        }
    }

    void Update()
    {
        //プレイヤーがはじめてバルーンを生成したら
        if (playerController.isFirstGenerateBallon && isSetUp == false)
        {
            //準備完了
            isSetUp = true;

            //TODO各ジェネレータを動かし始める
            ActivateGenerators();

            //タイトル曲を終了し、メイン曲を再生
            StartCoroutine(audioManager.PlayBGM(1));
        }
    }

    ///<summary>
    ///ゴール地点の生成
    ///</summary>
    private void GenerateGoal()
    {
        //ゴール地点を生成
        GoalChecker goalHouse = Instantiate(goalHousePrefab);

        //TODOゴール地点の初期設定
        Debug.Log("ゴール地点　生成");
    }

    ///<summary>
    ///ゲーム終了
    ///</summary>
    public void GameUp()
    {
        //ゲーム終了
        isGameUp = true;

        //TODO各ジェネレータを停止
        StopGenerators();
    }

    ///<summary>
    ///各ジェネレータを停止する
    ///</summary>
    private void StopGenerators()
    {
        for (int i = 0; i < randomObjectGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(false);
        }

        for (int i = 0; i < floorGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(false);
        }
    }

    ///<summary>
    ///各ジェネレータを動かし始める
    ///</summary>
    private void ActivateGenerators()
    {
        for (int i = 0; i < randomObjectGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(true);
        }

        for (int i = 0; i < floorGenerators.Length; i++)
        {
            randomObjectGenerators[i].SwitchActivation(true);
        }
    }
}
