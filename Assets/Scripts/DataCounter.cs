using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using TMPro;

public class DataCounter : MonoBehaviour
{
    public int EpisodeCounter,
            SuccessCounter,
            Reach1FloorCounter,
            Reach2FloorCounter,
            Reach3FloorCounter;

    public float SuccessAvarageWaterHeightCounter,
                FailAvarageWaterHeightCounter;

    public GameObject EpisodeCount,
                    SuccessCount,
                    Reach1FloorCount,
                    Reach2FloorCount,
                    Reach3FloorCount,
                    SuccessAvarageWaterHeight,
                    FailAvarageWaterHeight;

    private TextMeshProUGUI EpisodeCountText,
                            SuccessCountText,
                            Reach1FloorCountText,
                            Reach2FloorCountText,
                            Reach3FloorCountText,
                            SuccessAvarageWaterHeightText,
                            FailAvarageWaterHeightText;

    void Start()
    {
        EpisodeCountText = EpisodeCount.GetComponent<TextMeshProUGUI>();
        SuccessCountText = SuccessCount.GetComponent<TextMeshProUGUI>();
        Reach1FloorCountText = Reach1FloorCount.GetComponent<TextMeshProUGUI>();
        Reach2FloorCountText = Reach2FloorCount.GetComponent<TextMeshProUGUI>();
        Reach3FloorCountText = Reach3FloorCount.GetComponent<TextMeshProUGUI>();
        SuccessAvarageWaterHeightText = SuccessAvarageWaterHeight.GetComponent<TextMeshProUGUI>();
        FailAvarageWaterHeightText = FailAvarageWaterHeight.GetComponent<TextMeshProUGUI>();
    }

    // Text
    public void TextSet()
    {
        EpisodeCountText.text = "Episode : " + EpisodeCounter;

        double rate = 0.00;
        if (EpisodeCounter > 0) rate = Math.Round(((double)SuccessCounter / EpisodeCounter) * 100, 2, MidpointRounding.AwayFromZero);
        SuccessCountText.text = "Success : " + rate + "%";

        Reach1FloorCountText.text = "Finish at 1Floor : " + Reach1FloorCounter;
        Reach2FloorCountText.text = "Finish at 2Floor : " + Reach2FloorCounter;
        Reach3FloorCountText.text = "Finish at 3Floor : " + Reach3FloorCounter;

        double height = Math.Round((double)SuccessAvarageWaterHeightCounter / SuccessCounter, 2, MidpointRounding.AwayFromZero);
        SuccessAvarageWaterHeightText.text = "(Success)\r\nAvarage of Water Height : " + height;

        height = Math.Round((double)FailAvarageWaterHeightCounter / (EpisodeCounter - SuccessCounter), 2, MidpointRounding.AwayFromZero);
        FailAvarageWaterHeightText.text = "(Fail)\r\nAvarage of Water Height : " + height;
    }
}
