using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform cam;
    [SerializeField] private Transform objectGrabPointTransform;
    public Collider player;
    public Collider other;

    [SerializeField] private float grabPointSpeed;
    [SerializeField] private float interactDistance;
    [SerializeField] private float holdDistance;
    [SerializeField] private float throwForce;
    
    [SerializeField] private LayerMask interactables;
    [SerializeField] private Interactable Interactable;
    [SerializeField] private LayerMask grabables;
    [SerializeField] public ObjectGrabbable grabbedObject;
    [SerializeField] private LayerMask solidLayerMask;

    private Vector3 defaultGrabPoint;
    float mouseScrollY;
    Ray ray;
    RaycastHit hit;


    UnityEvent onInteract;
    UnityEvent onDisInteract;

    //Inputs
    #region 
    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        mouseScrollY = context.ReadValue<Vector2>().y;
    }
    
    public void OnLMB(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //dropping with lmb
            if(grabbedObject!=null)
            {
                Physics.IgnoreCollision(player, grabbedObject.GetComponent<Collider>(), false);
                grabbedObject.Drop();
                grabbedObject = null;
                //playerlayer.whatIsGround |= (1 << 3);
            }
            else if (Interactable!=null)
            {
                if (!Interactable.canInteract)
                {
                    return;
                }
                //if hits grabable object
                if (Physics.Raycast(ray, out hit, interactDistance, grabables))
                {
                    hit.transform.TryGetComponent(out grabbedObject);
                    Physics.IgnoreCollision(player,grabbedObject.GetComponent<Collider>(), true);
                    objectGrabPointTransform.position = defaultGrabPoint;
                    grabbedObject.Grab(objectGrabPointTransform,this);
                    //playerlayer.whatIsGround &= ~(1 << 3);
                }

            }
            
        }
    }
    public void OnRMB(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //throwing with rmb
            if (grabbedObject != null)
            {
                Physics.IgnoreCollision(player, grabbedObject.GetComponent<Collider>(), false);
                grabbedObject.ThrowObject(cam,throwForce);
                grabbedObject.Drop();
                grabbedObject = null;
                //playerlayer.whatIsGround |= (1 << 3);
            }
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //if hits interacable
            if (Interactable!=null)
            {
                
                //if players hand is empty
                onInteract = Interactable.onInteract;
                onInteract.Invoke();
            }
            
        }
    }
    #endregion
    public void GrabIngredient(GameObject Ingredient)
    {
        if (grabbedObject==null)
        {
            if (Interactable.GetComponent<IngredientTins>().ingredientCount > 0)
            {
                //GameObject ingredient = Instantiate(Ingredient, objectGrabPointTransform.position, Quaternion.Euler(Vector3.zero), null);
                GameObject ingredient = Interactable.GetComponent<IngredientTins>().ingredientPoll.Get();
                ingredient.transform.position = objectGrabPointTransform.position;
                ingredient.TryGetComponent(out grabbedObject);
                //Physics.IgnoreCollision(hit.collider.GetComponent<Collider>(), grabbedObject.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(player, grabbedObject.GetComponent<Collider>(), true);
                objectGrabPointTransform.position = defaultGrabPoint;
                grabbedObject.Grab(objectGrabPointTransform,this);
            }
        }
    }
    public void GrabBox()
    {
        if (grabbedObject == null)
        {
            if (Interactable.GetComponent<BoxHolder>().boxCount > 0)
            {
                GameObject box = Interactable.GetComponent<BoxHolder>().boxPoll.Get();
                box.GetComponent<Outline>().enabled = true;
                box.transform.position = objectGrabPointTransform.position;
                box.TryGetComponent(out grabbedObject);
                Physics.IgnoreCollision(player, grabbedObject.GetComponent<Collider>(), true);
                objectGrabPointTransform.position = defaultGrabPoint;
                grabbedObject.Grab(objectGrabPointTransform, this);
            }
        }
    }
    public void GrabIngredient(ShopBasket basket)
    {
        if (grabbedObject == null)
        {
            if (basket.ItemsInBasket.Count != 0)
            {
                GameObject ingredient = Instantiate(basket.ItemsInBasket[0], objectGrabPointTransform.position, Quaternion.Euler(Vector3.zero), null);
                //GameObject ingredient = Interactable.GetComponent<IngredientTins>().ingredientPoll.Get();
                ingredient.transform.position = objectGrabPointTransform.position;
                ingredient.TryGetComponent(out grabbedObject);
                Physics.IgnoreCollision(hit.collider.GetComponent<Collider>(), grabbedObject.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(player, grabbedObject.GetComponent<Collider>(), true);
                objectGrabPointTransform.position = defaultGrabPoint;
                grabbedObject.Grab(objectGrabPointTransform, this);
                basket.ItemsInBasket.RemoveAt(0);
            }
        }
    }
    void Update()
    {
        AdjuctGrabPoint();
        CheckForInteractables();

        defaultGrabPoint = cam.position + cam.forward * holdDistance;
    }
    public void AdjuctGrabPoint()
    {
        //adjusting the distance wiht mouse wheel
        if (grabbedObject != null)
        {
            if (mouseScrollY > 0f && Vector3.Distance(objectGrabPointTransform.position, cam.position) < interactDistance) // forward
            {
                objectGrabPointTransform.position += cam.forward * Time.deltaTime * grabPointSpeed*0.25f;
                if (Vector3.Distance(objectGrabPointTransform.position, cam.position) > interactDistance)
                {
                    objectGrabPointTransform.position = cam.position + cam.forward * interactDistance;
                }

            }
            else if (mouseScrollY < 0f && Vector3.Distance(objectGrabPointTransform.position, cam.position) > holdDistance) // backwards
            {
                objectGrabPointTransform.position -= cam.forward * Time.deltaTime * grabPointSpeed * 0.25f;
                if (Vector3.Distance(objectGrabPointTransform.position, cam.position) < holdDistance)
                {
                    objectGrabPointTransform.position = cam.position + cam.forward * holdDistance;
                }
            }
            //adjusting the point while near walls
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit raycastHit, Vector3.Distance(cam.position, objectGrabPointTransform.position), solidLayerMask))
            {
                grabbedObject.snapped = true;
                objectGrabPointTransform.position = raycastHit.point + (cam.forward * -0.1f);

            }
            else if (grabbedObject.snapped)
            {
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit raycastHit1, holdDistance, solidLayerMask))
                {
                    objectGrabPointTransform.position = raycastHit1.point + (cam.forward * -0.1f);
                }
                else
                {
                    grabbedObject.snapped = false;
                    objectGrabPointTransform.position = defaultGrabPoint;
                }
            }
        }
    }
    void CheckForInteractables()
    {
        if (grabbedObject != null)
        {
            Interactable pastInteractable = Interactable.GetComponent<Interactable>();
            Interactable = grabbedObject.GetComponent<Interactable>();
            if (pastInteractable!= Interactable)
            {
                pastInteractable.transform.GetComponent<Outline>().OutlineWidth = 0f;
            }
            Interactable.transform.GetComponent<Outline>().OutlineWidth = 0f;
        }
        else
        {
            ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out hit, interactDistance, interactables))
            {
                Interactable pastInteractable;
                //There is Interactable object
                if (Interactable != null)
                {
                    //check if 2 object is not highlighted at the same time
                    pastInteractable = Interactable;
                    hit.transform.TryGetComponent(out Interactable);
                    if (pastInteractable != Interactable)
                    {
                        pastInteractable.transform.GetComponent<Outline>().OutlineWidth = 0f;
                        if (!Interactable.canInteract)
                        {
                            return;
                        }
                        Interactable.transform.GetComponent<Outline>().OutlineWidth = 5f;
                    }
                    return;
                }
                hit.transform.TryGetComponent(out Interactable);
                //Highlight it
                if (!Interactable.canInteract)
                {
                    return;
                }
                Interactable.transform.GetComponent<Outline>().OutlineWidth = 5f;
                //UI

            }
            else if(Interactable != null)
            {
                Interactable.transform.GetComponent<Outline>().OutlineWidth = 0f;
                Interactable = null;
            }
        }
        
    }
    private void OnDrawGizmos()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(objectGrabPointTransform.position, 0.05f);
    }

}
