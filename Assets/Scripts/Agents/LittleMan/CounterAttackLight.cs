using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterAttackLight : MonoBehaviour {
    float keepTime = 0;
    public void StartCounterAttack(float viewRadius,float _keepTime)
    {
        GetComponent<FieldOfView>().ViewRadius = viewRadius;
        keepTime = _keepTime;
        GetComponent<FieldOfView>().lightEnable = true;
        GetComponent<FieldOfView>().enabled = true;
        StartCoroutine(counterAttackProcess());
    }
    IEnumerator counterAttackProcess()
    {
        float timeCount = 0;
        while (timeCount < keepTime)
        {
            yield return null;
            timeCount += Time.deltaTime;
        }
        GetComponent<FieldOfView>().lightEnable = false;
        GetComponent<FieldOfView>().enabled = false;
    }
    private void Awake()
    {
        GetComponent<FieldOfView>().lightEnable = false;
        GetComponent<FieldOfView>().enabled = false;
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
}
