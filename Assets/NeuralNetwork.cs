using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// Usage;
///     1. construct
///     2. set all inputs
///     3. fire network
///     4. get inputs
///     5. repeat from 2
///     
/// </summary>
public class NeuralNetwork
{

    Node_Layer input_layer;
    //List<Node_Layer> hidden_layers;
    Node_Layer output_layer;

    // needs overloads
    NeuralNetwork(List<string> inputs, List<string> outputs)
    {

        // Generate from input to output so you can pass parent layers.

        // Generate input layer
        input_layer = new Node_Layer(inputs);

        // Generate output layer
        // PASS PARENT LAYERS!

        output_layer = new Node_Layer(outputs, input_layer,  true);


    }


    // Fire the entire network from the output nodes
    void Fire_Network()
    {
        // Tell each node not on the input layer it's old
        output_layer.Invalidate();
        // invalidate old layers

        // go through all non-input layer nodes and generate new intensities
        // Because the function is recursive, only need to ask the output layer
        output_layer.Fire_nodes();
    }

    public double get_output_node(string node_name)
    {
        double temp = output_layer.nodes[node_name].get_intensity();
        return temp;
    }
}

//---------------------------------------------NODE LAYER
public class Node_Layer
{
    public Dictionary<string, Node> nodes;              // Name; Node

    // Can be null!
    public Node_Layer parent_layer;

    bool is_input_layer = false;
    bool is_output_layer = false;

    // Should only be called for input layers!
    public Node_Layer(List<string> node_names)
    {
        is_input_layer = true;
        foreach (string name in node_names)
        {
            nodes.Add( name, new Node() );
        }
    }

    // Can be called for any layer
    public Node_Layer(List<string> node_names, Node_Layer parentlayer, bool isoutptlyer)
    {
        parent_layer = parentlayer;
        is_output_layer = isoutptlyer;

        foreach (string name in node_names)
        {
            nodes.Add(name, new Node());
        }
    }

    // Tells all nodes in this layer that they're old and need to be regenerated when asked thier intensity
    public void Invalidate()
    {
        foreach (KeyValuePair<string, Node> nodepair in nodes)
        {
            nodepair.Value.intensity_is_new = false;
        }
    }

    public void Fire_nodes()
    {

    }
}

//---------------------------------------------NODE
/// <summary>
/// Node mostly exists for data storage
/// </summary>
public class Node
{
    public readonly double bias;
    public Node_Layer parent_layer;
    public Dictionary<string, double> parent_nodes_weights;

    double intensity = 0;
    public bool intensity_is_new = false;

    bool is_input = false;
    bool is_output = false;

    // Blank constructor, used for input layer
    public Node(double bias = 0)
    {
        is_input = true;
        this.bias = bias;
        // because there are no parents, there are no weights
    }

    // Have parents, no weights
    public Node(Node_Layer parent, double bias = 0, bool isoutpt = false)
    {
        this.bias = bias;
        is_output = isoutpt;

        parent_layer = parent;
        // give each node a weight of 1 since we haven't got any passed
        foreach(KeyValuePair<string, Node> nodepair in parent_layer.nodes)
        {
            parent_nodes_weights.Add(nodepair.Key, 1d);
        }
    }

    public double get_intensity()
    {
        return intensity;
    }

    // Only use this for input nodes, otherwise it'll pass to activate_node
    public void set_intensity( int intens = 0)
    {
        if (is_input)
        {
            intensity = intens;
        } else
        {
            activate_node();
        }
    }

    // RECURSIVE
    void activate_node()
    {
        if(is_input) {
            // 
        } else
        {
            double temp_intens = 0;
            foreach (KeyValuePair<string, double> entry in parent_nodes_weights)
            {
                if (parent_layer.nodes[entry.Key].intensity_is_new == false)
                {
                    parent_layer.nodes[entry.Key].activate_node();
                }
                temp_intens += parent_layer.nodes[entry.Key].get_intensity() * entry.Value;
            }
            intensity = Sigmoid(temp_intens + bias);
            intensity_is_new = true;
        }
    }

    public static double Sigmoid(double x)
    {
        return 1 / (1 + System.Math.Pow(System.Math.E, -x));
    }
}