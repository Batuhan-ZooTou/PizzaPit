using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PizzaBox : MonoBehaviour
{
    bool isOpen=true;
    Transform box;
    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask doughLayer;
    Pizza myPizza;
    // Start is called before the first frame update
    void Start()
    {
        box = transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, doughLayer))
        {
            if (isOpen && myPizza == null)
            {
                if (hit.collider.TryGetComponent(out myPizza))
                {
                    ObjectGrabbable Obj = myPizza.GetComponent<ObjectGrabbable>();
                    Obj.Drop();
                    Obj.objectRigidbody.velocity = Vector3.zero;
                    myPizza.gameObject.transform.position = transform.position;
                    myPizza.transform.SetParent(transform);
                    myPizza.GetComponent<Rigidbody>().useGravity = false;
                    myPizza.GetComponent<Collider>().isTrigger = true;
                }
            }
        }
        if (myPizza==null)
        {
            return;
        }
        if (myPizza.GetComponent<ObjectGrabbable>().objectGrabPointTransform == null)
        {
            myPizza.gameObject.transform.position = transform.position;
        }
        else
        {
            myPizza = null;
        }
    }
    public void Open()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            box.DOBlendableLocalRotateBy(new Vector3(75, 0, 0), 1f);

        }
        else
        {
            box.DOBlendableLocalRotateBy(new Vector3(-75, 0, 0), 1f);

        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents * 2);
    }
}
