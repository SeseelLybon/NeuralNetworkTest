using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeepleBahaviourScript : MonoBehaviour
{
    private GeneralBehaviourScript WAIS;

    public NeuralNetwork brain;

    public int food_reserves = 0;
    public int food_keep_reserves = 10;
    public int food_sell_willingnes;
    public int food_buy_willingnes;

    public int wood_reserves = 0;
    public int wood_keep_reserves = 0;
    public int wood_sell_willingnes;
    public int wood_buy_willingnes;

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

        food_sell_willingnes = random.Next(0, 9); // demand 10 food to be x gold to sell
        food_buy_willingnes = random.Next(1, 9); // demand to get x food for 10 gold


    }

    // Update is called once per frame
    // 4 steps/update is 1 day
    void FixedUpdate()
    {
        hunger += 5;
        age += 0.25f;

        // 1. Set inputs
        brain.set_input_intensity(0, food_reserves);
        brain.set_input_intensity(1, hunger);

        // 2. Fire network
        brain.Fire_network();

        // 3. Get outputs
        if( brain.get_output_intensity(0) >= 0.8)
        {
            consume_meal();
        }
        if( brain.get_output_intensity(0) >= 0.8)
        {
            gather_food();
        }


    }

    bool consume_meal()
    {
        if(food_reserves >= 5)
        {
            food_reserves -= 5;
            hunger -= 20;
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
    }

    void gather_wood()
    {
        wood_reserves += WAIS.gather_wood();
        hunger += 5;
    }
}
