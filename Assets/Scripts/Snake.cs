using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JLEE;
using MathNet.Numerics.LinearAlgebra.Complex;

public class Snake : MonoBehaviour
{
    public int arenasize = 40;

    public float speed = 0.03f;
    public GameObject tailprefab;
    public bool ate = false;
    public Vector2 dir = Vector2.right;
    public List<Transform> tail = new List<Transform>();
    public Transform Wall1;
    public Transform Wall2;
    public Transform Wall3;
    public Transform Wall4;
    public GameObject FoodPrefab;
    private Vector2 CurrFoodPosition;
    private List<float> inputs = new List<float>();
    private int foodposx;
    private int foodposy;

    public int timer = 200;

    private int steps = 0;

    private float distancetofood;

    public float fitness = 0;

    private List<float> directions;

    private int startxpos;
    private int startypos;

    public NNet neuralnetwork = new NNet();

    public void Awake()
    {
        InvokeRepeating("Move", speed, speed);
        Resetagent();
    }

    public void Resetagent()
    {
        if (arenasize >= 9)
            updatearena();


        int startx = (int)Random.Range(Wall1.position.x + 4, Wall3.position.x - 4);
        int starty = (int)Random.Range(Wall4.position.y + 4, Wall2.position.y - 4);

        startxpos = startx;
        startypos = starty;

        
        
        int direction = Random.Range(0, 3);

        if (direction == 0)
        {
            if (dir == Vector2.right)
                dir = Vector2.up;
            else if (dir == Vector2.up)
                dir = -Vector2.right;
            else if (dir == -Vector2.right)
                dir = -Vector2.up;
            else if (dir == -Vector2.up)
                dir = Vector2.right;

        }
        else if (direction == 1)
        {
            if (dir == Vector2.right)
                dir = -Vector2.up;
            else if (dir == -Vector2.up)
                dir = -Vector2.right;
            else if (dir == -Vector2.right)
                dir = Vector2.up;
            else if (dir == Vector2.up)
                dir = Vector2.right;
        }



        timer = 200;
        fitness = 0;
        steps = 0;

        tail.Clear();

        transform.position = new Vector2(startx, starty);

        var foods = GameObject.FindGameObjectsWithTag("food");
        for (int i = 0; i < foods.Length; i++)
            Destroy(foods[i]);

        var tails = GameObject.FindGameObjectsWithTag("tail");
        for (int i = 0; i < tails.Length; i++)
        {
            tail.Remove(tails[i].transform);
            Destroy(tails[i]);
        }

        Spawn();

        distancetofood = getdistancetofood();

        gettail(direction, startx, starty);
    }

    void gettail(int direction, int startx, int starty)
    {
        GameObject tail1 = null;
        GameObject tail2 = null;
        GameObject tail3 = null;
        GameObject tail4 = null;

        if (direction == 0)
        {
            tail1 = Instantiate(tailprefab, new Vector2(startx-1, starty), Quaternion.identity);
            tail2 = Instantiate(tailprefab, new Vector2(startx-2, starty), Quaternion.identity);
            tail3 = Instantiate(tailprefab, new Vector2(startx - 3, starty), Quaternion.identity);
            tail4 = Instantiate(tailprefab, new Vector2(startx - 4, starty), Quaternion.identity);
        }
        else if(direction == 1)
        {
            tail1 = Instantiate(tailprefab, new Vector2(startx, starty -1), Quaternion.identity);
            tail2 = Instantiate(tailprefab, new Vector2(startx, starty-2), Quaternion.identity);
            tail3 = Instantiate(tailprefab, new Vector2(startx, starty - 3), Quaternion.identity);
            tail4 = Instantiate(tailprefab, new Vector2(startx, starty - 4), Quaternion.identity);

        }
        else if(direction == 2)
        {
            tail1 = Instantiate(tailprefab, new Vector2(startx + 1, starty), Quaternion.identity);
            tail2 = Instantiate(tailprefab, new Vector2(startx + 2, starty), Quaternion.identity);
            tail3 = Instantiate(tailprefab, new Vector2(startx + 3, starty), Quaternion.identity);
            tail4 = Instantiate(tailprefab, new Vector2(startx + 4, starty), Quaternion.identity);
        }
        else if(direction == 3)
        {
            tail1 = Instantiate(tailprefab, new Vector2(startx, starty+1), Quaternion.identity);
            tail2 = Instantiate(tailprefab, new Vector2(startx, starty+2), Quaternion.identity);
            tail3 = Instantiate(tailprefab, new Vector2(startx, starty + 3), Quaternion.identity);
            tail4 = Instantiate(tailprefab, new Vector2(startx, starty + 4), Quaternion.identity);
        }

        tail.Add(tail1.transform);
        tail.Add(tail2.transform);
        tail.Add(tail3.transform);
        tail.Add(tail4.transform);
    }

    private float getdistancetofood()
    {
        float diffinx = foodposx - transform.position.x;
        float diffiny = foodposy - transform.position.y;
        return (Mathf.Sqrt(diffinx * diffinx + diffiny * diffiny));
    }

    private void getinput()
    {   
        CheckFood();
        CheckWall();
        CheckRayHitInfo();
        CheckDir();
        CheckTailDir();

    }


    void CheckDir()
    {
        if(dir == Vector2.up)
        {
            inputs.Add(1);
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(0);
        }
        else if(dir == Vector2.right)
        {
            inputs.Add(0);
            inputs.Add(1);
            inputs.Add(0);
            inputs.Add(0);
        }
        else if (dir == -Vector2.right)
        {
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(1);
            inputs.Add(0);
        }
        else if(dir == -Vector2.up)
        {
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(1);
        }
    }
    void CheckTailDir()
    {
        if (tail[0].position.y > transform.position.y && tail[0].position.x == transform.position.x)
        {
            inputs.Add(1);
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(0);
        }
        else if (tail[0].position.x > transform.position.x && tail[0].position.y == transform.position.y)
        {
            inputs.Add(0);
            inputs.Add(1);
            inputs.Add(0);
            inputs.Add(0);
        }
        else if (tail[0].position.x < transform.position.x && tail[0].position.y == transform.position.y)
        {
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(1);
            inputs.Add(0);
        }
        else if (tail[0].position.y < transform.position.y && tail[0].position.x == transform.position.x)
        {
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(0);
            inputs.Add(1);
        }
    }

    public void Resetwithnetwork(NNet net)
    {
        neuralnetwork = net;
        Resetagent();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 v = transform.position;
        if (collision.gameObject.tag == "food")
        {
            ate = true;
            Destroy(collision.gameObject);
            Spawn();
        }
        else if (collision.gameObject.tag == "tail" || collision.gameObject.tag == "wall")
        {
            if (timer > 194 && tail.Count == 4)
            {
                if (GameObject.FindObjectOfType<GeneticManager>().generation == 1)
                {
                    dead();
                    //neuralnetwork.Initialize();
                    //Resetagent();
                }
                else
                    dead();
                    //GameObject.FindObjectOfType<GeneticManager>().getbetterbrain();
            }

            else if(transform.position.x == startxpos || transform.position.y == startypos)
            {
                if (tail.Count <= 4)
                {
                    if (GameObject.FindObjectOfType<GeneticManager>().generation == 1)
                    {
                        dead();
                        //neuralnetwork.Initialize();
                        //Resetagent();
                    }
                    else
                        dead();
                        //GameObject.FindObjectOfType<GeneticManager>().getbetterbrain();
                }
                else
                    dead();
            }
            else
                dead();

        }
        
    }

    void dead()
    {
        calculatefitness();
        GameObject.FindObjectOfType<GeneticManager>().Death(fitness, neuralnetwork);
    }

    void calculatefitness() => fitness = steps + (Mathf.Pow(2, tail.Count - 4) + Mathf.Pow(tail.Count - 4, 2.1f) * 500) - (Mathf.Pow(tail.Count - 4, 1.2f) * Mathf.Pow(0.25f * steps, 1.3f));

    void updatearena()
    {
        Wall1.position = new Vector2(-1 * arenasize / 2, 0);
        Wall1.localScale = new Vector3(1, arenasize, 1);

        Wall2.position = new Vector2(0, arenasize / 2);
        Wall2.localScale = new Vector3(arenasize, 1, 1);

        Wall3.position = new Vector2(arenasize / 2, 0);
        Wall3.localScale = new Vector3(1, arenasize, 1);

        Wall4.position = new Vector2(0, -1 * arenasize / 2);
        Wall4.localScale = new Vector3(arenasize, 1, 1);
    }

    void Move()
    {
        steps++;
        timer--;
        if (speed != 0)
        {
            CancelInvoke();
            InvokeRepeating("Move", speed, speed);
        }
        //frontrays.CheckRays();
        //leftrays.CheckRays();
        //rightrays.CheckRays();

        inputs.Clear();
        getinput();

        if (inputs.Count != 32)
            getinput();
        else
            directions = neuralnetwork.Runnetwork(inputs);

        float finaldirection = directions.Max();
        for (int i = 0; i < directions.Count; i++)
        {
            if (directions[i] == finaldirection)
            {
                finaldirection = i;
                break;
            }
        }

        if(finaldirection == 0)
        {
            if (dir == Vector2.right)
                dir = Vector2.up;
            else if (dir == Vector2.up)
                dir = -Vector2.right;
            else if (dir == -Vector2.right)
                dir = -Vector2.up;
            else
                dir = Vector2.right;
        }

        else if(finaldirection == 1)
        {
            if (dir == Vector2.right)
                dir = -Vector2.up;
            else if (dir == -Vector2.up)
                dir = -Vector2.right;
            else if (dir == -Vector2.right)
                dir = Vector2.up;
            else
                dir = Vector2.right;
        }




        directions.Clear();



        Vector2 v = transform.position;
        transform.Translate(dir);
        if (ate)
        {
            GameObject n = (GameObject)Instantiate(tailprefab, v, Quaternion.identity);
            tail.Insert(0, n.transform);
            ate = false;
            timer += 100;
        }
        else if (tail.Count > 0)
        {
            tail[tail.Count - 1].position = v;
            tail.Insert(0, tail[tail.Count - 1]);
            tail.RemoveAt(tail.Count - 1);
        }

        //float tempdistance = getdistancetofood();
        //if (tempdistance < distancetofood)
        //    fitness += 1;
        //else
        //    fitness -= 1.5f;


        //distancetofood = tempdistance;


        if (timer == 0)
        {
            dead();
        }

    }

    void Spawn()
    {
        foodposx = (int)UnityEngine.Random.Range(Wall1.position.x, Wall3.position.x);
        foodposy = (int)UnityEngine.Random.Range(Wall4.position.y, Wall2.position.y);
        if (tail.Count > 0)
        {
            for (int i = 0; i < tail.Count; i++)
            {
                if (foodposx == tail[i].position.x && foodposy == tail[i].position.y)
                {
                    Spawn();
                    break;
                }
                else if (i == tail.Count - 1)
                {
                    Instantiate(FoodPrefab, new Vector2(foodposx, foodposy), Quaternion.identity);
                }
            }
        }
        else
        {
            Instantiate(FoodPrefab, new Vector2(foodposx, foodposy), Quaternion.identity);
        }
        CurrFoodPosition = new Vector2(foodposx, foodposy);

    }

    private void CheckRayHitInfo()
    {
        RaycastHit2D uphit = Physics2D.Raycast(transform.position, dir, 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D rightuphit = Physics2D.Raycast(transform.position, dir.Rotate(-45f), 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, dir.Rotate(-90f), 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D downrighthit = Physics2D.Raycast(transform.position, dir.Rotate(-135f), 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D downhit = Physics2D.Raycast(transform.position, dir.Rotate(-180f), 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D leftdownhit = Physics2D.Raycast(transform.position, dir.Rotate(135f), 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, dir.Rotate(90f), 40f, LayerMask.GetMask("raymask"));
        RaycastHit2D uplefthit = Physics2D.Raycast(transform.position, dir.Rotate(45f), 40f, LayerMask.GetMask("raymask"));

        if (uphit)
        {
            if (uphit.collider.gameObject.tag == "tail")
                inputs.Add(1/uphit.distance);
            Debug.DrawRay(transform.position, Vector2.up, Color.red);
        }
        else
            inputs.Add(1f);

        if (rightuphit)
        {
            if (rightuphit.collider.gameObject.tag == "tail")
                inputs.Add(1/rightuphit.distance);
            Debug.DrawRay(transform.position, new Vector2(1,1), Color.red);
        }
        else
            inputs.Add(1f);

        if (righthit)
        {
            if (righthit.collider.gameObject.tag == "tail")
                inputs.Add(1/righthit.distance);
            Debug.DrawRay(transform.position, Vector2.right, Color.red); 
        }
        else
            inputs.Add(1f);

        if (downrighthit)
        {
            if (downrighthit.collider.gameObject.tag == "tail")
                inputs.Add(1/downrighthit.distance);
            Debug.DrawRay(transform.position, new Vector2(1, -1), Color.red);
        }
        else
            inputs.Add(1f);

        if (downhit)
        {
            if (downhit.collider.gameObject.tag == "tail")
                inputs.Add(1/downhit.distance);
            Debug.DrawRay(transform.position, -Vector2.up, Color.red);
        }
        else
            inputs.Add(1f);

        if (leftdownhit)
        {
            if (leftdownhit.collider.gameObject.tag == "tail")
                inputs.Add(1/leftdownhit.distance);
            Debug.DrawRay(transform.position, new Vector2(-1, -1), Color.red);
        }
        else
            inputs.Add(1f);

        if (lefthit)
        {
            if (lefthit.collider.gameObject.tag == "tail")
                inputs.Add(1/lefthit.distance);
            Debug.DrawRay(transform.position, -Vector2.right, Color.red);
        }
        else
            inputs.Add(1f);

        if (uplefthit)
        {
            if (uplefthit.collider.gameObject.tag == "tail")
                inputs.Add(1/uplefthit.distance);
            Debug.DrawRay(transform.position, new Vector2(-1, 1), Color.red);
        }
        else
            inputs.Add(1f);
    }


    private void CheckWall()
    {
        RaycastHit2D uphit = Physics2D.Raycast(transform.position, dir, 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D rightuphit = Physics2D.Raycast(transform.position, dir.Rotate(-45f), 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, dir.Rotate(-90f), 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D downrighthit = Physics2D.Raycast(transform.position, dir.Rotate(-135f), 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D downhit = Physics2D.Raycast(transform.position, dir.Rotate(-180f), 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D leftdownhit = Physics2D.Raycast(transform.position, dir.Rotate(135f), 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, dir.Rotate(90f), 40f, LayerMask.GetMask("Wall"));
        RaycastHit2D uplefthit = Physics2D.Raycast(transform.position, dir.Rotate(45f), 40f, LayerMask.GetMask("Wall"));

        if (uphit)
        {
            if (uphit.collider.gameObject.tag == "wall")
                inputs.Add(1/uphit.distance);
            Debug.DrawRay(transform.position, Vector2.up, Color.red);
        }
        else
            inputs.Add(1f);

        if (rightuphit)
        {
            if (rightuphit.collider.gameObject.tag == "wall")
                inputs.Add(1/rightuphit.distance);
            Debug.DrawRay(transform.position, new Vector2(1, 1), Color.red);
        }
        else
            inputs.Add(1f);

        if (righthit)
        {
            if (righthit.collider.gameObject.tag == "wall")
                inputs.Add(1/righthit.distance);
            Debug.DrawRay(transform.position, Vector2.right, Color.red);
        }
        else
            inputs.Add(1f);

        if (downrighthit)
        {
            if (downrighthit.collider.gameObject.tag == "wall")
                inputs.Add(1/downrighthit.distance);
            Debug.DrawRay(transform.position, new Vector2(1, -1), Color.red);
        }
        else
            inputs.Add(1f);

        if (downhit)
        {
            if (downhit.collider.gameObject.tag == "wall")
                inputs.Add(1/downhit.distance);
            Debug.DrawRay(transform.position, -Vector2.up, Color.red);
        }
        else
            inputs.Add(1f);

        if (leftdownhit)
        {
            if (leftdownhit.collider.gameObject.tag == "wall")
                inputs.Add(1/leftdownhit.distance);
            Debug.DrawRay(transform.position, new Vector2(-1, -1), Color.red);
        }
        else
            inputs.Add(1f);

        if (lefthit)
        {
            if (lefthit.collider.gameObject.tag == "wall")
                inputs.Add(1/lefthit.distance);
            Debug.DrawRay(transform.position, -Vector2.right, Color.red);
        }
        else
            inputs.Add(1f);

        if (uplefthit)
        {
            if (uplefthit.collider.gameObject.tag == "wall")
                inputs.Add(1/uplefthit.distance);
            Debug.DrawRay(transform.position, new Vector2(-1, 1), Color.red);
        }
        else
            inputs.Add(1f);
    }

    private void CheckFood()
    {
        RaycastHit2D uphit = Physics2D.Raycast(transform.position, dir, 40f, LayerMask.GetMask("Food"));
        RaycastHit2D rightuphit = Physics2D.Raycast(transform.position, dir.Rotate(-45f), 40f, LayerMask.GetMask("Food"));
        RaycastHit2D righthit = Physics2D.Raycast(transform.position, dir.Rotate(-90f), 40f, LayerMask.GetMask("Food"));
        RaycastHit2D downrighthit = Physics2D.Raycast(transform.position, dir.Rotate(-135f), 40f, LayerMask.GetMask("Food"));
        RaycastHit2D downhit = Physics2D.Raycast(transform.position, dir.Rotate(-180f), 40f, LayerMask.GetMask("Food"));
        RaycastHit2D leftdownhit = Physics2D.Raycast(transform.position, dir.Rotate(135f), 40f, LayerMask.GetMask("Food"));
        RaycastHit2D lefthit = Physics2D.Raycast(transform.position, dir.Rotate(90f), 40f, LayerMask.GetMask("Food"));
        RaycastHit2D uplefthit = Physics2D.Raycast(transform.position, dir.Rotate(45f), 40f, LayerMask.GetMask("Food"));

        if (uphit)
        {
            if (uphit.collider.gameObject.tag == "food")
                inputs.Add(1/uphit.distance);
            Debug.DrawRay(transform.position, Vector2.up, Color.red);
        }
        else
            inputs.Add(1f);

        if (rightuphit)
        {
            if (rightuphit.collider.gameObject.tag == "food")
                inputs.Add(1/rightuphit.distance);
            Debug.DrawRay(transform.position, new Vector2(1, 1), Color.red);
        }
        else
            inputs.Add(1f);

        if (righthit)
        {
            if (righthit.collider.gameObject.tag == "food")
                inputs.Add(1/righthit.distance);
            Debug.DrawRay(transform.position, Vector2.right, Color.red);
        }
        else
            inputs.Add(1f);

        if (downrighthit)
        {
            if (downrighthit.collider.gameObject.tag == "food")
                inputs.Add(1/downrighthit.distance);
            Debug.DrawRay(transform.position, new Vector2(1, -1), Color.red);
        }
        else
            inputs.Add(1f);

        if (downhit)
        {
            if (downhit.collider.gameObject.tag == "food")
                inputs.Add(1/downhit.distance);
            Debug.DrawRay(transform.position, -Vector2.up, Color.red);
        }
        else
            inputs.Add(1f);

        if (leftdownhit)
        {
            if (leftdownhit.collider.gameObject.tag == "food")
                inputs.Add(1/leftdownhit.distance);
            Debug.DrawRay(transform.position, new Vector2(-1, -1), Color.red);
        }
        else
            inputs.Add(1f);

        if (lefthit)
        {
            if (lefthit.collider.gameObject.tag == "food")
                inputs.Add(1/lefthit.distance);
            Debug.DrawRay(transform.position, -Vector2.right, Color.red);
        }
        else
            inputs.Add(1f);

        if (uplefthit)
        {
            if (uplefthit.collider.gameObject.tag == "food")
                inputs.Add(1/uplefthit.distance);
            Debug.DrawRay(transform.position, new Vector2(-1, 1), Color.red);
        }
        else
            inputs.Add(1f);
    }
}
