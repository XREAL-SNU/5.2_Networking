using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using StarterAssets;
using ExitGames.Client.Photon;
public class ConnectToNetwork : MonoBehaviourPunCallbacks
{
    public enum ColorEnum
    {
        Red,
        Green,
        Blue
    }
    public static ConnectToNetwork Instance = null;
    string nickname = "Player";
    string gameVersion = "0.0.1";
    static bool isScene1;
    static Color nowColor;
    const string SUIT_COLOR_KEY = "SuitColor";
    Renderer suitRenderer;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
        }
        isScene1 = false;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        nickname += $"{Random.Range(1, 1000)}";
        nowColor = Color.red;
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isScene1)
        {
            isScene1 = false;
            PhotonNetwork.LeaveRoom();
        }
        if (Input.GetKeyDown(KeyCode.E)&& !isScene1)
        {
            isScene1 = true;
            PhotonNetwork.LeaveRoom();
        }
        if (Input.GetKeyDown(KeyCode.R)) SetColorProperty(ColorEnum.Red);
        if (Input.GetKeyDown(KeyCode.G)) SetColorProperty(ColorEnum.Green);
        if (Input.GetKeyDown(KeyCode.B)) SetColorProperty(ColorEnum.Blue);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PlayerManager/JoinedRoom as " + PhotonNetwork.LocalPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if(!isScene1)
            {
                PhotonNetwork.LoadLevel("SceneQ");
            }
            else
            {
                PhotonNetwork.LoadLevel("SceneE");
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server: " + cause.ToString());
        Application.Quit();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PhotonNetwork.OfflineMode || PhotonNetwork.InRoom)
        {
            InitializePlayer();
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey(SUIT_COLOR_KEY))
        {
            SetSuitColor(targetPlayer, (ColorEnum)changedProps[SUIT_COLOR_KEY]);
        }
    }
    void InitializePlayer()
    {
        //Resources.Load는 Resources폴더 안에서 주어진 경로의 prefab을 찾는데 이용
        var prefab = (GameObject)Resources.Load("PhotonPrefab/PlayerFollowCamera");
        var cam = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        cam.name = "PlayerFollowCamera";

        // instantiate player and link camera
        // PhotonNetwork.Instantiate 에 바로 경로를 주는 경우에도 Resources 밑의 경로만 사용.
        var player = PhotonNetwork.Instantiate("PhotonPrefab/CharacterPrefab", Vector3.zero, Quaternion.identity);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", nowColor);
        suitRenderer = player.transform.Find("Space_Suit/Tpose_/Man_Suit/Body").GetComponent<Renderer>();
        suitRenderer.SetPropertyBlock(block);
        // 이 줄은 지난 과제에서 cam을 어떻게 구현하셨는지에 따라 다를 수 잇습니다.
        if (cam != null && player != null)
        {
            cam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.Find("FollowTarget");
            player.GetComponent<ThirdPersonControllerMulti>().CinemachineCameraTarget = cam;
        }
    }

    public void SetSuitColor(Player player, ColorEnum col)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", GetColor(col));

        
        GameObject playerGo = (GameObject)player.TagObject;
        suitRenderer = playerGo.transform.Find("Space_Suit/Tpose_/Man_Suit/Body").GetComponent<Renderer>();

        suitRenderer.SetPropertyBlock(block);
    }

    private Color GetColor(ColorEnum col)
    {
        Color returnColor;
        switch (col)
        {
            case ColorEnum.Red:
                returnColor = Color.red;
                break;
            case ColorEnum.Green:
                returnColor = Color.green;
                break;
            case ColorEnum.Blue:
                returnColor = Color.blue;
                break;
            default:
                returnColor =  Color.black;
                break;
        }
        nowColor = returnColor;
        return returnColor;
    }

    public void SetColorProperty(ColorEnum col)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { SUIT_COLOR_KEY, col } });
    }
}
