using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;//发生淡入淡出的默认值
    public float displayImageDuration = 1f;//持续时间变量
    public GameObject player;//引用游戏对象玩家角色
    //为Canvas Group组件创建一个可以在Inspector中分配的公共变量
    public CanvasGroup exitBackgroundImageCanvasGroup;
    public AudioSource exitAudio;//引用音频源
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    public AudioSource caughtAudio;

    bool m_IsPlayerAtExit;//是否抵达出口
    bool m_isPlayerCaught;//是否被抓住
    float m_Timer;//计时器，确保游戏不会在淡入淡出之前结束
    bool m_HasAudioPlayed;//是否已经播放过（确保只播放一次）

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)//仅当玩家角色触发时
        {
            m_IsPlayerAtExit = true;//抵达出口信号为true
        }
    }

    public void CaughtPlayer()//构建公共方法，用于使外部可以改变私有变量m_m_isPlayerCaught
    {
        m_isPlayerCaught = true;
    }

    void Update()
    {
        if (m_IsPlayerAtExit)//如果抵达出口
        {
            EndLevel(exitBackgroundImageCanvasGroup, false, exitAudio);//调用结束关卡
        }
        else if (m_isPlayerCaught)
        {
            EndLevel(caughtBackgroundImageCanvasGroup, true, caughtAudio);//被抓时可以重开
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();//按Q键退出
        }
    }

    void EndLevel(CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)//设置参数传入内容，用于判断改变哪个alpha
    {
        if(!m_HasAudioPlayed)//如果没播放过
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }

        m_Timer += Time.deltaTime;
        //计时器为0时alpha为0，计时器>0且不超过fadeDuration(持续时间)时alpha为1
        imageCanvasGroup.alpha = m_Timer / fadeDuration;//改变参数传入的内容的alpha

        if (m_Timer > fadeDuration + displayImageDuration) 
        {
            if (doRestart) 
            {
                SceneManager.LoadScene(0);//重载场景，项目索引从0开始，所以第一个场景索引为0
            }
            //else
            //{
            //    Application.Quit();//退出游戏（该方法仅适用于完全构建的应用程序）
            //}
        }
    }
}
