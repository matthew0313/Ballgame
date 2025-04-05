using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Player : MonoBehaviour, ISavable
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float maxDragDistance;
    [SerializeField] float jumpForce;
    [SerializeField] AnimationCurve jumpCurve;

    [Header("Display")]
    [SerializeField] Transform arrow;
    [SerializeField] SpriteRenderer arrowSprite;
    [SerializeField] Color arrowColorLow, arrowColorHigh;
    [SerializeField] TrailRenderer trail;
    

    [Header("Audio")]
    [SerializeField] AudioSource jumpAudio;

    [Header("Debug")]
    [SerializeField] string FSMPath;
    Rigidbody2D rb;
    Player_TopLayer topLayer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        topLayer = new Player_TopLayer(this, new PlayerFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();
    }
    private void Update()
    {
        topLayer.OnStateUpdate();
    }
    bool isGrounded => collidingGrounds.Count > 0;
    List<Collider2D> collidingGrounds = new();
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.GetContact(0).point.y < transform.position.y - 0.05f)
        {
            if(!collidingGrounds.Contains(collision.collider))
            {
                collidingGrounds.Add(collision.collider);
            }
        }
        else
        {
            collidingGrounds.Remove(collision.collider);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collidingGrounds.Remove(collision.collider);
    }

    public void Save(SaveData data)
    {
        data.playerPosition = transform.position;
        data.playerVelocity = rb.linearVelocity;
        data.playerAngularVelocity = rb.angularVelocity;
    }

    public void Load(SaveData data)
    {
        transform.position = data.playerPosition;
        rb.linearVelocity = data.playerVelocity;
        rb.angularVelocity = data.playerAngularVelocity;
        trail.Clear();
    }

    class PlayerFSMVals : FSMVals { }
    class Player_TopLayer : TopLayer<Player, PlayerFSMVals>
    {
        public Player_TopLayer(Player origin, PlayerFSMVals values) : base(origin, values)
        {
            defaultState = new Airborne(origin, this);
            AddState("Airborne", defaultState);
            AddState("Grounded", new Grounded(origin, this));
        }
        class Grounded : Layer<Player, PlayerFSMVals>
        {
            public Grounded(Player origin, Layer<Player, PlayerFSMVals> parent) : base(origin, parent)
            {
                defaultState = new Idle(origin, this);
                AddState("Idle", defaultState);
                AddState("Dragging", new Dragging(origin, this));
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if(!origin.isGrounded) parentLayer.ChangeState("Airborne");
            }
            class Idle : State<Player, PlayerFSMVals>
            {
                public Idle(Player origin, Layer<Player, PlayerFSMVals> parent) : base(origin, parent) { }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (Input.GetMouseButtonDown(0) && !GameManager.Instance.paused) parentLayer.ChangeState("Dragging");
                }
            }
            class Dragging : State<Player, PlayerFSMVals>
            {
                public Dragging(Player origin, Layer<Player, PlayerFSMVals> parent) : base(origin, parent) { }
                Vector2 startPos;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    startPos = Input.mousePosition;
                    Vector2 tmp = Vector2.ClampMagnitude((Camera.main.ScreenToWorldPoint(startPos) - Camera.main.ScreenToWorldPoint(Input.mousePosition)), origin.maxDragDistance);
                    origin.arrow.up = tmp.normalized;
                    origin.arrow.localScale = new Vector2(1.0f, tmp.magnitude);
                    origin.arrow.gameObject.SetActive(true);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if(GameManager.Instance.paused)
                    {
                        parentLayer.ChangeState("Idle");
                        return;
                    }
                    Vector2 tmp = Vector2.ClampMagnitude((Camera.main.ScreenToWorldPoint(startPos) - Camera.main.ScreenToWorldPoint(Input.mousePosition)), origin.maxDragDistance);

                    origin.arrow.up = tmp.normalized;
                    origin.arrow.localScale = new Vector2(1.0f, tmp.magnitude);
                    origin.arrowSprite.color = Color.Lerp(origin.arrowColorLow, origin.arrowColorHigh, tmp.magnitude / origin.maxDragDistance);

                    if (!Input.GetMouseButton(0))
                    {
                        origin.jumpAudio.Play();
                        origin.rb.angularVelocity = 0.0f;
                        origin.rb.linearVelocity = tmp.normalized * origin.jumpForce * origin.jumpCurve.Evaluate(tmp.magnitude / origin.maxDragDistance);
                        parentLayer.ChangeState("Idle");
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.arrow.gameObject.SetActive(false);
                }
            }
        }
        class Airborne : State<Player, PlayerFSMVals>
        {
            public Airborne(Player origin, Layer<Player, PlayerFSMVals> parent) : base(origin, parent) { }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if(origin.isGrounded) parentLayer.ChangeState("Grounded");
            }
        }
    }
}