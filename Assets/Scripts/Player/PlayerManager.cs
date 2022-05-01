using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XReal.XTown.Persistance;

public enum SceneEnum
{
    LauncherScene, SampleScene, OtherScene
}

public class PlayerManager : MonoBehaviourPunCallbacks
{
    // singleton, dont destroy on load
    public static PlayerManager Instance = null;
    public PlayerData Data;
    protected const string _storageFileName = "player";
    protected string _dataPath;
    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
            Data = new PlayerData();

            InitializePlayerData();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0.75f * Screen.width, 0.85f * Screen.height, 200, 50), "Leave"))
            PhotonNetwork.LeaveRoom();

        GUI.Label(new Rect(10, 10, 600, 20), _dataPath);
    }

    void InitializePlayerData()
    {
        _dataPath = Application.dataPath;
        Storage storage = new Storage();
        object obj = storage.LoadData(_storageFileName);
        if (obj is null)
        {
            Data.myFaces = new List<int> { 11, 17, 2, 7 };
            Data.suitColor = SuitColorSync.ColorEnum.Blue;
            storage.SaveData((object)Data, _storageFileName);
            Debug.Log("Creating new player data file");
            return;
        }
        else
        {
            Data = (PlayerData)obj;
        }
    }
    public void SaveAllPlayerData()
    {
        Storage storage = new Storage();
        storage.SaveData(Data, _storageFileName);
        
        // following lines are debugging data
        object obj = storage.LoadData(_storageFileName);
        if (obj is null)
        {
            storage.SaveData((object)Data, _storageFileName);
            Debug.Log("Creating new player data file");
            return;
        }
        else
        {
            Data = (PlayerData)obj;
            Debug.Log("PlayerManager/SaveALL");

            Debug.Log(Data.myFaces.ToStringFull());
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("PlayerManager/OnSceneLoaded: " + scene.name);

        if(PhotonNetwork.OfflineMode || PhotonNetwork.InRoom)
        {
            InitializePlayer();
        }
    }


    public string SceneNameToLoad = "SampleScene";

    public override void OnJoinedRoom()
    {
        Debug.Log("PlayerManager/JoinedRoom as " + PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("PlayerManager/Room properties :" + PhotonNetwork.CurrentRoom.ToString());
        // do not call this in createRoom.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // Must load level with PhotonNetwork.LoadLevel, not SceneManager.LoadScene
            PhotonNetwork.LoadLevel(SceneNameToLoad);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"PlayerManager/Player {newPlayer.NickName} joined"); 
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("PlayerManager/CreatedRoom");
    }

    // Leave Room Callbacks
    public Action JoinedLobbyActions = null;

    public override void OnLeftRoom()
    {
        Debug.Log("PlayerManager/LeftRoom");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("PlayerManager/Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("PlayerManager/Joined Lobby");
        // if actions are specified, invoke, else, return to main.
        if (JoinedLobbyActions != null) JoinedLobbyActions();
    }


    void InitializePlayer()
    {
        Debug.Log("INITIALIZE PLAYER");
        // instantiate camera, locally
        var prefab = (GameObject)Resources.Load("PhotonPrefab/PlayerFollowCamera");
        var cam = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        cam.name = "PlayerFollowCamera";

        // instantiate player and link
        var player = PhotonNetwork.Instantiate("PhotonPrefab/CharacterPrefab", Vector3.zero, Quaternion.identity);
        if (cam != null && player != null) cam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.Find("FollowTarget");
    }


}
