using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerNameTag : MonoBehaviourPun
{
    // Start is called before the first frame update
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
