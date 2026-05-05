using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100.0f;
    private float currentHealth;
    public float GetCurrentHealth() => this.currentHealth;
    public float GetMaxHealth() => this.maxHealth;
    private bool isJumping = false;
    
    private float jumpCooldownTimer;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction jumpAction;


    [SerializeField]
    private float characterSpeed;

    [SerializeField]
    private float jumpCooldown;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float dampening;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private Transform cameraTransform;

    private Vector3 characterMovement;
    private Vector3 characterGravity;
    private Vector3 jumpVelocity;
    private Vector3 platformVelocity;

    private void Start() {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.currentHealth = this.maxHealth;
        this.jumpCooldownTimer = 0.0f;
    }

    public void InflictDamage(float amount)
    {
        this.currentHealth -= amount;
        this.currentHealth = Mathf.Clamp(this.currentHealth, 0.0f, this.maxHealth);
    }

    void HandleJumping()
    {
        if (this.controller.isGrounded && this.isJumping && this.jumpCooldownTimer <= 0.0f) {
            this.jumpVelocity = Vector3.zero;
            this.isJumping = false;
        }
        if(this.controller.isGrounded && !this.isJumping && this.jumpAction.WasPressedThisFrame()) {
            this.characterGravity = Vector3.zero;
            this.jumpVelocity = Vector3.zero;
            this.jumpVelocity.y = this.jumpSpeed;
            this.jumpCooldownTimer = this.jumpCooldown;
            this.isJumping = true;
        }
        if(this.jumpVelocity.y > 0.0f) {
            this.jumpVelocity.y -= Time.fixedDeltaTime;
        } else {
            this.jumpVelocity = Vector3.zero;
        }
        this.jumpCooldownTimer -= Time.fixedDeltaTime;
    }

    void OnTriggerStay(Collider other)
{
    if (other.gameObject.layer == LayerMask.NameToLayer("Platforms"))
    {
        MovingPlatform platform = other.gameObject.GetComponent<MovingPlatform>();
        if (platform != null)
        {
            Vector3 pushVelocity = platform.GetVelocity();
            pushVelocity.y = 0.0f;
            this.characterMovement += pushVelocity * Time.fixedDeltaTime;
        }
    }
}

    void OnControllerColliderHit(ControllerColliderHit hit)
{
    Debug.Log("Hit: " + hit.gameObject.name + " Layer: " + hit.gameObject.layer);
    if (hit.gameObject.layer == LayerMask.NameToLayer("Platforms"))
    {
        MovingPlatform platform = hit.gameObject.GetComponent<MovingPlatform>();
        if (platform != null && hit.normal.y < 0.5f)
        {
            this.characterMovement += platform.GetVelocity() * Time.fixedDeltaTime;
        }
    }
}

    private void GetPlatformVelocity()
    {
    RaycastHit hit;
    int platformLayer = LayerMask.GetMask("Platforms");
    
    if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 1.5f, platformLayer))
    {
        MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();
        if (platform != null)
        {
            this.platformVelocity = platform.GetVelocity();
            return;
        }
    }
    this.platformVelocity = Vector3.zero;
    }

    

    private void FixedUpdate()
    {

        this.HandleJumping();
        var inputMovement = this.moveAction.ReadValue<Vector2>();

        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;

        inputRightDirection.y = 0.0f;
        inputForwardDirection.y = 0.0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();

        if (this.controller.isGrounded)
        {
            this.characterGravity.y = 0.0f;
        }

        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;

        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;

        this.characterMovement += inputRightDirection * inputMovement.x * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * this.characterSpeed * Time.fixedDeltaTime;

        this.characterMovement *= (1.0f - this.dampening);

        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;

        if (characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero) {
            this.transform.forward = characterForward.normalized;
        }


        this.GetPlatformVelocity();
        var combinedMovement = this.characterMovement;
        if (!this.isJumping) {
            combinedMovement += this.platformVelocity * Time.fixedDeltaTime;
        }
        this.controller.Move(combinedMovement);
        this.controller.Move(this.characterMovement);
    }
   
}
