using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDouble : MonoBehaviour
{
    public float time = 10f;
    private float currentTime = 0f;
    private void Awake()
    {
        time = DataTableManager.UpGradeDataTable.Get(GameData.coinDoubleId).Item_Effect * 2;
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > time)
        {
            gameObject.SetActive(false);
        }
    }
    public void OnCoinDouble()
    {
        currentTime = 0f;
        gameObject.SetActive(true);
    }
}
