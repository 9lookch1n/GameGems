using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Time : MonoBehaviour
{
    float time = 0;
    float startTime = 60;

    [SerializeField]
    Text TimeCoutdown;

    // Start is called before the first frame update
    void Start()
    {
        time = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        time -= 1 * Time.deltaTime;
        TimeCoutdown.text = time.ToString("0");

        if (time <= 0)
        {
            time = 0;
        }
    }
}
