// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSystemSpawner.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
// For additive Scene Loading context, eventSystem can't be added to each scene and instead should be instantiated only if necessary.
// https://answers.unity.com/questions/1403002/multiple-eventsystem-in-scene-this-is-not-supporte.html
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Photon.Chat.UtilityScripts
{
    /// <summary>
    /// Event system spawner. Will add an EventSystem GameObject with an EventSystem component and a StandaloneInputModule component.
    /// Use this in additive scene loading context where you would otherwise get a "Multiple EventSystem in scene... this is not supported" error from Unity.
    /// </summary>
    public class EventSystemSpawner : MonoBehaviour
    {
        void OnEnable()

        
        {

            SceneManager.sceneLoaded += OnSceneLoaded;
            #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Debug.LogError("PUN Demos are not compatible with the New Input System, unless you enable \"Both\" in: Edit > Project Settings > Player > Active Input Handling. Pausing App.");
            Debug.Break();
            return;
            #endif

            EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
            if (sceneEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");

                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        void OnDisable()
        {

            SceneManager.sceneLoaded -= OnSceneLoaded;




        }

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

   





    }

}