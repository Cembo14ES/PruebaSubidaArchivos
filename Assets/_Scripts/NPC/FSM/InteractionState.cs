using UnityEngine;
using Huellas26.NPC.FSM;

namespace Huellas26.NPC.FSM.States
{
    /// <summary>
    /// Estado de interacción con el jugador (Hablar, Responder).
    /// Conecta con sistemas de diálogo.
    /// </summary>
    public class InteractionState : NPCState
    {
        private Transform _playerTransform;
        
        // Timer simple para salir si el jugador se aleja mucho tiempo
        private float _timeSinceLastInteraction;

        public InteractionState(NPC_Controller npc) : base(npc)
        {
            if (Camera.main != null)
                _playerTransform = Camera.main.transform;
        }

        public override void Enter()
        {
            _agent.isStopped = true;
            _animator.SetTrigger("Talk");
            _timeSinceLastInteraction = 0.0f;
            
            Debug.Log("[NPC] Entrando en Interacción.");
        }

        public override void Update()
        {
            _timeSinceLastInteraction += Time.deltaTime;

            if (_playerTransform != null)
            {
                // Rotar para mirar al jugador
                Vector3 direction = (_playerTransform.position - _npc.transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    _npc.transform.rotation = Quaternion.Slerp(_npc.transform.rotation, lookRotation, Time.deltaTime * 5.0f);
                }

                // Verificar distancia para romper interacción
                float distance = Vector3.Distance(_npc.transform.position, _playerTransform.position);
                if (distance > 5.0f) // Radio maximo de conversacion
                {
                    _npc.StopInteraction(); // Método helper para volver a Idle
                }
            }
        }

        public override void Exit()
        {
            _animator.SetBool("Explaining", false);
            Debug.Log("[NPC] Saliendo de Interacción.");
        }

        public override void OnInteract()
        {
            // Reiniciar timer
            _timeSinceLastInteraction = 0.0f;
            // Disparar respuesta de voz
        }
    }
}
