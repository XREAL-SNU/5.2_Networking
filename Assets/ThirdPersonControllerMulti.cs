using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

namespace StarterAssets
{
    public class ThirdPersonControllerMulti : ThirdPersonController
    {
        private PhotonView _view;
        bool heartMutex;
        bool isH;
        public GameObject heartPrefab;
        public GameObject heart;

        // Start is called before the first frame update
        protected override void Start()
        {
            if (_view.IsMine)
            {
                heartPrefab = Resources.Load("Heart/HeartPrefab") as GameObject;
                base.Start();
                heartMutex = false;
                isH = false;
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (_view.IsMine)
            {
                base.Update();
                if (Input.GetKeyDown(KeyCode.H) && !heartMutex)
                {
                    heart = MonoBehaviour.Instantiate(heartPrefab,this.transform) as GameObject;
                    heartMutex = true;
                    isH = true;
                }
                if (Input.GetKeyUp(KeyCode.H))
                {
                    if(isH)
                    {
                        heartMutex = false;
                        GameObject heart = transform.Find("HeartPrefab(Clone)").gameObject;
                        StartCoroutine(heart.GetComponent<HeartTransformView>().HeartDestroyed());
                        isH = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.J) && !heartMutex)
                {
                    GameObject heart = MonoBehaviour.Instantiate(heartPrefab,this.transform) as GameObject;
                    heart.transform.parent = this.transform;
                    heartMutex = true;
                    heart.transform.DOLocalMoveY(3f, 0.8f);
                    heart.transform.DOLocalMoveZ(0.8f, 0.8f)
                        .OnComplete(() => {
                            StartCoroutine(heart.GetComponent<HeartTransformView>().HeartDestroyed());
                            heartMutex = false;
                        });
                }
            }
        }

        protected override void Awake()
        {
            _view = GetComponent<PhotonView>();
            if (_view.IsMine)
            {
                base.Awake();
                heartPrefab = Resources.Load("Heart/HeartPrefab") as GameObject;
            }
        }
        protected override void LateUpdate()
        {
            if (_view.IsMine)
            {
                base.LateUpdate();
            }
        }

       /* public void showHeart()
        {
            
        }*/
    }
}
