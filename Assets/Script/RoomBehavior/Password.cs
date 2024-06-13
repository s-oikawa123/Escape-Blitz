using Container;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Password : RoomInfo
{
    private GameObject symbolObject;
    private string answer;

    // ルームプロパティ
    private int answerLength;
    private PasswordVariant passVariant;
    private float timePerChar;
    private int subAnswerLength;
    private bool suffuleFlag;
    private enum Direction
    {
        forward,
        left,
        right,
        back
    }
    private Direction passwordDirection;

    private protected override void SetUp()
    {
        doorManager.SetSignal(Container.Signal.Challenge);
        answer = "";
        exitTime = 3 + timePerChar * answerLength;
        SetPassword();
        SetSymbols(answer);
    }

    private protected override void First()
    {
        if (wrongRoomFrag)
        {
            gameManager.KeypadManager.SetUp(suffuleFlag, answer.Substring(0, answer.Length - 1), passVariant);
            summery = GameManager.Text_C.FailReason.PasswordRoom[0].GetString();
            exitTime = 7f;
        }
        else
        {
            gameManager.KeypadManager.SetUp(suffuleFlag, answer, passVariant);
        }
    }

    private protected override void Every()
    {

    }

    public override void Signal(Signal signal) 
    {
        switch (signal)
        {
            case Container.Signal.True:
                gameManager.ActiveNextRoom();
                roomClearedFrag = true;
                doorManager.SetActive(false);
                backDoorManager.SetActive(false);
                break;
            case Container.Signal.False:
                gameManager.GameOverInitiation(GameManager.Text_C.FailReason.Utility[1].GetString());
                break;
            case Container.Signal.Challenge:
                gameManager.DisplayKeypat();
                doorManager.SetActive(false);
                break;
            case Container.Signal.Complete:
                doorManager.Open();
                gameManager.ExitKeyPad(false);
                if (doorManager.FalseDoor)
                {
                    gameManager.GameOverInitiation(summery);
                }
                else
                {
                    roomClearedFrag = true;
                    gameManager.ActiveNextRoom();
                }
                break;
            case Container.Signal.Leave:
                doorManager.SetActive(true);
                break;
        }
    }

    private protected override void ForceParam(int number)
    {
        switch (number)
        {
            case 5:
                passVariant = PasswordVariant.Number;
                timePerChar = 0.6f;
                passwordDirection = Direction.forward;
                answerLength = 4;
                suffuleFlag = false;
                wrongRoomFrag = false;
                break;
            case 6:
                passVariant = PasswordVariant.Number;
                timePerChar = 0.6f;
                passwordDirection = Direction.back;
                answerLength = 5;
                suffuleFlag = false;
                wrongRoomFrag = false;
                break;
        }
    }

    [ParameterDecision(50, 0)]
    public void SetPassLength(float intensity)
    {
        for (int i = 1; i <= 5; i++)
        {
            if (intensity <= i * 10)
            {
                answerLength = Mathf.Max(3, Random.Range(2 + i, 4 + i) - subAnswerLength);
                break;
            }
        }
    }

    [ParameterDecision(80, 1)]
    public void SetPassVariant(float intensity)
    {
        if (intensity >= 50)
        {
            passVariant = PasswordVariant.Symbol;
            timePerChar = 1f;
            for (int i = 1; i <= 5; i++)
            {
                if (intensity - 50 <= i * 10)
                {
                    subAnswerLength = 5 - i;
                    break;
                }
            }
        }
        else
        {
            passVariant = PasswordVariant.Number;
            timePerChar = 0.6f;
        }
    }

    [ParameterDecision(10, 0)]
    public void SetPassPosition(float intensity)
    {
        int index;
        if (intensity >= 10)
        {
            index = Random.Range(0, 4);
        }
        else if (intensity >= 5)
        {
            index = Random.Range(0, 3);
        }
        else
        {
            index = 0;
        }

        switch (index)
        {
            case 0:
                passwordDirection = Direction.forward;
                break;
            case 1:
                passwordDirection = Direction.right;
                break;
            case 2:
                passwordDirection = Direction.left;
                break;
            case 3:
                passwordDirection = Direction.back;
                break;
        }
    }

    [ParameterDecision(10, 0)]
    public void SetSuffule(float intensity)
    {
        if (intensity >= 10 && Random.Range(0, 1) == 0)
        {
            suffuleFlag = true;
        }
        else
        {
            suffuleFlag = false;
        }
    }

    private void SetPassword()
    {
        for (int i = 0; i < answerLength; i++)
        {
            answer += Random.Range(1, 10).ToString();
        }
    }

    private void SetSymbols(string symbolArray)
    {
        symbolObject = new GameObject("Symbols");
        symbolObject.transform.SetParent(transform);
        symbolObject.transform.localPosition = new Vector3(0, Random.Range(4.5f, 7.5f), 0);
        float position;
        switch (passwordDirection)
        {
            case Direction.forward:
                symbolObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                position = Random.Range(-4.5f + 0.5f * answerLength, 4.5f - 0.5f * answerLength);
                symbolObject.transform.localPosition += new Vector3(position, 0, 19.89f);
                break;
            case Direction.back:
                symbolObject.transform.localEulerAngles = new Vector3(0, 180, 0);
                position = Random.Range(-4.5f + 0.5f * answerLength, 4.5f - 0.5f * answerLength);
                symbolObject.transform.localPosition += new Vector3(position, 0, 0.11f);
                break;
            case Direction.right:
                symbolObject.transform.localEulerAngles = new Vector3(0, 90, 0);
                position = Random.Range(0.5f + 0.5f * answerLength, 19.5f - 0.5f * answerLength);
                symbolObject.transform.localPosition += new Vector3(4.99f, 0, position);
                break;
            case Direction.left:
                symbolObject.transform.localEulerAngles = new Vector3(0, 270, 0);
                position = Random.Range(0.5f + 0.5f * answerLength, 19.5f - 0.5f * answerLength);
                symbolObject.transform.localPosition += new Vector3(-4.99f, 0, position);
                break;
        }

        for (int i = 0; i < symbolArray.Length; i++)
        {
            int index = symbolArray[i] - 49;
            GameObject go = Instantiate(roomContainer.GetRoomProp(RoomProp.Quad), symbolObject.transform);
            go.transform.localPosition = new Vector3(i - (symbolArray.Length - 1) / 2, 0, 0);
            go.GetComponent<Renderer>().material.SetTexture("_BaseMap", GameManager.Texture_C.GetPasswordTexture(passVariant)[index]);
        }
    }
}
