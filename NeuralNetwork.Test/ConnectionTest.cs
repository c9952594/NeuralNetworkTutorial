using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Test
{

    class ConnectionTest
    {
        [Test]
        public void ShouldFireIfInputChanged()
        {
            var connection = new Connection(0.5);
            var called = false;

            connection.InputChangedEvent += (object s, EventArgs e) 
                => called = true;
            connection.Input = 2;

            called.ShouldBeTrue();
            connection.Output.ShouldBe(1);
        }

        [Test]
        public void ShouldFireIfWeightChanged()
        {
            var connection = new Connection(0);
            var called = false;

            connection.WeightChangedEvent += (object s, EventArgs e)
                => called = true;

            connection.Input = 1;
            connection.Weight = 2;
            
            called.ShouldBeTrue();
            connection.Output.ShouldBe(2);
        }

        [Test]
        public void ShouldHonorGenerateRandomWeightLimits()
        {
            Connection.WithRandomWeight(() => 0.25, -1, 1).Weight.ShouldBe(-0.5, 0.001);
            Connection.WithRandomWeight(() => 0.75, -1, 1).Weight.ShouldBe(0.5, 0.001);

            Connection.WithRandomWeight(() => 0.25, 0, 0.4).Weight.ShouldBe(0.1, 0.001);
            Connection.WithRandomWeight(() => 0.75, 0, 0.4).Weight.ShouldBe(0.3, 0.001);

            Connection.WithRandomWeight(() => 0.25, 1, 5).Weight.ShouldBe(2, 0.001);
            Connection.WithRandomWeight(() => 0.75, 1, 5).Weight.ShouldBe(4, 0.001);
        }
    }
}
