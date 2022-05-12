using Photon.Pun;
using System.Collections;
using UnityEngine;

public class HeartTransformView : MonoBehaviourPun, IPunObservable
{
    private float m_Distance;
    private Vector3 m_Direction;
    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredPosition;
    private bool isDestroyed = false;

    [SerializeField]
    private bool m_SynchronizePosition = true;
    [SerializeField]
    private bool m_UseLocal;

    bool m_firstTake = false;

    public void Awake()
    {
        m_StoredPosition = transform.localPosition;
        m_NetworkPosition = Vector3.zero;
    }

    private void Reset()
    {
        // Only default to true with new instances. useLocal will remain false for old projects that are updating PUN.
        m_UseLocal = true;
    }

    void OnEnable()
    {
        m_firstTake = true;
    }

    public void Update()
    {
        var tr = transform;

        if (!this.photonView.IsMine)
        {
            if (m_UseLocal)
            {
                tr.localPosition = Vector3.MoveTowards(tr.localPosition, this.m_NetworkPosition, this.m_Distance  * Time.deltaTime * PhotonNetwork.SerializationRate);
            }
            else
            {
                tr.position = Vector3.MoveTowards(tr.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * PhotonNetwork.SerializationRate);
            }
        }
    }

    public IEnumerator HeartDestroyed()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            yield return new WaitForEndOfFrame();
            Destroy(this.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var tr = transform;
        // Write
        if (stream.IsWriting)
        {
            if (this.m_SynchronizePosition)
            {
                if (m_UseLocal)
                {
                    this.m_Direction = tr.localPosition - this.m_StoredPosition;
                    this.m_StoredPosition = tr.localPosition;
                    stream.SendNext(tr.localPosition);
                    stream.SendNext(this.m_Direction);
                    stream.SendNext(this.isDestroyed);
                }
                else
                {
                    this.m_Direction = tr.position - this.m_StoredPosition;
                    this.m_StoredPosition = tr.position;
                    stream.SendNext(tr.position);
                    stream.SendNext(this.m_Direction);
                    stream.SendNext(this.isDestroyed);
                }
            }

            if (isDestroyed)
            {
                Destroy(this.gameObject); //isDestroyed가 true로 전송되는 순간 게임 오브젝트를 파괴하고 끝
            }
        }
        // Read
        else
        {
            if (this.m_SynchronizePosition)
            {
                this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                this.m_Direction = (Vector3)stream.ReceiveNext();
                this.isDestroyed = (bool)stream.ReceiveNext();
                Debug.Log($"R: Pos: {m_NetworkPosition}, {m_Direction}, destroy?? {isDestroyed}");

                if (m_firstTake)
                {
                    if (m_UseLocal)
                        tr.localPosition = this.m_NetworkPosition;
                    else
                        tr.position = this.m_NetworkPosition;

                    this.m_Distance = 0f;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    this.m_NetworkPosition += this.m_Direction * lag;
                    if (m_UseLocal)
                    {
                        this.m_Distance = Vector3.Distance(tr.localPosition, this.m_NetworkPosition);
                    }
                    else
                    {
                        this.m_Distance = Vector3.Distance(tr.position, this.m_NetworkPosition);
                    }
                }

            }

            if (isDestroyed)
            {
                Debug.Log("PHOTON: this is destroyed");
                Destroy(this.gameObject); //isDestroyed가 true로 전송되는 순간 게임 오브젝트를 파괴하고 끝
            }
            if (m_firstTake)
            {
                m_firstTake = false;
            }
        }
    }
}
