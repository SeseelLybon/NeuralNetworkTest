using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralBehaviourScript : MonoBehaviour
{
    // Food wood stone gold
    private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

    public Transform meeple_group;
    public Transform meeple_prefab;

    public Text food_natural_text;
    public Text food_index_text;

    public Text wood_natural_text;
    public Text wood_index_text;

    public Text meeple_count;
    public Text world_age_text;

    //private List<Listing> listings_sell = new List<Listing>();
    public simple_Market simple_market_food;
    public simple_Market simple_market_wood;

    public float world_age = 0;

    private List<Transform> meeples_list = new List<Transform>();

    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        meeple_group = GameObject.Find("meeples_group").GetComponent<Transform>();

        resources.Add("Food", new Resource("Food", 1000, 1000, 20) );
        resources.Add("Wood", new Resource("Wood", 1000, 1000, 20));

        simple_market_food = new simple_Market(resources["Food"]);
        simple_market_wood = new simple_Market(resources["Wood"]);

        generate_meeple();

    }

    // 4 steps/update is 1 day
    void FixedUpdate()
    {
        world_age += 0.25f;
        foreach(KeyValuePair<string, Resource> res in resources)
        {
            if (res.Value.minable <= res.Value.max - res.Value.regen)
            {
                res.Value.minable += res.Value.regen * random.Next(1, 3);
            }

        }

        food_natural_text.text = "Food = " + resources["Food"].minable.ToString();
        food_index_text.text = "Food index = " + simple_market_food.get_index_value();

        wood_natural_text.text = "Wood = " + resources["Wood"].minable.ToString();
        wood_index_text.text = "Wood index = " + simple_market_wood.get_index_value();

        meeple_count.text = "Meeples: " + meeples_list.Count.ToString();
        world_age_text.text = "World age: " + world_age.ToString();
    }

    public int gather_food()
    {
        int gathered_amount = random.Next(3,7);
        if(resources["Food"].minable >= gathered_amount) {
            resources["Food"].minable -= gathered_amount;
            return gathered_amount;
        }
        return 0;
    }

    public int gather_wood()
    {
        int gathered_amount = random.Next(3, 7);
        if (resources["Wood"].minable >= gathered_amount)
        {
            resources["Wood"].minable -= gathered_amount;
            return gathered_amount;
        }
        return 0;
    }

    public bool generate_meeple()
    {
        Transform temp_meep = Instantiate(meeple_prefab, meeple_group);
        temp_meep.name = "Meeple";
        meeples_list.Add(temp_meep);
        return true;
    }

    public bool remove_meeple(Transform meeple)
    {
        if( meeples_list.Remove(meeple))
        {
            return true;
        }
        return false;
    }
}


public class Resource
{
    public int minable;
    public int max;
    public int regen;
    public string name;
    public Resource(string name, int minable, int max, int regen)
    {
        this.name = name;
        this.minable = minable;
        this.max = max;
        this.regen = regen;
    }
}

public class simple_Market
{
    private Resource resource;
    public string name;

    float index_value = 1f;
    float index_change = 0.02f;
    float index_min = 0.020f;
    float index_max = 9999;
    
    public float get_index_value()
    {
        return index_value;
    }

    public float get_r2g()
    {
        return 10 * index_value;
    }

    public float get_g2r()
    {
        return 10 / index_value;
    }

    public simple_Market( Resource resource )
    {
        this.resource = resource;
        this.name = resource.name;
    }

    public int Sell()
    {
        float temp = 10f* index_value;
        if (index_value - index_change < index_min)
        {
            index_value = index_min;
        } else
        {
            index_value -= index_change;
        }
        return (int)temp; // gold gained
    }
    public int Buy()
    {
        float temp = 10f / index_value;
        if (index_value + index_change > index_max)
        {
            index_value = index_max;
        }
        else
        {
            index_value += index_change;
        }
        return (int)temp; // food gained
    }
}




/*
class Market
{
    public Resource resource;
    public string name;
    public List<Listing> sell_listings = new List<Listing>();

    public Market(Resource resource)
    {
        this.resource = resource;
        this.name = resource.name;
    }

    public bool buy()
    {
        //  if there is an order:
        //      fullfill order
        //  else:
        //      list for sale
        return false;
    }

    public bool sell()
    {
        //  if there is a sale:
        //      fullfill sale
        //  else:
        //      list as order
        return false;
    }

    public bool list_sale()
    {

        return false;
    }

    public bool list_order()
    {

        return false;
    }

}

class Listing
{
    public string type; // "buy" or "sell"
    public int amountitems;
    public int amountgold;
    public GameObject meeple_owner; // VOLATILE, meeple might die!

    public Listing(string type, GameObject owner, amount_item, amount_gold)
    {
        this.type = type;
        meeple_owner = owner;
    }
}
*/