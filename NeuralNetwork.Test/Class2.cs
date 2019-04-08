using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Test
{
    class Node
    {
        private SimpleInputFunction summation;
        private SigmoidTransferFunction sigmoid;

        public Node(SimpleInputFunction summation, SigmoidTransferFunction sigmoid)
        {
            this.summation = summation;
            this.sigmoid = sigmoid;
        }
    }

    class Layer
    {
        public readonly Node[] Nodes;

        public Layer(int numberOfNodes, SimpleInputFunction summation, SigmoidTransferFunction sigmoid)
        {
            Nodes = new Node[numberOfNodes];
            for(var index = 0; index < numberOfNodes; index++)
            {
                Nodes[index] = new Node(summation, sigmoid);
            }
        }
    }

    class Network
    {
        private Layer[] layers;
        private (int, int, (int, int[], double, double)[])[] p;
        private (SimpleInputFunction summation, SigmoidTransferFunction sigmoid)[] p1;
        private (int, int, (int, int[], double, double)[])[] p2;
        private (int, SimpleInputFunction summation, SigmoidTransferFunction sigmoid)[] p3;

        public Network(Layer[] layers, (Layer fromLayer, int fromNode, (Layer toLayer, int[] toNodes, double weightMin, double weightMax)[])[] connections)
        {
            this.layers = layers;
        }

        public Network(Layer[] layers, (int fromLayer, int fromNode, (int toLayer, int[] toNodes, double weightMin, double weightMax)[] to)[] p)
        {
            this.layers = layers;
            this.p = p;
        }

        public Network((SimpleInputFunction summation, SigmoidTransferFunction sigmoid)[] p1, (int fromLayer, int fromNode, (int toLayer, int[] toNodes, double weightMin, double weightMax)[] to)[] p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Network((int layer, SimpleInputFunction summation, SigmoidTransferFunction sigmoid)[] p3, (int fromLayer, int fromNode, (int toLayer, int[] toNodes, double weightMin, double weightMax)[] to)[] p2)
        {
            this.p3 = p3;
            this.p2 = p2;
        }
    }

    public class NetworkTests
    {
       

        [Test]
        public void LayerShouldAcceptNodes()
        {
            var summation = new SimpleInputFunction();
            var sigmoid = new SigmoidTransferFunction();

            const int inputLayer = 0;
            const int hiddenLayer1 = 1;
            const int hiddenLayer2 = 2;
            const int outputLayer = 3;

            var network = new Network(new[] {
                (inputLayer, summation, sigmoid),
                (hiddenLayer1, summation, sigmoid),
                (hiddenLayer2, summation, sigmoid),
                (outputLayer, summation, sigmoid)
            }, new[] {
                (hiddenLayer1, 0, new [] {
                    (inputLayer, new[] { 0, 1 }, -1.0, -1.0)
                }),
                (hiddenLayer1, 1, new [] {
                    (inputLayer, new[] { 0, 1 }, 1.0, 1.0)
                }),

                (hiddenLayer1, 2, new [] {
                    (inputLayer, new[] { 2, 3 }, -1.0, -1.0)
                }),
                (hiddenLayer1, 3, new [] {
                    (inputLayer, new[] { 2, 3 }, 1.0, 1.0)
                }),

                (hiddenLayer2, 0, new [] {
                    (hiddenLayer1, new[] { 1, 2 }, 1.0, 1.0)
                }),
                (hiddenLayer2, 1, new [] {
                    (hiddenLayer1, new[] { 0, 3 }, 1.0, 1.0)
                }),

                (outputLayer, 0, new [] {
                    (hiddenLayer2, new[] { 0, 1 }, 1.0, 1.0)
                })
            });

        }
    }
    
}
