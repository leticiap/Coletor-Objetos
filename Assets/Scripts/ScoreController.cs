using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScoreController : MonoBehaviourPunCallbacks
{
    void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            return;

        if (other.gameObject.tag == ("Item") || other.gameObject.tag == ("SpecialItem"))
        {
            Destroy(other.gameObject);
            GameManager.UpdateScore(other.gameObject.tag == ("Special Item"));
            // play sound
        }
    }
}
