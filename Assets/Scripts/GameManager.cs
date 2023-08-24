using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    [Tooltip("The prefab to use for representing the regular item.")]
    public GameObject itemPrefab;

    [Tooltip("The prefab to use for representing the special item.")]
    public GameObject specialItemPrefab;

    [SerializeField]
    private const int itemValue = 10;
    [SerializeField]
    private const int specialItemValue = 30;
    [SerializeField]
    private const float specialItemChanceToDrop = 0.2f;

    private static int score = 0;

    #region Monobehavior Callbacks
    private void Start()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 0.4f, UnityEngine.Random.Range(-1.5f, 1.5f)), Quaternion.identity, 0);
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }

        if (itemPrefab == null)
            Debug.LogError("No prefab found to instantiate");

        else
            StartCoroutine(spawnObjects());
    }

    private void Update()
    {
        Debug.Log(score);
    }
    #endregion

    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    #endregion

    #region Private Methods

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        String sceneName = PhotonNetwork.CurrentRoom.PlayerCount == 1 ? "SinglePlayerRoom" : "MultiPlayerRoom";
        PhotonNetwork.LoadLevel(sceneName);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    IEnumerator spawnObjects()
    {
        Vector3 randomPos = new Vector3 (UnityEngine.Random.Range(-1.5f, 1.5f), 3, UnityEngine.Random.Range(-1.5f, 1.5f));
        float specialItemChance = UnityEngine.Random.Range(0f, 1f);
        Instantiate(specialItemChance < specialItemChanceToDrop ? specialItemPrefab : itemPrefab, randomPos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        StartCoroutine(spawnObjects());
    }

    public static void UpdateScore(bool isSpecialItem)
    {
        if (isSpecialItem)
            score += specialItemValue;
        else
            score += itemValue;
    }
    #endregion
}
