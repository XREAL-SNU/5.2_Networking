using Photon.Pun;
using UnityEngine;



namespace StarterAssets
{
    public class ThirdPersonControllerMulti : ThirdPersonController
	{
		private PhotonView _view;

		protected sealed override void Awake()
		{
			
			if (PhotonNetwork.OfflineMode)
            {
				base.Awake();
				return;
			}

			_view = GetComponent<PhotonView>();
            if (_view.IsMine)
            {
				base.Awake();
			}
		}

		protected sealed override void Start()
		{
			if (PhotonNetwork.OfflineMode)
			{
				base.Start();
				return;
			}
			if (_view.IsMine)
			{
				base.Start();
			}
		}

		protected sealed override void Update()
		{
			if (PhotonNetwork.OfflineMode)
			{
				base.Update();
				return;
			}
			if (_view.IsMine)
			{
				base.Update();
			}
		}

		protected sealed override void LateUpdate()
		{
			if (PhotonNetwork.OfflineMode)
			{
				base.LateUpdate();
				return;
			}
			if (_view.IsMine)
			{
				base.LateUpdate();
			}
		}


    }
}