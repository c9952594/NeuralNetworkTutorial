using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Shouldly;


namespace UseLibraries
{
    public static class LayerConfigurationExtensions
    {
        public static double[] Nodes(this int[] layerConfiguration)
        {
            return new double[layerConfiguration.Sum()];
        }

        public static double[][] Nodes(this int[] layerConfiguration, int populationSize)
        {
            var population = new double[populationSize][];
            for (var i = 0; i < population.Length; i++)
            {
                population[i] = layerConfiguration.Nodes();
            }
            return population;
        }

        public static int NodeIndex(this int[] layerConfiguration, int layer, int node)
        {
            var numberOfNodesInPreviousLayers = 0;
            for (var index = 0; index < layer; index++)
            {
                numberOfNodesInPreviousLayers += layerConfiguration[index];
            }
            return numberOfNodesInPreviousLayers + node;
        }
 
        static readonly Random Generator = new Random();

        public static double[] Weights(this SortedDictionary<(int layer, int node), (int layer, int node)[]> connections)
        {
            var weights = new double[connections.Sum(connection => connection.Value.Length)];
            for (var index = 0; index < weights.Length; index++)
            {
                weights[index] = (Generator.NextDouble() * 2) - 1;
            }
            return weights;
        }

        public static double[][] Weights(this SortedDictionary<(int layer, int node), (int layer, int node)[]> connections, int size)
        {
            var population = new double[size][];
            for (var index = 0; index < population.Length; index++)
            {
                population[index] = connections.Weights();
            }

            return population;
        }

        public static void ApplyOver(this SortedDictionary<(int layer, int node), (int layer, int node)[]> connections,
            double[] nodes,
            double[] weights,
            int[] layerConfiguration,
            Func<double[], int[], double[], (int layer, int node)[], double> inputFunc,
            Func<double, double> activationFunc)
        {
            var weightsIndex = 0;

            foreach (var connection in connections)
            {
                var connectionWeights = new double[connection.Value.Length];
                Array.Copy(weights, weightsIndex, connectionWeights, 0, connectionWeights.Length);
                weightsIndex += connectionWeights.Length;

                var input = inputFunc(nodes, layerConfiguration, connectionWeights, connection.Value);
                var output = activationFunc(input);
                nodes[layerConfiguration.NodeIndex(connection.Key.layer, connection.Key.node)] = output;
            }
        }

        public static void ApplyOver(this SortedDictionary<(int layer, int node), (int layer, int node)[]> connections,
            double[][] population,
            double[][] weights,
            int[] layerConfiguration,
            Func<double[], int[], double[], (int layer, int node)[], double> inputFunc,
            Func<double, double> activationFunc)
        {
            for (var index = 0; index < population.Length; index++)
            {
                connections.ApplyOver(population[index], weights[index], layerConfiguration, inputFunc, activationFunc);
            }
        }

        public static (int layer, int node)[] IncludeBias(this (int layer, int node)[] nodes, int bias)
        {
            return nodes.Append((bias, 0)).ToArray();
        }

        public static (int layer, int node)[] Nodes(this int layer, params int[] nodes)
        {
            return nodes.Select(node => (layer, node)).ToArray();
        }
    
        public static void SetInput(this double[] nodes, double[] values, int[] layerConfiguration, int layer)
        {
            var inputLayerIndex = layerConfiguration.NodeIndex(layer, 0);
            Array.Copy(values, 0, nodes, inputLayerIndex, values.Length);
        }

        public static void SetInput(this double[][] nodes, double[] values, int[] layerConfiguration, int layer)
        {
            for (var index = 0; index < nodes.Length; index++)
            {
                nodes[index].SetInput(values, layerConfiguration, layer);
            }
        }

        public static double ValueAt(this double[] nodes, int[] layerConfiguration, int layer, int node)
        {
            return nodes[layerConfiguration.NodeIndex(layer, node)];
        }

        public static double[] SetBias(this double[] nodes, int[] layerConfiguration, int biasLayer, int bias)
        {
            nodes[layerConfiguration.NodeIndex(biasLayer, 0)] = bias;
            return nodes;
        }

        public static double[][] SetBias(this double[][] population, int[] layerConfiguration, int biasLayer, int bias)
        {
            for (var index = 0; index < population.Length; index++)
            {
                population[index].SetBias(layerConfiguration, biasLayer, bias);
            }
            return population;
        }
    
        public static (double score, double[] member)[] Ranked(this double[][] population, Func<double[], double> scoreFunc) {
            var ranked = new (double, double[])[population.Length];
            for (var index = 0; index < population.Length; index++) {
                var score = scoreFunc(population[index]);
                ranked[index] = (score, population[index]);
            }
            return ranked.OrderByDescending(r => r.Item1).ToArray();
        }
    }

    public class Network : IEnumerable<(int, int)[]>
    {
        readonly int[] layerConfiguration;

        public Network(params int[] layerConfiguration)
        {
            this.layerConfiguration = layerConfiguration;
        }
        
        public IEnumerator<(int, int)[]> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class ConnectionBuilder
    {
        
    }

    public class Class1
    {
        [Test]
        public void SingleNetwork()
        {
            var layerConfiguration = new[] { 1, 4, 4, 2, 1 };
            var (bias, input, hidden1, hidden2, output) = layerConfiguration.Labels();
            var connections = layerConfiguration.Connections(new []
            {
                (new [] { hidden1, 0, 1 }, new [] {
                    new[] { input, 0, 1 },
                    new[] { bias }
                }),
                (new [] { hidden1, 2, 3 }, new [] {
                    new[] { input, 2, 3 },
                    new[] { bias }
                }),
                (new [] { hidden2, 0 }, new [] {
                    new[] { hidden1, 1, 2 },
                    new[] { bias }
                }),
                (new [] { hidden2, 1 }, new [] {
                    new[] { hidden1, 0, 3 },
                    new[] { bias }
                }),
                (new [] { output }, new [] {
                    new[] { hidden2 }
                })
            });

            var nodes = layerConfiguration.Nodes();
            nodes.Length.ShouldBe(layerConfiguration.Sum());
            
            var weights = layerConfiguration.Weights(-1, 1);
            connections.Length.ShouldBe(weights.Length);

            nodes[bias] = 1;
            nodes[input, 0, 1, 2, 3] = new[] {1, 2, 3, 4};

            connections.ApplyOver(nodes, weights);


            //var connections = new SortedDictionary<(int layer, int node), (int layer, int node)[]>()
            //{
            //    [(hidden1, 0)] = input.Nodes(0, 1).IncludeBias(bias),
            //    [(hidden1, 1)] = input.Nodes(0, 1).IncludeBias(bias),
            //    [(hidden1, 2)] = input.Nodes(2, 3).IncludeBias(bias),
            //    [(hidden1, 3)] = input.Nodes(2, 3).IncludeBias(bias),

            //    [(hidden2, 0)] = hidden1.Nodes(1, 2).IncludeBias(bias),
            //    [(hidden2, 1)] = hidden1.Nodes(0, 3).IncludeBias(bias),

            //    [(output, 0)] = hidden2.Nodes(0, 1)
            //};

            //var nodes = layerConfiguration.Nodes().SetBias(layerConfiguration, bias, 1);
            //var weights = connections.Weights();

            //nodes.SetInput(new double[] { 1, 1, -1, -1 }, layerConfiguration, input);
            //connections.ApplyOver(nodes, weights, layerConfiguration, Simple, StepChange);

            //nodes.ShouldSatisfyAllConditions(
            //    () => nodes.ValueAt(layerConfiguration, input, 0).ShouldBe(1),
            //    () => nodes.ValueAt(layerConfiguration, input, 1).ShouldBe(1),
            //    () => nodes.ValueAt(layerConfiguration, input, 2).ShouldBe(-1),
            //    () => nodes.ValueAt(layerConfiguration, input, 3).ShouldBe(-1),
            //    () => nodes.ValueAt(layerConfiguration, hidden1, 0).ShouldBe(0),
            //    () => nodes.ValueAt(layerConfiguration, hidden1, 1).ShouldBe(1),
            //    () => nodes.ValueAt(layerConfiguration, hidden1, 2).ShouldBe(1),
            //    () => nodes.ValueAt(layerConfiguration, hidden1, 3).ShouldBe(0),
            //    () => nodes.ValueAt(layerConfiguration, hidden2, 0).ShouldBe(1),
            //    () => nodes.ValueAt(layerConfiguration, hidden2, 1).ShouldBe(0),
            //    () => nodes.ValueAt(layerConfiguration, output, 0).ShouldBe(1),
            //    () => nodes.ValueAt(layerConfiguration, bias, 0).ShouldBe(1)
            //);
        }

        [Test]
        public void Shorter() {
            var (bias, input, hidden1, hidden2, output) = (0, 1, 2, 3, 4);
            var layerConfiguration = new[] { 1, 4, 4, 2, 1 };
            
            var connections = new SortedDictionary<(int layer, int node), (int layer, int node)[]>()
            {
                [(hidden1, 0)] = input.Nodes(0, 1).IncludeBias(bias),
                [(hidden1, 1)] = input.Nodes(0, 1).IncludeBias(bias),
                [(hidden1, 2)] = input.Nodes(2, 3).IncludeBias(bias),
                [(hidden1, 3)] = input.Nodes(2, 3).IncludeBias(bias),

                [(hidden2, 0)] = hidden1.Nodes(1, 2).IncludeBias(bias),
                [(hidden2, 1)] = hidden1.Nodes(0, 3).IncludeBias(bias),

                [(output, 0)] = hidden2.Nodes(0, 1)
            };

            var populationSize = 1000;
            var population = layerConfiguration.Nodes(populationSize).SetBias(layerConfiguration, bias, 1);
            var populationWeights = connections.Weights(populationSize);
            
            population.SetInput(new double[] { 1, 1, -1, -1 }, layerConfiguration, input);
            connections.ApplyOver(population, populationWeights, layerConfiguration, Simple, StepChange);




            var ranked = population.Ranked(member => {
                var score = 0.0;
                score += member.ValueAt(layerConfiguration, input, 0) == 1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, input, 1) == 1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, input, 2) == -1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, input, 3) == -1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, hidden1, 0) == 0 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, hidden1, 1) == 1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, hidden1, 2) == 1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, hidden1, 3) == 0 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, hidden2, 0) == 1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, hidden2, 1) == 0 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, output, 0) == 1 ? 1 : 0;
                score += member.ValueAt(layerConfiguration, bias, 0) == 1 ? 1 : 0;     
                return score;
            });

            
            var nodes = ranked[0].member;
            nodes.ShouldSatisfyAllConditions(
                () => nodes.ValueAt(layerConfiguration, input, 0).ShouldBe(1),
                () => nodes.ValueAt(layerConfiguration, input, 1).ShouldBe(1),
                () => nodes.ValueAt(layerConfiguration, input, 2).ShouldBe(-1),
                () => nodes.ValueAt(layerConfiguration, input, 3).ShouldBe(-1),
                () => nodes.ValueAt(layerConfiguration, hidden1, 0).ShouldBe(0),
                () => nodes.ValueAt(layerConfiguration, hidden1, 1).ShouldBe(1),
                () => nodes.ValueAt(layerConfiguration, hidden1, 2).ShouldBe(1),
                () => nodes.ValueAt(layerConfiguration, hidden1, 3).ShouldBe(0),
                () => nodes.ValueAt(layerConfiguration, hidden2, 0).ShouldBe(1),
                () => nodes.ValueAt(layerConfiguration, hidden2, 1).ShouldBe(0),
                () => nodes.ValueAt(layerConfiguration, output, 0).ShouldBe(1),
                () => nodes.ValueAt(layerConfiguration, bias, 0).ShouldBe(1)
            );
        }

        public static double Simple(double[] nodes, int[] layerConfiguration, double[] weights, (int layer, int node)[] inputs) {
            var total = 0.0;
            for (var index = 0; index < inputs.Length; index++) {
                var input = nodes.ValueAt(layerConfiguration, inputs[index].layer, inputs[index].node);
                total += input * weights[index];
            }
            return total;
        }

        public static double StepChange(double input)
        {
            return input > 0 ? 1 : 0;
        }
    }
}
