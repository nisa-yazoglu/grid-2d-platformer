using UnityEngine;
using UnityEditor;
using System.IO;

public class AudioAutoAssigner : EditorWindow
{
    [MenuItem("Tools/Auto Assign Audio")]
    public static void AssignAudio()
    {
        string movementPath1 = "Assets/VOiD1 Gaming - HALFTONE SFX PACK LITE/HALFTONE SFX Pack LITE/Gameplay/3. Movement/Step_1.wav";
        string movementPath2 = "Assets/VOiD1 Gaming - HALFTONE SFX PACK LITE/HALFTONE SFX Pack LITE/Gameplay/3. Movement/Step_2.wav";
        string jumpPath = "Assets/VOiD1 Gaming - HALFTONE SFX PACK LITE/HALFTONE SFX Pack LITE/Gameplay/3. Movement/Jump_8.wav";
        string coinPath = "Assets/VOiD1 Gaming - HALFTONE SFX PACK LITE/HALFTONE SFX Pack LITE/Gameplay/5. Collectibles/Collectibles_5.wav";
        string loosePath = "Assets/VOiD1 Gaming - HALFTONE SFX PACK LITE/HALFTONE SFX Pack LITE/Gameplay/2. Loose/Loose_12.wav";

        AudioClip step1 = AssetDatabase.LoadAssetAtPath<AudioClip>(movementPath1);
        AudioClip step2 = AssetDatabase.LoadAssetAtPath<AudioClip>(movementPath2);
        AudioClip jump = AssetDatabase.LoadAssetAtPath<AudioClip>(jumpPath);
        AudioClip coin = AssetDatabase.LoadAssetAtPath<AudioClip>(coinPath);
        AudioClip loose = AssetDatabase.LoadAssetAtPath<AudioClip>(loosePath);

        if (step1 == null || step2 == null || jump == null || coin == null || loose == null)
        {
            Debug.LogError("Bazı ses dosyaları bulunamadı! Lütfen dosya yollarını kontrol edin.");
            if (step1 == null) Debug.LogError("Eksik: " + movementPath1);
            if (step2 == null) Debug.LogError("Eksik: " + movementPath2);
            if (jump == null) Debug.LogError("Eksik: " + jumpPath);
            if (coin == null) Debug.LogError("Eksik: " + coinPath);
            if (loose == null) Debug.LogError("Eksik: " + loosePath);
            return;
        }

        Char1_0Controller player = FindFirstObjectByType<Char1_0Controller>();
        if (player != null)
        {
            Undo.RecordObject(player, "Assign Player Audio");
            player.jumpSound = jump;
            player.moveSounds = new AudioClip[] { step1, step2 };

            // AudioSource ekle yoksa
            if (player.GetComponent<AudioSource>() == null)
            {
                player.gameObject.AddComponent<AudioSource>();
            }

            EditorUtility.SetDirty(player);
            Debug.Log("Player sesleri atandı.");
        }
        else
        {
            Debug.LogWarning("Sahnede Player (Char1_0Controller) bulunamadı.");
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            Undo.RecordObject(levelManager, "Assign LevelManager Audio");
            levelManager.levelCompleteSound = loose;

            // AudioSource ekle yoksa
            if (levelManager.GetComponent<AudioSource>() == null)
            {
                levelManager.gameObject.AddComponent<AudioSource>();
            }

            EditorUtility.SetDirty(levelManager);
            Debug.Log("LevelManager sesleri atandı.");
        }
        else
        {
            Debug.LogWarning("Sahnede LevelManager bulunamadı.");
        }

        Coin[] coins = FindObjectsByType<Coin>(FindObjectsSortMode.None);
        foreach (Coin c in coins)
        {
            Undo.RecordObject(c, "Assign Coin Audio");
            c.collectSound = coin;
            EditorUtility.SetDirty(c);
        }
        Debug.Log(coins.Length + " adet Coin sesi güncellendi.");
    }
}
