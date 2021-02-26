using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctrl_Score : MonoBehaviour
{
    public Text scoreText;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = " "+ score.ToString();
    }
    public void IncreaseScroe(int amountToIncrease)
    {
        score += amountToIncrease;
    }
}
