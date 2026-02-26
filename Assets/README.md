# Proyecto NPC Sistema - Monasterio de Iratxe VR

## Descripción

Sistema profesional de NPCs con IA para experiencia VR en Meta Quest 2. Ambientado en el Monasterio de Iratxe (Navarra), siglo XII.

## Características

✨ **NPCs Inteligentes**

- Máquina de Estados Finita (FSM) modular
- Perfiles configurables (ScriptableObjects)
- Soporte para humanos y animales

🎮 **Minijuego Medieval**

- Sistema de latas con física realista
- Rounds configurables
- Feedback audiovisual

🎧 **Audio Espacial 3D**

- Optimizado para VR
- Object Pooling (sin stutters)
- Configuración automática de rolloff

🏗️ **Arquitectura Profesional**

- Patrones: State, Observer, Singleton, Object Pool
- Principios SOLID
- Código desacoplado con eventos
- Documentación XML completa

⚡ **Optimizado para VR**

- Target: 90 FPS constantes
- Componentes cacheados
- Sin allocations en runtime
- Escala 1:1 (1 unidad = 1 metro)

## Requisitos

- **Unity**: 2021.3 LTS o superior
- **Platform**: Meta Quest 2
- **Packages**:
  - XR Interaction Toolkit
  - NavMesh Components

## Instalación

1. Clona este repositorio en tu proyecto de Unity
2. Instala los packages requeridos (Window > Package Manager)
3. Configura Build Settings para Android (Quest 2)
4. Bake NavMesh en el suelo de tu escena

## Inicio Rápido

### 1. Crear un Monje NPC

1. Crea un objeto 3D (o modelo) en la escena
2. Añade componentes:
   - `NavMesh Agent`
   - `Animator` (con parámetros: `Speed` float, `Talk` trigger, `Celebrate` trigger)
   - `Audio Source`
   - `NPC_Controller` (script)

3. Crea un Perfil:
   - Click derecho en Project > `Create > Huellas26 > NPC Profile`
   - Configura:
     - **Name**: "Hermano Martín"
     - **Type**: Human
     - **System Prompt**: "Eres un monje..."
     - **Walk Speed**: 2.0
     - **Interaction Sound**: (opcional)

4. Arrastra el perfil al campo `Profile` del NPC_Controller

### 2. Configurar el Minijuego

1. Crea objeto vacío `GameManager` en la escena
2. Añade `MedievalGameManager` (script)
3. Configura:
   - `Win Score`: 50
   - `Game Time`: 60
   - `Max Rounds`: 3

4. Para las latas/jarras:
   - Crea primitivas (Cylinder) o modelos 3D
   - Añade:
     - `Rigidbody` (Mass: 0.5)
     - `Collider`
     - `CanTarget` (script)
   - Configura:
     - `Points Value`: 10
     - `Min Impact Force`: 2.0
     - `Hit Sound`: (arrástralo aquí)

### 3. Audio Manager

1. Crea objeto vacío `AudioManager` en la escena
2. Añade `AudioManager` (script)
3. Configura (valores por defecto ya optimizados para VR):
   - `Min Distance`: 1.0
   - `Max Distance`: 15.0
   - `Pool Size`: 10

### 4. Probar sin VR

1. Crea objeto vacío `Tester`
2. Añade `TestNPC` (script)
3. Arrastra tu monje al campo `Target NPC`
4. Dale a Play y usa teclado:
   - `1`: Idle
   - `2`: Interact
   - `3`: Tutorial
   - `4`: Celebrate
   - `5`: Simular voz

## Arquitectura

Consulta [ARCHITECTURE.md](./ARCHITECTURE.md) para documentación técnica completa.

### Estructura de Carpetas

```
_Scripts/
├── Core/               # Sistemas reutilizables
│   ├── Events/        # GameEvents (Observer)
│   ├── Audio/         # AudioManager
│   └── Pooling/       # ObjectPool<T>
├── NPC/               # Sistema de NPCs
│   ├── FSM/           # Estados (Idle, Interaction)
│   ├── Interfaces/    # IVoiceHandler
│   └── ScriptableObjects/  # NPCProfile
├── Minigame/          # Medieval Toss Game
├── Utils/             # Herramientas (ProceduralOxGenerator)
└── Tests/             # Scripts de prueba
```

## Eventos del Sistema

Suscríbete a estos eventos para extender funcionalidad:

```csharp
using Huellas26.Core.Events;

// En Start() o Awake()
GameEvents.OnNPCInteracted.AddListener((name, pos) => {
    Debug.Log($"{name} fue clickeado en {pos}");
});

GameEvents.OnScoreChanged.AddListener((score) => {
    // Actualizar UI
});

GameEvents.OnMinigameEnded.AddListener((won) => {
    // Mostrar pantalla de victoria/derrota
});
```

## Mejores Prácticas VR Aplicadas

✅ **Confort**

- Sin movimiento de cámara automático
- Audio espacial con rolloff lineal
- Escala 1:1 realista

✅ **Performance**

- Componentes cacheados (no GetComponent en Update)
- Object Pooling para audio
- Eventos en lugar de polling

✅ **Usabilidad**

- Interacciones desde distancia (ray cast)
- Feedback audiovisual inmediato
- Estados claros del NPC

## Extensión

### Añadir Nuevo Estado NPC

```csharp
// _Scripts/NPC/FSM/PatrolState.cs
using Huellas26.NPC.FSM;

public class PatrolState : NPCState {
    public PatrolState(NPC_Controller npc) : base(npc) {}
    
    public override void Enter() {
        _agent.isStopped = false;
        // Lógica de inicio
    }
    
    public override void Update() {
        // Lógica de patrullaje
    }
    
    public override void Exit() {
        // Limpieza
    }
}
```

### Añadir Evento Personalizado

```csharp
// En GameEvents.cs
public static UnityEvent<string> OnCustomEvent = new UnityEvent<string>();

// Emitir desde cualquier lugar
GameEvents.OnCustomEvent?.Invoke("data");

// Suscribirse desde cualquier lugar
GameEvents.OnCustomEvent.AddListener((data) => { /* Handle */ });
```

## Troubleshooting

**NPC levita sobre el suelo**

- Ajusta `Ground Offset` en NPC_Controller
- Verifica que el suelo tenga layer "Ground" o "Default"

**No se reproduce audio**

- Verifica que AudioManager existe en la escena
- Comprueba que `interactionSound` está asignado en NPCProfile

**Latas no suman puntos**

- Verifica que MedievalGameManager.Instance no es null
- Llama a `StartGame()` antes de tirar
- Comprueba `Min Impact Force` (muy alto = no detecta)

**FPS bajos en VR**

- Reduce polígonos de modelos 3D
- Usa texturas comprimidas
- Verifica que Object Pooling esté activo

## Próximas Funcionalidades

- [ ] Sistema de voz real (Whisper/Meta Voice SDK)
- [ ] Integración con LLM (OpenAI) para diálogos dinámicos
- [ ] PatrolState para NPCs que pasean
- [ ] Sistema de subtítulos en VR
- [ ] Analytics de interacciones

## Licencia

Proyecto educativo para desarrollo en Meta Quest 2.

## Contacto

Desarrollado con **Antigravity AI** + Skills:

- `csharp-pro`
- `game-development/vr-ar`
- `clean-code`

---

**Versión**: 1.0.0  
**Fecha**: 2026-02-09  
**Target Platform**: Meta Quest 2
