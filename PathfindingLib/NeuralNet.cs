using System;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingLib
{
    public class NeuralNet
    {
        readonly int hiddenLayers;
        readonly double bias;

        readonly List<NeuronLayer> layers;

        public NeuralNet(int inputs, int outputs, int hiddenLayers, int neuronsPerHiddenLayer, double bias)
        {
            this.bias = bias;
            this.hiddenLayers = hiddenLayers;
            layers = new List<NeuronLayer>();
            if (this.hiddenLayers > 0)
            {
                layers.Add(new NeuronLayer(neuronsPerHiddenLayer, inputs));
                for (var i = 0; i < this.hiddenLayers - 1; i++)
                {
                    layers.Add(new NeuronLayer(neuronsPerHiddenLayer, neuronsPerHiddenLayer));
                }
                layers.Add(new NeuronLayer(outputs, neuronsPerHiddenLayer));
            }
            else
            {
                layers.Add(new NeuronLayer(outputs, inputs));
            }
        }

        public List<double> GetWeights()
        {
            var weights = new List<double>();

            foreach (var layer in layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    weights.AddRange(neuron.Weights);
                }
            }

            return weights;
        }

        public void PutWeights(List<double> inputs)
        {
            var weight = 0;
            foreach (var layer in layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    for (var i = 0; i < neuron.Weights.Count; i++)
                    {
                        neuron.Weights[i] = inputs[weight++];
                    }
                }
            }
        }

        public List<double> FeedForward(List<double> inputs)
        {
            return FeedForward(inputs, Sigmoid);
        }

        public List<double> FeedForward(List<double> inputs, Func<double, double> activationFunc) 
        {
            var outputs = new List<double>();

            for (var i = 0; i < hiddenLayers + 1; i++)
            {
                var layer = layers[i];
                if (i > 0)
                {
                    inputs = outputs.ToList();
                }
                outputs.Clear();

                foreach (var neuron in layer.Neurons)
                {
                    var netinput = 0.0;

                    for (var k = 0; k < neuron.Weights.Count - 1; k++)
                    {
                        netinput += neuron.Weights[k] * inputs[k];
                    }
                    netinput += neuron.Weights[neuron.Weights.Count - 1] * bias;
                    outputs.Add(activationFunc(netinput));
                }
            }
            return outputs;
        }

        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }
}