using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendRay : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layerMask;
    private Ray ray;
    [SerializeField] private GameObject moveWall;



    private void Ray()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        ray.origin = transform.position;
        ray.direction = ray.direction;

        print("ray");

        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            print("switch");
            switch(hit.transform.tag)
            {

                case "Move Wall":
                moveWall = hit.transform.gameObject;
                MoveWall();
                break;

               
            }
        }
    }

    private void MoveWall()
    {

    }
   
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray();
        } 
    }
}
