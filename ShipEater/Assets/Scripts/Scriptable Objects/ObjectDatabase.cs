using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDatabase", menuName = "ScriptableObjects/ObjectDatabase", order = 1)]
public class ObjectDatabase : ScriptableObject
{
    public List<GameObject> objectsToSpawn; // List of prefabs to spawn in this layer
}
