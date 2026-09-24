using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool onGround;
    [SerializeField] private Collider2D groundCol;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float moveSpeed;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        onGround = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            
            rig.velocity = new UnityEngine.Vector2(rig.velocity.x, jumpForce);
            onGround = false;  
            
        }   

        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void OnCollisionEnter2D(Collision2D player)
    {
        if(player.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }
    private void FixedUpdate()
    {
        rig.velocity = new UnityEngine.Vector2(horizontalInput * moveSpeed, rig.velocity.y);
    }


}
