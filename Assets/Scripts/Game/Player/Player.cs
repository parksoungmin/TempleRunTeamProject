using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f;  // 걷는 속도
    public float swipeMovement = 1f;  // 좌우 스와이프 속도
    public float playerRotate = 90;
    public float tiltSpeed = 3f;

    public float raycastDistance = 1f;

    public float jumpForce = 5f;

    public float swipeTimeAccum = 0f;
    public float swipeTimeDelay = 2f;

    public bool canLeftSwipe = false;
    public bool canRightSwipe = false;

    private bool IsJumping = false;
    private bool UnityJump = false;
    public bool isTurn = false;
    public bool playerDead = false;
    //스코어
    private Vector3 lastPostion;
    public float totalDistance;
    public float speedUpDistance = 500f;
    private float speedDistance = 0f;

    public Magnet magnet;
    public Protect protect;
    public Invincibility invincibility;
    public CoinDouble coinDouble;
    public Animator animator;
    public GameObject dieEffect;

    private int speedUPCount = 0;
    public int speedUpMaxCount = 10;
    public float plus = 1.5f;

    private readonly float isTurnTime = 4f;
    private float isTurnCurrentTime = 0f;

    private float tiltInputX = 0f;
    private float filteredTiltInputX = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerDead = false;
        lastPostion = transform.position;
        totalDistance = 0f;
    }

    private void Update()
    {
        if(isTurn)
        {
            isTurnCurrentTime += Time.deltaTime;
            if(isTurnCurrentTime > isTurnTime)
            {
                isTurnCurrentTime = 0f;
                isTurn = false;
            }
        }
        //CheckHeight();
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            swipeTimeAccum = 0;
            canLeftSwipe = true;
            canRightSwipe = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            swipeTimeAccum = 0;
            canRightSwipe = true;
            canLeftSwipe = false;
        }
#elif UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Q))
        {
            swipeTimeAccum = 0;
            canLeftSwipe = true;
            canRightSwipe = false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            swipeTimeAccum = 0;
            canRightSwipe = true;
            canLeftSwipe = false;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (MultiTouch.Instance.SwipeDirection.sqrMagnitude != 0f)
        {
            if (Mathf.Abs(MultiTouch.Instance.SwipeDirection.x) > Mathf.Abs(MultiTouch.Instance.SwipeDirection.y))
            {
                CheckSwipeRotate();
            }
            else if (MultiTouch.Instance.SwipeDirection.y > 0)
            {
                CheckSwipeJump();
            }
        }
#endif
        if (canLeftSwipe)
        {
            swipeTimeAccum += Time.deltaTime;
            if (swipeTimeAccum >= swipeTimeDelay)
            {
                canLeftSwipe = false;
            }
        }
        if (canRightSwipe)
        {
            swipeTimeAccum += Time.deltaTime;
            if (swipeTimeAccum >= swipeTimeDelay)
            {
                canRightSwipe = false;
            }
        }

        //스코어
        Vector3 delta = transform.position - lastPostion;
        delta.y = 0f;
        var deltaMagnitude = delta.magnitude;
        totalDistance += deltaMagnitude;
        speedDistance += deltaMagnitude;
        SpeedUP();
        lastPostion = transform.position;
        if (Input.GetKeyDown(KeyCode.UpArrow) && !IsJumping && !playerDead)
        {
            UnityJump = true;
        }
    }

    private void FixedUpdate()
    {
        if (UnityJump)
        {
            UnityJump = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            IsJumping = true;
            animator.SetTrigger("Jumping");
            gameObject.GetComponent<AudioSource>().Play();
        }
        Vector3 forwardMovement = transform.forward * speed;

        var move = forwardMovement + GetMoveDirection();

        rb.MovePosition(rb.position + move * Time.deltaTime);
    }

    private void CheckHeight()
    {
        if (transform.position.y < -10)
        {
            //GameManager.Instance.Die();
        }
    }

    private void CheckSwipeRotate()
    {
        canLeftSwipe = (MultiTouch.Instance.SwipeDirection.x < 0);
        canRightSwipe = !canLeftSwipe;
        swipeTimeAccum = 0f;
    }

    private void CheckSwipeJump()
    {
        if (!IsJumping && !playerDead)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jumping");
            gameObject.GetComponent<AudioSource>().Play();
        }

        IsJumping = true;
    }

    private Vector3 GetMoveDirection()
    {
        // 회전 후의 방향을 기준으로 벽 감지
        Vector3 forwardDirection = transform.forward; // 현재 방향
        Vector3 rightDirection = transform.right; // 현재 오른쪽 방향

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        tiltInputX = Input.GetAxis("Horizontal"); // 좌우 이동

        // x축 이동에 대한 벽 감지 벽과 닿았을 시 true 리턴
        if (tiltInputX > 0 && IsWallDetected(rightDirection))
        {
            tiltInputX = 0;
        }
        else if (tiltInputX < 0 && IsWallDetected(-rightDirection))
        {
            tiltInputX = 0;
        }
#elif UNITY_ANDROID || UNITY_IOS
    tiltInputX = Input.acceleration.x; // 좌우 이동

    // x축 이동에 대한 벽 감지
    if (IsWallDetected(-rightDirection) && tiltInputX < 0)
    {
        tiltInputX = 0;
    }
    else if (IsWallDetected(rightDirection) && tiltInputX > 0)
    {
        tiltInputX = 0;
    }
#endif

        // 로우 패스 필터 적용
        float smoothingFactor = 0.1f; // 필터의 강도
        filteredTiltInputX = Mathf.Lerp(filteredTiltInputX, tiltInputX, smoothingFactor);

        Vector3 tiltMovement = new Vector3(filteredTiltInputX * tiltSpeed, 0, 0); // z축 이동 제거
        Vector3 moveDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * tiltMovement;

        return moveDirection;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetTrigger("Grounded");
            IsJumping = false;
        }
    }
    private void SpeedUP()
    {
        if (speedDistance > speedUpDistance && speedUPCount < speedUpMaxCount)
        {
            speedUPCount++;
            speedUpDistance += 50f;
            speedDistance = 0f;
            speed += plus;
        }
    }
    bool IsWallDetected(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }
}