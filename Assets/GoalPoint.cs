using UnityEngine;

public class GoalPoint : MonoBehaviour
{
 public  GameObject panel;

public void OnTriggerEnter2D(Collider2D collision)
{
panel.SetActive(true);
}

public void OnTriggerExit2D(Collider2D collision)
{
    panel.SetActive(false);
}

}
