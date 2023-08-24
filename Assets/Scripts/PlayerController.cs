using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
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

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = gameObject.AddComponent<CharacterController>();

        if (!animator)
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);

        if (!controller)
            Debug.LogError("PlayerAnimatorManager is Missing CharacterController Component", this);

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
        
        if (!animator || !controller)
            return;

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

        Vector3 lookAtDirection = gameObject.transform.position + gameObject.transform.forward + gameObject.transform.right * (Input.GetAxis("Horizontal") * rotationVelocity * Time.deltaTime);
        if (lookAtDirection != Vector3.zero)
        {
            Debug.Log(lookAtDirection);
            transform.LookAt(lookAtDirection);
        }
    }
    #endregion
}