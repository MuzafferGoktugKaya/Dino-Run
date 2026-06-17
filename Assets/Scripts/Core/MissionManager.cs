using TMPro;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [System.Serializable]
    public class Mission
    {
        public string title;
        public string description;
        public int target;
        [HideInInspector] public int progress;
        [HideInInspector] public bool completed;
    }

    [Header("Mission UI")]
    public TMP_Text missionText;

    [Header("Session Missions")]
    public Mission coinMission = new Mission
    {
        title = "Coin Hunter",
        description = "Collect coins",
        target = 25
    };

    public Mission zoneMission = new Mission
    {
        title = "Explorer",
        description = "Survive zone transitions",
        target = 3
    };

    public Mission comboMission = new Mission
    {
        title = "Combo Runner",
        description = "Reach a combo streak",
        target = 8
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        RefreshMissionUI();
    }

    public void RegisterCoinCollected(int amount = 1)
    {
        AddProgress(coinMission, amount);
    }

    public void RegisterZoneTransition()
    {
        AddProgress(zoneMission, 1);
    }

    public void RegisterCombo(int comboValue)
    {
        if (comboMission == null || comboMission.completed) return;

        comboMission.progress = Mathf.Max(comboMission.progress, comboValue);
        if (comboMission.progress >= comboMission.target)
        {
            CompleteMission(comboMission);
        }
        else
        {
            RefreshMissionUI();
        }
    }

    private void AddProgress(Mission mission, int amount)
    {
        if (mission == null || mission.completed) return;

        mission.progress = Mathf.Clamp(mission.progress + amount, 0, mission.target);
        if (mission.progress >= mission.target)
        {
            CompleteMission(mission);
        }
        else
        {
            RefreshMissionUI();
        }
    }

    private void CompleteMission(Mission mission)
    {
        mission.completed = true;
        mission.progress = mission.target;
        RefreshMissionUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(10);
            GameManager.Instance.ShowNotification("MISSION COMPLETE: " + mission.title + " +10", Color.yellow);
        }
    }

    private void RefreshMissionUI()
    {
        if (missionText == null) return;

        missionText.text = FormatMission(coinMission) + "\n" +
                           FormatMission(zoneMission) + "\n" +
                           FormatMission(comboMission);
    }

    private string FormatMission(Mission mission)
    {
        if (mission == null) return string.Empty;

        string state = mission.completed ? "DONE" : mission.progress + "/" + mission.target;
        return mission.title + ": " + state;
    }
}
