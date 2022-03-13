using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float speedMultiplyer = 1;
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

    // Update is called once per frame
    void Update()
    {
        if (canvasUpdate.current.gamePaused) return;

        move = Vector3.zero;
        if (characterController == null) characterController = GetComponent<CharacterController>();

        bool onGround = characterController.isGrounded;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        move = transform.right * x + transform.forward * z;
        move.Normalize();

        characterController.Move(move * speed * speedMultiplyer * Time.deltaTime);
        

        if (Physics.CheckBox(transform.position + Vector3.up * 1, new Vector3(0.25f, 0.01f, 0.25f)))
        {
            if (force.y > 0) force.y = 0;
        }

        force.y += gravity * Time.deltaTime;

        if (onGround && force.y < 0) force.y = 0f;
        if (Input.GetButton("Jump") && onGround)
        {
            force.y = Mathf.Sqrt(jumpHeight * jumpMultiplyer * -2f * gravity);
        }
        
        characterController.Move(force * Time.deltaTime);
    }
}
