using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeepleBahaviourScript : MonoBehaviour
{
    private GeneralBehaviourScript WAIS;


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

    public float age = 0;

    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        WAIS = GameObject.Find("World_AI").GetComponent<GeneralBehaviourScript>();

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

        // if too hungry or unhappy, die
        if( hunger >= 100 || happiness <= 0)
        {
            die();
        }

        // consume food if hungry
        if( hunger > 50)
        {
            // if can, consume meal, else buy food, else gather food
            if( consume_meal() )
            {
                change_happiness(5);
                return;
            } else
            {
                // if has the gold, buy the food, else gather it.
                if( gold_reserves >= 10 && food_sell_willingnes <= WAIS.simple_market_food.get_r2g())
                {
                    gold_reserves -= 10;
                    food_reserves += WAIS.simple_market_food.Buy();
                    return;
                } else
                {
                    gather_food();
                    change_happiness(-5);
                    return;
                }
            }
        // if not hungry and food reserves low, gather food anyway.
        } else if( food_reserves < food_keep_reserves)
        {
            food_reserves += WAIS.gather_food();
            return;
        }

        /*
        // if happy, duplicate
        if( happiness >= 75)
        {
            happiness = 25;
            WAIS.generate_meeple();
            return;
        }
        */

        // if has done nothing this tick, sell food
        if( food_reserves > 10 && food_sell_willingnes <= WAIS.simple_market_food.get_r2g())
        {
            food_reserves -= 10;
            gold_reserves += WAIS.simple_market_food.Sell();
            return;
        }
        if( wood_reserves >= 10)
        {
            wood_reserves -= 10;
            gold_reserves += WAIS.simple_market_wood.Sell();
            return;
        } else
        {
            gather_wood();
            change_happiness(-1);
            return;
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
