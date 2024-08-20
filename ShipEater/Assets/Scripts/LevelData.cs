using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public List<GameObject> activeSpawners; // Spawners to activate for this level
    public Color backgroundColor; // Background color or other visual settings
    public float transitionDuration = 1f; // Duration of the transition between levels
    public int requiredShipLevel; // The ship level required to transition to this level
}
