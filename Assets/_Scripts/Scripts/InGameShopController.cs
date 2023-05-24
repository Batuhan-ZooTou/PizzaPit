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
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void OpenShop()
    {
        isShopOpen = !isShopOpen;
        if (isShopOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            crosshair.SetActive(false);
            //player.SetActive(false);
            playerCam.SetActive(false);
            openShopButton.SetActive(false);
            shopCanvas.SetActive(true);
            shopCam.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            isShopOpen = false;
            shopCanvas.SetActive(false);
            shopCam.SetActive(false);
            openShopButton.SetActive(true);
            //player.SetActive(true);
            playerCam.SetActive(true);
            crosshair.SetActive(true);
        }
    }
    public void CloseShop()
    {
        isShopOpen = false;
        shopCanvas.SetActive(false);
        shopCam.SetActive(false);
        openShopButton.SetActive(true);
        //player.SetActive(true);
        playerCam.SetActive(true);
        crosshair.SetActive(true);
    }
}
