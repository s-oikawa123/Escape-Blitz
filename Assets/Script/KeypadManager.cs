using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Container;

public class KeypadManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform padTransform;
    [SerializeField] private GameObject[] button;
    [SerializeField] private Transform resultTransform;
    [SerializeField] private GameObject[] displays;
    [SerializeField] private GameObject image;
    [SerializeField] private AudioSource audioSource;
    private PasswordVariant passwordVariant;
    private string str;
    private float displaySize;
    private int correctLength;

    public void SetUp(bool shuffle, string answer, PasswordVariant variant)
    {
        str = answer;
        correctLength = 0;
        passwordVariant = variant;

        for (int i = 0; i < 9; i++)
        {
            button[i].GetComponentInChildren<RawImage>().texture = GameManager.Texture_C.GetPasswordTexture(passwordVariant)[i];
        }

        if (shuffle)
        {
            Shuffle();
        }

        displaySize = Mathf.Min(60, 300f / answer.Length);

        for (int i = 0; i < displays.Length; i++)
        {
            if (i < answer.Length)
            {
                RectTransform rectTransform = (RectTransform)displays[i].transform;
                rectTransform.sizeDelta = new Vector2(displaySize, displaySize);
                displays[i].SetActive(true);
                if (rectTransform.childCount > 0)
                {
                    Destroy(rectTransform.GetChild(0).gameObject);
                }
            }
            else
            {
                displays[i].SetActive(false);
            }
        }
    }

    public void Shuffle()
    {
        List<int> ints = Enumerable.Range(0, 9).ToList();
        List<int> order = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            int index = Random.Range(0, 9 - i);
            order.Add(ints[index]);
            ints.RemoveAt(index);
        }

        foreach (int i in order)
        {
            button[i].transform.SetAsFirstSibling();
        }
    }

    public void InsertSymbol(int num)
    {
        if (str[correctLength] - 48 == num)
        {
            GameObject go = Instantiate(image, displays[correctLength].transform);
            RectTransform rectTransform = (RectTransform)go.transform;
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(displaySize, displaySize);
            go.GetComponent<RawImage>().texture = GameManager.Texture_C.GetInvertPasswordTexture(passwordVariant)[num - 1];
            audioSource.PlayOneShot(GameManager.Audio_C.GetAudioClip(NameSE.KeyPad));
            correctLength++;
            if (correctLength == str.Length)
            {
                gameManager.SendSignal(Signal.Complete);
            }
        }
        else
        {
            audioSource.PlayOneShot(GameManager.Audio_C.GetAudioClip(NameSE.KeyPadFalse));
        }
        
    }

    public void Exit()
    {
        gameManager.ExitKeyPad(true);
    }
}
