using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.SoundManagerNamespace;

public class GameSoundManager : MonoBehaviour
{
    public AudioSource buttonClickSound;
    public AudioSource backgroundMusic;
    public AudioClip backgroundMusicClip;

    void Start()
    {
        // Assign the button click sound to all buttons in the scene
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayButtonClickSound());
        }

        // Play the background music on loop
        PlayBackgroundMusic();
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            buttonClickSound.PlayOneShotSoundManaged(buttonClickSound.clip);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && backgroundMusicClip != null)
        {
            backgroundMusic.clip = backgroundMusicClip;
            backgroundMusic.PlayLoopingMusicManaged(0.35f, 3.0f, true);
        }
    }
}
