using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    Vector3 startingPosition;

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    float movementFactor;

    [SerializeField] float period = 2f;

    const float tau = Mathf.PI * 2f;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the startingPosition variable to the game object's starting position
        startingPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (period <= Mathf.Epsilon) // Epsilon is the smallest floating point value - should be used instead of direct comparison to 0
            return;

        float cycles = Time.time / period;

        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        this.transform.position = startingPosition + offset;

        
    }
}
