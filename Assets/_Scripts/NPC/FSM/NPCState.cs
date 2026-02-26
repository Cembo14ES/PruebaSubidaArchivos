using UnityEngine;
using UnityEngine.AI;

namespace Huellas26.NPC.FSM
{
    /// <summary>
    /// Clase base abstracta para los estados del NPC.
    /// Define el contrato que heredarán los estados como Idle, Patrol, Talking.
    /// </summary>
    public abstract class NPCSState
    {
        protected NPC_Controller _npc;
        protected NavMeshAgent _agent;
        protected Animator _animator;

        public NPCSState(NPC_Controller npc)
        {
            _npc = npc;
            _agent = npc.GetComponent<NavMeshAgent>();
            _animator = npc.GetComponent<Animator>();
        }

        /// <summary>
        /// Se ejecuta al entrar al estado.
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// Lógica de actualización frame por frame (Movimiento, Rotación).
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Se ejecuta al salir del estado.
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// Maneja la interacción del jugador (click, voz, proximidad).
        /// </summary>
        public virtual void OnInteract() { }
    }
}
