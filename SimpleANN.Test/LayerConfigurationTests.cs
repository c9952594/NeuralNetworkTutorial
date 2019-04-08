using System;
using NUnit.Framework;
using Shouldly;

namespace SimpleANN.Test
{
    public class LayerConfigurationTests
    {
        [Test]
        public void ShouldAllowIdsToBeExtracted()
        {
            var layerConfiguration = new LayerConfiguration(1, 4, 4, 2, 1);
            var (bias, input, hidden1, hidden2, output) = layerConfiguration;
            bias.ShouldBe(0);
            input.ShouldBe(1);
            hidden1.ShouldBe(2);
            hidden2.ShouldBe(3);
            output.ShouldBe(4);
        }

        [Test]
        public void NumberOfIdsMustMatchNumberOfLayers()
        {
            var layerConfiguration = new LayerConfiguration(1, 4, 4, 2, 1);
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var (bias, input, hidden1, hidden2) = layerConfiguration;
            }).Message.ShouldBe("Was expecting a 5 Ids but found 4\r\nParameter name: layerConfiguration");
        }
    }
}