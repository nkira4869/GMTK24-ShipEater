using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletPatternDatabase", menuName = "ScriptableObjects/BulletPatternDatabase", order = 1)]
public class BulletPatternDatabase : ScriptableObject
{
    [System.Serializable]
    public class BulletPatternEntry
    {
        public string patternName; // Name for identification
        public GameObject patternPrefab; // Prefab to use for this pattern
    }

    public List<BulletPatternEntry> bulletPatterns; // List of available bullet patterns
}
