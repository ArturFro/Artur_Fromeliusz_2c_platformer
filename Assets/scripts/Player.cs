using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool onGround;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        onGround = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            print("jump");
            rig.velocity = new UnityEngine.Vector2(rig.velocity.x, jumpForce);
            
        }   
    }
}
