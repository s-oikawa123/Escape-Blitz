using Container;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator TitleAnimator;
    [SerializeField] private RoomContainer roomContainer;
    [SerializeField] private Player player;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private KeypadManager keypadManager;
    [SerializeField] private TextureContainer textureContainer;
    [SerializeField] private TextContainer textContainer;
    [SerializeField] private AudioContainer audioContainer;
    [SerializeField] private int startRoom;
    [SerializeField] private MusicPlayer musicPlayer;
    [SerializeField] private bool enableDebugText;
    private GameObject field;
    private Vector3 roomPoint;
    private float roomAngle;
    private IEnumerator excutingGameCoroutine;
    private IEnumerator excutingRoomCoroutine;
    private RoomInfo roomInfo;
    private RoomInfo nextRoomInfo;
    private RoomInfo previousRoomInfo;
    private float roomTime;
    private int roomCount;
    public static TextureContainer Texture_C;
    public static AudioContainer Audio_C;
    public static TextContainer Text_C;
    private bool interactable;
    [SerializeField] private bool tutorialFlag;
    [SerializeField] private int tutorialNumber;
    public int TutorialNumber { get { return tutorialNumber; } }
    public Player Player { get { return player; } }
    public int RoomCount { get { return roomCount; } }
    public KeypadManager KeypadManager { get {  return keypadManager; } }
    public MusicPlayer MusicPlayer { get { return musicPlayer; } }
    // Start is called before the first frame update
    void Start()
    {
        player.SetMovable(false, false);
        gameUI.Initialize();
        textureContainer.Invert();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Texture_C = textureContainer;
        Audio_C = audioContainer;
        Text_C = textContainer;
        Transition(Title());
        musicPlayer.SetMusicAndPlay(NameBGM.ARoomOfVoid, true);
    }

    // タイトル画面処理
    private IEnumerator Title()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (Setting.language == Language.English)
                {
                    Setting.language = Language.Japanese;
                }
                else
                {
                    Setting.language = Language.English;
                }
                gameUI.titleUI.Language();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                gameUI.titleUI.Credit();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameUI.titleUI.EndTitle();
                PlayIntiation();
                break;
            }
            yield return null;
        }
    }

    // プレイ初期化処理
    private void PlayIntiation()
    {
        roomPoint = Vector3.zero;
        roomAngle = 0;
        roomCount = startRoom;
        if (tutorialFlag)
        {
            tutorialNumber = 0;
        }
        if (field != null)
        {
            Destroy(field.gameObject);
        }
        field = new GameObject("field");
        roomTime = 0;
        musicPlayer.SetMusicAndPlay(NameBGM.NoLightsBehindIntro, NameBGM.NoLightsBehindLoop);
        player.SetMovable(true, true);
        gameUI.VisibleInGameUI(true);
        player.transform.position = new Vector3(0, 1, 1);
        player.transform.rotation = Quaternion.identity;
        interactable = true;
        roomInfo = GenerateRoom();
        nextRoomInfo = GenerateRoom();
        previousRoomInfo = null;
        roomInfo.gameObject.SetActive(true);
        excutingRoomCoroutine = roomInfo.UpdateManager();
        StartCoroutine(excutingRoomCoroutine);
        Transition(Play());
    }

    // プレイ中処理
    private IEnumerator Play()
    {
        while (true)
        {
            if (!tutorialFlag || tutorialNumber == 10)
            {
                roomTime += Time.deltaTime;
            }

            bool interact = false;
            Vector2 interactPos = Vector2.zero;

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit raycast;

            if (Physics.Raycast(ray, out raycast))
            {
                if (raycast.collider.CompareTag("Interactable") && raycast.distance < 2 && interactable)
                {
                    var manager = raycast.collider.GetComponent<InteractListener>().InteractableObjectManager;
                    interact = manager.IsActive;
                    interactPos = Camera.main.WorldToScreenPoint(manager.InteractIconPos);
                    
                    if (Input.GetKeyDown(KeyCode.E) && interact)
                    {
                        manager.Interact();
                    }
                }
            }

            gameUI.inGameUI.VisibleInteractUI(interact, interactPos);

            gameUI.inGameUI.ClockTimerAnimator(roomTime - roomInfo.ExitTime + 1);

            if (roomTime - roomInfo.ExitTime > 2)
            {
                if (previousRoomInfo != null)
                {
                    StartCoroutine(previousRoomInfo.LightOut());
                }
                StartCoroutine(roomInfo.LightOut());
                if (nextRoomInfo != null)
                {
                    StartCoroutine(nextRoomInfo.LightOut());
                }
                musicPlayer.PlayOneShot(audioContainer.GetAudioClip(NameSE.AllLightOut));
                StartCoroutine(GameOverIntiationDelay(1, textContainer.FailReason.Utility[0].GetString()));
                StopCoroutine(excutingGameCoroutine);
            }

            gameUI.inGameUI.EnableTutorialText(tutorialFlag);
            string tutorialText = textContainer.Tutorial[Mathf.Clamp(tutorialNumber - 2, 0, 8)].GetString();

            gameUI.inGameUI.SetTutorialText($"Tutorial : {tutorialNumber - 1} / 9\n{tutorialText}");

            string debugText = "";
            debugText += $"Room : {RoomCount - 1}\n";
            debugText += $"{roomTime:F2} / {roomInfo.ExitTime:F2}";
            gameUI.inGameUI.EnableDebugText(enableDebugText);
            gameUI.inGameUI.SetDebugText(debugText);

            yield return null;
        }
    }

    // ゲームオーバー初期化処理
    public void GameOverInitiation(string failReason)
    {
        interactable = false;
        musicPlayer.Stop();
        ExitKeyPad(false);
        StopCoroutine(excutingRoomCoroutine);
        player.SetMovable(false, true);
        gameUI.VisibleInGameUI(false);
        gameUI.VisibleGameOverUI(true);
        gameUI.gameOverUI.SetReachedRoomText(RoomCount - 1);
        gameUI.gameOverUI.SetFailReasonText(failReason);
        Transition(GameOver());
    }

    // ゲームオーバー初期化処理（待機可能）
    public IEnumerator GameOverIntiationDelay(float delay, string failReason)
    {
        interactable = false;
        musicPlayer.Stop();
        ExitKeyPad(false);
        StopCoroutine(excutingRoomCoroutine);
        player.SetMovable(false, true);
        gameUI.VisibleInGameUI(false);
        gameUI.gameOverUI.SetReachedRoomText(RoomCount - 1);
        gameUI.gameOverUI.SetFailReasonText(failReason);

        yield return new WaitForSeconds(delay);

        gameUI.VisibleGameOverUI(true);
        Transition(GameOver());
    }

    // ゲームオーバー処理
    private IEnumerator GameOver()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameUI.VisibleGameOverUI(false);
                PlayIntiation();
                break;
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
            yield return null;
        }
    }

    // 処理状態遷移
    private void Transition(IEnumerator enumerator)
    {
        if (excutingGameCoroutine != null)
        {
            StopCoroutine(excutingGameCoroutine);
        }
        excutingGameCoroutine = enumerator;
        StartCoroutine(enumerator);
    }

    // 部屋生成
    private RoomInfo GenerateRoom()
    {
        roomCount++;
        if (tutorialFlag)
        {
            tutorialNumber++;
        }
        GameObject go;
        switch (tutorialNumber)
        {
            case 1:
            case 2:
                go = Instantiate(roomContainer.GetRoom(0), field.transform);
                break;
            case 3:
            case 4:
                go = Instantiate(roomContainer.GetRoom(1), field.transform);
                break;
            case 5:
            case 6:
                go = Instantiate(roomContainer.GetRoom(2), field.transform);
                break;
            case 7:
                go = Instantiate(roomContainer.GetRoom(3), field.transform);
                break;
            case 8:
                go = Instantiate(roomContainer.GetRoom(0), field.transform);
                break;
            case 9:
                go = Instantiate(roomContainer.GetRoom(0), field.transform);
                break;
            case 10:
                go = Instantiate(roomContainer.GetRoom(Random.Range(0, 4)), field.transform);
                break;
            case 11:
                go = Instantiate(roomContainer.GetRoom(Random.Range(0, 4)), field.transform);
                tutorialFlag = false;
                break;
            default:
                go = Instantiate(roomContainer.GetRoom(Random.Range(0, 4)), field.transform);
                break;
        }
        go.transform.localPosition = roomPoint;
        go.transform.localEulerAngles = new Vector3(0, roomAngle, 0);
        RoomInfo roomInfo = go.GetComponent<RoomInfo>();
        if (Random.Range(0, 5) == 0 && roomCount != 1) roomInfo.SetAsWrongRoom();
        roomInfo.SetUpManager(this);
        roomInfo.gameObject.SetActive(false);
        float rad = roomAngle * Mathf.Deg2Rad;
        Vector3 exitPoint = new Vector3(Mathf.Sin(rad) * roomInfo.ExitPoint.z + Mathf.Cos(rad) * roomInfo.ExitPoint.x, roomInfo.ExitPoint.y, Mathf.Sin(rad) * roomInfo.ExitPoint.x + Mathf.Cos(rad) * roomInfo.ExitPoint.z);
        roomAngle += roomInfo.ExitRotation;
        roomPoint = roomPoint + exitPoint;
        return roomInfo;
    }

    // 部屋脱出完了
    public void RoomClear()
    {
        if (previousRoomInfo != null)
        {
            Destroy(previousRoomInfo.gameObject);
        }

        if (roomTime - roomInfo.ExitTime > 0)
        {
            gameUI.inGameUI.DisplayEvaluate(2);
        }
        else if(roomTime - roomInfo.ExitTime <= 0 && roomTime - roomInfo.ExitTime > -1)
        {
            gameUI.inGameUI.DisplayEvaluate(1);
        }
        else
        {
            gameUI.inGameUI.DisplayEvaluate(0);
        }

        if (roomCount == 50)
        {
            musicPlayer.ChangeMusicSeamless(NameBGM.NoLightsBehindIntenseLoop, 2);
        }

        previousRoomInfo = roomInfo;
        roomInfo = nextRoomInfo;
        nextRoomInfo = GenerateRoom();
        StopCoroutine(excutingRoomCoroutine);
        excutingRoomCoroutine = roomInfo.UpdateManager();
        StartCoroutine(excutingRoomCoroutine);
        roomTime = 0;
    }

    public void ActiveNextRoom()
    {
        nextRoomInfo.gameObject.SetActive(true);
    }

    public void DeactivatePreviousRoom()
    {
        if (previousRoomInfo != null)
        {
            previousRoomInfo.gameObject.SetActive(false);
        }
    }

    public void DisplayKeypat()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        keypadManager.gameObject.SetActive(true);
        player.SetMovable(false, false);
    }

    public void ExitKeyPad(bool sendSignal)
    {
        if (sendSignal)
        {
            roomInfo.Signal(Signal.Leave);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        keypadManager.gameObject.SetActive(false);
        player.SetMovable(true, true);
    }

    public void SendSignal(Signal signal)
    {
        roomInfo.Signal(signal);
    }
}


