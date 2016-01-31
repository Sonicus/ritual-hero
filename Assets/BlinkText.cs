using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlinkText : MonoBehaviour {

    private Text text;
    public bool showHands;
    private float alpha;
    private float alphaChange;

	// Use this for initialization
	void Start () {
        alpha = 0.02f;
        alphaChange = 0.015f;
        text = this.GetComponent<Text>();
        text.color = new Color(1.0f, 0, 0, alpha);
        showHands = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (!showHands)
        {
            this.enabled = false;
            alpha = 0.0f;
            text.color = new Color(1.0f, 0f, 0f, alpha);
            return;
        }
        if (alpha >= 0.99f || alpha<= 0.01f)
        {
            alphaChange *= -1;
        }
        text.color = new Color(1.0f, 0f, 0f, alpha);
        alpha += alphaChange;
	}
}
