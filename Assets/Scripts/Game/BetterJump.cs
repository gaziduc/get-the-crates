using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class BetterJump : MonoBehaviour
{
    private float fallMultiplier = 2.4f;
    private float lowJumpMultiplier = 10f;

    private Rigidbody2D rb;
    private Bot bot;

    private KeyCode upKey;
    private string upKeyGamepad;

    private PhotonView view;
    [SerializeField] private bool isBot = false;
    private Joystick joystick;
    private GameObject joystickGameobject;
    
    // Start is called before the first frame update
    void Start()
    {
        upKey = (KeyCode) PlayerPrefs.GetInt(Options.Controls.Jump.ToString(), (int) System.Enum.Parse(typeof(KeyCode), Options.instance.defaultControls[(int) Options.Controls.Jump]));
        upKeyGamepad = PlayerPrefs.GetString(Options.Controls.Jump.ToString() + "Controller", Options.instance.defaultControlsGamepad[(int) Options.Controls.Jump]);

        joystickGameobject = GameObject.FindWithTag("Joystick");
        if (joystickGameobject != null)
            joystick = joystickGameobject.GetComponent<Joystick>();
        
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        if (isBot)
            bot = GetComponent<Bot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0)
            {
                if (isBot && bot.direction.y < -0.1f)
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                else if (!isBot && !Input.GetKey(upKey) && (Gamepad.current == null || !Gamepad.current[upKeyGamepad].IsPressed()) && (joystick == null || joystick.Vertical < 0.7f))
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }
}
