using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Portal : MonoBehaviour
{
    const string SCENE_NAME_KEY = "SceneName";
    public string TargetSceneName = "OtherScene";
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TargetSceneName = "OtherScene";
            if (SceneManager.GetActiveScene().name.Equals(TargetSceneName)) return;
            Teleport();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TargetSceneName = "SampleScene";
            if (SceneManager.GetActiveScene().name.Equals(TargetSceneName)) return;
            Teleport();
        }
    }

    public void Teleport()
    {
        // just to be safe, delete this from list.
        PlayerManager.Instance.JoinedLobbyActions -= JoinRoomTeleport;
        PlayerManager.Instance.JoinedLobbyActions += JoinRoomTeleport;
        Debug.Log($"INSIDE TELEPORT TO ({TargetSceneName})");
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRoomTeleport()
    {
        // must bein lobby to execute
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomPropertiesForLobby = new string[] { SCENE_NAME_KEY };
        roomOptions.CustomRoomProperties = GetFilter(SCENE_NAME_KEY, TargetSceneName);
        Debug.Log($"JoinRoomTeleport/ do we have correct room options? ({roomOptions.CustomRoomProperties.ToString()})");

        PlayerManager.Instance.SceneNameToLoad = TargetSceneName;

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: GetFilter(SCENE_NAME_KEY, TargetSceneName),
            roomOptions: roomOptions
        );
        
        PlayerManager.Instance.JoinedLobbyActions -= JoinRoomTeleport;
    }

    public Hashtable GetFilter(string key, object value)
    {
        Hashtable ht = new Hashtable();
        ht.Add(key, value);
        return ht;
    }

}
