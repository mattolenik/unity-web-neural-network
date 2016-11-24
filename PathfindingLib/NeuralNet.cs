using System;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingLib
{
    public class NeuralNet
    {
        int hiddenLayers = 1;
        int bias = -1;
        double activationResponse = 1;

        readonly List<NeuronLayer> layers;

        public NeuralNet(int numInputs, int numOutputs, int neuronsPerHiddenLayer)
        {
            layers = new List<NeuronLayer>();
            if (hiddenLayers > 0)
            {
                layers.Add(new NeuronLayer(neuronsPerHiddenLayer, numInputs));
                for (var i = 0; i < hiddenLayers - 1; i++)
                {
                    layers.Add(new NeuronLayer(neuronsPerHiddenLayer, neuronsPerHiddenLayer));
                }
                layers.Add(new NeuronLayer(numOutputs, neuronsPerHiddenLayer));
            }
            else
            {
                layers.Add(new NeuronLayer(numOutputs, numInputs));
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

        public void PutWeights(IEnumerable<double> inputs)
        {
        }

        public List<double> FeedForward(List<double> inputs)
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
                    outputs.Add(Sigmoid(netinput, activationResponse));
                }
            }
            return outputs;
        }

        public double Sigmoid(double netinput, double response)
        {
            return 1 / (1 + Math.Exp(-netinput / response));
        }
    }
}