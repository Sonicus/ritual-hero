using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GetReadyText : MonoBehaviour {

    public bool showText;
    private Text text;

	// Use this for initialization
	void Start () {
        showText = false;
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.color = showText ? new Color(0.82f, 0, 0, 1) : new Color(0.82f, 0, 0, 0.0f);
	}
}
