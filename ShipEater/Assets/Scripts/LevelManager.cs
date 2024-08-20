using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<LevelData> levels; // List of levels
    public Camera mainCamera;
    public int currentLevelIndex = 0; // Track the current level
    public GameObject cloudTransitionPrefab; // Reference to the cloud transition prefab
    private GameObject activeCloudTransition; // The instance of the cloud transition
    public HullManager hullManager; // Reference to the HullManager to access the ship's level

    private void Start()
    {
        //SetupLevel(currentLevelIndex);
    }

    private void Update()
    {
        // Check if the ship's current level meets the requirement to transition to the next level
        if (hullManager.currentLevel >= levels[currentLevelIndex].requiredShipLevel)
        {
            AdvanceToNextLevel();
        }
    }

    public void SetupLevel(int levelIndex)
    {
        if (levelIndex >= levels.Count)
        {
            Debug.LogWarning("No more levels available.");
            return;
        }

        StartCoroutine(LevelTransition(levelIndex));
    }

    private IEnumerator LevelTransition(int levelIndex)
    {
        // Step 1: Deactivate all spawners
        

        // Step 2: Activate the cloud transition
        activeCloudTransition = Instantiate(cloudTransitionPrefab);
        activeCloudTransition.transform.SetParent(transform); // Make it a child of the LevelManager for easy cleanup
        activeCloudTransition.transform.localPosition = Vector3.zero;

        // Wait for the cloud transition to fully cover the screen
        yield return new WaitForSeconds(5f); // Adjust this delay based on the cloud animation speed
        DeactivateAllSpawners();
        // Step 3: Setup the next level while the cloud is active
        SetupLevelEnvironment(levelIndex);

        // Wait for the duration of the transition effect before removing it
        yield return new WaitForSeconds(levels[levelIndex].transitionDuration);

        // Remove or disable the cloud transition
        Destroy(activeCloudTransition);
    }

    private void SetupLevelEnvironment(int levelIndex)
    {
        // Activate spawners for the current level
        foreach (var spawner in levels[levelIndex].activeSpawners)
        {
            spawner.SetActive(true);
        }

        // Set the background color or other settings
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = levels[levelIndex].backgroundColor;
        }

        Debug.Log($"Level {levels[levelIndex].levelName} setup complete.");
    }

    private void DeactivateAllSpawners()
    {
        foreach (var level in levels)
        {
            foreach (var spawner in level.activeSpawners)
            {
                spawner.SetActive(false);
            }
        }
    }

    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Count)
        {
            SetupLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }
}