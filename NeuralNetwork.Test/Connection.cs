using System;

namespace NeuralNetwork.Test
{
    class Connections {
    }


    class Connection
    {
        private static Random generator = new Random();

        private double input;
        public double Input {
            get => input;
            set {
                input = value;
                output = CalculateOutput();
                InputChangedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        private double weight;
        public double Weight {
            get => weight;
            set {
                weight = value;
                output = CalculateOutput();
                WeightChangedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        private double output;
        public double Output => output;
        private double CalculateOutput() => this.input * this.weight;

        public event EventHandler InputChangedEvent;
        public event EventHandler WeightChangedEvent;

        public Connection(double weight) => this.weight = weight;

        public static Connection WithRandomWeight(double min = -1, double max = 1) 
            => WithRandomWeight(generator.NextDouble, min, max);

        public static Connection WithRandomWeight(Func<double> nextDouble, double min = -1, double max = 1)
        {
            var randomValue = nextDouble();
            var range = max - min;
            var scaled = randomValue * range;
            var adjusted = scaled + min;
            return new Connection(adjusted);
        }
    }
}
