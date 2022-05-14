using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAvatar : MonoBehaviour, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;

    }
}
