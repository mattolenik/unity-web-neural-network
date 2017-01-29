using System;

namespace Rollaround
{
    public struct NeuronLayer
    {
        public Neuron[] Neurons;

        public NeuronLayer(int numNeurons, int numInputsPerNeuron, Random rnd)
        {
            Neurons = new Neuron[numNeurons];
            for (var i = 0; i < numNeurons; i++)
            {
                Neurons[i] = new Neuron(numInputsPerNeuron, rnd);
            }
        }
    }
}