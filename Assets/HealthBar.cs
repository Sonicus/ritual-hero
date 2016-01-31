using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private LeapController leapController;
    private Image image;

    void Update()
    {
        if (leapController.started && !leapController.isDead)
        {
            image.enabled = true;
            this.GetComponentInChildren<Text>().enabled = true;
            image.fillAmount = leapController.hitpoints / 100.0f;
        }
        else
        {
            image.enabled = false;
            this.GetComponentInChildren<Text>().enabled = false;
        }
        
    }
    void Start ()
    {
        image = GetComponent<Image>();
        leapController = GameObject.FindObjectOfType<LeapController>();
	}
}
