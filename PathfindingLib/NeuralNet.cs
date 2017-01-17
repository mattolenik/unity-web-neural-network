using System;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingLib
{
    public class NeuralNet
    {
        readonly int numHiddenLayers;
        readonly float bias;

        readonly List<NeuronLayer> layers;
        readonly float[] tempInputs;
        readonly float[] tempOutputs;
        readonly int numOutputs;

        public NeuralNet(int numInputs, int numOutputs, int numHiddenLayers, int neuronsPerHiddenLayer, float bias, Random rnd = null)
        {
            rnd = rnd ?? new Random();
            this.bias = bias;
            this.numHiddenLayers = numHiddenLayers;
            this.numOutputs = numOutputs;
            layers = new List<NeuronLayer>();

            // These lists will get reused during feed forward, set capacity
            // to highest that will be needed
            tempOutputs = new float[neuronsPerHiddenLayer + 1];
            tempInputs = new float[neuronsPerHiddenLayer + 1];

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

        public void FeedForward(float[] inputs, float[] outputs, Func<float, float> activationFunc)
        {
            // Allocations should be kept low in this method (e.g. arrays),
            // while these are probably unneeded but shouldn't hurt.
            int k, n, j = 0;
            Neuron neuron;
            float netinput;
            for (var i = 0; i < numHiddenLayers + 1; i++)
            {
                // For the first layer, use the inputs passed in.
                // For each subsequent layer, use the previous layer's output.
                if (i > 0)
                {
                    for (n = 0; n <= j; n++)
                    {
                        tempInputs[n] = tempOutputs[n];
                    }
                    inputs = tempInputs;
                }

                // Compute outputs for this layer
                for (j = 0; j < layers[i].Neurons.Count; j++)
                {
                    neuron = layers[i].Neurons[j];
                    netinput = 0.0f;

                    for (k = 0; k < neuron.Weights.Length - 1; k++)
                    {
                        netinput += neuron.Weights[k] * inputs[k];
                    }
                    netinput += neuron.Weights[neuron.Weights.Length - 1] * bias;
                    tempOutputs[j] = activationFunc(netinput);
                }
            }
            for (j = 0; j < outputs.Length; j++)
            {
                outputs[j] = tempOutputs[j];
            }
        }
    }
}