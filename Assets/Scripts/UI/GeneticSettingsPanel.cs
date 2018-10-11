using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;


public class GeneticSettingsPanel : MonoBehaviour {

    /*
     * GeneticSettingsPanel
     * 
     * tenhle skript ovládá sdílení dat z geneticSettingsPanel
     * přes singleton.
     * 
     */


    public InputField InputField_numOfInputs;
    public int numOfInputs
    {
        get
        {
            return Convert.ToInt32(InputField_numOfInputs.text);
        }
    }


    public InputField InputField_numOfOutputs;
    public int numOfOutputs
    {
        get
        {
            return Convert.ToInt32(InputField_numOfOutputs.text);
        }
    }

    public InputField InputField_numOfHiddenLayers;
    public int numOfHiddenLayers
    {
        get
        {
            return Convert.ToInt32(InputField_numOfHiddenLayers.text);
        }
    }

    public InputField InputField_numOfNeuronsInHiddenLayer;
    public int numOfNeuronsInHiddenLayer
    {
        get
        {
            return Convert.ToInt32(InputField_numOfNeuronsInHiddenLayer.text);
        }
    }

    public Toggle Toggle_useRandomSeed;
    public bool useRandomSeed
    {
        get
        {
            return Toggle_useRandomSeed.isOn;
        }
    }

    public InputField InputField_seed;
    public int seed
    {
        get
        {
            return Convert.ToInt32(InputField_seed.GetHashCode());
        }
    }

    public InputField InputField_mutationChance;
    public float mutationChance
    {
        get
        {
            return Mathf.Clamp01((float)(Convert.ToDouble(InputField_mutationChance.text)));
        }
    }

    public InputField InputField_numOfEntities;
    public int numOfEntities
    {
        get
        {
            return Mathf.Clamp(Convert.ToInt32(InputField_numOfEntities.text), 1, 100000);
        }
    }

    private static GeneticSettingsPanel _instance;
    public static GeneticSettingsPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

}
