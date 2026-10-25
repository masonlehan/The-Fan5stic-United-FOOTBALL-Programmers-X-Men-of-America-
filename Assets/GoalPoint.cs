using UnityEngine;
using TMPro;

public class GoalPoint : MonoBehaviour
{
 public  GameObject panel;

 [Header("Panel Text")]
 public TMP_Text panelText;
 public string wintext;
 public string losetext;

public void OnTriggerEnter2D(Collider2D collision)
{
panelText.text = wintext;
panel.SetActive(true);

}

public void OnTriggerExit2D(Collider2D collision)
{
    panel.SetActive(false);
}

}
