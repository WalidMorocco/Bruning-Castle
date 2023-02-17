using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClockScript : MonoBehaviour
{
    public float timeRemaining;
    int minutes;
    int seconds;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        CountDown();
    }

    private void CountDown()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            minutes = Mathf.FloorToInt(timeRemaining / 60);
            seconds = Mathf.FloorToInt(timeRemaining % 60);

            if (seconds < 10)
            {
                GetComponent<Text>().text = "Time left " + minutes + ":0" + seconds;
            }
            else
            {
                GetComponent<Text>().text = "Time left " + minutes + ":" + seconds;
            }
        }
        else
        {
            timeRemaining = 0;
            GetComponent<Text>().text = "Time left 0:00";
            SceneManager.LoadScene("GameOver");
        }
    }
}
