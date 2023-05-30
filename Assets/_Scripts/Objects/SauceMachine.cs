using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SauceMachine : MonoBehaviour
{
    public Transform buttonPos;
    public Transform SauceMeter;
    public Transform SauceBallPoint;
    public Transform PizzaPoint;
    public GameObject saucePrefab;
    private bool isPressed;
    public int maxSauceCount;
    [SerializeField] private int sauceCount;

    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    [SerializeField] Vector3 colliderExtents1;
    [SerializeField] float maxDisstance1;
    public LayerMask doughLayer;
    Pizza myPizza;
    ObjectGrabbable Sauce;

    // Start is called before the first frame update
    void Start()
    {
        SauceMeter.DOScaleY((float)sauceCount / maxSauceCount, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, doughLayer))
        {
            if (!isPressed && myPizza == null)
            {
                if (hit.collider.TryGetComponent(out myPizza))
                {
                    if (myPizza.isSauced)
                    {
                        myPizza = null;
                        return;
                    }
                    ObjectGrabbable Obj = myPizza.GetComponent<ObjectGrabbable>();
                    Obj.Drop();
                    Obj.objectRigidbody.velocity = Vector3.zero;
                    myPizza.gameObject.transform.position = PizzaPoint.position;
                }
            }
        }
        else
        {
            myPizza = null;
        }
        if (sauceCount<maxSauceCount)
        {
            if (Physics.BoxCast(transform.position, colliderExtents1, Vector3.forward, out RaycastHit hits, Quaternion.Euler(Vector3.zero), maxDisstance1, doughLayer))
            {
                if (hits.collider.TryGetComponent(out Sauce))
                {
                    if (Sauce.GetComponent<Pizza>()!=null || sauceCount == maxSauceCount || Sauce.ItemSO.type!=BaseIngredient.sauce)
                    {
                        return;
                    }
                    Sauce.Drop();
                    AddSauce();
                    Sauce.gameObject.SetActive(false);
                }
            }
        }
        
    }
    public void Press(Transform button)
    {
        if (isPressed || sauceCount<=0 || myPizza==null)
        {
            return;
        }
        Instantiate(saucePrefab, SauceBallPoint.position,SauceBallPoint.rotation, null);
        sauceCount--;
        SauceMeter.DOScaleY((float)sauceCount / maxSauceCount, 1);
        Vector3 startpos = button.position;
        isPressed = true;
        button.transform.DOMove(buttonPos.position, 1).OnComplete(() => button.transform.DOMove(startpos, 1).OnComplete(() => ClearPizza()));
    }
    void ClearPizza()
    {
        isPressed = false;
        myPizza = null;
    }
    public void AddSauce()
    {
        sauceCount++;
        SauceMeter.DOScaleY((float)sauceCount / maxSauceCount, 1);
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents * 2);
        Gizmos.DrawCube(transform.position + Vector3.forward * maxDisstance1, colliderExtents1 * 2);
    }
}
