using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    private Animator animator;
    private Transform target;
    private bool skipMove;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    // Start is called before the first frame update
    protected override void Start()
    {
    //Gets components and determines the location of the player
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }



    protected override void AttemptMove<T>(int xDir, int yDir)
    {
    //Attempts to move the enemy, skipping every second turn
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;

    }
    public void MoveEnemy()
    {
    //Moves the enemy
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);
    }
    protected override void OnCantMove<T>(T component)
    {
    //Is called if the enemy can't move to the wanted space
        Player hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("Enemy1Chop");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        
        
    }
}
