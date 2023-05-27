using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SauceMachine : MonoBehaviour
{
    public Transform buttonPos;
    private bool isPressed;
    public Transform sauce;
    public int sauceCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Press(Transform button)
    {
        if (isPressed || sauceCount<=0)
        {
            return;
        }
        sauceCount--;
        sauce.DOScaleY(sauceCount * 0.2f, 1);
        Vector3 startpos = button.position;
        isPressed = true;
        button.transform.DOMove(buttonPos.position, 1).OnComplete(() => button.transform.DOMove(startpos, 1).OnComplete(() => isPressed = false));
    }
    public void AddSauce()
    {
        sauceCount++;
        sauce.DOScaleY(sauceCount*0.2f, 1);
    }
}
