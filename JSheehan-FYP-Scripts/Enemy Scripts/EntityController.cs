using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EntityController : MonoBehaviour
{
    private bool firstRun = true;
    private GameObject target;
    public GameObject patrolArea;
    private Animator anim;
    private AnimatorStateInfo animInfo;
    public List<GameObject> patrolPoints;
    private GameObject[] allPatrolPoints;
    private NavMeshAgent navAgent;

    private float walkingSpeed = 3.5f;
    private float runningSpeed = 6f;
    private float searchingSpeed = 4.5f;
    private float waitTime = 3f;

    private Vector3 originalSize;
    private Vector3 currentPatrolAreaSize;

    public GameObject player;

    int currentSearchLoop = 0;

    public int maxSearchLoops = 3;

    private float heartRateModifier = 0;

    float startingPatrolWidth;
    float startingPatrolLength;

    float lookAroundTimer;
    float keepChasingTimer;
    public float chaseDuration = 10f;
    public float lookAroundDuration = 1f;

    float vissionRange = 30f;

    public float noiseFallofRate = 2f;

    public Queue<GameObject> myQueue;
    private bool searching = false;
    private bool startASearch = false;
    private bool cantSee = true;

    public bool seenHiding = false;
    private float killRange = 3f;

    public GameObject camera;

    public float beginPatrol = 0;


    public float fov = 120;
    public bool cantSeeOveride = false;
    bool stoppedSeeing = false;

    public int hidableCheckChance = 5;

    GameObject hidingSpot;

    public Vector3 searchAreaSize = new Vector3(50,10,50);

    bool hasLookedAround = false;

    public float currentRunningSpeed;
    public float currentWalkingSpeed;
    public float currentSearchingSpeed;

    Vector3 lastSeen;
    // Start is called before the first frame update
    void Start()
    {

        currentRunningSpeed = runningSpeed;
        currentWalkingSpeed = walkingSpeed;
        currentSearchingSpeed = searchingSpeed;

        patrolPoints = new List<GameObject>();
        allPatrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");
        patrolArea.transform.position = transform.position;
        anim = GetComponent<Animator>();
        animInfo = anim.GetCurrentAnimatorStateInfo(0);

        GameObject creator = GameObject.FindGameObjectWithTag("Creator");
        if (creator != null)
        {
            float width = creator.GetComponent<LevelCreator>().dungeonWidth;
            float length = creator.GetComponent<LevelCreator>().dungeonLength;
            float height = creator.GetComponent<LevelCreator>().ceilingHeight;

            width = width / 2;
            length = length / 2;    
            Vector3 scale = new Vector3(width , height, length);

            patrolArea.GetComponent<BoxCollider>().size = scale;

            originalSize = new Vector3(width, height, length);
            startingPatrolWidth = width;
            startingPatrolLength = length;

        }

        player = GameObject.FindGameObjectWithTag("Player");

        SelectPatrolPoints();
        target = RandomPatrolPoint();
        navAgent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        animInfo = anim.GetCurrentAnimatorStateInfo(0);

        beginPatrol += Time.deltaTime;
        look();


        if (seenHiding)
        {
            HandleAttackHiddenPlayer();
        }
            
        if (animInfo.IsName("Idle"))
        {
            target = this.gameObject;
            if (beginPatrol > 3f)
            {
                anim.SetBool("roaming", true);
            }

        }
        else if (animInfo.IsName("Roaming"))
        {
            GetComponent<NavMeshAgent>().speed = currentWalkingSpeed;
            if (target == null)
            {
                HandleRoaming();
            }
            else if (Vector3.Distance(target.transform.position, transform.position) < 2f)
            {
                HandleRoaming();
            }
        }
        else if (animInfo.IsName("Searching"))
        {
            GetComponent<NavMeshAgent>().speed = currentSearchingSpeed;

            if (startASearch)
            {
                HandleSearching();
            }
            else
            {
                if (Vector3.Distance(target.transform.position, transform.position) < 1f)
                {

                    lookAroundTimer += Time.deltaTime;
                    if (lookAroundTimer > lookAroundDuration)
                    {
                        HandleSearching();
                        lookAroundTimer = 0;
                    }

                }
            }
        }
        else if (animInfo.IsName("Lost Sight"))
        {
            anim.SetBool("chasing", false);
            target = this.gameObject;
        }
        else if (animInfo.IsName("Chase"))
        {
            GetComponent<NavMeshAgent>().speed = currentRunningSpeed;
            HandleChasing();
        }
        else if (animInfo.IsName("Death"))
        {
            navAgent.isStopped = true;
            anim.SetBool("dying", true);
            Destroy(gameObject, 3f);
        }

        else if (animInfo.IsName("Kill"))
        {
            GetComponent<NavMeshAgent>().speed = currentRunningSpeed;

            if (player.GetComponent<InputController>().isHiding)
            {
                hidingSpot.SetActive(false);
            }
            target = this.gameObject;
            player.SetActive(false);
            camera.SetActive(true);
        }
        else if (animInfo.IsName("Check Hidable"))
        {
            GetComponent<NavMeshAgent>().speed = currentRunningSpeed;

            HandleCheckHidable();
        }
        else if (animInfo.IsName("Failed Check"))
        {
            target = this.gameObject;
        }
        GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
    }

    private GameObject RandomPatrolPoint()
        {
            int randomPatrol = Random.Range(0, patrolPoints.Count);
            return (patrolPoints[randomPatrol]);
        }

    private void startSearchingPatrol()
    {
        currentSearchLoop++;
        if(currentSearchLoop == 1)
        {
            patrolArea.GetComponent<BoxCollider>().size = searchAreaSize;
        }
        if (currentSearchLoop <= maxSearchLoops)
        {
            myQueue = new Queue<GameObject>();
            searching = true;

            myQueue.Clear();
            Vector3 newSize = new Vector3(
                patrolArea.GetComponent<BoxCollider>().size.x * .9f,
                patrolArea.GetComponent<BoxCollider>().size.y,
                patrolArea.GetComponent<BoxCollider>().size.z * .9f);

            patrolArea.GetComponent<BoxCollider>().size = newSize;
            uponResize();
            foreach (GameObject point in patrolArea.GetComponent<CheckContents>().contents)
            {
                myQueue.Enqueue(point);
            }
        }
        else
        {
            patrolArea.GetComponent<BoxCollider>().size = originalSize;
            searching = false;
            currentSearchLoop = 0;
            anim.SetBool("searching", false);
            anim.SetBool("roaming", true);
        }

    }

    public void uponResize()
    {
        List<GameObject> patrolPoints = patrolArea.GetComponent<CheckContents>().contents;
        List<GameObject> hidingSpaces = patrolArea.GetComponent<CheckContents>().hidingSpaces;

        int chanceOfSearch = UnityEngine.Random.Range(1, hidableCheckChance+1);
        if(chanceOfSearch == 1)
        {
            if (hidingSpaces.Count > 0)
            {
                int randomHidable = UnityEngine.Random.Range(0, hidingSpaces.Count);
                anim.SetBool("checkingHidden", true);
                target = hidingSpaces[randomHidable];
                currentSearchLoop = maxSearchLoops;
                startSearchingPatrol();
            }
        }
        else if (patrolPoints.Count < 2) 
        {
            currentSearchLoop = maxSearchLoops;
            startSearchingPatrol();
        }

    }
        public void HandleSearching()
    {
        if (searching)
        {
            GameObject searchPoint = Search();
            target = searchPoint;
        }
        else
        {
            startSearchingPatrol();
        }
    }

    private GameObject Search()
    {
        if (myQueue.Count > 1)
        {
            patrolPoints = myQueue.ToList();
            return myQueue.Dequeue();
        }
        else if (myQueue.Count == 1)
        {
            searching = false;
            return myQueue.Dequeue();
        }
        else
        {
            return null;
        }
    }

    private void SelectPatrolPoints()
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        patrolArea.transform.position = newPosition;
        patrolPoints.Clear();

        foreach (GameObject point in patrolArea.GetComponent<CheckContents>().contents)
        {
            patrolPoints.Add(point);
        }
    }

    public void HandleRoaming()
    {

        SelectPatrolPoints();
        target = RandomPatrolPoint();
    }

    public void SetHeartRateModifier(float modifier)
    {
        heartRateModifier = modifier;
    }

    public void HandleChasing()
    {
        if (cantSee)
        {
            keepChasingTimer += Time.deltaTime;
        }
        else
        {
            keepChasingTimer = 0;
        }

        if (keepChasingTimer > chaseDuration)
        {
            anim.SetTrigger("lost");
            target = transform.gameObject;
        }
        else
        {
            target = player;
        }
    }


    private bool look()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Debug.DrawRay(transform.position, direction * 100, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 100, Color.green);

        float angle = Vector3.Angle(direction, transform.forward);
        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = transform.position + Vector3.up * 0.7f;
        ray.direction = transform.forward * vissionRange;
        Debug.DrawRay(ray.origin, ray.direction * vissionRange, Color.red);

        if (Physics.Raycast(ray.origin, direction, out hit, vissionRange) && angle < fov/2 )
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                cantSee = false;
                if(Vector3.Distance(hit.transform.position, this.transform.position) < killRange)
                {
                    anim.SetBool("kill", true);
                }
                if (!animInfo.IsName("Chase"))
                { 
                    anim.SetTrigger("roar");
                    GetComponent<NavMeshAgent>().speed = currentRunningSpeed;
                }
                target = this.gameObject;
                return true;
            }
            cantSee = true;

        }
        return false;
    }

    void listen(Vector3 pos, float intensity)
    {
        float distance = Vector3.Distance(pos, transform.position);

        if ((distance * noiseFallofRate) < intensity)
        {
            anim.SetBool("heard", true);
        }
    }

    public void setSeenHiding(GameObject input)
    {
        float distanceToPlayer = Vector3.Distance(this.gameObject.transform.position, player.transform.position);
        if (look() || distanceToPlayer < 2f)
        {
            seenHiding = true;
            target = input;
            hidingSpot = input;
        }

    }

    void HandleAttackHiddenPlayer()
    {
        if (Vector3.Distance(target.transform.position, transform.position)
            < killRange)
        {
            anim.SetBool("chasing", false);
            anim.SetTrigger("kill");
        }
    }

    void HandleCheckHidable()
    {
        
        if (Vector3.Distance(target.transform.position, transform.position)
            < killRange)
        {
            if (player.GetComponent<InputController>().isHiding && target == hidingSpot)
            {
                anim.SetBool("checkingHidden", false);
                anim.SetTrigger("kill");
            }
            else
            {
                anim.SetBool("checkingHidden", false);
                anim.SetTrigger("failure");
            }
        }
    }
}
