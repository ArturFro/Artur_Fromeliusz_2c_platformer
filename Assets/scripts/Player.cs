using System.Collections;
using System.Collections.Generic;


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
    [SerializeField] private Vector2 tempData;
    [SerializeField] private KeyCode dashKey;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private float dashPower;
    [SerializeField] private bool duringDash;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float animTimer;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        onGround = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(jumpKey) && onGround)
        {
            tempData.Set(rig.velocity.x, jumpForce);
            rig.velocity = tempData;
            onGround = false;
          
            
        }  

        if(duringDash == true)
        {
            animTimer += Time.deltaTime;
            rig.velocity = new Vector2(curve.Evaluate(animTimer) * dashPower * Time.deltaTime, 0);
            if(animTimer >= 1)
            {
                duringDash = false;
                animTimer = 0;
            }
        }

        if(Input.GetAxis("Horizontal") != 0)
        {

            if(Input.GetKeyDown(dashKey))
            {
                Dash();    
            }
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
        
        if(duringDash == false)
        {
            tempData.Set(horizontalInput * moveSpeed, rig.velocity.y);
            rig.velocity = tempData;
        }
    }

    private void Dash()
    {
        print("dash");
        duringDash = true;
        //rig.AddForce(new Vector2(Input.GetAxis("Horizontal") * dashPower, 0));
        
    }
}
