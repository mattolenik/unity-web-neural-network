using System;
using System.Collections.Generic;

namespace Rollaround
{
    public class NeuronLayer
    {
        public List<Neuron> Neurons { get; }

        public NeuronLayer(int numNeurons, int numInputsPerNeuron, Random rnd)
        {
            Neurons = new List<Neuron>(numNeurons);
            for (var i = 0; i < numNeurons; i++)
            {
                Neurons.Add(new Neuron(numInputsPerNeuron, rnd));
            }
        }
    }
}