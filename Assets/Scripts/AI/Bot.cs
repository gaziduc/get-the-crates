using System.Collections;
using UnityEngine;
using Pathfinding;
using Photon.Pun;
using Random = UnityEngine.Random;

public class Bot : MonoBehaviour
{
    private Transform target;
    private Vector2 force;
    [HideInInspector] public bool canMove = false;
    [HideInInspector] public int score = 0;
    [HideInInspector] public Vector2 direction;
    
    private SpriteRenderer sp;
    private Animator anim;
    private PlayerWeapon weapon;
    private PlayerHealth health;
    private Transform[] feet;
    [SerializeField] private LayerMask platformsLayerMask;
    
    [Header("Pathfinding")]
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path path;
    private int currentWaypoint = 0;
    private bool jumped = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private PhotonView view;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        sp.flipX = true;
        anim = GetComponent<Animator>();
        weapon = GetComponent<PlayerWeapon>();
        health = GetComponent<PlayerHealth>();
        feet = new Transform[2];
        feet[0] = transform.GetChild(0).GetChild(0);
        feet[1] = transform.GetChild(0).GetChild(1);
        view = GetComponent<PhotonView>();
        
        if (view.IsMine)
        {
            StartCoroutine(SetTargetCoroutine());
            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        }

        StartCoroutine(SetAnimCoroutine());
    }

    private IEnumerator SetAnimCoroutine()
    {
        while (!LevelManager.instance.gameStarted)
            yield return null;
        
        anim.SetBool("IsRunning", true);

        if (view.IsMine)
        {
            while (true)
            {
                yield return new WaitForSeconds(1.5f);
                if (health.health > 0)
                {
                    view.RPC("ShootBulletRPC", RpcTarget.All, transform.position.x, transform.position.y, weapon.weaponNum, sp.flipX ? 1f : -1f, view.ViewID);
                    weapon.SetReloadBeginning();
                }
            }
        }
    }

    private IEnumerator SetTargetCoroutine()
    {
        GameObject crate = null;
        do
        {
            crate = GameObject.FindWithTag("Crate");
            yield return null;
        } while (crate == null);
        
        target = crate.GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (target != null && TargetInDistance() && followEnabled)
            {
                if (jumped)
                {
                    rb.velocity = new Vector2(force.x * Time.fixedDeltaTime, jumpModifier);
                    jumped = false;
                }
                else
                {
                    rb.velocity = new Vector2(force.x * Time.fixedDeltaTime, rb.velocity.y);
                }
            }
        }
    }

    private void Update()
    {
        if (view.IsMine)
        {
            if (target != null && TargetInDistance() && followEnabled)
            {
                PathFollow();
            }
        }
    }

    private void UpdatePath()
    {
        if (target != null && followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }
    
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(feet[0].position, Vector2.down, 0.1f, platformsLayerMask);
        if (raycastHit2D.collider != null)
            return true;

        raycastHit2D = Physics2D.Raycast(feet[1].position, Vector2.down, 0.1f, platformsLayerMask);
        return raycastHit2D.collider != null;
    }

    private void PathFollow()
    {
        if (path == null)
            return;

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
            return;

        if (!canMove || !LevelManager.instance.gameStarted)
            return;

        // Direction Calculation
        direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        force = direction * speed;

        // Jump and movement
        if (jumpEnabled && IsGrounded() && direction.y > jumpNodeHeightRequirement)
            jumped = true;
        
        force = new Vector2(force.x, 0);
        
        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
            currentWaypoint++;
        
        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
                sp.flipX = true;
            else if (rb.velocity.x < -0.05f)
                sp.flipX = false;
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (view.IsMine && other.CompareTag("Crate"))
        {
            Crate crate = other.GetComponent<Crate>();
            
            bool isWeaponCrate = crate.spriteNum == 0;

            // Add 1 to score if it is Get Most Crates mode
            if (LevelManager.instance.winCondition == LevelManager.WinCondition.GetMostCrates)
                score++;

            // Handle
            if (isWeaponCrate)
            {
                weapon.GetRandom();
                view.RPC("WeaponEffectRPC", RpcTarget.All, weapon.weaponNum, view.ViewID, true);
            }
            else
                view.RPC("HealRPC", RpcTarget.All, view.ViewID);

            // Transfer ownership...
            PhotonView crateView = other.GetComponent<PhotonView>();
            crateView.TransferOwnership(view.Owner);

            // ...to move position
            Vector3 newPos = SpawnManager.instance.GetCrateNewPosition(other.transform.position);
            crateView.RPC("SetNewCrateRPC", RpcTarget.All, newPos.x, newPos.y, Random.Range(0, 8) == 0 ? 1 : 0);
        }
    }
}
