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
        // Start is called before the first frame update
        protected override void Start()
        {
            if (_view.IsMine)
            {
                base.Start();
            }

        }

        // Update is called once per frame
        protected override void Update()
        {
            if (_view.IsMine)
            {
                base.Update();
            }

        }

        protected override void Awake()
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
    }
}
