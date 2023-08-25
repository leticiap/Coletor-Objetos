using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        if (PlayerPrefs.HasKey("Best Score"))
        {
            int score = PlayerPrefs.GetInt("Best Score");
            string name = PlayerPrefs.GetString("Best Player Name");

            text.text = "Melhor pontuação: " + score + " por " + name;
        }
        else
            text.text = "";
    }

}
