using UnityEngine;

public class FlyingDinoMovement : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private int minLane = -1;
    [SerializeField] private int maxLane = 1;

    [Header("Height Settings")]
    [SerializeField] private float baseHeight = 2f;
    [SerializeField] private float heightStep = 1.5f;
    [SerializeField] private int minHeightLevel = 0;
    [SerializeField] private int maxHeightLevel = 2;

    [Header("Movement Settings")]
    [SerializeField] private float moveSmoothness = 10f;

    private int currentLane = 0;
    private int currentHeightLevel = 1;

    private void Update()
    {
        HandleInput();
        MoveToTargetPosition();
    }

    private void HandleInput()
    {
        // Horizontal lane change input. The player can switch between lanes using A/D or Left/Right Arrow keys.
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLane = Mathf.Clamp(currentLane - 1, minLane, maxLane);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentLane = Mathf.Clamp(currentLane + 1, minLane, maxLane);
        }

        // Vertical air movement to change height levels. 
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentHeightLevel = Mathf.Clamp(currentHeightLevel + 1, minHeightLevel, maxHeightLevel);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentHeightLevel = Mathf.Clamp(currentHeightLevel - 1, minHeightLevel, maxHeightLevel);
        }
    }

    private void MoveToTargetPosition()
    {
        Vector3 targetPosition = new Vector3(
            currentLane * laneDistance,
            baseHeight + currentHeightLevel * heightStep,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSmoothness * Time.deltaTime
        );
    }
}