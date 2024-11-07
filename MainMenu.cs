using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;

[DisallowMultipleComponent]
public class MainMenu : MonoBehaviour
{
    private EntityManager entityManager;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void LoadEasyLevel()
    {
        SetDifficulty(false, false);
        SceneManager.LoadScene("Easy");
        Debug.Log("Loaded Easy Level");
    }

    public void LoadMediumLevel()
    {
        SetDifficulty(true, false);
        SceneManager.LoadScene("Medium");
        Debug.Log("Loaded Medium Level");
    }

    public void LoadHardLevel()
    {
        SetDifficulty(false, true);
        SceneManager.LoadScene("Hard");
        Debug.Log("Loaded Hard Level");
    }

    private void SetDifficulty(bool isMedium, bool isHard)
    {
        // Destroy any previous difficulty entity to reset settings
        var existingDifficultyEntities = entityManager.CreateEntityQuery(typeof(DifficultyComponent));
        entityManager.DestroyEntity(existingDifficultyEntities);

        // Create a new difficulty entity with updated settings
        var difficultyEntity = entityManager.CreateEntity(typeof(DifficultyComponent));
        entityManager.SetComponentData(difficultyEntity, new DifficultyComponent
        {
            IsMediumMode = isMedium,
            IsHardMode = isHard
        });
    }
}
