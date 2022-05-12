
using Photon.Pun;
using Photon.Realtime;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class PlayerAvatar : MonoBehaviour, IPunInstantiateMagicCallback
{
   
// sceneLoaded is Action<Scene, LoadSceneMode> type, so add them to parameters
void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if(PhotonNetwork.OfflineMode || PhotonNetwork.InRoom)
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
       // if (cam != null && player != null) cam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.Find("FollowTarget");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // store this gameobject as this player's charater in Player.TagObject
        info.Sender.TagObject = this.gameObject;
    }

}

  

