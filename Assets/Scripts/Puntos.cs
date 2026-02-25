using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Puntos : MonoBehaviour
{
    public string targetTag = "Gallina";
    private int puntos = 0;
    public TextMeshProUGUI textoUI;
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Gallina"))
        {
            puntos++;
            textoUI.text= "Dentro de la zona: " + puntos;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gallina"))
        {
            puntos--;
            textoUI.text = "Dentro de la zona: " + puntos;
        }
    }
}