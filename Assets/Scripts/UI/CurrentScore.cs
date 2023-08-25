using TMPro;
using UnityEngine;

public class CurrentScore : MonoBehaviour
{
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = GameManager.Instance.GetScore().ToString();
    }
}
