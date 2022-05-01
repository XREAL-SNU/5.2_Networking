using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviourPun
{
    private TextMeshPro nameText;
    void Start()
    {
        nameText = gameObject.GetComponent<TextMeshPro>();
        if (photonView.IsMine)
        {
            nameText.gameObject.SetActive(false);
            return;
        }
        SetName();
    }

    private void SetName() => nameText.text = photonView.Owner.NickName;
}