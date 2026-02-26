using UnityEngine;
using UnityEngine.Events;

namespace Huellas26.Core.Events
{
    /// <summary>
    /// Sistema centralizado de eventos para desacoplar componentes.
    /// Implementa el patrón Observer siguiendo SOLID (Open/Closed Principle).
    /// Referencia: Clean Architecture + Unity Best Practices
    /// </summary>
    public static class GameEvents
    {
        // ====== NPC Events ======
        public static UnityEvent<string, Vector3> OnNPCInteracted = new UnityEvent<string, Vector3>();
        public static UnityEvent<string> OnNPCStateChanged = new UnityEvent<string>();
        public static UnityEvent<string, string> OnNPCSpeech = new UnityEvent<string, string>(); // NPC Name, Speech Text

        // ====== Minigame Events ======
        public static UnityEvent OnMinigameStarted = new UnityEvent();
        public static UnityEvent<bool> OnMinigameEnded = new UnityEvent<bool>(); // Won?
        public static UnityEvent<int> OnScoreChanged = new UnityEvent<int>();
        public static UnityEvent<int> OnTargetHit = new UnityEvent<int>(); // Points

        // ====== Audio Events ======
        public static UnityEvent<AudioClip, Vector3> OnSpatialAudioRequested = new UnityEvent<AudioClip, Vector3>();
        public static UnityEvent<AudioClip> OnGlobalAudioRequested = new UnityEvent<AudioClip>();

        // ====== VR Comfort Events (para futuros ajustes de confort) ======
        public static UnityEvent<bool> OnVignetteToggled = new UnityEvent<bool>();
        public static UnityEvent<float> OnSnapTurnRequested = new UnityEvent<float>(); // Degrees

        /// <summary>
        /// Limpia todos los listeners (útil al cambiar de escena).
        /// </summary>
        public static void UnsubscribeAll()
        {
            OnNPCInteracted.RemoveAllListeners();
            OnNPCStateChanged.RemoveAllListeners();
            OnNPCSpeech.RemoveAllListeners();
            OnMinigameStarted.RemoveAllListeners();
            OnMinigameEnded.RemoveAllListeners();
            OnScoreChanged.RemoveAllListeners();
            OnTargetHit.RemoveAllListeners();
            OnSpatialAudioRequested.RemoveAllListeners();
            OnGlobalAudioRequested.RemoveAllListeners();
            OnVignetteToggled.RemoveAllListeners();
            OnSnapTurnRequested.RemoveAllListeners();
        }
    }
}
