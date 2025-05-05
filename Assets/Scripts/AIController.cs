using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform turretTransform;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float BulletSpeed = 10f;
    [SerializeField] private float BulletDamage = 40f;
    [SerializeField] private float reloadTime = 3f;

    private float remainReloadingTime = 0f;

    [Header("Movement")]
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player)
        {
            agent.SetDestination(player.position);
        }
    }

    void Update()
    {
        if (remainReloadingTime > 0)
        {
            remainReloadingTime -= Time.deltaTime;
        }

        if (player)
        {
            RotateTurret(player.position);
            Shoot();
        }

        if (agent.velocity.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void RotateTurret(Vector2 worldPosition)
    {
        turretTransform.up = worldPosition - (Vector2)turretTransform.position;
    }

    private void Shoot()
    {
        if (remainReloadingTime > 0) return;

        GameObject bul = Instantiate(Bullet, muzzle.position, muzzle.rotation);
        Rigidbody2D rb = bul.GetComponent<Rigidbody2D>();
        rb.AddForce(bul.transform.up * BulletSpeed, ForceMode2D.Impulse);

        remainReloadingTime = reloadTime;
    }

    private void MoveTo(Vector2 worldPosition)
    {
        if (!agent) return;
        agent.SetDestination(worldPosition);
    }

    // === Тактична поведінка ===
    public void ApplyTactic(string tactic)
{
    switch (tactic)
    {
        case "run away":
            RetreatWithoutStrategy();
            break;
        case "retreat backward without cover":
            RetreatStraightBack();
            break;
        case "retreat using cover":
            RetreatToCover();
            break;
        case "hold position defensively":
            HoldPosition();
            break;
        case "stand still and fire back":
            HoldPosition();
            break;
        case "advance slowly using cover":
            AdvanceWithCover();
            break;
        case "attack while keeping distance":
            AttackFromDistance();
            break;
        case "attack at close range":
            CloseRangeAttack();
            break;
        case "move in and attack from the side":
            FlankFromSide();
            break;
        case "move in and attack from behind":
            FlankFromBehind();
            break;
        default:
            Debug.LogWarning("Unknown tactic: " + tactic);
            HoldPosition();

            // Надсилання повідомлення з нагадуванням
            ChatGPTClient client = FindObjectOfType<ChatGPTClient>();
            if (client != null)
            {
                client.ResendReminder();
            }

            break;
    }
}


    private void RetreatWithoutStrategy()
    {
        Debug.Log("Tactic: Run away");
        Vector2 retreatDir = (transform.position - player.position).normalized;
        agent.SetDestination((Vector2)transform.position + retreatDir * 5f);
    }

    private void RetreatStraightBack()
    {
        Debug.Log("Tactic: Retreat backward without cover");
        Vector2 retreatDir = -transform.up;
        agent.SetDestination((Vector2)transform.position + retreatDir * 5f);
    }

    private void RetreatToCover()
    {
        Debug.Log("Tactic: Retreat using cover");
        Transform cover = FindNearestCover();
        if (cover)
        {
            Vector2 dirAway = (transform.position - player.position).normalized;
            Vector2 safePos = (Vector2)cover.position + dirAway * 1.5f;
            agent.SetDestination(safePos);
        }
        else
        {
            RetreatWithoutStrategy();
        }
    }

    private void HoldPosition()
    {
        Debug.Log("Tactic: Hold position defensively");
        agent.SetDestination(transform.position);
    }

    private void AdvanceWithCover()
    {
        Debug.Log("Tactic: Advance slowly using cover");
        Transform cover = FindNearestCover();
        if (cover)
        {
            Vector2 toPlayer = (player.position - cover.position).normalized;
            Vector2 approachPoint = (Vector2)cover.position + toPlayer * 1.5f;
            agent.SetDestination(approachPoint);
        }
        else
        {
            CloseRangeAttack();
        }
    }

    private void AttackFromDistance()
    {
        Debug.Log("Tactic: Attack while keeping distance");
        float safeDistance = 10f;
        Vector2 dir = (transform.position - player.position).normalized;
        Vector2 targetPos = (Vector2)player.position + dir * safeDistance;
        agent.SetDestination(targetPos);
    }

    private void CloseRangeAttack()
    {
        Debug.Log("Tactic: Attack at close range");
        agent.SetDestination(player.position);
    }

    private void FlankFromSide()
    {
        Debug.Log("Tactic: Move in and attack from the side");
        Vector2 sideOffset = Vector2.Perpendicular(player.position - transform.position).normalized * 5f;
        agent.SetDestination(player.position + (Vector3)sideOffset);
    }

    private void FlankFromBehind()
    {
        Debug.Log("Tactic: Move in and attack from behind");
        Vector2 backOffset = -(player.position - transform.position).normalized * 5f;
        agent.SetDestination(player.position + (Vector3)backOffset);
    }

    private Transform FindNearestCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        float minDistance = float.MaxValue;
        Transform closest = null;

        foreach (GameObject cover in covers)
        {
            float distance = Vector2.Distance(transform.position, cover.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = cover.transform;
            }
        }

        return closest;
    }

    // === Публічні геттери для ChatGPTClient ===
    public float GetDamage() => BulletDamage;
    public float GetReloadTime() => reloadTime;
    public float GetRemainingReload() => Mathf.Max(0f, remainReloadingTime);
}
