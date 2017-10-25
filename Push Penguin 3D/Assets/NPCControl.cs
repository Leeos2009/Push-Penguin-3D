﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCControl : MonoBehaviour, IDestoryable {

    /// <summary>
    /// waiting = can move, waiting for destination;
    /// moving = has destination, moving towards it;
    /// still = can't move;
    /// </summary>
    private enum Status { waiting, moving, still };
    private Status status;

    private Vector3 destination;
    private static Transform NPCParent;
    private Transform raycastTarget;

    private int moveSpeed = 3;
    private const float STOPPING_DISTANCE = 0.06f, WAIT_TIME = 0.25f;
    
    void Start () {
        //If there is no NPC parent, create and assign NPC parent
        if (NPCParent == null)
            NPCParent = new GameObject("NPC Parent").transform;

        transform.SetParent(NPCParent);
        status = Status.waiting;
        raycastTarget = transform.GetChild(0);
	}
	
	void Update ()
    {
        //Debug.DrawRay(transform.position, transform.right * 2, Color.red);

        //If can move and has no destination
        if (status == Status.waiting)
        {
            RaycastHit hit;
            
            //Raycast, if the raycast hits something (ice block or wall)
            Physics.Raycast(transform.position, transform.right, out hit);
            if (hit.rigidbody != null)
            {
                //set destionation to 1 unit 'in front' of raycast his position
                destination = hit.rigidbody.position - (hit.rigidbody.position - transform.position).normalized;
                status = Status.moving;
            }
        }
        else if (status == Status.moving)
        {
            //If NPC is more than STOPPING_DISTANCE away from destination
            if ((destination - transform.position).magnitude > STOPPING_DISTANCE)
            {
                //Move towards destination
                Vector3 velocity = moveSpeed * (destination - transform.position).normalized;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                status = Status.still;
                StartCoroutine(WaitForNewDestination());
            }
        }
        //Raycast to player
    }

    //Kill enemy
    public bool DoDestroy()
    {
        Destroy(gameObject);
        return true;
    }
    
    /// <summary>
    /// Wait for WAIT_TIME seconds before rotating 90 degrees and then allowing the NPC to move again.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForNewDestination()
    {
        yield return new WaitForSeconds(WAIT_TIME);
        transform.Rotate(0, -90, 0);
        status = Status.waiting;
    }
}
