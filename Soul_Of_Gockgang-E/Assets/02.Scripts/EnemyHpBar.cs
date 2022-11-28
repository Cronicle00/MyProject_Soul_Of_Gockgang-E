using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    public Transform target;
    public RectTransform canvas;
    public RectTransform hpBar;
    public Camera mainCam;
    public GameObject hpBarPrefab;
    public GameObject hpObj;
    public int maxHp;

    void Start()
    {
        hpObj = Instantiate(hpBarPrefab);
        target = gameObject.transform;
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        hpObj.transform.parent = canvas;
        hpBar = hpObj.GetComponent<RectTransform>();
        hpBar.localScale = new Vector3(1, 1, 1);
        hpBar.localRotation = Quaternion.Euler(0, 0, 0);
        mainCam = Camera.main;
        maxHp = transform.parent.GetComponent<MonsterController>().hp;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curPos = target.transform.position;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(curPos);
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, mainCam, out canvasPos);
        if (hpBar != null)
        {
            hpBar.localPosition = canvasPos;
            hpBar.GetComponent<Slider>().value = (float)transform.parent.GetComponent<MonsterController>().hp / (float)maxHp;
        }
    }
    public void DestroyHpBar()
    {
        Destroy(hpBar.gameObject);
    }
}
