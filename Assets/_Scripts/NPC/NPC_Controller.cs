using UnityEngine;
using UnityEngine.AI;
using Huellas26.Core.Events;
using Huellas26.NPC.FSM;
using Huellas26.NPC.FSM.States;

namespace Huellas26.NPC
{
    /// <summary>
    /// Controlador principal del NPC optimizado para VR.
    /// Implementa FSM (Finite State Machine) para comportamiento modular.
    /// Performance: Compone ntes cacheados, eventos desacoplados, sin referencias directas.
    /// Compliance: VR Best Practices (1 unit = 1m, optimizado para 90 FPS).
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class NPC_Controller : MonoBehaviour, IVoiceHandler
    {
        [Header("Configuracion Principal")]
        [Tooltip("Perfil de datos (Personalidad, Stats) del NPC.")]
        [SerializeField] private NPCProfile profile;

        [Header("Configuracion Fisica")]
        [Tooltip("Altura adicional para evitar que el NavMesh 'flote' o se hunda.")]
        [SerializeField] private float groundOffset = 0.05f;
        
        [Tooltip("Velocidad de rotacion para encarar al jugador.")]
        [SerializeField] private float turnSpeed = 5.0f;

        // FSM
        private NPCSState _currentState;

        // Componentes Publicos para los Estados
        public NPCSState CurrentState => _currentState;
        
        // Componentes cacheados (Performance: evitar GetComponent en Update)
        private NavMeshAgent _agent;
        private Animator _animator;
        private AudioSource _audioSource;
        
        // Propiedad publica para acceder al estado
        public bool IsMoving => _agent.velocity.sqrMagnitude > 0.1f && !(_currentState is InteractionState);

        protected void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();

            // Validar Perfil
            if (profile == null)
            {
                Debug.LogError($"[NPC] {gameObject.name} no tiene asignado un NPCProfile! Usando valores por defecto.");
            }
            else
            {
                // Aplicar configuracion del ScriptableObject
                ApplyProfileSettings();
            }
        }
        
        private void ApplyProfileSettings()
        {
            if (profile == null) return;
            
            _agent.speed = profile.walkSpeed;
            if (_audioSource != null)
            {
                _audioSource.pitch = profile.pitch;
            }
            gameObject.name = $"NPC_{profile.npcName}"; // Renombrar en jerarquia para claridad
        }

        protected void Start()
        {
            // Estado Inicial: Idle
            ChangeState(new IdleState(this));
            
            // Emitir evento de inicialización
            if (profile != null)
            {
                GameEvents.OnNPCStateChanged?.Invoke($"{profile.npcName} initialized");
            }
        }

        protected void Update()
        {
            // Delegar al estado actual (FSM Pattern)
            _currentState?.Update();

            // Performance: Agrupamos actualizaciones para reducir overhead
            UpdateAnimation();
            ApplyGroundFix();
        }

        /// <summary>
        /// Cambia el estado actual de la FSM.
        /// </summary>
        public void ChangeState(NPCSState newState)
        {
            if (_currentState != null) _currentState.Exit();
            
            _currentState = newState;
            
            if (_currentState != null) _currentState.Enter();
        }

        public void StopInteraction()
        {
             ChangeState(new IdleState(this));
        }

        public void MoveTo(Vector3 targetPosition)
        {
            if (_agent.isOnNavMesh)
            {
                // ToDo: Implementar MoveToState
                // ChangeState(new MoveToState(this, targetPosition));
                _agent.SetDestination(targetPosition);
            }
        }

        /// <summary>
        /// Maneja la interacción con el NPC (VR click, voz, proximidad).
        /// </summary>
        public void Interact()
        {
            if (_currentState != null) _currentState.OnInteract();
            
            // Audio espacial usando el sistema centralizado (mejor para VR)
            if (profile != null && profile.interactionSound != null)
            {
                GameEvents.OnSpatialAudioRequested?.Invoke(profile.interactionSound, transform.position);
            }
            
            // Emitir evento para analytics o UI
            GameEvents.OnNPCInteracted?.Invoke(profile?.npcName ?? gameObject.name, transform.position);
        }

        /// <summary>
        /// Activa la animación de celebración (ganó minijuego).
        /// </summary>
        public void Celebrate()
        {
            _animator.SetTrigger("Celebrate");
        }

        /// <summary>
        /// Inicia modo explicación/tutorial.
        /// </summary>
        public void StartExplanation()
        {
            ChangeState(new InteractionState(this));
            _animator.SetBool("Explaining", true);
        }

        /// <summary>
        /// Detiene la explicación y vuelve a Idle>
        /// </summary>
        public void StopExplanation()
        {
            _animator.SetBool("Explaining", false);
            StopInteraction();
        }


        /// <summary>
        /// Fix para evitar la 'levitacion' común en NavMeshAgents.
        /// Realiza un Raycast hacia abajo y ajusta la posición visual si es necesario.
        /// </summary>
        private void ApplyGroundFix()
        {
            if (!_agent.enabled) return;

            // Lógica simple: Si NavMesh dice que estamos en Y=1.0 pero el suelo esta en Y=0.9, NavMesh gana. 
            // A veces NavMesh flota. Podemos usar Raycast para ajustar el modelo visual (hijo) 
            // o ajustar el baseOffset del agente.
            
            RaycastHit hit;
            // Lanzamos rayo desde un poco arriba del centro hacia abajo
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2.0f, LayerMask.GetMask("Ground", "Default")))
            {
                // Si la distancia al suelo real es diferente a la del agente, ajustamos baseOffset
                float distanceToGround = hit.distance - 0.5f; // Restamos la altura inicial del rayo
                
                // Si hay mucha diferencia, ajustamos la altura del agente base
                if (Mathf.Abs(distanceToGround) > 0.01f)
                {
                    _agent.baseOffset = -distanceToGround + groundOffset;
                }
            }
        }
        
        public void StartListening() { Debug.Log("NPC Listening..."); }
        public void StopListening() { Debug.Log("NPC Stopped Listening."); }
        public void ProcessSpeech(string spokenText) 
        {
             Debug.Log($"NPC heard: {spokenText}");
             // Aqui conectaríamos con el LLM
             Interact();
        }

        private void UpdateAnimation()
        {
            // Sincronizar velocidad del agente con el Animator Blend Tree
            float speed = _agent.velocity.magnitude / _agent.speed;
            _animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        }
    }
}
