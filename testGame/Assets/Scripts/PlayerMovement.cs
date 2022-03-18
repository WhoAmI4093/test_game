using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CanvasUpdate canvasUpdate;

    public float gravity = -9.81f;

    #region moving
    [SerializeField]
    bool previousFrameOnGround = true;
    [SerializeField]
    bool onGround = true;
    Vector3 previousFrameMove = Vector3.zero;

    public float speed = 5f;
    float speedMultiplyer = 1;
    public float defaultSpeedMultiplyer = 1;
    public float crouchSpeedMultiplyer = -0.5f;
    public float sprintSpeedMultiplyer = 1.25f;
    public float sprintStaminaCost = -10f;
    public float sprintStaminaRequirement = 15f;

    public float jumpHeight = 5f;
    public float jumpMultiplyer = 1;
    public float jumpStaminaCost = -5f;
    public float jumpStaminaRequirement = 5f;
    public Vector3 force;
    public Vector3 move = new Vector3();

    CharacterController characterController;
    #endregion
    public float hp = 100f;
    public float maxHp = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenerationSpeed = 1f;
    public float staminaWaitSecondsToRegenerate = 1f;
    float staminaLastUpdated;
    void Start()
    {

        canvasUpdate = FindObjectOfType<CanvasUpdate>();

        characterController = GetComponent<CharacterController>();

        StartCoroutine("staminaRegeneration");
    }

    void Update()
    {
        if (canvasUpdate == null) canvasUpdate = CanvasUpdate.current;
        if (canvasUpdate.gamePaused) return;


        onGround = Physics.CheckBox(transform.position - Vector3.up * 1, new Vector3(0.25f, 0.01f, 0.25f));
        if (onGround) speedMultiplyer = defaultSpeedMultiplyer;

        move = Vector3.zero;
        if (characterController == null) characterController = GetComponent<CharacterController>();

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        move = transform.right * x + transform.forward * z;
        move.Normalize();

        if (!onGround && previousFrameOnGround)
        {
            force.x = previousFrameMove.x;
            force.z = previousFrameMove.z;
        }

        transform.localScale = Vector3.one;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (onGround) speedMultiplyer *= crouchSpeedMultiplyer;
            transform.localScale = Vector3.one - Vector3.up * 0.5f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (onGround && stamina >= sprintStaminaRequirement)
            {
                speedMultiplyer *= sprintSpeedMultiplyer;
                StaminaUpdate(sprintStaminaCost * Time.deltaTime);
            }
        }

        move *= speed * speedMultiplyer;
        
        if (onGround)
        {
            force.x = 0f;
            force.z = 0f;
            characterController.Move(move * Time.deltaTime);
        }
        if (Physics.CheckBox(transform.position + Vector3.up * 1, new Vector3(0.25f, 0.01f, 0.25f)))
        {
            if (force.y > 0) force.y = 0f;
        }

        force.y += gravity * Time.deltaTime;
        

        if (onGround && force.y < 0) force.y = 0f;
        if (Input.GetButton("Jump") && onGround && stamina > jumpStaminaRequirement)
        {
            force.y = Mathf.Sqrt(jumpHeight * jumpMultiplyer * -2f * gravity);
            force.x = move.x;
            force.z = move.z;
            StaminaUpdate(jumpStaminaCost);
        }
        
        characterController.Move(force * Time.deltaTime);

        if (transform.position.y < 0)
        {
            TakeDamage(-10 * Time.deltaTime);
        }

        previousFrameMove = move;
        previousFrameOnGround = onGround;
    }

    public void TakeDamage(float dmg)
    {
        hp = Mathf.Clamp(hp + dmg, 0, maxHp);
    }
    public void StaminaUpdate(float value)
    {
        stamina = Mathf.Clamp(stamina + value, 0, maxStamina);
        if (value < 0) staminaLastUpdated = Time.realtimeSinceStartup;
    }
    IEnumerator staminaRegeneration()
    {
        while (true)
        {
            if (Time.realtimeSinceStartup >= staminaLastUpdated + staminaWaitSecondsToRegenerate) 
                stamina = Mathf.Clamp(stamina + staminaRegenerationSpeed * Time.deltaTime, 0, maxStamina);
            yield return new WaitForEndOfFrame();
        }
    }
}
