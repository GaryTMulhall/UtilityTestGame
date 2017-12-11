using System.Collections;
using UnityEngine;

public class SeekTarget : MonoBehaviour
{
    public float maxSpeed = 0.6f; //Setting the maximum speed the agent can achieve
    public float arrivalRadius = 0.01f;
    public Vector3 target;
    float waitTime;

    enum State { Idle, Moving, Busy } //States available to the agent
    State state = State.Idle;

    private void Start()
    {

    }

    /*An IEnumerator which changes the agents state to "busy", makes the agent
     *wait for the waitTime which is attached to the desires
     *then sets the agent back to "idle" which allows it to fulfill another desire*/
    IEnumerator WaitAtTarget()
    {
        state = State.Busy;
        yield return new WaitForSeconds(waitTime);
        state = State.Idle;
    }

    private void FixedUpdate()
    {
        /*Setting what happens if the agent switches to the "moving" state
         *This includes checking the distance between the agent and the desired target
         *and checking if the agent is outside the arrivalRadius of the target
         *if so it is moved towards the target until it reaches it and then the
         *WaitAtTarget CoRoutine begins*/
        if (state == State.Moving)
        {
            Vector3 distance = target - transform.position;
            if (distance.magnitude > arrivalRadius)
            {
                Vector3 direction = distance.normalized;
                transform.position += direction * maxSpeed;
            }
            else
            {
                StartCoroutine(WaitAtTarget());
            }
        }
    }
    /*Setting the target the agent should move towards and setting the
     *waitTime to the time applied to the desire/target*/
    public void SetTarget(Vector3 Target, float WaitTime)
    {
        if (state == State.Idle)
        {
            target = Target;
            waitTime = WaitTime;
            state = State.Moving;
        }
    }
}
