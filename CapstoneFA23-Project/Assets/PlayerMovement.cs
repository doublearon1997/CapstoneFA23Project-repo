using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
   
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Animator animator;
    public Tilemap obstacles;

    Vector2 movement;
    Vector3 moveToPosition;

    // Update is called once per frame
    void Update()
    {


         

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

       

    }

    void FixedUpdate()
    {
        
          rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        
    }

}
