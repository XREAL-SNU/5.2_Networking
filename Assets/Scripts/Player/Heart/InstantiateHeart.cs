using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class InstantiateHeart : MonoBehaviourPun
{
    [SerializeField]
    public float DestroyTime = 2.7f;
    private GameObject _player;
    private Vector3[] wayPoints;
    private Vector3 wayPoint1, wayPoint2;
    HeartTransformView heartTransformView;
    bool exist = false;

    // Update is called once per frame
    void Update()
    {
        if(this.photonView.IsMine)
        {
            ProcessInputs ();
        }
        //과제2: 개인한테만 하트가 뜨게 해두고 이 것 동기화하게 해보기
        //먼저 ProcessInputs의 조건을 this.photonView.IsMine으로 변경해야 함.
    }

    void ProcessInputs()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(!exist)
            {

                exist = true;
                Vector3 mainCamRotation = Camera.main.transform.rotation.eulerAngles;
                Vector3 forwardRotation = new Vector3(0, mainCamRotation.y, 0);
                _player = GameObject.FindWithTag("Player");
                GameObject Heart = PhotonNetwork.Instantiate("PhotonViewHeart", new Vector3(_player.transform.position.x, _player.transform.position.y+1.3f, 
                _player.transform.position.z), Quaternion.Euler(270, 0, 0), 0);
                heartTransformView = Heart.GetComponent<HeartTransformView>();

                wayPoints = new Vector3[2];

                wayPoint1 = new Vector3(_player.transform.position.x+0.250f, _player.transform.position.y+1.7f, 
                _player.transform.position.z-0.25f);
                wayPoint2 = new Vector3(_player.transform.position.x+0.255f, _player.transform.position.y+2f, 
                _player.transform.position.z-0.25f);
                

                
                wayPoints.SetValue(wayPoint1, 0);
                wayPoints.SetValue(wayPoint2, 1);


                Heart.transform.DOPath(wayPoints, DestroyTime)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(()=> {StartCoroutine (heartTransformView.HeartDestroyed()); exist = false; });


                //heartTransformView = Heart.GetComponent<HeartTransformView>();
                //PhotonNetwork.Destroy(Heart.GetPhotonView());
                //heartTransformView.HeartDestroyed();

            }
        }
    }
}
