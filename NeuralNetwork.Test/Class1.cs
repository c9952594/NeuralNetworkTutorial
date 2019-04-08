//using NUnit.Framework;
//using Shouldly;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;

//namespace NeuralNetwork.Test
//{ 
//    public class Node
//    {
//        private static Random generator = new Random();
//        private double _output;
//        private List<(Node node, double weight)> inputs = new List<(Node, double)>();
//        private int inputsChanged = 0;
//        private readonly Func<List<(Node node, double weight)>, double> sumFunc;
//        private readonly Func<double, double> transferFunc;
//        public readonly int Id;

//        public Node(Func<List<(Node node, double weight)>, double> sumFunc, Func<double, double> transferFunc, int id)
//        {
//            this.sumFunc = sumFunc;
//            this.transferFunc = transferFunc;
//            this.Id = id;
//        }

//        public static double Simple(List<(Node node, double weight)> inputs) {
//            return inputs.Sum(m => m.node.Output * m.weight);
//        }

//        public static double StepChange(double input)
//        {
//            return input > 0 ? 1 : 0;
//        }

//        public double Output {
//            get => _output;
//            set {
//                _output = value;
//                OutputChangedEvent?.Invoke(this, new EventArgs());
//            }
//        }

//        public event EventHandler OutputChangedEvent;

//        public void ConnectTo(Node node, double? weight = null)
//        {
//            weight = weight ?? generator.NextDouble();
//            inputs.Add((node, weight.Value));
//            node.OutputChangedEvent += InputChanged;
//        }

//        private void InputChanged(object sender, EventArgs e)
//        {
//            if ((++inputsChanged) == inputs.Count())
//            {
//                var input = this.sumFunc(inputs);
//                var output = transferFunc(input);
//                Output = output;
//                inputsChanged = 0;
//            }
//        }

//        public override string ToString()
//        {
//            return $"{Id},{Output}";
//        }
//    }

//    public class Network
//    {
//        public readonly Node bias = new Node(Node.Simple, Node.StepChange, -1);
//        private Node[] nodes;

//        public Node this[int index] => nodes[index];

//        public Network(DNA dna)
//        {
//            var nodesRequired = dna.Sequence.Max(m => m.from) + 1;
//            nodes = Enumerable.Range(0, nodesRequired).Select(m => new Node(Node.Simple, Node.StepChange, m)).ToArray();
//            foreach (var connection in dna.Sequence)
//            {
//                var to = connection.to < 0 ? bias : nodes[connection.to];
//                nodes[connection.from].ConnectTo(to, connection.weight);
//            }
//        }

//        public void SetInput(params (int from, double value)[] inputs)
//        {
//            bias.Output = 1;
//            foreach (var input in inputs)
//            {
//                nodes[input.from].Output = input.value;
//            }
//        }
//    }

//    public class DNABuilder
//    {

//    }

//    public class DNA
//    {
//        public readonly (int from, int to, double weight)[] Sequence;

//        public DNA(params (int from, int to, double weight)[] sequence)
//        {
//            this.Sequence = sequence;
//        }

//        public DNA(IEnumerable<(int from, int to, double weight)> sequence) : this(sequence.ToArray())
//        {
//        }

//        public DNA(params (int layer, int index, (int layer, int index, int weight)[] to)[] connections) : this(GenerateConnections(connections))
//        {

//        }

//        private static IEnumerable<(int from, int to, double weight)> GenerateConnections((int layer, int index, (int layer, int index, int weight)[] to)[] connections)
//        {
//            foreach (var connection in connections)
//            {
//                var fromLayer = connection.layer;
//                var fromIndex = connection.index;
//                var to = connection.to;
//                to[0].
//            }
//        }
//    }

//    public class Class1
//    {
//        [Test]
//        public void BuilderTest()
//        {
//            var dna = new DNA(
//                (layer: 1, index: 0, to: new[] {
//                    (layer: 0, index: 0, weight: -1),
//                    (layer: 0, index: 1, weight: -1)
//                }),
//                (layer: 1, index: 1, to: new[] {
//                    (layer: 0, index: 0, weight: 1),
//                    (layer: 0, index: 1, weight: 1)
//                })
//            );

//            var dna2 = new DNA(
//                (layer: 1, index: 0, to: new[] {
//                    (layer: 0, index: new [] { 0, 1 }, weight: -1)
//                }),
//                (layer: 1, index: 1, to: new[] {
//                    (layer: 0, index: new [] { 0, 1 }, weight: 1)
//                })
//            );
//        }

//        [Test]
//        public void NetworkTest()
//        {
//            var dna = new DNA(
//                (4, 0, -1),
//                (4, 1, -1),
//                (4, -1, -1),

//                (5, 0, 1),
//                (5, 1, 1),
//                (5, -1, -1),

//                (6, 2, -1),
//                (6, 3, -1),
//                (6, -1, -1),

//                (7, 2, 1),
//                (7, 3, 1),
//                (7, -1, -1),

//                (8, 5, 1),
//                (8, 6, 1),
//                (8, -1, -1),

//                (9, 4, 1),
//                (9, 7, 1),
//                (9, -1, -1)

//                (10, 8, 1),
//                (10, 9, 1),

//            var network = new Network(dna);

//            network.SetInput(
//                (0, 1),
//                (1, 1),
//                (2, -1),
//                (3, -1)
//            );

//            network.ShouldSatisfyAllConditions(
//                () => network[4].Output.ShouldBe(0),
//                () => network[5].Output.ShouldBe(1),
//                () => network[6].Output.ShouldBe(1),
//                () => network[7].Output.ShouldBe(0),

//                () => network[8].Output.ShouldBe(1),
//                () => network[9].Output.ShouldBe(0),

//                () => network[10].Output.ShouldBe(1)
//            );

//            network.SetInput(
//                (0, -1),
//                (1, 1),
//                (2, 1),
//                (3, 1)
//            );

//            network.ShouldSatisfyAllConditions(
//                () => network[4].Output.ShouldBe(0),
//                () => network[5].Output.ShouldBe(0),
//                () => network[6].Output.ShouldBe(0),
//                () => network[7].Output.ShouldBe(1),

//                () => network[8].Output.ShouldBe(0),
//                () => network[9].Output.ShouldBe(0),

//                () => network[10].Output.ShouldBe(0)
//            );


//        }
//    }
//}