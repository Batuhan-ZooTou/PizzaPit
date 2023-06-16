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
            isCanvasVisible = !isCanvasVisible; // Canvas'in g�r�n�rl���n� tersine �evir

            canvas.SetActive(isCanvasVisible); // Canvas'in g�r�n�rl���n� ayarla
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