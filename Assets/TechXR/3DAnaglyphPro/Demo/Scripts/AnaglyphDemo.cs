using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("")]
public class AnaglyphDemo : MonoBehaviour
{
    public Transform anaglyphTransform;
    public Text anaglyphText;
    public Transform[] spinners;
    public Transform cam;
    public float moveSpeed = 8.0f;
    public float lookSpeed = 8.0f;

    private AnaglyphCamera anaglyph;
    private bool lookMode = true;
    private float m_Timer = 0.0f;
    private Vector2 m_Angle;

    void Update()
    {
        m_Timer += Time.deltaTime;

        // Rotate the text.
        foreach (Transform t in spinners)
        {
            t.localEulerAngles = new Vector3(0.0f, m_Timer * 90.0f, 0.0f);
        }

        PlayerMovement();

        AnaglyphSetting();
    }

    void PlayerMovement()
    {
        if (lookMode)
        {
            m_Angle.x += Input.GetAxis("Mouse X") * lookSpeed;
            m_Angle.y -= Input.GetAxis("Mouse Y") * lookSpeed;
        }

        if (Input.GetKeyDown(KeyCode.L)) lookMode ^= true;

        if (m_Angle.y < -360.0f) m_Angle.y += 360.0f;
        if (m_Angle.y > 360.0f) m_Angle.y -= 360.0f;
        m_Angle.y = Mathf.Clamp(m_Angle.y, -90.0f, 90.0f);

        cam.rotation = Quaternion.Slerp(cam.rotation, Quaternion.Euler(m_Angle.y, m_Angle.x, 0.0f), Time.deltaTime * 10.0f);

        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movement.z += 1.0f;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1.0f;
        if (Input.GetKey(KeyCode.S)) movement.z -= 1.0f;
        if (Input.GetKey(KeyCode.D)) movement.x += 1.0f;
        if (Input.GetKey(KeyCode.Space))     movement.y += 1.0f;
        if (Input.GetKey(KeyCode.LeftShift)) movement.y -= 1.0f;
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        Vector3 nextvelo = Vector3.zero;
        nextvelo += cam.right * movement.x;
        nextvelo += (new Vector3(cam.forward.x, 0.0f, cam.forward.z)).normalized * movement.z;
        nextvelo += Vector3.up * movement.y;
        cam.position += nextvelo;
    }

    void AnaglyphSetting()
    {
        if (!anaglyphTransform) return;

        if (!anaglyph)
        {
            if (anaglyphText) anaglyphText.text = "";

            anaglyph = anaglyphTransform.GetComponent<AnaglyphCamera>();
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            anaglyph.eyeSeparation = Mathf.Max(anaglyph.eyeSeparation + Time.deltaTime, 0.0f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            anaglyph.eyeSeparation = Mathf.Max(anaglyph.eyeSeparation - Time.deltaTime, 0.0f);
        }
        anaglyph.convergence += Input.GetAxis("Mouse ScrollWheel") * Mathf.Min(anaglyph.convergence + 1.0f, 75.0f);
        if (anaglyph.convergence <= 0.05f) anaglyph.convergence = 0.05f;

        if (anaglyphText)
        {
            anaglyphText.text =
                $"Convergence Distance: {anaglyph.convergence.ToString("F2")}\n" +
                $"Eye Separation: {anaglyph.eyeSeparation.ToString("F2")}";
        }
    }
}
