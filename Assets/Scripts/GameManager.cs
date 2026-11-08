using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio clips")]
    [SerializeField] private AudioClip oopsTrack;
    [SerializeField] private AudioClip toxicTrack;
    [SerializeField] private AudioClip oopsSound;
    [SerializeField] private AudioClip iDidItSound;

    [Header("Timing")]
    [SerializeField] private float restartDelay = 1.8f;

    private bool runEnded;

    private void Awake()
    {
        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        musicSource.playOnAwake = false;
        musicSource.Stop();
    }

    public void StartGame()
    {
        runEnded = false;

        if (startPanel != null) startPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        player.BeginRun();
        PlayTrack(oopsTrack);
    }

    public void PlayToxicTrack()
    {
        if (!runEnded)
            PlayTrack(toxicTrack);
    }

    private void PlayTrack(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.time = 0f;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayerDied()
    {
        if (runEnded) return;

        runEnded = true;
        player.StopForEnd();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        musicSource.Stop();

        if (oopsSound != null)
            sfxSource.PlayOneShot(oopsSound);

        StartCoroutine(ReloadAfterDeath());
    }

    private IEnumerator ReloadAfterDeath()
    {
        yield return new WaitForSeconds(restartDelay);
        RestartLevel();
    }

    public void LevelWon()
    {
        if (runEnded) return;

        runEnded = true;
        player.StopForEnd();
        musicSource.Stop();

        if (winPanel != null) winPanel.SetActive(true);

        if (iDidItSound != null)
            sfxSource.PlayOneShot(iDidItSound);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}