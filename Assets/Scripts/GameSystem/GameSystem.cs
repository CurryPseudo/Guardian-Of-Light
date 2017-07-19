using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class GameSystem : MonoBehaviour {
    int LittleManCount = 0;
    int allLittleManCount;
    public int needToSurviveCount = 3;
    Text littleManCountText;
    GameObject player;
    public List<GameObject> monsters = new List<GameObject>();
    public List<GameObject> littleMans = new List<GameObject>();
    [HideInInspector]
    public GameObject canvas;
    [HideInInspector]
    public GameObject monsterPrefab;
    [HideInInspector]
    public GameObject fastMonsterPrefab;
    GameObject background;
    [HideInInspector]
    public GameObject DeadPanel;
    [HideInInspector]
    public GameObject DeadButton;
    [HideInInspector]
    public GameObject Timer;
    //List<GameObject> obstacleList = new List<GameObject>();
    public float nightTime = 120;
    [HideInInspector]
    public new GameObject camera;
    public float nightFadeTime = 1;
    int tempMonsterCount = 3;
    Vector2 worldSize = Vector2.zero;
    float timerOriginalScale;
    // Use this for initialization
    public List<Upgrade> upgrades;
    [HideInInspector]
    public Text debugText;
    Coroutine monsterGroupNightTimeIncreaseNow;
    public void AddMonster(GameObject monster)
    {
        monsters.Add(monster);
    }
    public void LittleManCountIncrease()
    {
        LittleManCount++;
        allLittleManCount++;
        //littleManCountText.text = LittleManCount.ToString();
        littleManCountText.text = countToString();
    }
    public void LittleManCountDecline()
    {
        LittleManCount--;
        //littleManCountText.text = LittleManCount.ToString();
        littleManCountText.text = countToString();
        if (LittleManCount < needToSurviveCount)
        {
            PlayerDead();
        }
    }
    void PlayerDead()
    {
        DeadPanel.SetActive(true);
        gamePause();

    }
    void gamePause()
    {
        player.GetComponent<PlayerController>().enabled = false;
        StopCoroutine(TimerDecline());
    }
    void PlayAgain()
    {
        Singleton<Datas>.Instance.dataReload();
        SceneManager.LoadScene(0);
    }
    string countToString()
    {
        if (LittleManCount > needToSurviveCount)
        {
            string countString = new string('^', LittleManCount);
            countString = countString.Insert(needToSurviveCount, "|");
            return countString;
        }
        if (LittleManCount == needToSurviveCount)
        {
            return new string('^', LittleManCount);
        }
        return "Dead";
    }
    public static void delayDoSomething(MonoBehaviour father,float time,System.Action action)
    {
        father.StartCoroutine(DelayDoSomethingHelper(time,action));
    }
    public static IEnumerator DelayDoSomethingHelper(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
        yield break;
    }
    void NightBegin()
    {
        group.nightBegin();
        monsterGroupNightTimeIncreaseNow =  StartCoroutine(groupNightTimeIncreasePerSecond());
        Timer.transform.localScale = new Vector3(Timer.transform.localScale.x, timerOriginalScale, Timer.transform.localScale.z);
        StartCoroutine(TimerDecline());
    }
    void UpgradeEnd()
    {
        foreach (Upgrade upgrade in upgrades)
        {
            Destroy(upgrade.ButtonInstance);
        }
        Camera.main.GetComponent<MainCameraEffect>().nightBegin(nightFadeTime);
         StartCoroutine(DelayDoSomethingHelper(nightFadeTime, NightBegin));
        player.GetComponent<PlayerController>().ChangeUpState(FSM.State.Normal);
        Singleton<Datas>.Instance.isNight = true;

    }
    IEnumerator groupNightTimeIncreasePerSecond()
    {
        while (true)
        {
            group.addNightTime(1);
            group.setNowMonsterInfo();
            yield return new WaitForSeconds(1);
        }
    }
    void NightEnd()
    {
        Singleton<Datas>.Instance.isNight = false;
        GameObject[] lights = GameObject.FindGameObjectsWithTag("PutLight");
        foreach(GameObject light in lights)
        {
            Destroy(light);
        }
        group.nightEnd();
        StopCoroutine(monsterGroupNightTimeIncreaseNow);
        Camera.main.GetComponent<MainCameraEffect>().nightEnd(nightFadeTime);
        player.GetComponent<PlayerController>().ChangeUpState(FSM.State.WaitForUpgrade);
        if (group.gameWined)
        {
            winGame();
        }
        else
        {
            upgrades = Singleton<Datas>.Instance.UpgradeList.getRandUpgrades(3);
            StartCoroutine(DelayDoSomethingHelper(nightFadeTime, ShowButton));
        }
    }
    void winGame()
    {
        DeadPanel.SetActive(true);
        DeadPanel.transform.FindChild("Text").GetComponent<Text>().text = "YOU WIN!";
        gamePause();
    }
    IEnumerator TimerDecline()
    {
        float timeCount = 0;
        Vector3 scale = Timer.transform.localScale;
        while (timeCount < nightTime)
        {
            yield return null;
            timeCount += Time.deltaTime;
            Timer.transform.localScale = new Vector3(scale.x, scale.y * (1 - timeCount / nightTime), scale.z);
        }
        Timer.transform.localScale = new Vector3(scale.x,0,scale.z);
        NightEnd();
        yield break;
    }
  
    // Update is called once per frame


    #region MonoBehaviour
    void Awake()
    {
        littleManCountText = GameObject.Find("LittleManSurviveText").GetComponent<Text>();
        player = GameObject.Find("Player");
        if (DeadPanel == null)
        {
            DeadPanel = GameObject.Find("DeadPanel");
        }
        if (DeadButton == null)
        {
            DeadButton = GameObject.Find("DeadButton");
        }
        if (Timer == null)
        {
            Timer = GameObject.Find("Timer");
        }
        DeadButton.GetComponent<Button>().onClick.AddListener(PlayAgain);
        if (camera == null)
        {
            camera = GameObject.Find("Main Camera");
        }
        if (monsterPrefab == null) monsterPrefab = Resources.Load("SceneObjects/Monster") as GameObject;
        if (fastMonsterPrefab == null) fastMonsterPrefab = Resources.Load("SceneObjects/FastMonster") as GameObject;

        background = GameObject.Find("Background");
        worldSize = background.transform.localScale;
        DeadPanel.SetActive(false);
        timerOriginalScale = Timer.transform.localScale.y;
    }
    void Start()
    {
        group = new MonsterGroup(nightTime, worldSize);
        NightBegin();

    }
    void Update()
    {
        debugText.text = string.Format("GroupNowInfo:\n speed{0:G} \n escapeSpeed{1:G} \n attack{2:G} \n", group.NowInfo.originalSpeed,group.NowInfo.originalEscapeSpeed,group.NowInfo.originalAttack);
        GameObject joystickBg = null;
        if (joystickBg = GameObject.Find("JoystickBackground"))
        {
            debugText.text += "Joystick worldPosition " + joystickBg.GetComponent<RectTransform>().position.ToString() + "\n";
            debugText.text += "Joystick localPosition " + joystickBg.GetComponent<RectTransform>().localPosition.ToString() + "\n";
        }
    }

    #endregion
    #region Upgrades
    void ShowButton()
    {
        float distance = Screen.width / (upgrades.Count + 1);
        if (canvas == null) canvas = GameObject.Find("Canvas");
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].ShowOnScreen(new Vector3(distance * (i + 1), Screen.height / 2, 0));
            upgrades[i].ButtonInstance.GetComponent<Button>().onClick.AddListener(UpgradeEnd);
            upgrades[i].ButtonInstance.GetComponent<Button>().onClick.AddListener(player.GetComponent<PlayerController>().RefleshUpgrades);
        }
    }

    #endregion
    #region MonsterGroup
    MonsterGroup group;

    #endregion
}
