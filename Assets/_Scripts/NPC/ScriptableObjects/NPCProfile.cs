using UnityEngine;

namespace Huellas26.NPC
{
    /// <summary>
    /// Configuración serializable para definir la 'Personalidad' y atributos
    /// de un NPC sin ensuciar los scripts de código.
    /// Best Practice: ScriptableObjects para datos estáticos.
    /// </summary>
    public enum NPCType { Human, Animal }

    [CreateAssetMenu(fileName = "New NPC Profile", menuName = "Huellas26/NPC Profile")]
    public class NPCProfile : ScriptableObject
    {
        [Header("Identidad")]
        public string npcName = "Hermano Iratxe";
        public NPCType type = NPCType.Human;

        [TextArea(3, 10)]
        public string systemPrompt = "Eres un monje cisterciense del siglo XII...";
        
        [Header("Voz e Interaccion")]
        [Tooltip("Sonido característico (ej: Mugido para buey, saludo para humano).")]
        public AudioClip interactionSound;
        [Tooltip("Velocidad al hablar (ej: 0.9 para hablar pausado).")]
        [Range(0.5f, 2.0f)]
        public float speechRate = 0.9f;
        
        [Tooltip("Pitch de la voz (ej: 0.8 para voz grave).")]
        [Range(0.5f, 2.0f)]
        public float pitch = 0.85f;
        
        [Header("Comportamiento")]
        public float walkSpeed = 2.0f;
        public float detectionRadius = 5.0f;
    }
}
