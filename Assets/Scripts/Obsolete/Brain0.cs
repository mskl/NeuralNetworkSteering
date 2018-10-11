using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brain0 {

    /*
     *  Hlavní schéma:
     *  Inputs >>> process >>> outputs  
     *  
     */

    //Neuron[] hiddenLayer1;
    //Neuron[] hiddenLayer2;
    //Neuron[] output;

    public Neuron[][] neuronLayers;    // jagged array neureonLayers[početLayerů][početNeuronůVLayeru]
    public int inputCount;

     // 3 inputs
     // 2 outpus
     // 2 hidden layers
     // 9 neurons in each hidden layer

    public Brain0(int numOfInputs, int numOfHiddenLayers, int numOfNeuronsInHiddenLayers, int numOfOutputs, int seed)
    {
        // layer 0 ... první hidden, layer 1 ... druhý hidden...
        // layer[lenght] ... output layer

        System.Random rnd = new System.Random(seed);

        inputCount = numOfInputs;

        neuronLayers = new Neuron[numOfHiddenLayers + 1][]; // +1 pro output layer

        for(int i = 0; i < neuronLayers.Length; i++)        // Projeď všechny layery včetně outputu
        {
            // nultý layer který bere z inputů
            if (i == 0)                                                     
            {
                neuronLayers[i] = new Neuron[numOfNeuronsInHiddenLayers];   // V nultém layeru (i = 0) vytvoř počet neuronů, který je v hidden (numOfNeuronsInHiddenLayers)
                for (int y = 0; y < neuronLayers[i].Length; y++)            // Každý neuron v nultém layeru (i = 0) projdi
                {
                    neuronLayers[i][y] = new Neuron(numOfInputs, rnd.Next(int.MinValue, int.MaxValue));           // a inicializuj. Neuron v nultém layeru bude mít počet vstupů stejně jako inputs..
                }
            }
            // Pokud se jedná o poslední (output) layer
            else if (i == neuronLayers.Length - 1)                          
            {
                neuronLayers[i] = new Neuron[numOfOutputs];   
                for (int y = 0; y < neuronLayers[i].Length; y++)            
                {
                    neuronLayers[i][y] = new Neuron(neuronLayers[i - 1].Length, rnd.Next(int.MinValue, int.MaxValue));    // Vstupem je počet neuronů z předchozího layeru
                }
            }
            // Pokud se jedná o layer, který není ani první, ani poslední
            else  
            {
                neuronLayers[i] = new Neuron[numOfHiddenLayers];                    // počet neuronů v layeru (== numOfNeuronsInHiddenLayers)
                for (int y = 0; y < neuronLayers[i].Length; y++)            
                {
                    neuronLayers[i][y] = new Neuron(neuronLayers[i - 1].Length, rnd.Next(int.MinValue, int.MaxValue));    // Vstupem bude počet neuronů z předchozího layeru     
                }
            }

        }


    }

    //public Brain(int numOfInputs, int numOfHiddenLayers, int numOfNeuronsInHiddenLayers, int numOfOutputs, int seed)
    //{
    //    // layer 0 ... první hidden, layer 1 ... druhý hidden...
    //    // layer[lenght] ... output layer

    //    System.Random rnd = new System.Random(seed);

    //    inputCount = numOfInputs;

    //    neuronLayers = new Neuron[numOfHiddenLayers + 1][]; // +1 pro output layer

    //    for (int i = 0; i < neuronLayers.Length; i++)        // Projeď všechny layery včetně outputu
    //    {
    //        // nultý layer který bere z inputů
    //        if (i == 0)
    //        {
    //            neuronLayers[i] = new Neuron[numOfNeuronsInHiddenLayers];   // V nultém layeru (i = 0) vytvoř počet neuronů, který je v hidden (numOfNeuronsInHiddenLayers)
    //            for (int y = 0; y < neuronLayers[i].Length; y++)            // Každý neuron v nultém layeru (i = 0) projdi
    //            {
    //                neuronLayers[i][y] = new Neuron(numOfInputs, rnd.Next(int.MinValue, int.MaxValue));           // a inicializuj. Neuron v nultém layeru bude mít počet vstupů stejně jako inputs..
    //            }
    //        }
    //        // Pokud se jedná o poslední (output) layer
    //        else if (i == neuronLayers.Length - 1)
    //        {
    //            neuronLayers[i] = new Neuron[numOfOutputs];
    //            for (int y = 0; y < neuronLayers[i].Length; y++)
    //            {
    //                neuronLayers[i][y] = new Neuron(neuronLayers[i - 1].Length, rnd.Next(int.MinValue, int.MaxValue));    // Vstupem je počet neuronů z předchozího layeru
    //            }
    //        }
    //        // Pokud se jedná o layer, který není ani první, ani poslední
    //        else
    //        {
    //            neuronLayers[i] = new Neuron[numOfHiddenLayers];                    // počet neuronů v layeru (== numOfNeuronsInHiddenLayers)
    //            for (int y = 0; y < neuronLayers[i].Length; y++)
    //            {
    //                neuronLayers[i][y] = new Neuron(neuronLayers[i - 1].Length, rnd.Next(int.MinValue, int.MaxValue));    // Vstupem bude počet neuronů z předchozího layeru     
    //            }
    //        }

    //    }


    //}

    public float process(float[] inputs)
    {

        // Vlož inputy do nultého layeru
        foreach (Neuron n in neuronLayers[0])
        {
            n.inputs = inputs;
        }


        // Pro každý další layer (než 0tý) vygeneruj hodnoty
        for(int i = 1; i < neuronLayers.Length; i++)
        {
            foreach(Neuron n in neuronLayers[i])
            {
                float[] valuesFromNeuronFromPrevLayer = new float[neuronLayers[i-1].Length];

                for (int x = 0; x < neuronLayers[i].Length; x++)
                {
                    valuesFromNeuronFromPrevLayer[x] = neuronLayers[i - 1][x].output();

                }

                n.inputs = valuesFromNeuronFromPrevLayer;
            }
        }

        return Mathf.Clamp(neuronLayers[neuronLayers.Length-1][1].output() - neuronLayers[neuronLayers.Length-1][0].output(), -1, 1);
    }


}



//hiddenLayer1 = new Neuron[9];
//for(int x = 0; x < hiddenLayer1.Length; x++)
//{
//    hiddenLayer1[x] = new Neuron(3);
//}

//hiddenLayer2 = new Neuron[9];
//for (int y = 0; y < hiddenLayer1.Length; y++)
//{
//    hiddenLayer2[y] = new Neuron(9);
//}
//output = new Neuron[2];
//for (int z = 0; z < output.Length; z++)
//{
//    output[z] = new Neuron(9);
//}



//foreach(Neuron n in hiddenLayer1)
//{
//    n.inputs = inputs;
//}

//foreach(Neuron n in hiddenLayer2)
//{
//    float[] valuesFromNeuronFromPrevLayer = new float[9];

//    for(int x = 0; x < hiddenLayer1.Length; x++)
//    {
//        valuesFromNeuronFromPrevLayer[x] = hiddenLayer1[x].output();

//    }


//    n.inputs = valuesFromNeuronFromPrevLayer;
//}

//foreach(Neuron n in output)
//{
//    float[] valuesFromNeuronFromPrevLayer = new float[9];

//    for (int x = 0; x < hiddenLayer2.Length; x++)
//    {
//        valuesFromNeuronFromPrevLayer[x] = hiddenLayer2[x].output();
//    }

//    n.inputs = valuesFromNeuronFromPrevLayer;
//}

//// [0] = vlevo
//// [1] = vpravo

//// vpravo = 1
//// vlevo = -1;

//return Mathf.Clamp(output[1].output() - output[0].output(), -1, 1);