using UnityEngine;
using UnityEngine.UIElements;

public class ControladorDeMenus : MonoBehaviour
{
    [Header("UI Toolkit")]
    public UIDocument documentoUI;

    [Header("Control del Jugador")]
    // Arrastra aquí a tu objeto Jugador (FPSController, PlayerCapsule, etc.)
    public GameObject jugador;

    // Nombres de los scripts que mueven a tu personaje (CAMBIALOS si usas otros)
    // Ejemplos comunes: "FirstPersonController", "PlayerMovement", "LookController"
    public string[] scriptsDeMovimiento = { "FirstPersonController", "CharacterController", "PlayerInput" };

    private VisualElement root;
    private Button btnInicio;
    private Button btnSalir;

    void OnEnable()
    {
        // 1. Configurar UI
        if (documentoUI == null) documentoUI = GetComponent<UIDocument>();
        root = documentoUI.rootVisualElement;

        btnInicio = root.Q<Button>("BtnInicio");
        btnSalir = root.Q<Button>("BtnSalir");

        if (btnInicio != null) btnInicio.clicked += IniciarJuego;
        if (btnSalir != null) btnSalir.clicked += SalirDelJuego;

        // 2. ESTADO INICIAL: Mostrar menú, CONGELAR jugador
        MostrarMenu(true);
    }

    public void IniciarJuego()
    {
        // Ocultar menú y LIBERAR jugador
        MostrarMenu(false);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- LA MAGIA: Congelar / Descongelar ---
    void MostrarMenu(bool mostrar)
    {
        // A. Mostrar/Ocultar la UI
        root.style.display = mostrar ? DisplayStyle.Flex : DisplayStyle.None;

        // B. Control del Cursor (Importante para PC/Web, en VR es distinto)
        if (mostrar)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None; // Cursor libre para clicar
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; // Cursor atrapado para jugar
            UnityEngine.Cursor.visible = false;
        }

        // C. Desactivar/Activar scripts de movimiento
        if (jugador != null)
        {
            // Opción 1: Desactivar componentes específicos por nombre
            foreach (var nombreScript in scriptsDeMovimiento)
            {
                // Intentamos buscar el script como Componente (MonoBehaviour)
                var script = jugador.GetComponent(nombreScript) as MonoBehaviour;
                if (script != null)
                {
                    script.enabled = !mostrar; // Si hay menú -> script disabled
                }
            }

            // Opción 2 (Más bruta): Desactivar el CharacterController si usas uno
            var cc = jugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = !mostrar;
        }
    }
}