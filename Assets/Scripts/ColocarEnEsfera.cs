using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ColocarEnEsfera
{
    static ColocarEnEsfera()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // Mantén pulsada la tecla C (de Colocar) y mueve el ratón sobre la esfera
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C && Selection.activeTransform != null)
        {
            Transform seleccionado = Selection.activeTransform;
            Ray rayo = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            
            // Obtenemos todos los impactos para ignorar los colliders del propio objeto seleccionado
            RaycastHit[] impactos = Physics.RaycastAll(rayo);

            foreach (RaycastHit hit in impactos)
            {
                // Si el rayo golpea algo que NO es el propio objeto ni ninguno de sus hijos
                if (!hit.transform.IsChildOf(seleccionado) && hit.transform != seleccionado)
                {
                    Undo.RecordObject(seleccionado, "Alinear con esfera");
                    seleccionado.position = hit.point;
                    seleccionado.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    e.Use();
                    break;
                }
            }
        }
    }
}