using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] exitNodes;

    public float breakingDistance = 30f, breakingSpeed = 20f, targetReachedBuffer = 12f;
}
