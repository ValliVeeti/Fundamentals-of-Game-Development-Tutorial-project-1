using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;
    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        base.Start();
    }
    private void OnDisable()
    {
    //Saves the food amount to an outside variable between levels
        GameManager.instance.playerFoodPoints = food;
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if(Move (xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }
    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Food: " + food);
        
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
            vertical = 0;
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);

    }
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("PlayerChop");
        
    }
    private void Restart()
    {
        SceneManager.LoadScene(0);

    }
    public void LoseFood(int loss)
    {
        animator.SetTrigger("PlayerHit");
        food -= loss;
        foodText.text = foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
    //Handles the interactions between the consumables and the exit
        if(other.tag == "Exit")
        {
            
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            other.gameObject.SetActive(false);
        }
    }
    
        
    }

