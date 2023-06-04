using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Doors : MonoBehaviour
{
    bool isOpen;
    public Vector3 rotation;
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Open()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            transform.DOBlendableLocalRotateBy(rotation, 1f);

        }
        else
        {
            transform.DOBlendableLocalRotateBy(-rotation, 1f);

        }
    }
}
