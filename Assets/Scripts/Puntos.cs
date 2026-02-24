using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Puntos : MonoBehaviour
{
    public string targetTag = "Gallina";
    private int puntos = 0;
    public TextMeshProUGUI textoUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Gallina")
        {
            puntos++;
            textoUI.text= "Dentro de la zona: " + puntos;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Gallina")
        {
            puntos--;
            textoUI.text = "Dentro de la zona: " + puntos;
        }
    }
}