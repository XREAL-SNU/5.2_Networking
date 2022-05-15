using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Cinemachine;

public class ConnectToNetwork : MonoBehaviourPunCallbacks
{
    public static ConnectToNetwork Instance = null;
    string nickname = "Player";
    string gameVersion = "0.0.1";
    private string SceneNameToLoad;
    GameObject heart;
    Renderer suitRenderer;
    bool isInitiated;

    const string SUIT_COLOR_KEY = "SuitColor";

    public enum ColorEnum
    {
        Red,
        Green,
        Blue
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        SceneNameToLoad = "SceneQ";
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        nickname += $"{Random.Range(1, 1000)}";
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); // 마스터 서버와 연결시킵니다
        isInitiated = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) &&
            SceneManager.GetActiveScene().name != "SceneQ")
        {
            SceneNameToLoad = "SceneQ";
            PhotonNetwork.LeaveRoom();
        }
        if (Input.GetKeyDown(KeyCode.E) &&
            SceneManager.GetActiveScene().name != "SceneE")
        {
            SceneNameToLoad = "SceneE";
            PhotonNetwork.LeaveRoom();
        }
        if (Input.GetKeyDown(KeyCode.R)) SetColorProperty(ColorEnum.Red);
        if (Input.GetKeyDown(KeyCode.G)) SetColorProperty(ColorEnum.Green);
        if (Input.GetKeyDown(KeyCode.B)) SetColorProperty(ColorEnum.Blue);


    }

    public override void OnConnectedToMaster() // 마스터와 연결되었을 때
    {
        PhotonNetwork.JoinLobby(); // 마스터 서버 연결 후 로비에 접속
    }

    public override void OnJoinedLobby() // 로비에 접속했을 때
    {
        if (SceneNameToLoad == "SceneQ")
        {
            PhotonNetwork.JoinOrCreateRoom("RoomQ", new RoomOptions(), TypedLobby.Default);
        }
        else
        {

            PhotonNetwork.JoinOrCreateRoom("RoomE", new RoomOptions(), TypedLobby.Default);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PlayerManager/JoinedRoom as " + PhotonNetwork.LocalPlayer.NickName);
        // load scene with PhotonNetwork.LoadLevel ONLY if I'm the only player in the room
        // we rely on AutomaticallySyncScene if there are other players in the room
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // Must load level with PhotonNetwork.LoadLevel, not SceneManager.LoadScene
            PhotonNetwork.LoadLevel(SceneNameToLoad);
        }

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // this will not get called on myself.
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

    void InitializePlayer()
    {
        //Resources.Load는 Resources폴더 안에서 주어진 경로의 prefab을 찾는데 이용
        var prefab = (GameObject)Resources.Load("PhotonPrefab/PlayerFollowCamera");
        var cam = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        cam.name = "PlayerFollowCamera";

        // instantiate player and link camera
        // PhotonNetwork.Instantiate 에 바로 경로를 주는 경우에도 Resources 밑의 경로만 사용.
        var player = PhotonNetwork.Instantiate("PhotonPrefab/CharacterPrefab", Vector3.zero, Quaternion.identity);
        // 이 줄은 지난 과제에서 cam을 어떻게 구현하셨는지에 따라 다를 수 잇습니다.
        if (cam != null && player != null)
        {
            cam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.Find("FollowTarget");
            player.GetComponent<StarterAssets.ThirdPersonControllerMulti>().CinemachineCameraTarget = cam;
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



    public void SetSuitColor(Player player, ColorEnum col)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", ToColor(col.ToString()));

        // get the renderer of the player
        GameObject playerGo = (GameObject)player.TagObject;
        suitRenderer = playerGo.transform.Find("Space_Suit/Tpose_/Man_Suit/Body").GetComponent<Renderer>();

        suitRenderer.SetPropertyBlock(block);
    }

    private Color ToColor(string color)
    {
        return (Color)typeof(Color).GetProperty(color.ToLowerInvariant()).GetValue(null, null);
    }

    public void SetColorProperty(ColorEnum col)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { SUIT_COLOR_KEY, col } });
    }

    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable updatedProps)
    {
        if (updatedProps.ContainsKey(SUIT_COLOR_KEY))
        {
            SetSuitColor(player, (ColorEnum)updatedProps[SUIT_COLOR_KEY]);
        }
    }
}