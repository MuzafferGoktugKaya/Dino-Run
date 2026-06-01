using UnityEngine;
using UnityEngine.SceneManagement;

public class AirZoneAutoReturn : MonoBehaviour
{
    [Header("Air Zone Settings")]
    public float returnAfterSeconds = 30f;

    [Header("Return Scene")]
    public string returnSceneName = "LandScene";

    private void Start()
    {
        Invoke(nameof(ReturnToLandScene), returnAfterSeconds);
    }

    private void ReturnToLandScene()
    {
        SceneManager.LoadScene(returnSceneName);
    }
}