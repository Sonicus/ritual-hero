using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;
using System.Collections.Generic;
using UnityEngine.UI;

public class LeapController : MonoBehaviour {
    private Controller leapController;
    private RigidHand[] hands;
    private Dictionary<int, List<int>> handGesturePairs;
    private List<Ritual> rituals;
    public Ritual ritualPrefab;
    public GameObject explosionPrefab;
    public GameObject puffPrefab;
    private AudioSource explosionSound;

    private bool showHands = true;
    public bool isDead = false;
    public bool started = false;
    
    private int currentChain = 0;
    private int score = 0;

    public float hitpoints = 100.0f;
    private float damageValue = 20.0f;
    private float healValue = 2.5f;
    private float timeBonus = 0;
    private float timerSpeed = 0;
    private float showGetReadyTimer = 0.0f;
    public float timeToWait = 2.0f;
    
    private Text chainText;
    private Text scoreText;
    private Text resetText;
    
    void Start () {
        handGesturePairs = new Dictionary<int, List<int>>();
        leapController = new Controller();
        explosionSound = this.GetComponent<AudioSource>(); 

        leapController.EnableGesture(Gesture.GestureType.TYPESWIPE);
        leapController.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
        leapController.Config.SetFloat("Gesture.Swipe.MinVelocity", 750f);
        leapController.Config.Save();

        chainText = GameObject.FindGameObjectWithTag("chaintext").GetComponent<Text>();
        scoreText = GameObject.FindGameObjectWithTag("scoretext").GetComponent<Text>();
        resetText = GameObject.FindGameObjectWithTag("spacetoreset").GetComponent<Text>();
        resetText.enabled = false;
        chainText.enabled = false;
        scoreText.enabled = false;
        rituals = new List<Ritual>();
    }

    void GenerateRituals()
    {
        Ritual newRitual;
        for(int i=0; i < 8; i++)
        {
            newRitual = Instantiate(ritualPrefab);
            newRitual.transform.position = new Vector3(-10 + i * 3, 8.4f, 17.0f);
            rituals.Add(newRitual);
        }
        timerSpeed += 0.002f;
    }

    Vector3 CheckSwipeDirection(SwipeGesture gesture)
    {
        var isHorizontal = Mathf.Abs(gesture.Direction.x) > Mathf.Abs(gesture.Direction.y);

        if(isHorizontal)
        {
            if (gesture.Direction.x > 0)
            {
                return Vector3.right;
            }
            else
            {
                return Vector3.left;
            }
        }
        else
        {
            if(gesture.Direction.y > 0)
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.down;
            }
        }
    }

    void CheckRitual(Gesture gesture)
    {
        if (rituals.Count > 0)
        {
            if (gesture.Type.Equals(rituals[0].correctGestureType) && gesture.Hands[0].IsLeft.Equals(rituals[0].isGestureLeftHanded) && CheckSwipeDirection(new SwipeGesture(gesture)).Equals(rituals[0].correctDirection))
            {
                var newExplosion = Instantiate(explosionPrefab);
                newExplosion.transform.position = rituals[0].transform.position;
                Destroy(newExplosion, 3);
                Destroy(rituals[0].gameObject);
                rituals.RemoveAt(0);
                explosionSound.Play();
                currentChain++;
                score += (int) (100 * (0.25 * currentChain) + timeBonus * 300);
                hitpoints += healValue;
                hitpoints = System.Math.Min(100.0f, hitpoints);
            }
            else
            {
                currentChain = 0;
                hitpoints -= damageValue * 0.3f;
            }
        }
        if(rituals.Count == 0 && !showHands)
        {
            GenerateRituals();
        }
    }

    void CheckGestures()
    {
        foreach (Gesture gesture in leapController.Frame().Gestures())
        {
            if (handGesturePairs.ContainsKey(gesture.Hands[0].Id))
            {
                if (!handGesturePairs[gesture.Hands[0].Id].Contains(gesture.Id))
                {
                    handGesturePairs[gesture.Hands[0].Id].Add(gesture.Id);
                    CheckRitual(gesture);
                }
            }
            else
            {
                handGesturePairs.Add(gesture.Hands[0].Id, new List<int> { gesture.Id });
                CheckRitual(gesture);
            }
        }
    }
	
	void Update () {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        if (isDead)
        {
            if (Input.GetKeyDown("space"))
            {
                SceneManager.LoadScene(0);
            }
            return;
        }
        if (showHands)
        {
            if (leapController.Frame().Hands.Count >= 2)
            {
                showHands = false;
                GameObject.FindGameObjectWithTag("showhands").GetComponent<BlinkText>().showHands = false;
                showGetReadyTimer = Time.time;
                GameObject.FindGameObjectWithTag("getready").GetComponent<GetReadyText>().showText = true;
            }
            else
            {
                return;
            }
        }
        if(showGetReadyTimer != 0.0f && !started)
        {
            if(Time.time - showGetReadyTimer > timeToWait)
            {
                GameObject.FindGameObjectWithTag("getready").GetComponent<GetReadyText>().showText = false;
                started = true;
                GenerateRituals();
                chainText.enabled = true;
                scoreText.enabled = true;
            }
            else
            {
                return;
            }
        }
        if (!started) { return; }

        if (!isDead && hitpoints <= 0.0f)
        {
            Die();
            return;
        }

        UpdateTimer();
        CheckGestures();
        UpdateScore();
	}

    void UpdateTimer()
    {
        var blackMask = rituals[0].transform.Find("Black Mask");
        blackMask.Translate(0, -timerSpeed, 0);
        if(blackMask.transform.position.y <= 8.4f)
        {
            FailRitual();
        }
        else
        {
            timeBonus = 1.0f - ((11.0f - blackMask.transform.position.y) / (11.0f - 8.4f));
        }
    }

    void FailRitual()
    {
        var puff = Instantiate(puffPrefab);
        puff.transform.position = rituals[0].transform.position;
        Destroy(puff, 3);
        Destroy(rituals[0].gameObject);
        rituals.RemoveAt(0);
        currentChain = 0;
        hitpoints -= damageValue;
    }

    void UpdateScore()
    {
        chainText.text = "Current chain "+ currentChain;
        scoreText.text = "Score " + score;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("DEAD!");
        chainText.enabled = false;
        scoreText.enabled = false;
        foreach(Ritual ritual in rituals)
        {
            Destroy(ritual.gameObject);
        }
        var gameOverText = GameObject.FindGameObjectWithTag("gameovertext").GetComponent<Text>();
        gameOverText.enabled = true;
        gameOverText.text = "Game Over\nFinal Score\n" + score;
        resetText.enabled = true;
    }
}
