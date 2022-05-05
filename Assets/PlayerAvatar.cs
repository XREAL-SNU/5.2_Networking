using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerAvatar : MonoBehaviour, IPunInstantiateMagicCallback
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
        Debug.Log(info.ToString());
    }
}
