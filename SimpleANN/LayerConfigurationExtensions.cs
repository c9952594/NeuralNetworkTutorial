using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleANN
{
    public class Connections
    {
        public readonly Instruction[] Instructions;
        private readonly LayerConfiguration layerConfiguration;

        public Connections(Instruction[] instructions)
        {
            this.Instructions = instructions;
        }

        public Connections(LayerConfiguration layerConfiguration, int[][][] connections)
        {
            this.layerConfiguration = layerConfiguration;
            this.Instructions = Create(connections);
        }

        private Instruction[] Create(int[][][] connections)
        {
            connections = (int[][][])connections.Clone();

            var cleaned = connections.Select(connection => {
                for (var connectionIndex = 0; connectionIndex < connection.Length; connectionIndex++)
                {
                    var line = connection[connectionIndex];
                    var layer = line[0];

                    // Fix missing nodes
                    bool noNodes = line.Length == 1;
                    if (noNodes)
                    {
                        var missingNodes = layerConfiguration.NodesAt(layer);
                        line = missingNodes.Prepend(layer).ToArray();
                        connection[connectionIndex] = line;
                    }

                    // Turn into node indexes
                    var skipLayer = line.Skip(1);
                    var indexes = skipLayer.Select(node => layerConfiguration.IndexAt(layer, node));
                    connection[connectionIndex] = indexes.ToArray();
                }
                return connection;
            });
          
            var instructions = cleaned.SelectMany(connection =>
            {
                var fromNodes = connection[0];
                return fromNodes.Select(fromNode =>
                {
                    var toNodes = connection.Skip(1).SelectMany(m => m).ToArray();
                    return new Instruction(fromNode, toNodes);
                }).ToArray();
            }).ToArray();

            return instructions;
        }
    }

    public class LayerConfiguration
    {
        readonly int[] nodesPerLayer;

        public LayerConfiguration(params int[] nodesPerLayer)
        {
            this.nodesPerLayer = nodesPerLayer;
        }

        public int IndexAt(int layer, int node) {
            var nodesRequired = 0;
            for(var index = 0; index < layer; index++)
            {
                nodesRequired += nodesPerLayer[index];
            }
            return nodesRequired + node;
        }

        public int[] NodesAt(int layer)
        {
            return Enumerable.Range(0, nodesPerLayer[layer]).ToArray();
        }

        public void Deconstruct(out int a, out int b)
        {
            CheckSize(2);
            (a, b) = (0, 1);
        }

        public void Deconstruct(out int a, out int b, out object c)
        {
            CheckSize(3);
            (a, b, c) = (0, 1, 2);
        }

        public void Deconstruct(out int a, out int b, out int c, out int d)
        {
            CheckSize(4);
            (a, b, c, d) = (0, 1, 2, 3);
        }

        public void Deconstruct(out int a, out int b, out int c, out int d, out int e)
        {
            CheckSize(5);
            (a, b, c, d, e) = (0, 1, 2, 3, 4);
        }

        void CheckSize(int size)
        {
            var numberOfLayers = nodesPerLayer.Length;
            if (numberOfLayers != size)
            {
                throw new ArgumentException(nameof(numberOfLayers), $"Was expecting a {numberOfLayers} Ids but found {size}");
            }
        }

        
    }
}
