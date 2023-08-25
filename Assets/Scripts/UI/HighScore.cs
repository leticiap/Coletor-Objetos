using UnityEngine;
using UnityEngine.UI;
public class HighScore : MonoBehaviour
{
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
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
