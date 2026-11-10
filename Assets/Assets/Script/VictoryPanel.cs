using UnityEngine;

public class VictoryPanel : MonoBehaviour
{
    // "Sonraki Bölüm" butonu
    public void OnNextLevelClicked()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }

    // "Tekrar Dene" butonu
    public void OnRestartClicked()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }
    }
}
