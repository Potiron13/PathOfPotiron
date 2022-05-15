using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    //NavMeshAgent variable control player movement
    public NavMeshAgent playerNavMeshAgent;

    //A Camera that follow player movement
    public Camera playerCamera;
    public bool isAutomode;
    public bool isStoped;

    public LayerMask walkableLayerMask;

    // Update is called once per frame
    private void Update()
    {
        //if the left button of is clicked
        if (Input.GetMouseButton(0) && !isAutomode)
        {
            //Unity cast a ray from the position of mouse cursor on-screen toward the 3D scene.
            Ray myRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit myRaycastHit;

            if (Physics.Raycast(myRay, out myRaycastHit, 500, walkableLayerMask))
            {
                //Assign ray hit point as Destination of Navemesh Agent (Player)                
                playerNavMeshAgent.SetDestination(myRaycastHit.point);
            }            
        }        
        if(playerNavMeshAgent.remainingDistance > 0.5 && gameObject.name == "Player")
        {
            GetComponentInChildren<Animator>().SetTrigger("Walk");
        } else
        {
            GetComponentInChildren<Animator>().SetTrigger("Idle");
        }

    }
}
