using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFadingChangerScript : MonoBehaviour {

    public Animator animator;

    private int levelToLoad;

    public void FadeToNextLevel()
    {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FadeToLevel(int levelIndex)
    {
        if (levelIndex < 0)
        {
            levelIndex = 0;
        }

        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FadeToLevel(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}
