using UnityEngine;
using System.Collections;
using Leap;

public class Ritual : MonoBehaviour  {

    public Gesture.GestureType correctGestureType;
    public bool isGestureLeftHanded;
    public Vector3 correctDirection;
    public Sprite leftHandSprite;
    public Sprite rightHandSprite;
    public Sprite arrowSprite;
    private SpriteRenderer handIcon;
    private SpriteRenderer actionIcon;

    void Start()
    {
        correctGestureType = Gesture.GestureType.TYPESWIPE;
        handIcon = this.transform.Find("HandIcon").GetComponent<SpriteRenderer>();
        actionIcon = this.transform.Find("ActionIcon").GetComponent<SpriteRenderer>();

        RandomizeHand();
        RandomizeAction();
    }

    private void RandomizeHand()
    {
        isGestureLeftHanded = Random.value > 0.5f;
        handIcon.sprite = isGestureLeftHanded ? leftHandSprite : rightHandSprite;
    }

    private void RandomizeAction()
    {
        actionIcon.sprite = arrowSprite;
        var direction = Random.Range(0, 4);
        correctDirection = Vector3.left;
        switch (direction)
        {
            case 1:
                correctDirection = Vector3.up;
                actionIcon.transform.Rotate(0, 0, -90);
                break;
            case 2:
                correctDirection = Vector3.right;
                actionIcon.transform.Rotate(0, 0, 180);
                break;
            case 3:
                correctDirection = Vector3.down;
                actionIcon.transform.Rotate(0, 0, 90);
                break;
        }
    }
}
