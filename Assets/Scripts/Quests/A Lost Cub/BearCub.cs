using UnityEngine;
using UnityEngine.AI;

public class BearCub : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private bool isFollowing = false;

    public Transform motherBear;
    public float completionDistance = 2f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isFollowing && player != null)
        {
            agent.SetDestination(player.position);
        }

        if (motherBear != null && Vector3.Distance(transform.position, motherBear.position) < completionDistance)
        {
            CompleteQuest();
        }
    }

    public void StartFollowing()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            isFollowing = true;
            Debug.Log("Bear Cub started following the player.");
        }
        else
        {
            Debug.LogError("Player not found! Make sure the Player has the 'Player' tag.");
        }
    }

    private void CompleteQuest()
    {
        Debug.Log("Bear Cub has reached the Mother Bear! Quest Completed.");
        MainQuestManager.instance.CompleteQuest("A Lost Cub");

        isFollowing = false;
        agent.SetDestination(transform.position);
    }
}
