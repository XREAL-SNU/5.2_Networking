using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SuitColorSync : MonoBehaviourPunCallbacks
{
    const string SUIT_COLOR_KEY = "SuitColor";
    public enum ColorEnum
    {
        Red, Green, Blue
    }

    Color GetColor(ColorEnum col)
    {
        switch (col)
        {
            case ColorEnum.Red:
                return Color.red;
            case ColorEnum.Green:
                return Color.green; 
            case ColorEnum.Blue:
                return Color.blue;
        }
        return Color.black;
    }

    MaterialPropertyBlock block;
    Renderer suitRenderer;
    void Start()
    {
        Debug.Log("Start on SuitColorSync by " + gameObject.GetPhotonView().Owner.NickName);

        if (PhotonNetwork.OfflineMode || !PhotonNetwork.InRoom) return;
        InitializeSuitColor();
    }

    void InitializeSuitColor()
    {
        // set my property on spawn
        if (gameObject.GetPhotonView().AmOwner) SetColorProperty(PlayerManager.Instance.Data.suitColor);

        // retrieve other player's properties
        else
        {
            Player owner = gameObject.GetPhotonView().Owner;
            Hashtable ownerProps = owner.CustomProperties;
            if (ownerProps.ContainsKey(SUIT_COLOR_KEY)) SetSuitColor(owner, (ColorEnum)ownerProps[SUIT_COLOR_KEY]);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player player, Hashtable updatedProps)
    {
        if (updatedProps.ContainsKey(SUIT_COLOR_KEY))
        {
            SetSuitColor(player, (ColorEnum)updatedProps[SUIT_COLOR_KEY]);
        }
    }

    public void SetSuitColor(Player player, ColorEnum col)
    {
        if (block == null) block = new MaterialPropertyBlock();
        block.SetColor("_Color", GetColor(col));

        // get the renderer of the player
        GameObject playerGo = (GameObject)player.TagObject;
        suitRenderer = playerGo.transform.Find("Space_Suit/Tpose_/Man_Suit/Body").GetComponent<Renderer>();

        suitRenderer.SetPropertyBlock(block);
    }

    public void SetColorProperty(ColorEnum col)
    {
        // save the current color
        PlayerManager.Instance.Data.suitColor = col;
        PlayerManager.Instance.SaveAllPlayerData();

        // update custom Properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { SUIT_COLOR_KEY, col } });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SetColorProperty(ColorEnum.Red);
        if (Input.GetKeyDown(KeyCode.G)) SetColorProperty(ColorEnum.Green);
        if (Input.GetKeyDown(KeyCode.B)) SetColorProperty(ColorEnum.Blue);
    }

}
