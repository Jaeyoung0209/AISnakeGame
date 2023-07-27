using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNet
{
    public int inputneurons = 32;
    public int outputneurons = 3;
    public int firsthiddenneuroncount = 20;
    public int secondhiddenneuroncount = 12;

    public List<float> firstlayer = new List<float>();
    public List<float> secondlayer = new List<float>();

    public List<float> weight1 = new List<float>();
    public List<float> weight2 = new List<float>();
    public List<float> weight3 = new List<float>();
    public List<float> biases = new List<float>();

    public void Initialize()
    {
        weight1.Clear();
        weight2.Clear();
        weight3.Clear();
        biases.Clear();

        for(int i = 0; i<inputneurons; i++)
        {
            for(int j = 0; j<firsthiddenneuroncount; j++)
            {
                weight1.Add(Random.Range(-1f, 1f));
            }
        }
        for(int i = 0; i<firsthiddenneuroncount; i++)
        {
            for(int j=0; j<secondhiddenneuroncount; j++)
            {
                weight2.Add(Random.Range(-1f, 1f));
            }
        }

        for (int i = 0; i < secondhiddenneuroncount; i++)
        {
            for (int j = 0; j < outputneurons; j++)
            {
                weight3.Add(Random.Range(-1f, 1f));
            }
        }


        biases.Add(Random.Range(-0.5f, 0.5f));
        biases.Add(Random.Range(-0.5f, 0.5f));
        biases.Add(Random.Range(-0.5f, 0.5f));

    }

    public List<float> Runnetwork(List<float> inputs)
    {
        List<float> outputlayer = new List<float>();
        List<float> templayer = new List<float>();
        int weightcounter = 0;

        outputlayer.Clear();
        firstlayer.Clear();
        secondlayer.Clear();

        for(int i = 0; i<firsthiddenneuroncount; i++)
        {
            weightcounter = i;
            for (int j = 0; j < inputs.Count; j++)
            {
                templayer.Add(inputs[j] * weight1[weightcounter] + biases[0]);
                weightcounter += firsthiddenneuroncount;
            }
        }

        int counter = 0;
        float tempint = 0;
        for (int i = 0; i<templayer.Count; i ++)
        {
            counter++;
            tempint += templayer[i];

            if (counter == inputneurons)
            {
                firstlayer.Add(relu(tempint));
                tempint = 0;
                counter = 0;
            }
        }
 
        weightcounter = 0;
        templayer.Clear();
        for (int i = 0; i < secondhiddenneuroncount; i++)
        {
            weightcounter = i;
            for (int j = 0; j < firstlayer.Count; j++)
            {
                templayer.Add(firstlayer[j] * weight2[weightcounter] + biases[1]);
                weightcounter += secondhiddenneuroncount;
            }
        }

        counter = 0;
        tempint = 0;
        for (int i = 0; i < templayer.Count; i++)
        {
            counter++;
            tempint += templayer[i];

            if (counter == firstlayer.Count)
            {
                secondlayer.Add(relu(tempint));
                tempint = 0;
                counter = 0;
            }
        }

        weightcounter = 0;
        templayer.Clear();


        for (int i = 0; i < outputneurons; i++)
        {
            weightcounter = i;
            for (int j = 0; j < secondlayer.Count; j++)
            {
                templayer.Add(secondlayer[j] * weight3[weightcounter] + biases[2]);
                weightcounter += outputneurons;
            }
        }

        counter = 0;
        tempint = 0;
        for (int i = 0; i < templayer.Count; i++)
        {
            counter++;
            tempint += templayer[i];

            if (counter == secondlayer.Count)
            {
                outputlayer.Add(sigmoid(tempint));
                tempint = 0;
                counter = 0;
            }
        }

        //for (int i = 0; i < outputlayer.Count; i++)
        //    Debug.Log(outputlayer[i]);
        //Debug.Log("-");
        return outputlayer;
    }

    private float sigmoid(float x) => 1 / (1 + Mathf.Exp(-x));

    private float relu(float x)
    {
        if (x > 0)
            return x;
        else
            return 0;
    }
}
