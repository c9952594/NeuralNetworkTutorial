using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;

namespace SimpleANN.Test
{
    public class SimpleExample
    {
        [Test]
        public void SeeDiagonals()
        {
            var layerConfiguration = new LayerConfiguration( 1, 4, 4, 2, 1);
            var (bias, input, hidden1, hidden2, output) = layerConfiguration;
            var connections = new Connections(layerConfiguration, new[]
            {
                new[] {
                    new[] { hidden1, 0, 1 },
                    new[] { input, 0, 1 },
                    new[] { bias }
                },
                new [] {
                    new[] { hidden1, 2, 3 },
                    new[] { input, 2, 3 },
                    new[] { bias }
                },
                new [] {
                    new[] { hidden2, 0 },
                    new[] { hidden1, 1, 2 },
                    new[] { bias }
                },
                new [] {
                    new[] { hidden2, 1 },
                    new[] { hidden1, 0, 3 },
                    new[] { bias }
                },
                new [] {
                    new[] { output },
                    new[] { hidden2 }
                }
            });
            var conns = connections.Instructions;
            
            Debugger.Break();
            

            //var nodes = layerConfiguration.Nodes();
            //var weights = layerConfiguration.Weights(-1, 1);

            //nodes.Length.ShouldBe(layerConfiguration.Sum());


            //connections.Length.ShouldBe(weights.Length);

            //nodes[bias] = 1;
            //nodes[input, 0, 1, 2, 3] = new[] { 1, 2, 3, 4 };

            //connections.ApplyOver(nodes, weights);

            Assert.Inconclusive();
        }
        
    }
}
