using Container;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Return : RoomInfo
{
    [SerializeField] private GameObject returnObject;
    [SerializeField] private GameObject letters;
    [SerializeField] private AudioSource lightAuio;
    [SerializeField] private TriggerObserver triggerObserver;
    [SerializeField] private TriggerObserver fallTriggerObserver;
    private bool changed;
    private RoomInfo roomInfo;
    private IEnumerator coroutine;

    private protected override void SetUp()
    {
        int cloneRoomNumber = Random.Range(0, roomContainer.GetRoomCount() - 1);
        if (cloneRoomNumber >= 3)
        {
            cloneRoomNumber++;
        }
        GameObject go = Instantiate(roomContainer.GetRoom(cloneRoomNumber), transform);
        roomInfo = go.GetComponent<RoomInfo>();
        roomInfo.SetUpManager(gameManager);
        exitTime = roomInfo.ExitTime;
        letters.GetComponent<Renderer>().material.SetTexture("_BaseMap", GameManager.Texture_C.ReturnLetterTexture[0]);
        coroutine = roomInfo.UpdateManager();
        returnObject.SetActive(false);
    }

    private protected override void First()
    {
        StartCoroutine(coroutine);
    }

    private protected override void Every()
    {
        if (!changed && triggerObserver.Enter)
        {
            roomInfo.gameObject.SetActive(false);
            StopCoroutine(coroutine);
            returnObject.SetActive(true);
            gameManager.DeactivatePreviousRoom();
            lightAuio.PlayOneShot(GameManager.Audio_C.GetAudioClip(NameSE.LightOut));
            exitTime = 2.5f;
            changed = true;
        }

        if (fallTriggerObserver.Enter)
        {
            gameManager.MusicPlayer.PlayOneShot(GameManager.Audio_C.GetAudioClip(NameSE.Fall));
            gameManager.GameOverInitiation(GameManager.Text_C.FailReason.ReturnRoom[0].GetString());
        }
    }

    public override void Signal(Signal signal)
    {
        gameManager.ActiveNextRoom();
        roomClearedFrag = true;
        doorManager?.SetActive(false);
        backDoorManager?.SetActive(false);
    }

    private protected override void ForceParam(int number)
    {

    }

    public override IEnumerator LightOut()
    {
        float t = 1f;
        float[] intensity = roomLight.Select(a => a.intensity).ToArray();
        gameManager.StartCoroutine(roomInfo.LightOut());
        while (t > 0)
        {
            t -= Time.deltaTime;
            for (int i = 0; i < intensity.Length; i++)
            {
                roomLight[i].intensity = intensity[i] * t;
            }
            yield return null;
        }
    }
}
