using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScenesPanelExample : MonoBehaviour
{
    static ScenesPanelExample I;

    void Awake()
    {
        if (I != null) Destroy(this.gameObject);
        else I = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

	void Update()
	{
	    if (Input.GetKeyDown(KeyCode.LeftArrow)) {
	        int currSceneIndex = SceneManager.GetActiveScene().buildIndex;
	        int nextSceneIndex = currSceneIndex > 0 ? currSceneIndex - 1 : SceneManager.sceneCountInBuildSettings - 1;
            SceneManager.LoadScene(nextSceneIndex);
	    } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            int currSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currSceneIndex < SceneManager.sceneCountInBuildSettings - 1 ? currSceneIndex + 1 : 0;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}