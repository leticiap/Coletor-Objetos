using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CatCollisionManager : MonoBehaviourPunCallbacks
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Item") || other.gameObject.tag == ("SpecialItem"))
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(other.gameObject);

            if (photonView.IsMine)
                GameManager.Instance.UpdateScore(other.gameObject.tag == ("Special Item"));

        }
    }
}
