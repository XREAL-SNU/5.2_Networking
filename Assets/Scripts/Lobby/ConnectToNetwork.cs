using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.EventSystems;
using XReal.XTown.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ConnectToNetwork : MonoBehaviourPunCallbacks
{

    const string SCENE_NAME_KEY = "SceneName";
    const string DefaultSceneName = "SampleScene";


    public static ConnectToNetwork Instance = null;


    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        UIManager.UI.ShowSceneUI<ConnectionUIScript>("ConnectionUI");
        Debug.Log("<<<< WELCOME : NETWORK HW BUILD >>>>>", this);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Player" + Random.Range(1,100);
        PhotonNetwork.GameVersion = "1.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected with cause: " + cause.ToString());
        Application.Quit();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propsThatChanged)
    {
        Debug.Log(propsThatChanged.ToString());
        if (propsThatChanged.ContainsKey("Level")) Debug.Log("Level " + propsThatChanged["Level"]);
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "level up"))
        {
            Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (!roomProps.ContainsKey("Level"))
            {
                roomProps.Add("Level", 1);
            }
            else
            {
                roomProps["Level"] = (int)roomProps["Level"] + 1;
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
    }

    public void LoadFirstRoom()
    {
        // must bein lobby to execute
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[] { SCENE_NAME_KEY };
        roomOptions.CustomRoomProperties = GetFilter(SCENE_NAME_KEY, DefaultSceneName);
        Debug.Log($"JoinRoomTeleport/ do we have correct room options? ({roomOptions.CustomRoomProperties.ToString()})");

        PlayerManager.Instance.SceneNameToLoad = DefaultSceneName;

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: GetFilter(SCENE_NAME_KEY, DefaultSceneName),
            roomOptions: roomOptions
        );
    }

    public Hashtable GetFilter(string key, object value)
    {
        Hashtable ht = new Hashtable();
        ht.Add(key, value);
        return ht;
    }
}