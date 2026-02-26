using UnityEngine;

namespace Huellas26.Utils
{
    /// <summary>
    /// Genera una malla de buey (estilo low poly Voxel/Bloques).
    /// Útil para prototipar sin assets externos.
    /// Inspirado en Minecraft/Crossy Road.
    /// </summary>
    public class ProceduralOxGenerator : MonoBehaviour
    {
        [Header("Materiales")]
        public Material bodyMat;
        public Material hornMat;

        public void GenerateOx()
        {
            // Crear el contenedor principal
            GameObject oxRoot = new GameObject("Procedural_Ox");
            oxRoot.transform.position = Vector3.zero;

            // --- CUERPO ---
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.parent = oxRoot.transform;
            body.transform.localScale = new Vector3(1.5f, 1.2f, 2.5f); // Ancho, Alto, Largo
            body.transform.localPosition = new Vector3(0, 1.0f, 0); // Elevado por patas
            if (bodyMat != null) body.GetComponent<Renderer>().material = bodyMat;

            // --- CABEZA ---
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "Head";
            head.transform.parent = oxRoot.transform;
            head.transform.localScale = new Vector3(1.0f, 1.0f, 1.2f);
            head.transform.localPosition = new Vector3(0, 1.8f, 1.5f); // Mas arriba y adelante
            if (bodyMat != null) head.GetComponent<Renderer>().material = bodyMat;

            // --- PATAS (4) ---
            CreateLeg(oxRoot.transform, new Vector3(-0.6f, 0.5f, 1.0f)); // Delantera Izq
            CreateLeg(oxRoot.transform, new Vector3(0.6f, 0.5f, 1.0f));  // Delantera Der
            CreateLeg(oxRoot.transform, new Vector3(-0.6f, 0.5f, -1.0f)); // Trasera Izq
            CreateLeg(oxRoot.transform, new Vector3(0.6f, 0.5f, -1.0f));  // Trasera Der

            // --- CUERNOS ---
            CreateHorn(head.transform, new Vector3(-0.6f, 0.5f, 0.2f), -30f);
            CreateHorn(head.transform, new Vector3(0.6f, 0.5f, 0.2f), 30f);

            // Añadir componentes necesarios
            oxRoot.AddComponent<BoxCollider>().size = new Vector3(1.5f, 2.0f, 3.0f);
            oxRoot.AddComponent<UnityEngine.AI.NavMeshAgent>();
            oxRoot.AddComponent<AudioSource>();
            
            // Añadir nuestro controlador NPC
            // Nota: Se añadira via Inspector o Script, aqui solo generamos malla
            Debug.Log("Buey generado en la escena!");
        }

        private void CreateLeg(Transform parent, Vector3 pos)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.name = "Leg";
            leg.transform.parent = parent;
            leg.transform.localScale = new Vector3(0.4f, 1.0f, 0.4f);
            leg.transform.localPosition = pos;
            if (bodyMat != null) leg.GetComponent<Renderer>().material = bodyMat;
        }

        private void CreateHorn(Transform parent, Vector3 pos, float angleZ)
        {
            GameObject horn = GameObject.CreatePrimitive(PrimitiveType.Capsule); // Cuerno curvo dificil con primitivas, usamos capsula
            horn.name = "Horn";
            horn.transform.parent = parent;
            horn.transform.localScale = new Vector3(0.2f, 0.8f, 0.2f);
            horn.transform.localPosition = pos;
            horn.transform.localRotation = Quaternion.Euler(0, 0, angleZ);
            if (hornMat != null) horn.GetComponent<Renderer>().material = hornMat;
        }
    }
}
