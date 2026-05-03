using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System.Collections;

public class ThirdPersonController : MonoBehaviour
{
    [FoldoutGroup("References")]
    public InputSystem_Actions inputs;

    [FoldoutGroup("References")]
    private CharacterController controller;

    [FoldoutGroup("References")]
    public CinemachineCamera characterCamera;

    [FoldoutGroup("References")]
    public Animator animator;

    [FoldoutGroup("References")]
    [SerializeField] 
    private CinemachineImpulseSource source;

    [SerializeField] 
    private Vector2 moveInput;

    #region Controlador

    [FoldoutGroup("Controller")]
    public float moveSpeed = 5f;

    [FoldoutGroup("Controller")]
    public float rotationSpeed = 200f;

    [FoldoutGroup("Controller")]
    public float verticalVelocity = 0;

    [FoldoutGroup("Controller")]
    public float jumpForce = 10;

    [FoldoutGroup("Controller")]
    public float pushForce = 4;
    #endregion

    #region Impulso
    [FoldoutGroup("Controller/Dash")]
    private bool IsDashing;

    [FoldoutGroup("Controller/Dash")]
    public float dashForce = 20f;

    [FoldoutGroup("Controller/Dash")]
    public float dashDuration = 0.2f;

    [FoldoutGroup("Controller/Dash")]
    public float dashCooldown = 1.5f;

    [FoldoutGroup("Controller/Dash")]
    private bool canDash = true;

    [FoldoutGroup("Controller/Dash")]
    private float dashTimer;
    #endregion

    #region Correr
    [FoldoutGroup("WallRun")]
    public float rayLenght = 2f;

    [FoldoutGroup("WallRun")]
    public float cameraTitlt = 15;

    [FoldoutGroup("WallRun")]
    public float maxTimeInAir;

    [FoldoutGroup("WallRun")]
    public bool enableWallRun;
    #endregion

    #region Saltar
    [FoldoutGroup("Wall Jump")]
    public float wallJumpForce = 10f;

    [FoldoutGroup("Wall Jump")]
    public float wallJumpUpForce = 8f;

    [FoldoutGroup("Wall Jump")]
    public float wallJumpCooldown = 1f;

    [FoldoutGroup("Wall Jump")]
    private bool canWallJump = true;
    #endregion

    #region Vida
    [FoldoutGroup("Health")]
    public int health = 100;
    #endregion

    #region Debug
    Vector3 normalDebug;
    Vector3 impactPoint;
    Vector3 crossResult;
    #endregion

    #region Metodos
    private void Awake()
    {
        inputs = new();

        controller = GetComponent<CharacterController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        inputs.Enable();

        inputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();

        inputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputs.Player.Jump.performed += OnJump;

        inputs.Player.Sprint.performed += OnDash;
    }

    private void Update()
    {
        EnableWallRun();

        OnMove();
    }
    #endregion

    #region Movimiento
    public void OnMove()
    {
        Vector3 cameraForwardDir = characterCamera.transform.forward;

        cameraForwardDir.y = 0;

        cameraForwardDir.Normalize();

        if (moveInput != Vector2.zero)
        {
            Quaternion targetQuaternion = Quaternion.LookRotation(cameraForwardDir);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);
        }

        Vector3 moveDir;

        if (!enableWallRun)
        {
            moveDir =
                (cameraForwardDir * moveInput.y + transform.right * moveInput.x) * moveSpeed;
        }
        else
        {
            moveDir = (crossResult * moveInput.y) * moveSpeed;
        }

        float magnitud = Mathf.Abs(controller.velocity.magnitude);

        animator.SetFloat("Speed", magnitud);

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if (enableWallRun)
            verticalVelocity = 0;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        moveDir.y = verticalVelocity;

        animator.SetBool("Grounded", controller.isGrounded);
        #endregion

        #region Impulso
        if (IsDashing)
        {
            moveDir =transform.forward * dashForce * (dashTimer / dashDuration);

            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
                IsDashing = false;
        }

        controller.Move(moveDir * Time.deltaTime);
    }
    #endregion

    #region Salto
    private void OnJump(InputAction.CallbackContext context)
    {
        if (enableWallRun && canWallJump)
        {
            StartCoroutine(WallJumpCoroutine());
            return;
        }

        if (!controller.isGrounded) return;

        animator.SetTrigger("Jump");

        source.GenerateImpulse();

        verticalVelocity = jumpForce;
    }
    #endregion

    #region OnImpulso
    private void OnDash(InputAction.CallbackContext context)
    {
        if (!canDash) return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;

        IsDashing = true;

        dashTimer = dashDuration;

        yield return new WaitForSeconds(dashDuration);

        IsDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    #endregion

    #region WallSalto
    private IEnumerator WallJumpCoroutine()
    {
        canWallJump = false;

        animator.SetTrigger("Jump");

        source.GenerateImpulse();

        Vector3 jumpDirection = normalDebug + Vector3.up;

        controller.Move(jumpDirection.normalized * wallJumpForce * Time.deltaTime);

        verticalVelocity = wallJumpUpForce;

        enableWallRun = false;

        yield return new WaitForSeconds(wallJumpCooldown);

        canWallJump = true;
    }
    #endregion

    #region EnableCorrer
    public void EnableWallRun()
    {
        if (controller.isGrounded)
        {
            enableWallRun = false;

            characterCamera.Lens.Dutch = 0;

            return;
        }

        RaycastHit hit = default;

        Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, rayLenght);

        Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, rayLenght);

        if (hitRight.collider != null && hitRight.collider.gameObject.CompareTag("Wall"))
        {
            hit = hitRight;

            characterCamera.Lens.Dutch = cameraTitlt;
        }
        else if (hitLeft.collider != null && hitLeft.collider.gameObject.CompareTag("Wall"))
        {
            hit = hitLeft;

            characterCamera.Lens.Dutch = -cameraTitlt;
        }
        else
        {
            characterCamera.Lens.Dutch = 0;

            enableWallRun = false;
        }

        if (hit.collider != null)
        {
            enableWallRun = true;

            normalDebug = hit.normal;

            impactPoint = hit.point;

            crossResult = Vector3.Cross(normalDebug, transform.up);

            if (Vector3.Dot(crossResult, transform.forward) < 0)
            {
                crossResult *= -1;
            }
        }
    }
    #endregion

    #region MovimientoSimple
    public void OnSimpleMove()
    {
        transform.Rotate(Vector3.up * moveInput.x * rotationSpeed * Time.deltaTime);

        Vector3 moveDir =transform.forward * moveSpeed * moveInput.y;

        controller.SimpleMove(moveDir);
    }
    #endregion

    #region ColisionPush
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 pushDir = (hit.transform.position - transform.position).normalized;

        if (hit.rigidbody != null && hit.rigidbody.linearVelocity == Vector3.zero)
        {
            print(hit.gameObject.name);

            hit.rigidbody.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }
    }
    #endregion

    #region Daño
    public void TakeDamage(int damage)
    {
        health -= damage;

        source.GenerateImpulse();

        Debug.Log("Daño recibido");

        if (health <= 0)
        {
            Debug.Log("Jugador muerto");
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, transform.right * rayLenght);

        Gizmos.DrawRay(transform.position, -transform.right * rayLenght);

        Gizmos.color = Color.blue;

        Gizmos.DrawRay(impactPoint, normalDebug * 2);

        Gizmos.color = Color.green;

        Gizmos.DrawRay(impactPoint, crossResult * 2);
    }
    #endregion
}