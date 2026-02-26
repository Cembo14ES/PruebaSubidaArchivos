using UnityEngine;

namespace Huellas26.NPC
{
    /// <summary>
    /// Interfaz para desacoplar el sistema de interacción por voz.
    /// Útil para conectar con Whisper/OpenAI/Wit.ai.
    /// </summary>
    public interface IVoiceHandler
    {
        void StartListening();
        void StopListening();
        void ProcessSpeech(string spokenText);
    }
}
