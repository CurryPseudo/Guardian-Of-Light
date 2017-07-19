using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGroup{
    public List<Monster> monsters;
    float nightTime = 0;
    float maxNightTime = 0;
    int nightNumber = 0;
    MonsterInfo nowInfo;
    MonsterData data;
    Vector2 worldSize;
    public MonsterInfo NowInfo
    {
        get
        {
            return nowInfo;
        }
    }
    public bool gameWined
    {
        get
        {
            return nightNumber == data.monsterInfoEachNight.Count-1;
        }
    }
    public void addNightTime(float deltaTime)
    {
        nightTime += deltaTime;
    }
    public void nightEnd()
    {
        nightNumber++;
        for(int i = 0; i < monsters.Count; i++)
        {
            monsters[i].frozeDown();
        }
    }
    public void nightBegin()
    {
        nightTime = 0;
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].activeAgain();
        }
        setNowMonsterInfo();
        generateNewMonster();
        monsterMutation();
    }
    void generateNewMonster()
    {
        GameObject monsterPrefab = Resources.Load<GameObject>("SceneObjects/Monster");
        monsterPrefab.GetComponent<Monster>().group = this;
        monsterPrefab.GetComponent<Monster>().MutationStatus = new MutationStatus(data);
        for (int i = 0; i < nowInfo.monsterNum; i++)
        {
            
            GameObject monsterGameObject = GameObject.Instantiate(monsterPrefab, GameObject.Find("Monsters").transform);
            monsterGameObject.transform.Translate(new Vector3((Random.value - 0.5f) * worldSize.x * 0.8f, (Random.value - 0.5f) * worldSize.y * 0.8f, 0));
            monsterGameObject.transform.localScale = new Vector3(1, 1, 1);
            Monster monster = monsterGameObject.GetComponent<Monster>();
            monster.group = this;
            monster.MutationStatus = new MutationStatus(data);
            monsters.Add(monster);
            
        }
    }
    public void setNowMonsterInfo()
    {
        nowInfo = data.getNowMonsterInfo(nightNumber, nightTime / maxNightTime);
    }
    void monsterMutation()
    {
        for(int i = 0; i < monsters.Count; i++)
        {
            Monster monster = monsters[i];
            float randValue = Random.value;
            if (randValue < nowInfo.mutationProbability)
            {
                monster.MutationStatus.randMutation();
                monster.updateStatus();
            }
        }
    }
	public MonsterGroup(float _maxNightTime,Vector2 _worldSize)
    {
        maxNightTime = _maxNightTime;
        data = Singleton<Datas>.Instance.MonsterData;
        worldSize = _worldSize;
        monsters = new List<Monster>();
    }
}
