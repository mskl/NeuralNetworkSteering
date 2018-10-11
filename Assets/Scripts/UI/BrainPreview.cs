using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrainPreview : MonoBehaviour
{

    public void GeneratePreview()
    {
        Brain brainToPreview = new Brain(GeneticSettingsPanel.Instance.numOfInputs, GeneticSettingsPanel.Instance.numOfHiddenLayers, GeneticSettingsPanel.Instance.numOfNeuronsInHiddenLayer, GeneticSettingsPanel.Instance.numOfOutputs, 0);
        DrawBrain.Instance.ShowBrainPreview(brainToPreview);
    }
    

}