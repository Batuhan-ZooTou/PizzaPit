using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameShopController : MonoBehaviour
{
    public GameObject player;
    public GameObject playerCam;
    public GameObject crosshair;
    public GameObject shopCam;
    public GameObject shopCanvas;
    public GameObject openShopButton;
    public bool isShopOpen = false;

    private void Start()
    {
        shopCam.SetActive(false);
        shopCanvas.SetActive(false);
    }
    public void OpenShop()
    {
        isShopOpen = true;
        crosshair.SetActive(false);
        player.SetActive(false);
        playerCam.SetActive(false);
        openShopButton.SetActive(false);
        shopCanvas.SetActive(true);
        shopCam.SetActive(true);
        

    }
    public void CloseShop()
    {
        isShopOpen = false;
        shopCanvas.SetActive(false);
        shopCam.SetActive(false);
        openShopButton.SetActive(true);
        player.SetActive(true);
        playerCam.SetActive(true);
        crosshair.SetActive(true);
        
        
        
    }
}
