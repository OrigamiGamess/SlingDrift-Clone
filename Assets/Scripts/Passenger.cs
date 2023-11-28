using UnityEngine;

public class Passenger : MonoBehaviour
{
    private Rigidbody[] passengerBodies;

    private void Awake()
    {
        passengerBodies = transform.GetComponentsInChildren<Rigidbody>();
    }

    public void KinematicBody(bool value) //ragdoll
    {
        foreach (Rigidbody rb in passengerBodies)
        {
            rb.isKinematic = value;
        }
    }
    public void ApplyRagdollEffect(int direction)
    {
        Quaternion angle = Quaternion.AngleAxis(direction * 25, new Vector3(0, 1, 0));
        Vector3 vector = angle * transform.forward;

        transform.Translate(8f * Time.deltaTime * vector);
        transform.Translate(5f * Time.deltaTime * Vector3.up);

        /*Quaternion leftSpreadAngle = Quaternion.AngleAxis(-15, new Vector3(0, 1, 0));
        //Quaternion rightSpreadAngle = Quaternion.AngleAxis(15, new Vector3(0, 1, 0));
        Vector3 leftVector = leftSpreadAngle * transform.forward;
        //Vector3 rightVector = rightSpreadAngle * transform.forward;
            transform.Translate(5f * Time.deltaTime * leftVector);

       *//* if (straightDrive)
            transform..Translate(-Vector3.forward * 5f * Time.deltaTime);
        else if (leftDrive)
            transform.Translate(leftVector * 5f * Time.deltaTime);
        else if (rightDrive)
            transform.Translate(rightVector * 5f * Time.deltaTime);
        else if (turnLeftDrive)
            transform.Translate(-leftVector * 5f * Time.deltaTime, Space.World);
        else if (turnRightDrive)
            transform.Translate(-rightVector * 5f * Time.deltaTime, Space.World);*//*

        transform.Translate(2f * Time.deltaTime * Vector3.up);*/
    }
}
