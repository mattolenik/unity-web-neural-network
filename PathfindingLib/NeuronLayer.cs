using System.Collections.Generic;

namespace PathfindingLib
{
    public class NeuronLayer
    {
        public List<Neuron> Neurons { get; private set; }

        public NeuronLayer(int numNeurons, int numInputsPerNeuron)
        {
            Neurons = new List<Neuron>(numNeurons);
            for (var i = 0; i < numNeurons; i++)
            {
                Neurons.Add(new Neuron(numInputsPerNeuron));
            }
        }
    }
}