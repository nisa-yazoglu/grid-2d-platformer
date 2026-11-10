using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Char1_0Animations : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the specific character controller")]
    public Char1_0Controller controller;

    [Header("Animator Parameter Names")]
    [Tooltip("Parameter name for movement speed (Float)")]
    public string speedParam = "Speed";
    
    [Tooltip("Parameter name for grounded state (Bool)")]
    public string groundedParam = "IsGrounded";
    
    [Tooltip("Parameter name for dashing state (Bool)")]
    public string dashingParam = "IsDashing";
    
    [Tooltip("Parameter name for vertical velocity (Float)")]
    public string verticalVelocityParam = "VerticalVelocity";
    
    [Tooltip("Parameter name for jump trigger (Trigger)")]
    public string jumpTriggerParam = "Jump";

    [Tooltip("Parameter name for collect trigger (Trigger)")]
    public string collectTriggerParam = "Collect";

    [Tooltip("Name of the Jump state in the Animator window (Case Sensitive!)")]
    public string jumpStateName = "jump";

    private Animator animator;

    // Hashes for performance
    private int speedHash;
    private int groundedHash;
    private int dashingHash;
    private int verticalVelocityHash;
    private int jumpHash;
    private int collectHash;
    private int jumpStateHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        // If controller is not assigned, try to find it on the same GameObject or parents
        if (controller == null)
        {
            controller = GetComponentInParent<Char1_0Controller>();
        }

        if (controller == null)
        {
            Debug.LogError("Char1_0Animations: No Char1_0Controller found! Please assign it in the inspector.");
            this.enabled = false; // Disable script to prevent errors
            return;
        }

        // Convert strings to hashes
        speedHash = Animator.StringToHash(speedParam);
        groundedHash = Animator.StringToHash(groundedParam);
        dashingHash = Animator.StringToHash(dashingParam);
        verticalVelocityHash = Animator.StringToHash(verticalVelocityParam);
        jumpHash = Animator.StringToHash(jumpTriggerParam);
        collectHash = Animator.StringToHash(collectTriggerParam);
        jumpStateHash = Animator.StringToHash(jumpStateName);
    }

    private void OnEnable()
    {
        if (controller != null)
        {
            controller.OnJump += HandleJump;
            controller.OnCollect += HandleCollect;
        }
    }

    private void OnDisable()
    {
        if (controller != null)
        {
            controller.OnJump -= HandleJump;
            controller.OnCollect -= HandleCollect;
        }
    }

    private void Update()
    {
        if (controller == null) return;

        // Calculate horizontal speed (absolute value)
        float horizontalSpeed = Mathf.Abs(controller.Velocity.x);
        
        // Update Animator parameters
        animator.SetFloat(speedHash, horizontalSpeed);
        animator.SetBool(groundedHash, controller.IsGrounded);
        animator.SetBool(dashingHash, controller.IsDashing);
        animator.SetFloat(verticalVelocityHash, controller.Velocity.y);

        // Reset jump trigger if we are already jumping or falling to prevent double animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Checking if the current state is the Jump state
        if (stateInfo.shortNameHash == jumpStateHash) 
        {
            animator.ResetTrigger(jumpHash);
        }
    }

    private void HandleJump()
    {
        animator.SetTrigger(jumpHash);
    }

    private void HandleCollect()
    {
        animator.SetTrigger(collectHash);
    }
}
