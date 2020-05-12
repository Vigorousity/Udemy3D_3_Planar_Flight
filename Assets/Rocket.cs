using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float thrustSpeed = 1000f;
    [SerializeField] float rotationSpeed = 100f;

    private Rigidbody rb;
    private AudioSource audioSource;

    private enum PlayerStates { alive, dying, transcending };
    PlayerStates state = PlayerStates.alive;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == PlayerStates.alive)
        {
            ProcessThrusting();
            ProcessRotations();
        }

        else
        {
            ThrustSound(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != PlayerStates.alive)
            return;

        switch (collision.gameObject.tag)
        {
            case "Safe":
                // Do nothing
                break;

            case "Finish":
                state = PlayerStates.transcending;
                Invoke("LoadNextLevel", 1f);
                break;

            default:
                state = PlayerStates.dying;
                Invoke("ResetLevel", 1f);
                break;
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
        state = PlayerStates.alive;
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        state = PlayerStates.alive;
    }

    private void ProcessThrusting()
    {
        float thrustPerFrame = thrustSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * thrustPerFrame);
            ThrustSound(true);
        }

        else
        {
            ThrustSound(false);
        }
    }

    private void ThrustSound(bool thrusting)
    {
        if (thrusting)
        {
            audioSource.volume = 0.5f;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        else
        {
            if (audioSource.isPlaying && audioSource.volume > 0.01)
            {
                audioSource.volume *= 0.8f;
            }

            else
            {
                audioSource.Stop();
            }
        }
    }

    private void ProcessRotations()
    {
        rb.freezeRotation = true; // Gives manual control to rotations
        float rotationPerFrame = rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationPerFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationPerFrame);
        }

        rb.freezeRotation = false; // Returns control of rotations to physics engine
    }
}
