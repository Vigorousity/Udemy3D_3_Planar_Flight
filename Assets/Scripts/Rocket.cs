using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float thrustSpeed = 1000f;
    [SerializeField] float rotationSpeed = 100f;

    [SerializeField] float levelLoadDelay = 3f;

    [SerializeField] AudioClip thrustAudioClip;
    [SerializeField] AudioClip deathExplosionAudioClip;
    [SerializeField] AudioClip levelCompleteAudioClip;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem deathExplosionParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;

    private Rigidbody rb;

    private AudioSource audioSource;

    private enum PlayerStates { alive, dying, transcending }; // Creates an enumerator with 3 possible values for states
    PlayerStates state = PlayerStates.alive; // Sets the initial state

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the current state == alive before processing thrust
        if (state == PlayerStates.alive)
        {
            ProcessThrusting();
            ProcessRotations();
        }

        // If state != alive, stop the thrusting sounds and particles
        else
        {
            ThrustSound(false);
            thrustParticles.Stop();
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
                LevelComplete();
                break;

            default:
                Death();
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
            thrustParticles.Play();
        }

        else
        {
            ThrustSound(false);
            thrustParticles.Stop();
        }
    }

    private void ThrustSound(bool thrusting)
    {
        if (state != PlayerStates.alive)
            return;

        if (thrusting)
        {
            audioSource.volume = 0.5f;

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(thrustAudioClip);
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

    private void Death()
    {
        audioSource.Stop();
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(deathExplosionAudioClip);
        deathExplosionParticles.Play();
        state = PlayerStates.dying;
        Invoke("ResetLevel", levelLoadDelay);
    }

    private void LevelComplete()
    {
        audioSource.Stop();
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(levelCompleteAudioClip);
        levelCompleteParticles.Play();
        state = PlayerStates.transcending;
        Invoke("LoadNextLevel", levelLoadDelay);
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
