using UnityEngine;

public class VictoryPanel : MonoBehaviour
{
    // Bu fonksiyonu "Sonraki Bölüm" butonunun OnClick olayına bağlayın
    public void OnNextLevelClicked()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }

    // Bu fonksiyonu "Tekrar Dene" butonunun OnClick olayına bağlayın
    public void OnRestartClicked()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartLevel();
        }
    }
}
