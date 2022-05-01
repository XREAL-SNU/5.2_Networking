using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class FaceScript : MonoBehaviour
{
    public Material AvatarFace;
    public float ShowFaceTime;
    public List<Texture> FaceTextures;
    protected List<int> _indexMapping;
    public byte DefaultFaceIndex;
    protected MaterialPropertyBlock block;

    int _currentTextureIndex;
    bool _crRunning;
    // coroutine for displaying face
    IEnumerator _coroutine;


    private void Start()
    {
        AvatarFace = transform.Find("Space_Suit/Tpose_/Man_Suit/Face")
            .GetComponent<Renderer>().materials[0];
        _indexMapping = PlayerManager.Instance.Data.myFaces;
        if (DefaultFaceIndex < FaceTextures.Count - 1)
        {
            SetFace(DefaultFaceIndex);
        }
        else
        {
            throw new System.IndexOutOfRangeException();
        }
    }

    private void Update()
    {
        if (!PhotonView.Get(this).IsMine) return;
        if (Input.GetKeyUp(KeyCode.Alpha1)) PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)0);
        if (Input.GetKeyUp(KeyCode.Alpha2)) PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)1);
        if (Input.GetKeyUp(KeyCode.Alpha3)) PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)2);
        if (Input.GetKeyUp(KeyCode.Alpha4)) PhotonView.Get(this).RPC("ShowFace", RpcTarget.All, (byte)3);
    }

    [PunRPC]
    void ShowFace(byte index)
    {
        if (_crRunning)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = ShowFaceCoroutine(index);
        StartCoroutine(_coroutine);
    }

    IEnumerator ShowFaceCoroutine(byte index)
    {
        _crRunning = true;

        SetFace(index);

        yield return new WaitForSeconds(ShowFaceTime);

        SetFace(0);

        _crRunning = false;
    }


    public void SetFace(byte index)
    {
        
        if(block is null) block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", FaceTextures[_indexMapping[index]]);

        // get the renderer of the player
        var faceRenderer = transform.Find("Space_Suit/Tpose_/Man_Suit/Face").GetComponent<Renderer>();
        faceRenderer.SetPropertyBlock(block);
        

        _currentTextureIndex = index;
    }
}