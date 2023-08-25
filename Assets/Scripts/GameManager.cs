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

    public static GameManager Instance;

    [SerializeField]
    private const int itemValue = 10;
    [SerializeField]
    private const int specialItemValue = 30;
    [SerializeField]
    private const float specialItemChanceToDrop = 0.2f;
    [Tooltip("Time in seconds between each spawn.")]
    [SerializeField]
    private const float itemSpawnTime = 1.5f;

    [Tooltip("Time of each game in seconds.")]
    [SerializeField]
    private const int time = 50;

    private int score;

    private float timer;

    #region Monobehavior Callbacks
    private void Awake()
    {
        // guarantees only one instance of the GameManager
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Cursor.visible = false;
        Instance = this;
        score = 0;

        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManagerController.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 1, UnityEngine.Random.Range(-1.5f, 1.5f)), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }



        if (itemPrefab == null)
            Debug.LogError("No prefab found to instantiate");

        // Spawn objects only from master
        else if (PhotonNetwork.IsMasterClient)
            StartCoroutine(spawnObjects());

        timer = time;
    }

    private void Update()
    {
        ManageTime();
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
    private void ManageTime()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ExitGame();
        }
    }

    private void ExitGame()
    {
        int bestScore = PlayerPrefs.HasKey("Best Score") ? PlayerPrefs.GetInt("Best Score") : 0;
        if (bestScore < score)
        {
            PlayerPrefs.SetInt("Best Score", score);
            PlayerPrefs.SetString("Best Player Name", PhotonNetwork.NickName);
        }

        if (PhotonNetwork.IsMasterClient)
            LeaveRoom();
    }

    private void LoadArena()
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
        Vector3 randomPos = new Vector3 (UnityEngine.Random.Range(-5, 5), 3, UnityEngine.Random.Range(-5, 5));
        float specialItemChance = UnityEngine.Random.Range(0f, 1f);
        PhotonNetwork.Instantiate(specialItemChance < specialItemChanceToDrop ? specialItemPrefab.name : itemPrefab.name, randomPos, Quaternion.identity);
        
        yield return new WaitForSeconds(itemSpawnTime);
        StartCoroutine(spawnObjects());
    }

    public int GetScore()
    {
        return score;
    }

    public void UpdateScore(bool isSpecialItem)
    {
        if (isSpecialItem)
            score += specialItemValue;
        else
            score += itemValue;
    }

    public int CurrentTime()
    {
        return (int) Math.Ceiling(timer);
    }
    #endregion
}
