using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DrawNodes : MonoBehaviour
{
#if UNITY_EDITOR
    Node[] nodes;

    [Header("Gizmos parameter")]
    [SerializeField]
    bool wantGizmos = true;

    [SerializeField]
    Color nodeColor = Color.blue,
        breakingDistanceColor = new Color(1, 0, 0, 0.2f),
        lineToExitColor = Color.blue;

    [SerializeField]
    float lineToExitThickness = 10;

    void OnDrawGizmos()
    {
        if (!wantGizmos)
            return;

        if(nodes == null)
            nodes = gameObject.GetComponentsInChildren<Node>();

        if (nodes == null)
            return;

        foreach (Node node in nodes)
        {
            // Node
            Gizmos.color = nodeColor;
            Gizmos.DrawSphere(node.transform.position, node.targetReachedBuffer);

            // Break distance
            Gizmos.color = breakingDistanceColor;
            Gizmos.DrawSphere(node.transform.position, node.breakingDistance);

            // Line to exit nodes
            foreach (Node exitNode in node.exitNodes)
            {
                var startPos = node.transform.position;
                var endPos = exitNode.transform.position;

                Handles.DrawBezier(startPos, endPos, startPos, endPos, lineToExitColor, null, lineToExitThickness);
            }
        }
    }
#endif
}
