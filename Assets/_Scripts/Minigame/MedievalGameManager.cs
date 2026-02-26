using UnityEngine;
using UnityEngine.Events;
using Huellas26.Core.Events;

namespace Huellas26.Minigames
{
    /// <summary>
    /// Gestor profesional del minijuego medieval optimizado para VR.
    /// Arquitectura desacoplada usando eventos.
    /// Performance: Evita FindObjectOfType y referencias directas.
    /// </summary>
    public class MedievalGameManager : MonoBehaviour
    {
        [Header("Configuracion del Minijuego")]
        [Tooltip("Puntos necesarios para ganar.")]
        public int winScore = 50;
        
        [Tooltip("Tiempo máximo en segundos.")]
        public float gameTime = 60.0f;
        
        [Tooltip("Número de rounds (0 = infinito).")]
        public int maxRounds = 3;

        [Header("Estados")]
        [SerializeField] private bool _isGameActive = false;
        [SerializeField] private int _currentRound = 0;
        public UnityEvent onGameStart;
        public UnityEvent onGameEnd;
        public UnityEvent<int> onScoreUpdate; // Envia la puntuación actual

        private int _currentScore = 0;
        private float _currentTime = 0;
        private bool _isGameActive = false;
        
        // Singleton pattern (simple)
        public static MedievalGameManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton Pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ResetGame();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnTargetHit.AddListener(AddScore);
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnTargetHit.RemoveListener(AddScore);
        }
        
        private void Update()
        {
            if (_isGameActive)
            {
                _currentTime -= Time.deltaTime;
                
                if (_currentTime <= 0)
                {
                    EndGame();
                }
            }
        }

        private void ResetGame()
        {
            _currentScore = 0;
            _currentTime = gameTime;
            _isGameActive = false;
            GameEvents.OnScoreChanged?.Invoke(_currentScore);
        }

        public void StartGame()
        {
            if (_isGameActive)
            {
                Debug.LogWarning("[Minigame] Game already running!");
                return;
            }

            _currentRound++;
            _isGameActive = true;
            _currentTime = gameTime;
            _currentScore = 0;
            
            Debug.Log($"[Minigame] Round {_currentRound} started!");
            GameEvents.OnMinigameStarted?.Invoke();
            onGameStart?.Invoke();
        }
        
        public void AddScore(int points)
        {
            if (!_isGameActive)
            {
                Debug.LogWarning("[Minigame] Cannot add score, game not active.");
                return;
            }

            _currentScore += points;
            GameEvents.OnScoreChanged?.Invoke(_currentScore);
            onScoreUpdate?.Invoke(_currentScore);
            
            Debug.Log($"[Minigame] Score: {_currentScore}/{winScore}");

            // Victoria
            if (_currentScore >= winScore)
            {
                EndGame(true);
            }
        }
        private void EndGame(bool won)
        {
            _isGameActive = false;
            
            GameEvents.OnMinigameEnded?.Invoke(won);
            onGameEnd?.Invoke();
            
            string result = won ? "WON" : "LOST";
            Debug.Log($"[Minigame] Round {_currentRound} ended: {result}! Score: {_currentScore}");
            
            // Decidir si hay más rounds
            if (won || (maxRounds > 0 && _currentRound >= maxRounds))
            {
                Debug.Log("[Minigame] Game Over!");
                // Aqui podríamos resetear o mostrar pantalla final
            }
        }
    }
}
