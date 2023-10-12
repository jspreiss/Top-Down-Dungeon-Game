using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    Vector2 movementInput;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called once per physic update
    private void FixedUpdate() {
        if(canMove == true) {
            if(movementInput != Vector2.zero) {
                bool success = TryMove(movementInput);

                if(!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));

                    if(!success) {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }

            // isMoving depends on success of movement
                animator.SetBool("isMoving", success);
            } else {
                animator.SetBool("isMoving", false);
            }

            // set sprite direction facing movement direction
            if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if(movementInput.x > 0) {
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction) {
        if(direction != Vector2.zero) {
            // counts potential collisions
            int count = rb.Cast(
                direction, // direction between -1 t0 1 from body to look for collision
                movementFilter, // setting to filter what layers we count as colliders
                castCollisions, // list of collisions to store found collisions
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // max distance to cast shape

            if(count == 0) {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            return false; // can't move with no space to move into
        }
        
    }
    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();
        if(spriteRenderer.flipX == true) {
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
    }

    public void EndSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
    
}
