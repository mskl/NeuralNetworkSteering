using UnityEngine;
using System.Collections;
using System;

/*
 * Tenhle skript se stará o zatáčení a o inicializaci mozku
 * 
 */

public class Handling : MonoBehaviour {

    [HideInInspector()]
    public int seed;    // Seed se nastaví z generátoru
    [HideInInspector()]
    public bool isAlive = false;

    public float fitness = 0;

    public Vector3 middleEndpoint;
    public Vector3 middleVector;
    public Vector3 leftEndpoint;
    public Vector3 leftVector;
    public Vector3 rightEndpoint;
    public Vector3 rightVector;

    LayerMask wallLayer;
    public Brain entityBrain;

    float vektorRozevreni = 1.3F;   // definuje vektor roztažení očí
    float range = 10F;              // Dosah senzorů
    float turnSpeed = 3;            // otáčení v stupních/tick
    float fwSpeed = 0.2F;           // rychlost soustavného pohybu vpřed

    // Tato funkce se volá z cizího skriptu, aby nahradila Start(). Slouží k předání seedu před inicializací.
    public void Initialise(int _seed, Brain _brain)
    {
        seed = _seed;
        entityBrain = _brain;

        GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);    // Nastaví se random barva

        wallLayer = 1 << 8;                                                                                 // Bitová maska pro layer 8 (wallLayer má id 8)

        isAlive = true;                                                                                     // Oživ
        prevPos = transform.position;                                                                       // nastav pozici pro vyhodnocení fitness
    }

    public void Deactivate()                                            // Deaktivuje objekt a přiřadí mozku fitness
    {
        if (isAlive)
        {
            isAlive = false;                                            // "zabije objekt"
            entityBrain.fitness = Mathf.Pow(fitness, 2);                // Fitness funkce je x^2
        }

        Color currentColor = GetComponent<Renderer>().material.color;   // Změní barvu objektu
        float grayScale = ((currentColor.r * 0.2989F) + (currentColor.g * 0.5870F) + (currentColor.b * 0.1140F)); // Vytvoř BW z RGB
        Color newColor = new Color(grayScale, grayScale, grayScale);
        GetComponent<Renderer>().material.color = newColor;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")                  // Zemři po střetnutí se zdí
        {
            Deactivate();
        }
        if (collision.gameObject.tag == "FinishLine")              // Získá bonus pokud dojede do cíle
        {
            fitness *= 2;
            Deactivate();
        }
    }

    Ray[] sightRay;
    RaycastHit[] sightRayHit;
    float[] sightRayDistance;
    float[] processedInputsForBrain;

    Vector3 prevPos;
    void FixedUpdate () {
        if (isAlive == true)    // Alive začíná defaultně na false! Oživuje se až funkcí Initialise()!
        {
            sightRay = new Ray[entityBrain.numOfInputs];
            sightRayHit = new RaycastHit[sightRay.Length];
            sightRayDistance = new float[sightRay.Length];
            processedInputsForBrain = new float[entityBrain.numOfInputs];

            // vytoř raye pro raycasting
            for (int i = 0; i < sightRay.Length; i++)   // Bude fungovat jen pro lichý počet!
            {
                Vector3 rayStart = transform.position + transform.forward * transform.localScale.z / 2 + (i - 1) * (transform.right * transform.localScale.x / 2);
                Vector3 rayVector = transform.forward + (i - 1) * transform.right / vektorRozevreni;
                sightRay[i] = new Ray(rayStart, rayVector);
            }

            // raycastuj jednotlivé raye
            for (int i = 0; i < sightRay.Length; i++)
            {
                sightRayDistance[i] = range;

                if (Physics.Raycast(sightRay[i], out sightRayHit[i], range, wallLayer))
                {
                    sightRayDistance[i] = sightRayHit[i].distance;
                }
            }

            // procestuj raycastlé ouputy pro mozek
            for (int i = 0; i < sightRay.Length; i++)
            {
                processedInputsForBrain[i] = normaliseInput(sightRayDistance[i], range);
            }

            // Výstup z mozku
            //float processed = entityBrain.process(processedInputsForBrain);
            float[] outputs = entityBrain.process(processedInputsForBrain);

            float steering = outputs[0];
            float speeding = (outputs[1]) + 1f;//* 2;

            // print(processed + "\tL: " + leftDist + " M: " + middleDist + " R: " + rightDist);

            //if (Input.GetAxis("Horizontal") != 0)
            //{
            //    transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed, 0);
            //}

            // Posuň dopředu a zatoč
            transform.Translate(Vector3.forward * fwSpeed * speeding);
            transform.Rotate(0, steering * turnSpeed, 0);

            addFitness();
        }
    }

    float normaliseInput(float inp, float range)
    {
        float ret = Mathf.Clamp01(inp / range);

        ret = ret * 2 - 1;
        return ret;
    }

    void addFitness()
    {
        fitness += (transform.position - prevPos).magnitude;
        prevPos = transform.position;
    }

    public void OnRenderObject()    // Nakresli lines of sight
    {
        if (isAlive && sightRay != null)
        {
            DrawBrain.preDrawLine(Color.yellow);
            for (int i = 0; i < sightRay.Length; i++)
            {
                if (sightRayDistance[i] != range)
                {
                    Color col = new Color(1, Mathf.Clamp01((processedInputsForBrain[i] + 1) / 2), 0);
                    //Color col = new Color(1, Mathf.Clamp01((entityBrain.neuronLayers[0][i].output() + 1) / 2), 0);
                    DrawBrain.drawLine(sightRay[i].origin, sightRay[i].origin + sightRay[i].direction.normalized * sightRayDistance[i], col);
                }
            }
            DrawBrain.postDrawLine();
        }
    }
}