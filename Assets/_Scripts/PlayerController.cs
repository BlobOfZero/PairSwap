using System.Collections;
using TempleRun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Variables")]
    [Header("Movement References")]
    [SerializeField] private float initialPlayerSpeed = 4f;
    [SerializeField] private float maximumPlayerSpeed = 30f;
    [SerializeField] private float speedIncreaseRate = 0.1f;
    [SerializeField] private float jumpHeight = 1.0f;

    [SerializeField] private float initialGravityValue = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask turnLayer;
    [SerializeField] private AnimationClip slideAnim;

    private float playerSpeed;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;

    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private CharacterController controller;
    private Animator animator;

    private bool isSliding = false;
    private float score = 0;
    private float scoreMultiplier = 10;

    [SerializeField] private UnityEvent<Vector3> turnEvent;
    [SerializeField] private UnityEvent<int> gameOverEvent;
    [SerializeField] private UnityEvent<int> scoreUpdateEvent;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
    }

    private void OnEnable()
    {
        turnAction.performed += PlayerTurn;
        slideAction.performed += PlayerSlide;
        jumpAction.performed += PlayerJump;
    }

    private void OnDisable()
    {
        turnAction.performed -= PlayerTurn;
        slideAction.performed -= PlayerSlide;
        jumpAction.performed -= PlayerJump;
    }

    private void Start()
    {
        playerSpeed = initialPlayerSpeed;
        gravity = initialGravityValue;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PlayerTurn(InputAction.CallbackContext context)
    {
        Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
        if(!turnPosition.HasValue)
        {
            GameOver();
            return;
        }
        Vector3 targetDirection = Quaternion.AngleAxis(90* context.ReadValue<float>(), Vector3.up) * movementDirection;

        turnEvent.Invoke(targetDirection);
        Turn(context.ReadValue<float>(), turnPosition.Value);

        playerSpeed += speedIncreaseRate;
        playerSpeed = Mathf.Clamp(playerSpeed, initialPlayerSpeed, maximumPlayerSpeed);
    }

    private Vector3? CheckTurn(float turnValue)
    {
       Collider[] hitCollider = Physics.OverlapSphere(transform.position, 0.1f, turnLayer);
        if(hitCollider.Length != 0)
        {
            Tile tile = hitCollider[0].transform.parent.GetComponent<Tile>();
            TileType type = tile.type;
            if((type == TileType.LEFT && turnValue == -1) || (type == TileType.RIGHT && turnValue == 1) || (type == TileType.SIDEWAYS))
            {
                return tile.pivot.position;
            }
        }
        return null;
    }

    private void Turn(float turnValue, Vector3 turnPostion)
    {
        
        Vector3 tempPlayerPostion = new Vector3(turnPostion.x, transform.position.y, turnPostion.z);
        controller.enabled = false;
        transform.position = tempPlayerPostion;
        controller.enabled = true;

        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
        transform.rotation = targetRotation;
        movementDirection = transform.forward.normalized;
    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {
        if(!isSliding && IsGrounded())
        {
            StartCoroutine(Slide());
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        // shrink collider
        Vector3 originControllerCenter = controller.center;
        Vector3 newControllerCenter = originControllerCenter;
        controller.height /= 2;
        newControllerCenter.y -= controller.height / 2;
        controller.center = newControllerCenter;

        animator.Play("Slide");

        print("Slide function being called");
        yield return new WaitForSeconds(2f);
        isSliding = false;

        // setting character controller back to normal after slide animation
        controller.height *= 2;
        controller.center = originControllerCenter;
        isSliding = false;
    }

    private void Update()
    {
        if(!IsGrounded(20f))
        {
            GameOver();
            return;
        }

        // score functionality
        score += scoreMultiplier * Time.deltaTime;
        scoreUpdateEvent.Invoke((int)score);

        controller.Move(transform.forward * playerSpeed * Time.deltaTime);

        if(IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    private bool IsGrounded(float length = 0.2f)
    {
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += 0.1f;

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * 0.2f;
        raycastOriginSecond += transform.forward * 0.2f;

        if(Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) ||
            Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        gameOverEvent.Invoke((int)score);
        gameObject.SetActive(false);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
        {
            GameOver();
        }
    }
}
