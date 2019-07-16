using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeepleBahaviourScript : MonoBehaviour
{
    private GeneralBehaviourScript WAIS;

    public NeuralNetwork brain;

    public int food_reserves = 0;

    public int wood_reserves = 0;

    public int gold_reserves = 0;

    public int hunger = 0;
    public int happiness = 50;

    public float age;

    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        WAIS = GameObject.Find("World_AI").GetComponent<GeneralBehaviourScript>();
        brain = gameObject.GetComponent<NeuralNetwork>();

        hunger = random.Next(0,30);
        happiness = random.Next(25, 75);
    }

    // Update is called once per frame
    // 4 steps/update is 1 day
    void FixedUpdate()
    {
        age += 0.25f;

        hunger += 5;
        hunger = (hunger > 100) ? 100 : hunger;
        hunger = (hunger < 0) ? 0 : hunger;

        happiness = (happiness > 100) ? 100 : happiness;
        happiness = (happiness < 0 ) ? 0 : happiness;

        // 1. Set inputs
        brain.set_input_intensity(0, food_reserves);
        brain.set_input_intensity(1, wood_reserves);
        brain.set_input_intensity(2, hunger);
        brain.set_input_intensity(3, age);

        // 2. Fire network
        brain.Fire_network();

        // 3. Get outputs
        if (brain.get_output_intensity(1) >= 0.5)
        {
            gather_food();
        }
        else if (brain.get_output_intensity(0) >= 0.5)
        {
            consume_meal();
        }
        else if (brain.get_output_intensity(2) >= 0.5)
        {
            gather_wood();
        }
        else if (brain.get_output_intensity(3) >= 0.5)
        {
            //
        }


    }

    bool consume_meal()
    {
        if(food_reserves >= 5)
        {
            food_reserves -= 5;
            hunger -= 20;
            happiness+=5;
            return true;
        } else
        {
            return false;
        }
    }

    void gather_food()
    {
        food_reserves += WAIS.gather_food();
        hunger += 5;
        if(hunger >= 75)
        {
            happiness -= 5;
        } else
        {
            happiness -= 2;
        }
    }

    void gather_wood()
    {
        wood_reserves += WAIS.gather_wood();
        hunger += 5;
        happiness -= 2;
    }
}
