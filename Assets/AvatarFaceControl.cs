using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class AvatarFaceControl : MonoBehaviour
{
    [SerializeField] Material _avatarFace;
    [SerializeField] Texture _defaultTexture;

    bool _crRunning = false;
    IEnumerator _coroutine;
    // Start is called before the first frame update
    void Start()
    {
        _avatarFace.SetTexture("_MainTex", _defaultTexture);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonView.Get(this).IsMine) return;
        if (Input.GetKey(KeyCode.Alpha1))
            PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)QuickSlotManager_Sol.s_quickSlots[0].fid);
        if (Input.GetKey(KeyCode.Alpha2))
            PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)QuickSlotManager_Sol.s_quickSlots[1].fid);
        if (Input.GetKey(KeyCode.Alpha3))
            PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)QuickSlotManager_Sol.s_quickSlots[2].fid);
        if (Input.GetKey(KeyCode.Alpha4))
            PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)QuickSlotManager_Sol.s_quickSlots[3].fid);
    }

    public void ChangeFace(int faceIndex)
    {
        _avatarFace.SetTexture("_MainTex", QuickSlotManager_Sol.s_faceTextures[faceIndex]);
    }

    [PunRPC]
    void ShowFace(byte index)
    {
        if (_crRunning && _coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        //_coroutine = ShowFaceCoroutine(QuickSlotManager_Sol.s_quickSlots[(int)index].fid);
        _coroutine = ShowFaceCoroutine(index);
        StartCoroutine(_coroutine);
    }

    IEnumerator ShowFaceCoroutine(int index)
    {
        _crRunning = true;

        ChangeFace(index);

        yield return new WaitForSeconds(10f);

        ChangeFace(11);

        _crRunning = false;
    }
}
