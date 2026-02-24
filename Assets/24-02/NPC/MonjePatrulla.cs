using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
public class MonjePatrulla : MonoBehaviour
{
    public Transform[] puntos;
    private int indiceActual = 0;
    private NavMeshAgent agente;
    public Animator anim;
    private bool hablado;
    public AudioSource altavoz;
    public AudioClip[] misAudios;
    private int pista = 0;
    private bool estaDentro = false;
    public GameObject texto;
    public GameObject jugador;
    private bool acabado;
    public TextMeshProUGUI textoTMP;
    private bool gatillo;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        hablado = false;
    }

    void Update()
    {
        gatillo = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool t) && t;
        if (!hablado && !estaDentro)
        {
            anim.SetBool("Saludo", true);
            texto.SetActive(false);
            MirarAlJugador();
        }
        else
        {
            anim.SetBool("Saludo", false);
        }

        if (estaDentro && gatillo && !altavoz.isPlaying && !hablado)
        {
            texto.SetActive(false);
            ReproducirSiguiente();
        }
        if (hablado && gatillo)
        {
            Irse();
        }

        if (altavoz.isPlaying)
        {
            anim.SetBool("Hablado", true);
            agente.isStopped = true;
            MirarAlJugador();
        }
        else
        {
            anim.SetBool("Hablado", false);
            if (estaDentro && !hablado && !altavoz.isPlaying)
            {
                texto.SetActive(true);
                MirarAlJugador();
            }
        }

        if (hablado && acabado)
        {
            agente.isStopped = false;
            if (!agente.hasPath || agente.remainingDistance < 0.5f)
            {
                if (!agente.pathPending)
                {
                    agente.destination = puntos[indiceActual].position;
                    indiceActual = (indiceActual + 1) % puntos.Length;
                }
            }
            anim.SetBool("Andando", agente.velocity.magnitude > 0.1f);
        }
        else
        {
            agente.isStopped = true;
            anim.SetBool("Andando", false);
        }
    }

    void ReproducirSiguiente()
    {
        if (pista >= misAudios.Length || altavoz == null || hablado) return;

        altavoz.clip = misAudios[pista];
        altavoz.Play();
        pista++;
        textoTMP.text = "Pulsa Para Continuar";

        if (pista >= misAudios.Length)
        {
            textoTMP.text = "Pulsa Para Finalizar";
            StartCoroutine(FinalizarConversacion(altavoz.clip.length));
        }
    }

    IEnumerator FinalizarConversacion(float delay)
    {
        yield return new WaitForSeconds(delay);
        hablado = true;
        texto.SetActive(true);
    }

    void MirarAlJugador()
    {
        if (jugador != null)
        {
            Vector3 direccion = jugador.transform.position - transform.position;
            direccion.y = 0;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacion = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * 5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Persona"))
        {
            estaDentro = true;
            if (!hablado && !altavoz.isPlaying)
            {
                texto.SetActive(true);
            }
            else if (!acabado)
            {
                texto.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Persona"))
        {
            estaDentro = false;
            texto.SetActive(false);
        }
    }
    private void Irse()
    {
        if (hablado)
        {
            texto.SetActive(false);
            acabado = true;
        }
    }
}