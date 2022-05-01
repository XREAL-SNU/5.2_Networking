using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XReal.XTown.UI;

public class EmoticonUI : UIPopup
{
    void Start()
    {
        Init();
    }

    enum Images
    {
        ExitButton
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        GetUIComponent<Image>((int)Images.ExitButton).gameObject.BindEvent(OnClick_Complete);
    }

        
    public void OnClick_Complete(PointerEventData data)
    {
        UIManager.UI.ClosePopupUI();
    }

}
