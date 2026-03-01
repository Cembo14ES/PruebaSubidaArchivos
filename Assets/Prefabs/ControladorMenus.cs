using UnityEngine;
using UnityEngine.UIElements;

public class ControladorDeMenus : MonoBehaviour
{
    [Header("UI Toolkit")]
    public UIDocument documentoUI;

    [Header("Control del Jugador")]
    public GameObject jugador; // Arrastra aquí tu XR Origin

    public string[] scriptsDeMovimiento = {
        "DynamicMoveProvider",
        "ContinuousMoveProvider",
        "ActionBasedContinuousMoveProvider",
        "ContinuousTurnProvider",
        "ActionBasedContinuousTurnProvider",
        "SnapTurnProvider",
        "ActionBasedSnapTurnProvider",
        "TeleportationProvider"
    };

    private VisualElement root;
    private Button btnInicio;
    private Button btnSalir;
    private Button btnAjustes;
    private Button btnAccesibilidad;
    private VisualElement menuPrincipal;
    private VisualElement menuAjustes;
    private VisualElement menuAccesibilidad;
    private VisualElement overlayBloqueo;
    private Button btnVolverAjustes;
    private Button btnVolverAccesibilidad;

    void OnEnable()
    {
        if (documentoUI == null) documentoUI = GetComponent<UIDocument>();
        root = documentoUI.rootVisualElement;

        btnInicio = root.Q<Button>("BtnInicio");
        btnSalir = root.Q<Button>("BtnSalir");
        btnAjustes = root.Q<Button>("BtnAjustes");
        btnAccesibilidad = root.Q<Button>("BtnAccesibilidad");
        menuPrincipal = root.Q<VisualElement>("MonasteryDoor");
        menuAjustes = root.Q<VisualElement>("MenuAjustes");
        menuAccesibilidad = root.Q<VisualElement>("MenuAccesibilidad");
        overlayBloqueo = root.Q<VisualElement>("OverlayBloqueo");

        if (menuAjustes != null)
            btnVolverAjustes = menuAjustes.Q<Button>("BtnVolverAjustes");
        if (menuAccesibilidad != null)
            btnVolverAccesibilidad = menuAccesibilidad.Q<Button>("BtnVolverAccesibilidad");

        if (btnInicio != null) btnInicio.clicked += IniciarJuego;
        if (btnSalir != null) btnSalir.clicked += SalirDelJuego;
        if (btnAjustes != null) btnAjustes.clicked += AbrirMenuAjustes;
        if (btnAccesibilidad != null) btnAccesibilidad.clicked += AbrirMenuAccesibilidad;
        if (btnVolverAjustes != null) btnVolverAjustes.clicked += CerrarMenusLaterales;
        if (btnVolverAccesibilidad != null) btnVolverAccesibilidad.clicked += CerrarMenusLaterales;

        if (overlayBloqueo != null)
            overlayBloqueo.RegisterCallback<ClickEvent>(evt => CerrarMenusLaterales());

        CerrarMenusLaterales();

        // Al arrancar, mostramos el menú (y bloqueamos al jugador)
        MostrarMenu(true);
    }

    public void IniciarJuego()
    {
        // Ocultamos el menú (y desbloqueamos al jugador)
        MostrarMenu(false);
        if (documentoUI != null)
            documentoUI.gameObject.SetActive(false);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void AbrirMenuAjustes()
    {
        CerrarMenusLaterales();
        if (menuAjustes != null) menuAjustes.style.display = DisplayStyle.Flex;
        if (overlayBloqueo != null) overlayBloqueo.style.display = DisplayStyle.Flex;
        
        if (btnAjustes != null) btnAjustes.AddToClassList("menu-button-active");
    }

    void AbrirMenuAccesibilidad()
    {
        CerrarMenusLaterales();
        if (menuAccesibilidad != null) menuAccesibilidad.style.display = DisplayStyle.Flex;
        if (overlayBloqueo != null) overlayBloqueo.style.display = DisplayStyle.Flex;

        if (btnAccesibilidad != null) btnAccesibilidad.AddToClassList("menu-button-active");
    }

    void CerrarMenusLaterales()
    {
        if (menuAjustes != null) menuAjustes.style.display = DisplayStyle.None;
        if (menuAccesibilidad != null) menuAccesibilidad.style.display = DisplayStyle.None;
        if (overlayBloqueo != null) overlayBloqueo.style.display = DisplayStyle.None;

        // Quitar la clase de seleccionado a todos los botones
        if (btnAjustes != null) btnAjustes.RemoveFromClassList("menu-button-active");
        if (btnAccesibilidad != null) btnAccesibilidad.RemoveFromClassList("menu-button-active");
    }

    void MostrarMenu(bool mostrar)
    {
        root.style.display = mostrar ? DisplayStyle.Flex : DisplayStyle.None;

        if (mostrar)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

        if (jugador != null)
        {
            Debug.Log("[ControladorMenus] === INICIO BUSQUEDA === Menu activo: " + mostrar);

            // Obtenemos TODOS los componentes tipo 'Behaviour' (que pueden ser activados/desactivados) en el jugador y sus hijos
            Behaviour[] todosLosComponentes = jugador.GetComponentsInChildren<Behaviour>(true);

            foreach (Behaviour comp in todosLosComponentes)
            {
                string nombreDelComponente = comp.GetType().Name;

                // Comparamos si el nombre de este componente está en tu lista de scripts
                foreach (string scriptTarget in scriptsDeMovimiento)
                {
                    if (nombreDelComponente.Contains(scriptTarget))
                    {
                        comp.enabled = !mostrar;
                        Debug.Log($"[ControladorMenus] ENCONTRADO: {nombreDelComponente} en el objeto '{comp.gameObject.name}' -> {(comp.enabled ? "ACTIVADO" : "DESACTIVADO")}");
                        break; // Pasamos al siguiente componente, ya hemos procesado este
                    }
                }
            }

            Debug.Log("[ControladorMenus] === FIN BUSQUEDA ===");
        }
        else
        {
            Debug.LogWarning("[ControladorMenus] No has asignado el GameObject del Jugador en el Inspector.");
        }
    }
}