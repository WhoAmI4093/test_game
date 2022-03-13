using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool previousFrameOnGround = true;
    Vector3 previousFrameMove = Vector3.zero;

    public float speed = 5f;
    public float defaultSpeedMultiplyer = 1;
    float speedMultiplyer = 1;

    public float crouchSpeedMultiplyer = -0.5f;
    public float sprintSpeedMultiplyer = 1.25f;

    public float gravity = -9.81f;
    
    public float jumpHeight = 5f;
    public float jumpMultiplyer = 1;
    public Vector3 force;
    public Vector3 move = new Vector3();
    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (canvasUpdate.current.gamePaused) return;

        bool onGround = Physics.CheckBox(transform.position - Vector3.up * 1, new Vector3(0.25f, 0.01f, 0.25f));
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
            if (onGround) speedMultiplyer *= sprintSpeedMultiplyer;
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
        if (Input.GetButton("Jump") && onGround)
        {
            force.y = Mathf.Sqrt(jumpHeight * jumpMultiplyer * -2f * gravity);
            force.x = move.x;
            force.z = move.z;
        }
        
        characterController.Move(force * Time.deltaTime);

        previousFrameMove = move;
        previousFrameOnGround = onGround;
    }
}
