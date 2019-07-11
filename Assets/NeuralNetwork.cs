using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public List<Node> input_layer;
    public List<Node> output_layer;

    public Dictionary<string, Node> input_layer_dict = new Dictionary<string, Node>();


    public NeuralNetwork(int amount_inputs, int amount_outputs)
    {
        for(int i = 0; i < amount_inputs; i++)
        {
            input_layer.Add(new Node(0));
        }
        for (int i = 0; i < amount_outputs; i++)
        {
            input_layer.Add(new Node(amount_inputs));
        }
    }


    public void Fire_network()
    {
        // Top-to-bottom search - for optimization
        for(int i = 0; i < output_layer.Count; i++)
        {
            bool node_needs_update = false;
            for( int j = 0; i < input_layer.Count; i++)
            {
                if (input_layer[j].has_changed)
                {
                    node_needs_update = true;
                    break;
                }
            }
            if (node_needs_update)
            {
                output_layer[i].fire_node(ref input_layer);
                output_layer[i].has_changed = true;
            }
        }

        //


        // finally
        // set the Network that every intensity is as it was the last (this) time.
        for (int i = 0; i < input_layer.Count; i++)
        {
            input_layer[i].has_changed = false;
        }
        for (int i = 0; i < output_layer.Count; i++)
        {
            output_layer[i].has_changed = false;
        }
    }

    public void  set_input_intensity(int node, double intense)
    {
        input_layer[node].intensity = intense;
        input_layer[node].has_changed = true;
    }

    public double get_output_intensity(int node)
    {
        return output_layer[node].intensity;
    }

    [System.Serializable]
    public class Node
    {
        public double bias = 0d;
        public double intensity = 0d;
        public double[] weights;
        public bool has_changed = true;

        public Node(int amount_parent_nodes)
        {
            weights = new double[amount_parent_nodes];
            for(int i = 0; i<= amount_parent_nodes; i++)
            {
                weights[i] = 1d;
            }
            bias = 0d;
        }
        public Node(double[] weights, double bias)
        {
            this.weights = weights;
            this.bias = bias;
        }

        public void fire_node(ref List<Node> parents)
        {
            double sum = 0d;
            for(int i = 0; i<parents.Count; i++)
            {
                sum += parents[i].intensity * weights[i];
            }
            sum += bias;
            intensity = Sigmoid(sum);
        }
    }

    public static double Sigmoid(double x)
    {
        return 1 / (1 + System.Math.Pow(System.Math.E, -x));
    }
}
