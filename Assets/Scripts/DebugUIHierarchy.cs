using UnityEngine;
using System.Text;

/// <summary>
/// Debug tool to print the entire hierarchy of DiscussionUI.
/// Attach to any GameObject, press Space in Play mode to print.
/// </summary>
public class DebugUIHierarchy : MonoBehaviour
{
    [SerializeField] private string objectToDebug = "DiscussionUI";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugObject();
        }
    }

    private void DebugObject()
    {
        GameObject obj = GameObject.Find(objectToDebug);
        
        if (obj == null)
        {
            Debug.LogError($"[DebugUIHierarchy] Could not find '{objectToDebug}'");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"\n========================================");
        sb.AppendLine($"HIERARCHY OF: {objectToDebug}");
        sb.AppendLine($"========================================");
        
        PrintHierarchy(obj.transform, 0, sb);
        
        sb.AppendLine($"========================================\n");
        Debug.Log(sb.ToString());
    }

    private void PrintHierarchy(Transform t, int indent, StringBuilder sb)
    {
        string indentStr = new string(' ', indent * 2);
        
        // Print this object
        sb.Append($"{indentStr}├─ {t.name}");
        
        // Add component info
        Component[] components = t.GetComponents<Component>();
        if (components.Length > 1) // More than just Transform
        {
            sb.Append(" [");
            bool first = true;
            foreach (var comp in components)
            {
                if (comp is Transform) continue;
                if (!first) sb.Append(", ");
                sb.Append(comp.GetType().Name);
                first = false;
            }
            sb.Append("]");
        }
        
        sb.AppendLine();
        
        // Print children
        foreach (Transform child in t)
        {
            PrintHierarchy(child, indent + 1, sb);
        }
    }
}
