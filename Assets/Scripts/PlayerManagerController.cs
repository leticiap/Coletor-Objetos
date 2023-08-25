using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class PlayerManagerController : MonoBehaviourPunCallbacks
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    #region Serialized fields
    [SerializeField]
    private float playerSpeed = 1.5f;
    #endregion

    private Animator animator;
    private CharacterController controller;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private float rotationVelocity = 2.5f;

    #region MonoBehaviour Callbacks

    private void Awake()
    {

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManagerController.LocalPlayerInstance = gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();

        if (!animator)
            Debug.LogError("PlayerController is Missing Animator Component", this);

        if (!controller)
            Debug.LogError("PlayerController is Missing CharacterController Component", this);

        CameraWork cameraWork = gameObject.GetComponent<CameraWork>();

        if (cameraWork != null)
        {
            if (photonView.IsMine)
            {
                cameraWork.OnStartFollowing();
            }
        }

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            return;


        if (!animator || !controller)
            return;

        Move();
        Rotate();
    }

    #endregion

    private void Rotate()
    {
        Vector3 lookAtDirection = gameObject.transform.position + gameObject.transform.forward + gameObject.transform.right * (Input.GetAxis("Horizontal") * rotationVelocity * Time.deltaTime);
        if (lookAtDirection != Vector3.zero)
            transform.LookAt(lookAtDirection);
    }

    private void Move()
    {
        bool groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        if (!groundedPlayer)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
            // set animation to sit
        }

        Vector3 move = gameObject.transform.forward * Input.GetAxis("Vertical");
        move *= playerSpeed;

        if (move != Vector3.zero)
        {
            // set animation to walking
            animator.SetBool("IsWalking", true);
        }
        else
        {
            // set animation to stop walking
            animator.SetBool("IsWalking", false);
        }

        controller.Move((move + playerVelocity) * Time.deltaTime);
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        CalledOnLevelWasLoaded(scene.buildIndex);
    }

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            // new random postion if we are outside
            transform.position = new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 1, UnityEngine.Random.Range(-1.5f, 1.5f));
        }        
    }

    public override void OnDisable()
    {
        // Always call the base to remove callbacks
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
