using System.Collections;
using System.Collections.Generic;

public class NeuralNetwork{

    Node_Layer input_layer;
    List<Node_Layer> hidden_layers;
    Node_Layer output_layer;

    readonly int amount_hidden_layers; // Doesn't need to have any layers (but it's probably wrong if it doesn't have any)
    readonly int amount_input_nodes; // Always has at least 1 node
    readonly int amount_output_nodes; // Always has at least 1 node

    // needs overloads
    NeuralNetwork(List<string> inputs, List<string> outputs, int hid_lyr = 0)
    {
        amount_hidden_layers = hid_lyr;


    }

    // Depricated atm, needs a bit of rewriting. Like get_intensity, but does nothing.
    void Fire_Network()
    {
        // Fire the entire network from the output nodes

        // Tell each node not on the input layer it's old

        output_layer.Invalidate();
        if(hidden_layers != null)
        {
            foreach (Node_Layer nd_lyer in hidden_layers)
            {
                nd_lyer.Invalidate();
            }
        }

        // go through all non-input layer nodes and generate new intensities
        // Because the function is recursive, only need to ask the output layer

        foreach(KeyValuePair<string,Node> nodepair in output_layer.nodes)
        {
            nodepair.Value.get_intensity();
        }

        // 
    }

    public double[] get_output()
    {
        // prep return array
        double[] temp = new double[amount_output_nodes];

        // tell the network it's values are considered old
        output_layer.Invalidate();
        foreach (Node_Layer nd_lyer in hidden_layers)
        {
            nd_lyer.Invalidate();
        }

        // convert the output_layer to double array
        for (int i =0; i< amount_output_nodes; i++)
        {
            temp[i] = output_layer.nodes[i].get_intensity() ;
        }

        return temp;
    }
}

public class Node_Layer
{
    public Dictionary<string, Node> nodes;

    // input layer without weights
    public Node_Layer( List<string> input_names )
    {
        foreach( string name in input_names)
        {
            nodes.Add( name, new Node() );
        }
    }

    // Used if hidden layer or output layer
    public Node_Layer()
    {
        //

    }

    // Tells all nodes in this layer that they're old and need to be regenerated when asked thier intensity
    public void Invalidate()
    {
        foreach(KeyValuePair<string,Node> nodepair in nodes)
        {
            nodepair.Value.intensity_is_new = false;
        }
    }
}

public class Node
{
    public readonly double bias;

    double intensity = 0;
    public bool intensity_is_new = false;

    bool is_input; // if this layer doesn't have nodes as parent, but an input

    // Don't use external inputs yet. Input has to tell value to Node as for now.
    // bool has_extern_input;
    // int *extern_input;

    public Dictionary<Node,  double> parents; // parent, weight; can be empty!



    // Blank constructor, used for input layer
    public Node()
    {
        is_input = true;
    }

    // Have parents, but no weights to set
    public Node( List<Node> prnts, double bias = 0 )
    {
        this.bias = bias;
        foreach(Node node in prnts ) {
            parents.Add(node, 1d);
        }
    }

    // Have parents and weights
    public Node(List<KeyValuePair<Node, int>> prnts, double bias = 0)
    {
        this.bias = bias;
        foreach (KeyValuePair<Node, int> node in prnts)
        {
            parents.Add(node.Key, node.Value);
        }
    }

    // RECURSIVE
    public double get_intensity()
    {
        if(intensity_is_new == false)
        {
            activate_node();
            intensity_is_new = true;
        }
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

    void activate_node()
    {
        if(is_input) {
            // 
        } else
        {
            double temp_intens = 0;
            foreach (KeyValuePair<Node, double> entry in parents)
            {
                temp_intens += entry.Key.get_intensity() * entry.Value;
            }
            intensity = Sigmoid(temp_intens + bias);
        }
    }

    public static double Sigmoid(double x)
    {
        return 1 / (1 + System.Math.Pow(System.Math.E, -x));
    }
}