using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class CompanionController : MonoBehaviour
{
    private Vector3 updatedPosition;
    private NavMeshAgent Agent;
    [SerializeField]
    private Player Player;
    [SerializeField]
    private Companion Companion;
    //[Header("Idle Configs")]
    //[SerializeField]
    //[Range(0, 10f)]
    //private float RotationSpeed = 2f;
    [Header("Follow Configs")]
    [SerializeField]
    private float FollowRadius = 2f;

    private Coroutine MovementCoroutine;
    private Coroutine StateChangeCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player.OnStateChange += HandleStateChange;
    }

    private void HandleStateChange(PlayerState OldState, PlayerState NewState)
    {
        if (StateChangeCoroutine != null)
        {
            StopCoroutine(StateChangeCoroutine);
        }

        switch (NewState)
        {
            case PlayerState.Idle:
                StateChangeCoroutine = StartCoroutine(HandleIdlePlayer());
                break;
            case PlayerState.Moving:
                HandleMovingPlayer();
                break;
        }
    }

    private IEnumerator HandleIdlePlayer()
    {
        switch(Companion.State)
        {
            case CompanionState.Follow:
                yield return new WaitUntil(() => !Agent.pathPending);
                yield return new WaitUntil(() => Companion.State == CompanionState.Idle);
                goto case CompanionState.Idle;
            case CompanionState.Idle:
                if (MovementCoroutine != null)
                {
                    StopCoroutine(MovementCoroutine);
                }
                Agent.enabled = false;
                break;
        }
    }

    private IEnumerator FollowPlayer()
    {
        check:
            Vector3 PlayerDestination = updatedPosition;

            Vector3 positionOffset = FollowRadius * new Vector3(
                Mathf.Cos(2 * Mathf.PI * Random.value),
                0,
                Mathf.Sin(2 * Mathf.PI * Random.value)
            ).normalized;

            Agent.SetDestination(PlayerDestination + positionOffset);

            yield return new WaitUntil(() => !Agent.pathPending);   //takes time to set a course to destination
            yield return new WaitUntil(() => Agent.remainingDistance <= Agent.stoppingDistance);    //wait for the agent to reach near the player

        if (PlayerDestination != updatedPosition)
            goto check;
        
        Companion.ChangeState(CompanionState.Idle);
    }

    private void HandleMovingPlayer()
    {
        Companion.ChangeState(CompanionState.Follow);
        if(MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }

        if(!Agent.enabled)
        {
            Agent.enabled = true;
            Agent.Warp(transform.position);
        }
        MovementCoroutine = StartCoroutine(FollowPlayer());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updatedPosition = Player.transform.position;
    }
}
