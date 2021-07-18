using UnityEngine;

public class BetterJump : MonoBehaviour
{
    private float fallMultiplier = 2.4f;
    private float lowJumpMultiplier = 10f;

    private Rigidbody2D rb;

    private KeyCode upKey;
    
    // Start is called before the first frame update
    void Start()
    {
        upKey = (KeyCode) PlayerPrefs.GetInt(Options.Controls.Jump.ToString(), (int) System.Enum.Parse(typeof(KeyCode), Options.instance.defaultControls[(int) Options.Controls.Jump]));
        
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(upKey) && !Input.GetKey(KeyCode.Joystick1Button0))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
