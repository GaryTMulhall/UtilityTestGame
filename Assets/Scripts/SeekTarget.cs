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
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, 20.0f))
        {
            //check that its not hitting itself
            //then add the normalised hit direction to your direction plus some repulsion force -in my case // 400f

            if (hit.transform != transform)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);

                target += hit.normal * 2.5f;
            }
        }

        //now make two more raycasts out to the left and right to make the cornering more accurate and reducing collisions more

        Vector3 leftR = transform.position;
        Vector3 rightR = transform.position;

        leftR.x -= 2;
        rightR.x += 2;

        if (Physics.Raycast(leftR, -transform.right, out hit, 20.0f))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(leftR, hit.point, Color.red);
                target += hit.normal * 2.5f;
            }

        }
        if (Physics.Raycast(rightR, transform.right, out hit, 20.0f))
        {
            if (hit.transform != transform)
            {
                Debug.DrawLine(rightR, hit.point, Color.red);

                target += hit.normal * 2.5f;
            }
        }

        // then set the look rotation toward this new target based on the collisions

        Quaternion lookAtTarget = Quaternion.LookRotation(target);

        //then slerp the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, Time.deltaTime * 10.0f);

        //finally add some propulsion to move the object forward based on this rotation
        //mine is a little more complicated than below but you hopefully get the idea…

        transform.position += transform.right * 5.0f * Time.deltaTime;
    
        if (state == State.Idle)
        {
            target = Target;
            waitTime = WaitTime;
            state = State.Moving;
        }
    }
}
