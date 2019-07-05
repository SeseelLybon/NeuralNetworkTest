using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
///     1. construct
///     2. set all inputs
///     3. fire network
///     4. get inputs
///     5. repeat from 2
///     
/// </summary>
public class NeuralNetwork : MonoBehaviour
{

    Node_Layer input_layer;
    //List<Node_Layer> hidden_layers;
    Node_Layer output_layer;

    // needs overloads
    public NeuralNetwork(List<string> inputs, List<string> outputs)
    {

        // Generate from input to output so you can pass parent layers.

        // Generate input layer
        input_layer = new Node_Layer(inputs);

        // Generate output layer
        // PASS PARENT LAYERS!

        output_layer = new Node_Layer(outputs, input_layer,  true);


    }


    // Fire the entire network from the output nodes
    public void Fire_Network()
    {
        // Tell each node not on the input layer it's old
        output_layer.Invalidate();
        // invalidate hidden layers
        // But no need to invalidate input_layer

        // go through all non-input layer nodes and generate new intensities
        // Because the function is recursive, only need to ask the output layer
        output_layer.Fire_nodes();
    }

    // allows the owner of the NN to set input intensities.
    public void Set_input_node_intensity(string node_name, double new_intens)
    {
        Node node = input_layer.nodes[node_name];
        node.intensity = new_intens;
        node.intensity_is_new = true;
    }


    public double get_output_node_intensity(string node_name)
    {
        Node node = output_layer.nodes[node_name];
        double temp = node.intensity;
        return temp;
    }
}

//---------------------------------------------NODE LAYER
public class Node_Layer
{
    public Dictionary<string, Node> nodes;              // Name; Node

    // Can be null in the case of input_layer!
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
    // For input layer, this means 'mark as has_changed'.
    public void Invalidate()
    {
        foreach (KeyValuePair<string, Node> nodepair in nodes)
        {
            nodepair.Value.intensity_is_new = false;
        }
    }

    public bool Fire_nodes()
    {
        if (is_input_layer == false)
        {
            foreach (KeyValuePair<string, Node> nodepair in nodes)
            {
                if (nodepair.Value.intensity_is_new == false)
                {
                    // This node has not yet been check if it needs updating, check parent layer if it has changed.
                    if( parent_layer.Fire_nodes() == false )
                    {
                        // if previous layer was unchanged
                        // probably don't need to generate a new intensity
                        return false; // layer has not changed
                    } else
                    {
                        // if previous layer has changed
                        // generate a new intensity
                        double sum = 0;
                        foreach(KeyValuePair<string,Node>nodepairparent in parent_layer.nodes)
                        {
                            // parentnode.intensity * parentnode.weight
                            sum += nodepairparent.Value.intensity * nodepair.Value.parent_nodes_weights[nodepairparent.Key];
                        }
                        sum += nodepair.Value.bias;
                        nodepair.Value.intensity = Node.Sigmoid(sum);
                        return true; // layer has changed, and children should regenerate.
                    }
                }
            }
            return false; // layer has not changed
        }
        else
        {
            // I am input layer
            // If I have not changed, the next layer is still valid
            bool layer_has_changed = true;
            foreach (KeyValuePair<string, Node> nodepair in nodes)
            {
                if( nodepair.Value.intensity_is_new == true)
                {
                    layer_has_changed = true;
                }
            }
            return layer_has_changed;
        }
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

    public double intensity = 0;
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

    public static double Sigmoid(double x)
    {
        return 1 / (1 + System.Math.Pow(System.Math.E, -x));
    }
}