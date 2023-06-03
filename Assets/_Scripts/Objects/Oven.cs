using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask ingredientLayer;
    Pizza myPizza;
    ObjectGrabbable Obj;

    public Transform targetTransform; 

    private bool isPizzaCooking = false; 
    public float cookingTime = 25f; 
    public float burntTime = 15f; 
    private float currentTimer = 0f; 

    private void Update()
    {
        if (!isPizzaCooking)
        {
            if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, ingredientLayer))
            {
                if (hit.collider.TryGetComponent(out Obj))
                {
                    if (hit.collider.TryGetComponent(out myPizza))
                    {
                        if (myPizza.isCooked!=Cooked.notCooked ||myPizza.insideBox)
                        {
                            return;
                        }
                        Obj.Drop();
                        Obj.objectRigidbody.velocity = Vector3.zero;
                        myPizza.gameObject.transform.position = targetTransform.position;
                        myPizza.gameObject.transform.rotation = targetTransform.rotation;
                        isPizzaCooking = true;
                        currentTimer = cookingTime;
                        Obj.GetComponent<Interactable>().canInteract = false;
                        Invoke("AllowPickupPizza", cookingTime);
                        Obj.enabled = false;
                    }
                }
            }
        }
        if (isPizzaCooking)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= -burntTime && Obj.objectGrabPointTransform != null)
            {
                Obj.enabled = true;
                currentTimer = 0f;
                myPizza.isCooked = Cooked.OverCooked;
                myPizza.CookPizza();
                isPizzaCooking = false;
                myPizza = null;
                Obj = null;
            }
            else if (currentTimer <= 0 && Obj.objectGrabPointTransform != null)
            {
                Obj.enabled = true;
                myPizza.isCooked = Cooked.Cooked;
                myPizza.CookPizza();
                isPizzaCooking = false;
                myPizza = null;
                Obj = null;
            }
        }
    }
    public void AllowPickupPizza()
    {
        Obj.GetComponent<Interactable>().canInteract = true;
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents * 2);
    }
}

