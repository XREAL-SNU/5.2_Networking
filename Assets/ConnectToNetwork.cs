using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using StarterAssets;
public class ConnectToNetwork : MonoBehaviourPunCallbacks
{
    public static ConnectToNetwork Instance = null;
    string nickname = "Player";
    string gameVersion = "0.0.1";
    static bool isScene1;

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
            player.GetComponent<ThirdPersonControllerMulti>().CinemachineCameraTarget = cam;
        }
    }
}
