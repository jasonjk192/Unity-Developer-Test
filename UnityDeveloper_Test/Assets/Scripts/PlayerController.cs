using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Animator animator;
    private Camera camera;

    public float speed = 10f;
    public float rotate_speed = 1000f;
    public float jumpSpeed = 2.0f;

    public GameObject hologram;
    public GameManagerScript gameManager;

    private bool isGrounded = false;
    private bool isManipulatingGravity = false;
    private int groundLayerMask = 1 << 6;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + transform.up * 0.1f, -transform.up);
        isGrounded = Physics.Raycast(ray.origin, ray.direction, out hit, 0.2f, groundLayerMask);
    }

    void Update()
    {
        // Grounded and Jump/Fall logic
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump") && !isManipulatingGravity)
                rigidbody.AddForce(transform.up * jumpSpeed);
            else
                animator.SetBool("isFalling", false);
        }
        else
        {
            animator.SetBool("isFalling", true);
        }


        // Movement logic
        Vector3 movement = (Input.GetAxis("Horizontal") * camera.transform.right + Input.GetAxis("Vertical") * camera.transform.forward).normalized;
        Debug.DrawRay(transform.position, movement.x * Vector3.right, Color.red, 1f);
        Debug.DrawRay(transform.position, movement.y * Vector3.up, Color.green, 1f);
        Debug.DrawRay(transform.position, movement.z * Vector3.forward, Color.blue, 1f);

        if (movement.magnitude != 0 && !isManipulatingGravity)
        {
            rigidbody.AddForce(movement * speed);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), rotate_speed * Time.deltaTime);
            transform.forward = movement;
            transform.up = hologram.transform.up;
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Gravity manip logic

        // Depends on camera's perspective (Also won't allow to invert direction of gravity directly)
        Vector3 gravityDir = (Input.GetAxis("GravityHorizontal") * camera.transform.right + Input.GetAxis("GravityVertical") * camera.transform.forward).normalized;

        if(gravityDir.magnitude > 0 && isGrounded)
        {
            isManipulatingGravity = true;

            // Make sure to snap the directions to x,y or z axes
            float gx = Mathf.Abs(gravityDir.x);
            float gy = Mathf.Abs(gravityDir.y);
            float gz = Mathf.Abs(gravityDir.z);
            if (gx > gy && gx > gz) gravityDir = Vector3.right * Mathf.Sign(gravityDir.x);
            else if (gy > gx && gy > gz) gravityDir = Vector3.up * Mathf.Sign(gravityDir.y);
            else if (gz > gx && gz > gy) gravityDir = Vector3.forward * Mathf.Sign(gravityDir.z);

            hologram.SetActive(true);
            hologram.transform.position = transform.position;
            hologram.transform.up = -gravityDir;
            hologram.transform.position += transform.up;
        }

        if(isManipulatingGravity && Input.GetAxis("Submit") > 0) 
        {
            isManipulatingGravity = false;
            Physics.gravity = hologram.transform.up * -9.81f;
            transform.up = hologram.transform.up;
            hologram.SetActive(false);

            camera.transform.up = hologram.transform.up;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Collectible"))
        {
            Destroy(collision.gameObject);
            gameManager.DeductCollectible(1);
        }
    }
}
