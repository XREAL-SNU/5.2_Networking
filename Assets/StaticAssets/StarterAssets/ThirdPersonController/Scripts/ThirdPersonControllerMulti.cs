using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace StarterAssets
{
    public class ThirdPersonControllerMulti : ThirdPersonController
    {
        private PhotonView _view;
        GameObject heart;
        protected sealed override void Awake()
        {
            _view = GetComponent<PhotonView>();
            if (_view.IsMine)
            {
                base.Awake();
            }
        }

        protected override void LateUpdate()
        {
            if (_view.IsMine)
            {
                base.LateUpdate();
            }
        }


        protected override void Start()
        {
            if (_view.IsMine)
            {
                base.Start();
            }
        }

        protected override void Update()
        {
            if (_view.IsMine)
            {
                base.Update();
                if (Input.GetKeyDown(KeyCode.H))
                {
                    heart = PhotonNetwork.Instantiate("PhotonViewHeart", transform.position, Quaternion.identity);
                    heart.transform.parent = gameObject.transform;
                    heart.transform.Rotate(-90f, 0f, 0f);
                    Debug.Log(heart.ToString());
                }
                if (Input.GetKeyUp(KeyCode.H))
                {
                    Debug.Log("Key up");
                    StartCoroutine(heart.GetComponent<HeartTransformView>().HeartDestroyed());
                }
            }
        }

    }
}

