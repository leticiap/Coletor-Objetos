using UnityEngine;
using UnityEngine.UI;

public class CurrentScore : MonoBehaviour
{
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = GameManager.Instance.GetScore().ToString();
    }
}
