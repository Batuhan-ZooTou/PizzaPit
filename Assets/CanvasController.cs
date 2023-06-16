using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject canvasTarif;

    private bool isCanvasTarifVisible = false;
    private bool isCanvasVisible = false; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isCanvasVisible = !isCanvasVisible; // Canvas'in görünürlüðünü tersine çevir

            canvas.SetActive(isCanvasVisible); // Canvas'in görünürlüðünü ayarla
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isCanvasTarifVisible = !isCanvasTarifVisible;

            canvasTarif.SetActive(isCanvasTarifVisible); 
        }
    }

    public void CloseTarifCanvas()
    {
        isCanvasTarifVisible = !isCanvasTarifVisible;

        canvasTarif.SetActive(isCanvasTarifVisible);
    }
}