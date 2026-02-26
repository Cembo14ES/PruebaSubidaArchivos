using UnityEngine;

namespace Huellas26.Tests
{
    using Huellas26.NPC;

    /// <summary>
    /// Script de prueba para validar los Estados y Animaciones del NPC
    /// sin necesidad de entrar a VR.
    /// Controles:
    /// [1] -> Forzar Idle (Parar Interaccion)
    /// [2] -> Iniciar Interaccion (Talk)
    /// [3] -> Explicacion Tutorial
    /// [4] -> Celebrar (Ganar Minijuego)
    /// [5] -> Simular Voz ("Hola Monje")
    /// </summary>
    public class TestNPC : MonoBehaviour
    {
        [Tooltip("Arrastra aqui tu NPC Controller")]
        public NPC_Controller targetNPC;

        private void Update()
        {
            if (targetNPC == null) return;

            // 1. Volver a la Calma
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("[Test] Forzando IDLE");
                targetNPC.StopInteraction();
            }

            // 2. Interactuar Normal
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("[Test] Forzando INTERCAMBIO");
                targetNPC.Interact();
            }

            // 3. Modo Tutorial (Explicar Minijuego)
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("[Test] Forzando EXPLICACION");
                targetNPC.StartExplanation();
            }

            // 4. Celebracion
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("[Test] Forzando CELEBRACION");
                targetNPC.Celebrate();
            }

            // 5. Simular Voz (IVoiceHandler)
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("[Test] Simulando Voz: 'Hola padre'");
                targetNPC.ProcessSpeech("Hola padre, ¿que tal el vino?");
            }
        }
    }
}
