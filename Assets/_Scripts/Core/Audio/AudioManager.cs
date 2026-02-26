using UnityEngine;
using System.Collections.Generic;

namespace Huellas26.Core.Audio
{
    /// <summary>
    /// Gestor centralizado de audio con soporte de audio espacial (3D) para VR.
    /// Implementa Singleton pattern y Object Pooling para evitar allocations en runtime.
    /// Best Practice: Evita llamadas a GetComponent repetidas y centraliza configuración de audio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Configuración VR Spatial Audio")]
        [Tooltip("Distancia mínima para audio espacial (recomendado: 1m)")]
        [SerializeField] private float minDistance = 1.0f;
        
        [Tooltip("Distancia máxima para audio espacial (recomendado: 15m en VR)")]
        [SerializeField] private float maxDistance = 15.0f;

        [Header("Audio Pool")]
        [SerializeField] private int poolSize = 10;
        
        private Queue<AudioSource> _audioSourcePool;
        private List<AudioSource> _activeAudioSources;

        private void Awake()
        {
            // Singleton Pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePool();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void InitializePool()
        {
            _audioSourcePool = new Queue<AudioSource>(poolSize);
            _activeAudioSources = new List<AudioSource>(poolSize);

            for (int i = 0; i < poolSize; i++)
            {
                CreateNewAudioSource();
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject audioObj = new GameObject($"PooledAudio_{_audioSourcePool.Count}");
            audioObj.transform.SetParent(transform);
            AudioSource source = audioObj.AddComponent<AudioSource>();
            
            // Configuración para VR Spatial Audio
            source.spatialBlend = 1.0f; // Full 3D
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = AudioRolloffMode.Linear; // Más predecible en VR
            source.playOnAwake = false;
            
            _audioSourcePool.Enqueue(source);
            return source;
        }

        private void SubscribeToEvents()
        {
            Events.GameEvents.OnSpatialAudioRequested.AddListener(PlaySpatialAudio);
            Events.GameEvents.OnGlobalAudioRequested.AddListener(PlayGlobalAudio);
        }

        private void UnsubscribeFromEvents()
        {
            Events.GameEvents.OnSpatialAudioRequested.RemoveListener(PlaySpatialAudio);
            Events.GameEvents.OnGlobalAudioRequested.RemoveListener(PlayGlobalAudio);
        }

        /// <summary>
        /// Reproduce audio en una posición 3D del mundo.
        /// </summary>
        public void PlaySpatialAudio(AudioClip clip, Vector3 position)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] Clip is null!");
                return;
            }

            AudioSource source = GetAudioSource();
            source.transform.position = position;
            source.spatialBlend = 1.0f;
            source.clip = clip;
            source.Play();

            _activeAudioSources.Add(source);
            StartCoroutine(ReturnToPoolWhenFinished(source, clip.length));
        }

        /// <summary>
        /// Reproduce audio global (2D, no espacial).
        /// </summary>
        public void PlayGlobalAudio(AudioClip clip)
        {
            if (clip == null) return;

            AudioSource source = GetAudioSource();
            source.spatialBlend = 0.0f; // 2D
            source.clip = clip;
            source.Play();

            _activeAudioSources.Add(source);
            StartCoroutine(ReturnToPoolWhenFinished(source, clip.length));
        }

        private AudioSource GetAudioSource()
        {
            if (_audioSourcePool.Count == 0)
            {
                Debug.LogWarning("[AudioManager] Pool exhausted! Creating new AudioSource.");
                return CreateNewAudioSource();
            }

            return _audioSourcePool.Dequeue();
        }

        private System.Collections.IEnumerator ReturnToPoolWhenFinished(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            _activeAudioSources.Remove(source);
            source.Stop();
            source.clip = null;
            _audioSourcePool.Enqueue(source);
        }

        /// <summary>
        /// Para todos los sonidos activos (útil al cambiar escena o pausar).
        /// </summary>
        public void StopAllAudio()
        {
            foreach (var source in _activeAudioSources)
            {
                if (source != null) source.Stop();
            }
            StopAllCoroutines();
        }
    }
}
