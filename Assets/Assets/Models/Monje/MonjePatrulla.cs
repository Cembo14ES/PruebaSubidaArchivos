using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

public class Monje : MonoBehaviour
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

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        hablado = false;
    }

    void Update()
    {
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

        if (estaDentro && Keyboard.current.eKey.wasPressedThisFrame && !altavoz.isPlaying && !hablado)
        {
            texto.SetActive(false);
            ReproducirSiguiente();
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

        if (hablado)
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

        if (pista >= misAudios.Length)
        {
            StartCoroutine(FinalizarConversacion(altavoz.clip.length));
        }
    }

    IEnumerator FinalizarConversacion(float delay)
    {
        yield return new WaitForSeconds(delay);
        hablado = true;
        texto.SetActive(false);
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
}