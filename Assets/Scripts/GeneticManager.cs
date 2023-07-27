using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class GeneticManager : MonoBehaviour
{
    public int population = 499;
    public int mutationrate = 2;
    public int repopulatingmember = 50;
    public int currentagent = 0;

    public int generation = 1;

    public Snake snake;

    public List<float> fitnesslist = new List<float>();
    public List<NNet> networklist = new List<NNet>();

    public List<NNet> newpopulation = new List<NNet>();

    public float AverageFitness = 0;

    public float BestFitness = 0;

    public List<NNet> fittestnetwork = new List<NNet>();

    private List<float> percentagelist = new List<float>();

    void Awake()
    {
        for (int i = 0; i <= population; i++)
        {
            networklist.Add(new NNet());
            networklist[i].Initialize();
        }
        Resettocurrentagent();
    }

    private void Resettocurrentagent()
    {
        snake.Resetwithnetwork(networklist[currentagent]);
    }

    public void Death(float fitness, NNet network)
    {
        fitnesslist.Add(fitness);
        networklist[currentagent] = network;
        if (fitness > BestFitness)
            BestFitness = fitness;

        if(currentagent < population)
        {
            currentagent++;
            Resettocurrentagent();
        }
        else
        {
            generation++;
            repopulate();
        }
    }

    void repopulate()
    {
        updateaveragefitness();
        percentagelist.Clear();
        List<int> highestelement = reorderpopulation();
        fittestnetwork.Clear();
        crossover(highestelement);
        mutate();
        for (int i = 0; i < networklist.Count; i++)
            networklist[i] = newpopulation[i];
        newpopulation.Clear();
        currentagent = 0;
        Resettocurrentagent();
    }
 
    private void updateaveragefitness()
    {
        float tempval = 0;
        for(int i = 0; i < fitnesslist.Count; i++)
        {
            tempval += fitnesslist[i];
        }
        AverageFitness = tempval / fitnesslist.Count;
    }
    private List<int> reorderpopulation()
    {
        List<int> highestelements = new List<int>();
        List<float> templist = new List<float>();

        for (int i = 0; i < fitnesslist.Count; i++)
            templist.Add(fitnesslist[i]);

        while(highestelements.Count < repopulatingmember)
        {
            float highestvalue = templist.Max();
            highestelements.Add(templist.IndexOf(highestvalue));
            percentagelist.Add(highestvalue);
            templist[templist.IndexOf(highestvalue)] = 0;
        }
        //for (int i = 0; i < highestelements.Count; i++)
        //    Debug.Log(highestelements[i]);
        //Debug.Log("-");

        fitnesslist.Clear();
        return highestelements;
    }

    private void crossover(List<int> highestelements)
    {
        for(int i = 0; i < highestelements.Count; i++)
            fittestnetwork.Add(networklist[highestelements[i]]);

        for (int i = 1; i < percentagelist.Count; i++)
            percentagelist[i] = percentagelist[i] + percentagelist[i - 1];

        for (int i = 0; i < (population+1)/2; i++)
        {
            int parent1index = 0;
            int parent2index = 0;

            while (parent1index == parent2index)
            {
                float randomparent1 = Random.Range(0, percentagelist[percentagelist.Count - 1]);
                float randomparent2 = Random.Range(0, percentagelist[percentagelist.Count - 1]);


                for (int n = 1; n < percentagelist.Count; n++)
                {
                    if (randomparent1 > percentagelist[n - 1] && randomparent1 <= percentagelist[n])
                        parent1index = n;

                    if (randomparent2 > percentagelist[n - 1] && randomparent2 <= percentagelist[n])
                        parent2index = n;
                }
            }


            NNet child1 = new NNet();
            NNet child2 = new NNet();

            for (int j = 0; j < 640; j++)
            {
                if (Random.Range(0, 9) < 5)
                {
                    child1.weight1.Add(fittestnetwork[parent1index].weight1[j]);
                    child2.weight1.Add(fittestnetwork[parent2index].weight1[j]);
                }
                else
                {
                    child2.weight1.Add(fittestnetwork[parent1index].weight1[j]);
                    child1.weight1.Add(fittestnetwork[parent2index].weight1[j]);
                }
            }

            for (int j = 0; j < 240; j++)
            {
                if (Random.Range(0, 9) < 5)
                {
                    child1.weight2.Add(fittestnetwork[parent1index].weight2[j]);
                    child2.weight2.Add(fittestnetwork[parent2index].weight2[j]);
                }
                else
                {
                    child2.weight2.Add(fittestnetwork[parent1index].weight2[j]);
                    child1.weight2.Add(fittestnetwork[parent2index].weight2[j]);
                }
            }

            for (int j = 0; j < 36; j++)
            {
                if (Random.Range(0, 9) < 5)
                {
                    child1.weight3.Add(fittestnetwork[parent1index].weight3[j]);
                    child2.weight3.Add(fittestnetwork[parent2index].weight3[j]);
                }
                else
                {
                    child2.weight3.Add(fittestnetwork[parent1index].weight3[j]);
                    child1.weight3.Add(fittestnetwork[parent2index].weight3[j]);
                }
            }

            for (int j = 0; j < 3; j++)
            {
                if (Random.Range(0, 9) < 5)
                {
                    child1.biases.Add(fittestnetwork[parent1index].biases[j]);
                    child2.biases.Add(fittestnetwork[parent2index].biases[j]);
                }
                else
                {
                    child2.biases.Add(fittestnetwork[parent1index].biases[j]);
                    child1.biases.Add(fittestnetwork[parent2index].biases[j]);
                }
            }

            newpopulation.Add(child1);
            newpopulation.Add(child2);

        }
    }

    private void mutate()
    {
        for(int i = 0; i < newpopulation.Count; i++)
        {
            for(int j = 0; j < newpopulation[i].weight1.Count; j++)
            {
                int random = Random.Range(0, 100);

                if (random < mutationrate)
                {
                    newpopulation[i].weight1[j] = Random.Range(-1f, 1f);
                }
            }

            for (int j = 0; j < newpopulation[i].weight2.Count; j++)
            {
                int random = Random.Range(0, 100);

                if (random < mutationrate)
                {
                    newpopulation[i].weight2[j] = Random.Range(-1f, 1f);
                }
            }

            for (int j = 0; j < newpopulation[i].weight3.Count; j++)
            {
                int random = Random.Range(0, 100);

                if (random < mutationrate)
                {
                    newpopulation[i].weight3[j] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public void getbetterbrain()
    {
        float randomparent1 = Random.Range(0, percentagelist[percentagelist.Count - 1]);
        float randomparent2 = Random.Range(0, percentagelist[percentagelist.Count - 1]);

        int parent1index = 0;
        int parent2index = 0;

        for (int n = 1; n < percentagelist.Count; n++)
        {
            if (randomparent1 > percentagelist[n - 1] && randomparent1 <= percentagelist[n])
                parent1index = n;

            if (randomparent2 > percentagelist[n - 1] && randomparent2 <= percentagelist[n])
                parent2index = n;
        }

        NNet Betterchild = new NNet();

        for(int i = 0; i < Betterchild.inputneurons * Betterchild.firsthiddenneuroncount; i++)
        {
            int random = Random.Range(0, 9);
            if (random < 5)
                Betterchild.weight1.Add(fittestnetwork[parent1index].weight1[i]);
            else
                Betterchild.weight1.Add(fittestnetwork[parent2index].weight1[i]);
        }

        for(int i = 0; i < Betterchild.firsthiddenneuroncount * Betterchild.secondhiddenneuroncount; i++)
        {
            int random = Random.Range(0, 9);
            if (random < 5)
                Betterchild.weight2.Add(fittestnetwork[parent1index].weight2[i]);
            else
                Betterchild.weight2.Add(fittestnetwork[parent2index].weight2[i]);
        }

        for(int i = 0; i<Betterchild.secondhiddenneuroncount * Betterchild.outputneurons; i++)
        {
            int random = Random.Range(0, 9);
            if (random < 5)
                Betterchild.weight3.Add(fittestnetwork[parent1index].weight3[i]);
            else
                Betterchild.weight3.Add(fittestnetwork[parent2index].weight3[i]);
        }

        for(int i = 0; i< 3; i++)
        {
            int random = Random.Range(0, 9);
            if (random < 5)
                Betterchild.biases.Add(fittestnetwork[parent1index].biases[i]);
            else
                Betterchild.biases.Add(fittestnetwork[parent2index].biases[i]);
        }

        snake.Resetwithnetwork(Betterchild);
    }

}
