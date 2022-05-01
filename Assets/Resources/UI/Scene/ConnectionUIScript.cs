using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XReal.XTown.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ConnectionUIScript : UIScene
{


    void Start()
    {
        Init();
    }

    enum Images
    {
        JoinBtn
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        GetUIComponent<Image>((int)Images.JoinBtn).gameObject.BindEvent(OnClick_JoinRandom);
        Debug.Log("UI init");
    }

    public const string GAME_LEVEL_KEY = "Level";
    public void OnClick_JoinRandom(PointerEventData data)
    {
        Debug.Log("OnClick_JoinRandom");
        UIManager.UI.CloseSceneUI();
        UIManager.UI.ShowPopupUI<EmoticonUI>();
    }


}
