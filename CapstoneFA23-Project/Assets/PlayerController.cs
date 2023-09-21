using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;

    public float collisionOffset = 0.05f;

    public ContactFilter2D movementFilter;


    Vector2 movement;

    Rigidbody2D rb;

    Animator animator;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            bool success = TryMove(movement);

            if (!success)
            {
                success = TryMove(new Vector2(movement.x, 0));

                if (!success)
                {
                    success = TryMove(new Vector2(0, movement.y));
                }
            }

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);


        }
    }

      
    private bool TryMove(Vector2 direction)
    {
        int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {

            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;

        }
        else
        {
            return false;
        }
    }


    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<Vector2>();

        // Reset movement to (0,0) if there is no input
        if (movement == Vector2.zero)
        {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
            animator.SetFloat("Speed", 0f);
        }
    }

}
