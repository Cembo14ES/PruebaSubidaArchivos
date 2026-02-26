using UnityEngine;
using Huellas26.Utils;

// Editor script simple para mostrar boton en Inspector
namespace Huellas26.Utils.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(ProceduralOxGenerator))]
    public class OxGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ProceduralOxGenerator generator = (ProceduralOxGenerator)target;
            if (GUILayout.Button("Generate Ox Mesh"))
            {
                generator.GenerateOx();
            }
        }
    }
}
