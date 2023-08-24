using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class PlayerController : MonoBehaviourPunCallbacks
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
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = gameObject.AddComponent<CharacterController>();

        if (!animator)
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);

        if (!controller)
            Debug.LogError("PlayerAnimatorManager is Missing CharacterController Component", this);

        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }

        // setting up manually because of the ghost character controller that keeps reapearing
        controller.radius = 0.1f;
        controller.height = 0.2f;
        controller.center = new Vector3(0, 0.1f, 0);
        controller.stepOffset = 0.05f;
        controller.skinWidth = 0.001f;
        controller.minMoveDistance = 0;

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
}
