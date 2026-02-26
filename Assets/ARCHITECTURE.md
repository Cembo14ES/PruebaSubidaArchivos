# Arquitectura del Sistema - Monasterio de Iratxe VR

## Visión General

Este proyecto implementa un sistema modular de NPCs y minijuegos para Meta Quest 2, siguiendo principios SOLID y mejores prácticas de desarrollo VR.

## Estructura del Proyecto

```
_Scripts/
├── Core/                    # Sistemas fundamentales reutilizables
│   ├── Events/             # Sistema de eventos (Observer Pattern)
│   ├── Audio/              # Audio Manager con pooling
│   └── Pooling/            # Object Pool genérico
├── NPC/                    # Sistema de personajes no jugables
│   ├── FSM/                # Finite State Machine
│   │   ├── NPCState.cs     # Clase base abstracta
│   │   ├── IdleState.cs    
│   │   └── InteractionState.cs
│   ├── Interfaces/         # Contratos para desacoplar sistemas
│   │   └── IVoiceHandler.cs
│   ├── ScriptableObjects/  
│   │   └── NPCProfile.cs   # Datos configurables (Separación de Lógica/Datos)
│   └── NPC_Controller.cs   # Controlador principal (Contexto FSM)
├── Minigame/              
│   ├── MedievalGameManager.cs
│   └── CanTarget.cs
├── Utils/                  # Herramientas y generadores
│   └── ProceduralOxGenerator.cs
└── Tests/                  # Scripts de prueba
    └── TestNPC.cs
```

## Patrones de Diseño Implementados

### 1. **State Pattern (FSM)**

- **Ubicación**: `_Scripts/NPC/FSM/`
- **Propósito**: Modularizar comportamiento del NPC sin usar if/switch gigantes
- **Estados actuales**:
  - `IdleState`: Reposo, micro-animaciones
  - `InteractionState`: Diálogo con jugador
- **Beneficio**: Añadir nuevos estados (Patrol, Combat) sin modificar código existente (Open/Closed Principle)

### 2. **Observer Pattern (Eventos)**

- **Ubicación**: `_Scripts/Core/Events/GameEvents.cs`
- **Propósito**: Comunicación desacoplada entre sistemas
- **Ejemplo**:

  ```csharp
  // En lugar de:
  MedievalGameManager.Instance.AddScore(10); // Acoplamiento fuerte
  
  // Usamos:
  GameEvents.OnTargetHit?.Invoke(10); // Desacoplado
  ```

- **Beneficio**: Los sistemas no se conocen entre sí. Fácil testing y debugging.

### 3. **Singleton Pattern**

- **Ubicación**: `AudioManager`, `MedievalGameManager`
- **Propósito**: Acceso global a servicios únicos
- **Implementación**: Con `DontDestroyOnLoad` solo donde necesario (AudioManager)

### 4. **Object Pooling**

- **Ubicación**: `_Scripts/Core/Pooling/ObjectPool.cs`, `AudioManager.cs`
- **Propósito**: Evitar `Instantiate/Destroy` en runtime (causa drops de FPS en VR)
- **Crítico para VR**: Mantener 90 FPS constantes (requirement de la skill `vr-ar`)

### 5. **ScriptableObject Pattern**

- **Ubicación**: `NPCProfile.cs`
- **Propósito**: Separar datos de lógica
- **Beneficio**:
  - Crear múltiples NPCs sin duplicar código
  - Diseñadores pueden iterar sin tocar C#
  - Reduce errores de configuración en Inspector

## Principios SOLID Aplicados

### Single Responsibility (SRP)

- ✅ `NPC_Controller`: Solo maneja la FSM y delegación
- ✅ `AudioManager`: Solo gestiona reproducción de audio
- ✅ Estados: Cada uno maneja un comportamiento específico

### Open/Closed Principle (O CP)

- ✅ Añadir nuevos estados sin modificar `NPC_Controller`
- ✅ Nuevos tipos de objetivos sin modificar `CanTarget` base

### Liskov Substitution (LSP)

- ✅ Todos los `NPCState` son intercambiables
- ✅ `IVoiceHandler` permite sustituir implementaciones de voz

### Interface Segregation (ISP)

- ✅ `IVoiceHandler` solo expone métodos de voz

### Dependency Inversion (DIP)

- ✅ `NPC_Controller` depende de `NPCState` (abstracción), no de estados concretos
- ✅ Sistemas se comunican vía eventos (abstracción)

## Optimizaciones para VR

### Performance (Target: 90 FPS en Quest 2)

1. **Componentes Cacheados**

   ```csharp
   // ❌ MAL (GetComponent cada frame = lento)
   void Update() {
       GetComponent<Animator>().SetFloat("Speed", speed);
   }
   
   // ✅ BIEN (cached en Awake)
   private Animator _animator;
   void Awake() { _animator = GetComponent<Animator>(); }
   void Update() { _animator.SetFloat("Speed", speed); }
   ```

2. **Object Pooling**
   - AudioSources: Pool de 10 (expandible)
   - Preparado para poolear partículas y proyectiles

3. **Eventos en lugar de Update polling**
   - En vez de buscar cada frame, los sistemas reaccionan a eventos

### Confort VR (Según skill `vr-ar`)

1. **Escala Real**: 1 unidad Unity = 1 metro
   - Validado en `NPCProfile` y `ProceduralOxGenerator`

2. **Audio Espacial 3D**
   - `AudioManager`:
     - `spatialBlend = 1.0` (full 3D)
     - `minDistance = 1m`
     - `maxDistance = 15m`
     - `rolloffMode = Linear` (más predecible en VR)

3. **Sin camera shake automático** (causa mareo)
   - El jugador controla siempre la cámara

## Flujo de Datos

### Interacción con NPC

```
Usuario (VR Click/Voz) 
  → NPC_Controller.Interact()
    → CurrentState.OnInteract() 
      → ChangeState(InteractionState)
        → GameEvents.OnNPCInteracted.Invoke()
          → AudioManager reproduce sonido espacial
          → Animator dispara animación "Talk"
```

### Minijuego - Derribar Lata

```
Lata recibe colision fuerte
  → CanTarget.Score()
    → GameEvents.OnTargetHit.Invoke(points)
      → MedievalGameManager.AddScore()
        → GameEvents.OnScoreChanged.Invoke()
          → UI actualiza puntuación
      → AudioManager.PlaySpatialAudio(hitSound)
      → ParticleSystem.Play() (opcional)
```

## Extensibilidad

### Añadir un Nuevo Estado NPC

1. Crear `_Scripts/NPC/FSM/PatrolState.cs`:

```csharp
public class PatrolState : NPCState {
    public PatrolState(NPC_Controller npc) : base(npc) {}
    public override void Enter() { /* Logic */ }
    public override void Update() { /* Logic */ }
    public override void Exit() { /* Logic */ }
}
```

1. Usar desde cualquier lugar:

```csharp
_npc.ChangeState(new PatrolState(_npc));
```

### Añadir Nuevo Tipo de Minijuego

1. Solo suscríbete a `GameEvents.OnTargetHit`
2. Implementa tu lógica de rounds/muerte súbita
3. Emite `GameEvents.OnMinigameEnded`

## Testing

### Manual (Sin VR)

- Usa `TestNPC.cs` con teclado (1-5)
- Logs de debug en consola para cada transición

### Automatizado (Futuro)

- Los eventos permiten unit testing fácil
- Mock `GameEvents` para testear sistemas aislados

## Dependencias de Unity

- **Unity 2021.3+ LTS** (recomendado para Quest 2)
- **XR Interaction Toolkit** (para VR)
- **NavMesh Components** (para navegación NPCs)

## Próximos Pasos Recomendados

1. **Integración de Voz Real**
   - Implementar `IVoiceHandler` con Meta Voice SDK o Whisper
   - Conectar con OpenAI/LLM para diálogos dinámicos

2. **Estado PatrolState**
   - NPCs pasean por waypoints del monasterio

3. **Sistema de Diálogo Visual**
   - Subtítulos en Canvas world-space
   - Sincronización lip-sync con TextToSpeech

4. **Analytics**
   - Usar `GameEvents` para enviar métricas
   - Tracking de interacciones jugador-NPC

---

**Autor**: Generado con Antigravity AI  
**Fecha**: 2026-02-09  
**Skills Aplicadas**: `csharp-pro`, `game-development/vr-ar`, `clean-code`
