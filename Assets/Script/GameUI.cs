using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public InGameUI inGameUI;
    public GameOverUI gameOverUI;
    public TitleUI titleUI;
    [SerializeField] private GameObject inGameUIObj;
    [SerializeField] private GameObject gameOverUIObj;

    public void Initialize()
    {
        inGameUIObj.SetActive(false);
        gameOverUIObj.SetActive(false);
    }

    public void VisibleInGameUI(bool active)
    {
        inGameUIObj.SetActive(active);
    }

    public void VisibleGameOverUI(bool active)
    {
        gameOverUIObj.SetActive(active);
        if (active)
        {
            gameOverUI.Intialize();
        }
    }
}

[System.Serializable]
public class TitleUI
{
    [SerializeField] private Animator TitleAnimator;
    [SerializeField] private TMP_Text info;
    [SerializeField] private TMP_Text credit;

    public void EndTitle()
    {
        TitleAnimator.SetTrigger("Start");
    }

    public void Language()
    {
        credit.text = GameManager.Text_C.Credit.GetString();
        info.text = GameManager.Text_C.TitleInfo.GetString();
    }

    public void Credit()
    {
        credit.text = GameManager.Text_C.Credit.GetString();
        TitleAnimator.SetTrigger("Credit");
    }
}

[System.Serializable]
public class InGameUI
{
    [SerializeField] private GameObject interact;
    [SerializeField] private Animator timerAnimator;
    [SerializeField] private Transform evaluateTransfrom;
    [SerializeField] private GameObject[] evaluateText;
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private TMP_Text tutorialText;

    public void VisibleInteractUI(bool active, Vector2 pos)
    {
        interact.SetActive(active);
        interact.transform.position = pos;
    }

    public void ClockTimerAnimator(float exceed)
    {
        timerAnimator.SetFloat("Exceed", exceed);
    }

    public void EnableDebugText(bool active)
    {
        debugText.gameObject.SetActive(active);
    }

    public void SetDebugText(string text)
    {
        debugText.text = text;
    }

    public void EnableTutorialText(bool active)
    {
        tutorialText.gameObject.SetActive(active);
    }

    public void SetTutorialText(string text)
    {
        tutorialText.text = text;
    }

    public void DisplayEvaluate(int evaluate)
    {
        Object.Instantiate(evaluateText[evaluate], evaluateTransfrom);
    }
}

[System.Serializable]
public class GameOverUI
{
    [SerializeField] private TMP_Text reachedRoomText;
    [SerializeField] private TMP_Text failReasonText;
    [SerializeField] private TMP_Text infoText;

    public void Intialize()
    {
        infoText.text = GameManager.Text_C.GameOver[2].GetString();
    }

    public void SetReachedRoomText(int reachedRoom)
    {
        reachedRoomText.text = $"{GameManager.Text_C.GameOver[0].GetString()} : {reachedRoom}";
    }

    public void SetFailReasonText(string failReason)
    {
        failReasonText.text = $"{GameManager.Text_C.GameOver[1].GetString()} : {failReason}";
    }
}
