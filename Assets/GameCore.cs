using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCore : MonoBehaviour
{
    public Rigidbody2D Sword;
    public float ForceMin = 30f;
    public float ForceMax = 100f;
    public float m_timerClick = 0;
    public Image ImageSword;
    static int s_currentLevel;
    public Level CurrentLevel;
    public List<Level> Levels;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = Levels[s_currentLevel];
        CurrentLevel.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_timerClick += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_timerClick > 1.0f)
                m_timerClick = 1;

            float force = Mathf.Lerp(ForceMin, ForceMax, m_timerClick);

            Sword.simulated = true;
            Sword.AddForce(new Vector2(force, force), ForceMode2D.Impulse);
        }

        if (Sword.velocity.magnitude > 4)
        {
            float angle = Mathf.Atan2(Sword.velocity.y, Sword.velocity.x) * Mathf.Rad2Deg;
            Sword.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        ImageSword.fillAmount = m_timerClick;

        int heroAlive = 0;
        foreach (var item in CurrentLevel.Heroes)
        {
            if (item != null)
                heroAlive++;
        }

        if (heroAlive == 0)
        {
            s_currentLevel++;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            return;
        }
    }

    public static Vector2[] PreviewPhysics(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];

        float timestep = Time.fixedDeltaTime;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
        }

        return results;
    }

    private void OnDrawGizmos()
    {
        Vector2[] pointsMin = PreviewPhysics(Sword, Sword.transform.position, new Vector2(ForceMin / Sword.mass, ForceMin / Sword.mass), 200);

        foreach (var item in pointsMin)
        {
            Gizmos.DrawSphere(item, 0.05f);
        }

        Vector2[] pointsMax = PreviewPhysics(Sword, Sword.transform.position, new Vector2(ForceMax / Sword.mass, ForceMax / Sword.mass), 200);

        foreach (var item in pointsMax)
        {
            Gizmos.DrawSphere(item, 0.05f);
        }
    }
}
