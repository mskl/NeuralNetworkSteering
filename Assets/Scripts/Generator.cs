using UnityEngine;
using System.Collections.Generic;

public class Generator : MonoBehaviour {

    /*
     *  Generátor je generátor pro GA
     * 
     * Postup:
     *  1) vytvoř jednotlivce s random weights
     *  2) nech je žít a vyhodnoť jejich fitness
     *  3) vyhodnoť jejich fitness
     *  4) spař je podle fitness
     *  5) další generace
     * 
     */



    public GameObject entity;

    [Header("Genetic algorithm settings")]
    public int GA_NumOfEntitiesInGeneration;
    public float GA_MutationRateInPercent01 = 0.03F;


    /* Nastavení */
    int brain_numOfInputs = 3;
    int brain_numOfHiddenLayers = 4;
    int brain_numOfNeuronsInHiddenLayer = 9;
    int brain_numOfOutputs = 2;

    [Header("Seed settings")]
    public int globalSeed;
    int seedIterator = 0;

    [HideInInspector()]
    public List<GameObject> entityList;

    int tickCounter = 0;
    int generation = 0;

    public bool GeneratorEnabled = false;

    private static Generator _instance;
    public static Generator Instance { get { return _instance; } }
    void Awake() { _instance = this; }

    // Use this for initialization
    void Start () {
        entityList = new List<GameObject>();    // Inicializuj entityList
	}
    void FixedUpdate()
    {
        if (GeneratorEnabled)
        {
            tickCounter++;

            if (tickCounter == 1000|| Input.GetKeyDown(KeyCode.N))
            {
                CreateNextGenerationAndKillPrevious();
                tickCounter = 0;
                generation++;
                Debug.Log("Generation: " + generation);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Enable();
            }
        }
    }

    public void Enable()
    {
        DrawBrain.Instance.HideBrain();

        destroyAllEntities();

        GeneratorEnabled = true;

        brain_numOfInputs = GeneticSettingsPanel.Instance.numOfInputs;
        brain_numOfOutputs = GeneticSettingsPanel.Instance.numOfOutputs;
        brain_numOfHiddenLayers = GeneticSettingsPanel.Instance.numOfHiddenLayers;
        brain_numOfNeuronsInHiddenLayer = GeneticSettingsPanel.Instance.numOfNeuronsInHiddenLayer;

        GA_MutationRateInPercent01 = GeneticSettingsPanel.Instance.mutationChance;
        GA_NumOfEntitiesInGeneration = GeneticSettingsPanel.Instance.numOfEntities;

        if (GeneticSettingsPanel.Instance.useRandomSeed)
        {
            globalSeed = System.DateTime.Now.GetHashCode();
        }
        else
        {
            globalSeed = GeneticSettingsPanel.Instance.seed;
        }

        FirstGenerate();
    }

    public void Disable()
    {
        GeneratorEnabled = false;
        destroyAllEntities();
    }

    public void CreateNextGenerationAndKillPrevious()   // Tady probíhá iterace jednotlivých generací
    {
        DrawBrain.Instance.HideBrain();

        var newEntityBrainList = Genetic.ChildrenBrainList(Functions.EntitiesToBrainDictionary(entityList), GA_MutationRateInPercent01, globalSeed + seedIterator);
        destroyAllEntities();

        GenerateFromBrains(newEntityBrainList);
    }

    public void destroyAllEntities()
    {
        foreach (GameObject go in entityList)
        {
            go.GetComponent<Handling>().Deactivate();
            Destroy(go);
        }

        entityList.Clear();

        foreach (Transform t in GameObject.Find("BrainDrawer").transform)   // Pro všechny child GameObjecty
        {
            Destroy(t.gameObject);
        }
    }

    public void FirstGenerate()
    {
        for (int x = 0; x < GA_NumOfEntitiesInGeneration; x++)
        {
            GameObject ga = Instantiate(entity, transform.position, Quaternion.Euler(0, -90, 0)) as GameObject;     // vygeneruj entity na start
            ga.transform.SetParent(transform);                                                                      // nastav parent transform (kvůli přehlednosti)

            Brain newBrain = new Brain(brain_numOfInputs, brain_numOfHiddenLayers, brain_numOfNeuronsInHiddenLayer, brain_numOfOutputs, globalSeed + seedIterator); // Inicializuj mozek

            ga.GetComponent<Handling>().Initialise(globalSeed + seedIterator, newBrain);                            // aktivuj entitu

            entityList.Add(ga);                                                                                     // přidej do listu
            seedIterator++;
        }
    }

    public void GenerateFromBrains(List<Brain> _newBrains)
    {
        foreach(Brain br in _newBrains)
        {
            GameObject ga = Instantiate(entity, transform.position, Quaternion.Euler(0, -90, 0)) as GameObject;     // vygeneruj entity na start
            ga.transform.SetParent(transform);                                                                      // nastav parent transform (kvůli přehlednosti)

            ga.GetComponent<Handling>().Initialise(globalSeed + seedIterator, br);                                  // aktivuj entitu

            entityList.Add(ga);                                                                                     // přidej do listu
            seedIterator++;
        }
    }
	
}
