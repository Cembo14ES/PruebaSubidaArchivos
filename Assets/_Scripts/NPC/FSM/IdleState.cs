using UnityEngine;
using Huellas26.NPC.FSM;

namespace Huellas26.NPC.FSM.States
{
    /// <summary>
    /// Estado en reposo del NPC.
    /// Puede hacer micro-animaciones o esperar al jugador.
    /// </summary>
    public class IdleState : NPCState
    {
        private float _timeInState;
        private float _timeToWait;

        public IdleState(NPC_Controller npc) : base(npc) { }

        public override void Enter()
        {
            _agent.isStopped = true;
            _animator.SetFloat("Speed", 0.0f);
            
            // Randomizar tiempo de espera
            _timeToWait = Random.Range(2.0f, 5.0f);
            _timeInState = 0.0f;
            
            Debug.Log("[NPC] Entrando en Idle.");
        }

        public override void Update()
        {
            _timeInState += Time.deltaTime;
            
            // Lógica de transición basica
            if (_timeInState >= _timeToWait)
            {
                // Decisión aleatoria: Ir a Patrol o Seguir en Idle
                // _npc.ChangeState(new PatrolState(_npc)); // Implementaremos Patrol luego
            }
        }

        public override void Exit()
        {
            Debug.Log("[NPC] Saliendo de Idle.");
        }

        public override void OnInteract()
        {
            // Transición a estado de Conversación/Interacción
            _npc.ChangeState(new InteractionState(_npc));
        }
    }
}
