using System;
using System.Collections.Generic;

namespace PathfindingLib
{
    public class NeuralNet
    {
        readonly int numHiddenLayers;
        readonly float bias;

        readonly List<NeuronLayer> layers;
        readonly List<float> outputs;

        public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int neuronsPerHiddenLayer, float bias, Random rnd = null)
        {
            rnd = rnd ?? new Random();
            this.bias = bias;
            this.numHiddenLayers = numHiddenLayers;
            outputs = new List<float>(numOutputs);
            layers = new List<NeuronLayer>();
            if (this.numHiddenLayers > 0)
            {
                layers.Add(new NeuronLayer(neuronsPerHiddenLayer, numInputs, rnd));
                for (var i = 0; i < this.numHiddenLayers - 1; i++)
                {
                    layers.Add(new NeuronLayer(neuronsPerHiddenLayer, neuronsPerHiddenLayer, rnd));
                }
                layers.Add(new NeuronLayer(numOutputs, neuronsPerHiddenLayer, rnd));
            }
            else
            {
                layers.Add(new NeuronLayer(numOutputs, numInputs, rnd));
            }
        }

        public List<float> GetWeights()
        {
            var weights = new List<float>();

            foreach (var layer in layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    weights.AddRange(neuron.Weights);
                }
            }

            return weights;
        }

        public void PutWeights(List<float> inputs)
        {
            var weight = 0;
            foreach (var layer in layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    for (var i = 0; i < neuron.Weights.Length; i++)
                    {
                        neuron.Weights[i] = inputs[weight++];
                    }
                }
            }
        }

        public List<float> FeedForward(List<float> inputs, Func<float, float> activationFunc) 
        {
            for (var i = 0; i < numHiddenLayers + 1; i++)
            {
                var layer = layers[i];
                if (i > 0)
                {
                    inputs = outputs;
                }
                outputs.Clear();

                foreach (var neuron in layer.Neurons)
                {
                    var netinput = 0.0f;

                    for (var k = 0; k < neuron.Weights.Length - 1; k++)
                    {
                        netinput += neuron.Weights[k] * inputs[k];
                    }
                    netinput += neuron.Weights[neuron.Weights.Length - 1] * bias;
                    outputs.Add(activationFunc(netinput));
                }
            }
            return outputs;
        }
    }
}