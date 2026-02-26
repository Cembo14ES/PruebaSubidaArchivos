using UnityEngine;
using Huellas26.Core.Events;

namespace Huellas26.Minigames
{
    /// <summary>
    /// Script para las 'latas' o jarras del minijuego.
    /// Detecta colisiones fuertes y emite evento de puntuación.
    /// Arquitectura desacoplada: No conoce al GameManager directamente.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class CanTarget : MonoBehaviour
    {
        [Header("Configuracion")]
        [Tooltip("Puntos que otorga esta lata al caer.")]
        public int pointsValue = 10;
        
        [Tooltip("Fuerza minima del impacto para contar como derribo.")]
        public float minImpactForce = 2.0f;
        
        [Tooltip("Si es true, la lata se destruye despues de ser golpeada.")]
        public bool destroyOnHit = false;

        [Header("Audio")]
        public AudioClip hitSound;

        [Header("Efectos Visuales (Opcional)")]
        [Tooltip("Partículas al ser derribada.")]
        public ParticleSystem hitParticles;

        private bool _hasScored = false;
        private Rigidbody _rb;
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
            // Validaciones
            if (_rb == null)
            {
                Debug.LogError($"[CanTarget] {gameObject.name} requires a Rigidbody!");
            }
        }

        private void Start()
        {
            // Guardar posicion inicial para resetear (opcional si quisieramos reiniciar)
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasScored) return; // Ya puntuó
            
            // Verificar fuerza del impacto
            float impactMagnitude = collision.relativeVelocity.magnitude;
            if (impactMagnitude > minImpactForce)
            {
                Score();
            }
        }
        
        /// <summary>
        /// Metodo para probar desde el editor o eventos
        /// </summary>
        public void ForceScore()
        {
             Score();
        }

        private void Score()
        {
            if (_hasScored) return;
            
            _hasScored = true;
            
            // Emitir evento (desacoplado del Manager)
            GameEvents.OnTargetHit?.Invoke(pointsValue);
            
            // Efectos de feedback
            PlayFeedback();
            
            // Destruir o desactivar
            if (destroyOnHit)
            {
                Destroy(gameObject, 0.5f); // Delay para que se vea/oiga
            }
        }

        private void PlayFeedback()
        {
            // Audio espacial
            if (hitSound != null)
            {
                GameEvents.OnSpatialAudioRequested?.Invoke(hitSound, transform.position);
            }

            // Partículas
            if (hitParticles != null)
            {
                hitParticles.Play();
            }
        }
        
        /// <summary>
        /// Reinicia la lata a su posición original (para nuevo round).
        /// </summary>
        public void ResetTarget()
        {
            _hasScored = false;
            if (_rb != null)
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            transform.position = _startPosition;
            transform.rotation = _startRotation;
            gameObject.SetActive(true);
        }
    }
}
