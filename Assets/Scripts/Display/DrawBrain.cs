using UnityEngine;
using System.Collections.Generic;

public class DrawBrain : MonoBehaviour {


    GameObject selectedEntity;
    Handling selectedHandling;
    public Brain brainToDraw;

    public GameObject[][] drawnNeurons;
    public bool neuronsSpawned;

    public GameObject neuronPrefab;

    public Vector2 space = new Vector2(10F, 5F);
    bool drawHandles = false;

    private static DrawBrain _instance;
    public static DrawBrain Instance { get { return _instance; }}
    void Awake() { _instance = this; }

    void Update () {
        checkInputs();
    }

    void OnGUI()
    {
        if(selectedHandling != null)
        {
            GUI.Label(new Rect(30, 90, 200, 200), "Fitness: " + selectedHandling.fitness.ToString());
        }

        if (neuronsSpawned && drawnNeurons != null)
        {

            GUIStyle st = new GUIStyle();
            st.normal.textColor = Color.black;
            st.fontStyle = FontStyle.Bold;

            for (int x = 0; x < drawnNeurons.Length; x++)
            {
                for (int y = 0; y < drawnNeurons[x].Length; y++)
                {
                    if (drawHandles)                                                                                                        // Udělej popisky
                    {
                        Vector3 handlePos = drawnNeurons[x][y].transform.position + new Vector3(0.5F, 0, -1);

                        if (Application.isEditor) {
						//	UnityEditor.Handles.Label(handlePos, System.Convert.ToString(brainToDraw.neuronLayers[x][y].output()), st);
                        }
                    }
                    Color neuronColor = new Color(1, Mathf.Clamp01((brainToDraw.neuronLayers[x][y].output() + 1) / 2), 0);                  // Změň barvu neuronu
                    drawnNeurons[x][y].GetComponent<Renderer>().material.color = neuronColor;
                }
            }
        }
    }

    // Zkontroluje inputy a v případě raycastu zvolí GameObject a jeho handling
    void checkInputs()  
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // Raycast z kamery

        if (Input.GetButtonDown("Fire1"))                               // Po stisknutí levého tlačítka
        {
            if (Physics.Raycast(ray, out hit))                          // Pokud raycast něco trefil
            {
                if (hit.rigidbody != null)
                {
                    if (hit.rigidbody.tag == "Entity")                  // A trefený GameObject má tag Entity
                    {
                        if (hit.transform.gameObject != selectedEntity) // Pokud tento objekt ještě nebyl načtený
                        {
                            ShowBrain(hit);
                        }
                    }
                }
                else // Pokud raycast nic netrefí
                {
                    HideBrain();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            drawHandles = !drawHandles;
        }

    }

    void ShowBrain(RaycastHit hit)
    {
        DeleteNeurons();

        selectedEntity = hit.transform.gameObject;
        selectedHandling = selectedEntity.GetComponent<Handling>();
        brainToDraw = selectedHandling.entityBrain;

        SpawnNeurons();
    }

    public void ShowBrainPreview(Brain _brainToPreview)
    {
        brainToDraw = _brainToPreview;

        DeleteNeurons();
        SpawnNeurons();
    }

    public void HideBrain()
    {
        DeleteNeurons();
        selectedEntity = null;
        selectedHandling = null;
        brainToDraw = null;
    }

    void SpawnNeurons()
    {
        drawnNeurons = new GameObject[brainToDraw.numOfHiddenLayers + 2][];
        for (int x = 0; x < brainToDraw.neuronLayers.Length; x++)
        {
            drawnNeurons[x] = new GameObject[brainToDraw.neuronLayers[x].Length];
            Vector3 layerAveragePos = new Vector3(0, 0, (0 + drawnNeurons[x].Length * space.y) / 2);
            for (int y = 0; y < brainToDraw.neuronLayers[x].Length; y++)
            {
                
                drawnNeurons[x][y] = Instantiate(neuronPrefab, new Vector3(x * space.x, 0, y * space.y) + transform.position - layerAveragePos, Quaternion.identity) as GameObject;
                drawnNeurons[x][y].transform.SetParent(transform);
            }
        }
        neuronsSpawned = true;
    }

    public void DeleteNeurons()
    {
        foreach(Transform t in transform)   // Pro všechny child GameObjecty
        {
            Destroy(t.gameObject);
        }

        neuronsSpawned = false;
    }


    /*
     *  ////////////// Drawing lines using GL /////////////
     *  
     *  
     */


    public void OnRenderObject()
    {
        if (neuronsSpawned)
        {
            preDrawLine(Color.white);

            if(selectedHandling != null)
            {
                drawSquare(selectedEntity.transform.position, 3F);
            }
            
            for (int a = 1; a < drawnNeurons.Length; a++)
            {
                for (int b = 0; b < drawnNeurons[a].Length; b++)
                {
                    for (int c = 0; c < drawnNeurons[a - 1].Length; c++)
                    {
                        drawLine(drawnNeurons[a][b].transform.position, drawnNeurons[a - 1][c].transform.position);
                    }
                }
            }

            postDrawLine();
        }
    }


    static Material lineMaterial;
    public static void preDrawLine(Color col)
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }

        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();

        //GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);

        GL.Color(col);
    }
    public static void drawLine(Vector3 start, Vector3 end)
    {
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
    }

    public static void drawLine(Vector3 start, Vector3 end, Color col)
    {
        GL.Color(col);

        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
    }

    public static void drawSquare(Vector3 center, float diameter)
    {
        Vector3 p1 = new Vector3(center.x + diameter / 2, 0, center.z + diameter / 2);
        Vector3 p2 = new Vector3(center.x + diameter / 2, 0, center.z - diameter / 2);
        Vector3 p3 = new Vector3(center.x - diameter / 2, 0, center.z - diameter / 2);
        Vector3 p4 = new Vector3(center.x - diameter / 2, 0, center.z + diameter / 2);

        drawLine(p1, p2);
        drawLine(p2, p3);
        drawLine(p3, p4);
        drawLine(p4, p1);
    }

    public static void postDrawLine()
    {
        GL.End();
        GL.PopMatrix();
    }

}