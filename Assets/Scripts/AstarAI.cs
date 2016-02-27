using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour {

    //The point to move to
    public Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    //The calculated path
    public Path path;
    //The AI's speed per second
    public float speed = 100;
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
    private UnitController unitController;
    public void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        unitController = GetComponent<UnitController>();
    }
    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
    public void Update()
    {
        if (path == null)
        {
            //We have no path to move after yet
            return;
        }
        else {
            transform.LookAt(targetPosition);
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (!unitController.isAttack)
                unitController.Idle();

            if (targetPosition == unitController.endPoint && !unitController.isDead)
            {
                unitController.Death();
            }
            Debug.Log("End Of Path Reached");
            return;
        }
        if (!unitController.isAttack)
        {
            Debug.Log("Move");
            if (!unitController.isMove)
            {
                unitController.Move();
            }
            //Direction to the next waypoint
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir *= speed * Time.deltaTime;
            controller.SimpleMove(dir);
            //Debug.Log(dir);
            //Check if we are close enough to the next waypoint
            //If we are, proceed to follow the next waypoint
            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }
    }
    public void ChangePath()
    {
        path = null;
        currentWaypoint = 0;
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }
}
