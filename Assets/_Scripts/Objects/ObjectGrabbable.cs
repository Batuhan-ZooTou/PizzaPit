using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectGrabbable : MonoBehaviour
{
    public ItemSO ItemSO;
    public Rigidbody objectRigidbody;
    public Transform objectGrabPointTransform;
    [SerializeField] private Collider collider;
    private Interactor player;
    public bool insideSocket;
    public float moveSpeed;
    public bool snapped;
    public float socketspeed;
    public IObjectPool<GameObject> ingredientPoll;

    private void OnEnable()
    {
    }
    public void Grab(Transform objectGrabPointTransform,Interactor _player)
    {
        //Physics.IgnoreLayerCollision(3, 6, true);
        player = _player;
        if (insideSocket)
        {
            insideSocket = false;
            objectRigidbody.isKinematic = false;
            //socket.Close();

        }
        this.objectGrabPointTransform = objectGrabPointTransform;
            objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        objectRigidbody.useGravity = false;

    }
    public void Drop()
    {
        //Physics.IgnoreLayerCollision(3, 6, false);
        player.grabbedObject = null;
        this.objectGrabPointTransform = null;
        if (snapped)
        {
            objectRigidbody.velocity = Vector3.zero;

        }
            objectRigidbody.constraints = RigidbodyConstraints.None;
        objectRigidbody.useGravity = true;
    }
    public void CostumerHold(Transform point)
    {
        this.objectGrabPointTransform = null;
        transform.position = point.position;
        transform.rotation = point.rotation;
        collider.enabled = false;
        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        objectRigidbody.useGravity = false;
    }
    //public void LockOnSocket(Socket _socket)
    //{
    //    socket = _socket;
    //    insideSocket = true;
    //    this.objectGrabPointTransform = null;
    //    objectRigidbody.isKinematic = true;
    //}
    public void ThrowObject(Transform playerCameraTransform,float throwForce)
    {
        objectRigidbody.AddForce(playerCameraTransform.forward * throwForce);
    }
    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            // Simple
            // float lerpSpeed=50f;
            // Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            // objectRigidbody.MovePosition(newPosition);

            //physic based
            Vector3 DirectionToPoint = objectGrabPointTransform.position - transform.position;
            float DistanceToPoint = DirectionToPoint.magnitude;

            objectRigidbody.velocity = DirectionToPoint.normalized * moveSpeed * DistanceToPoint;
        }
        if (insideSocket)
        {
            //transform.position = Vector3.Lerp(transform.position, socket.transform.position, socketspeed);
            //transform.rotation = Quaternion.Lerp(transform.rotation, socket.transform.rotation, socketspeed);
        }
        
    }
}
