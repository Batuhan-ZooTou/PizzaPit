using UnityEngine;
using UnityEngine.UI;

public class MouseOverInfo : MonoBehaviour
{
    Shop shop;
    
    public Text infoText;



    void Start()
    {
        shop = FindObjectOfType<Shop>();
        infoText = this.transform.parent.parent.parent.parent.parent.parent.GetChild(1).GetComponent<Text>();
        
    }

    void OnMouseEnter()
    {
        infoText.text=shop.Item.transform.GetChild(3).GetComponent<Text>().text;
        infoText.enabled = true;
        Debug.Log("info");
    }

    void OnMouseExit()
    {
        infoText.enabled = false;
        Debug.Log("info gitti");
    }
}
