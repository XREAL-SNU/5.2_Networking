using Photon.Pun;
using System;
using TMPro;

public class PlayerNameTag : MonoBehaviourPun
{
    // Start is called before the first frame update
    private TextMeshPro nameText;
    void Start()
    {
        nameText = gameObject.GetComponent<TextMeshPro>();
        if(photonView.IsMine)
        {
            nameText.enabled = false;
            return;
        }
        SetName();
    }

    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
    }
}
