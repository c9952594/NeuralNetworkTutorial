using System.Collections.Generic;

namespace SimpleANN
{
    public class Instruction
    {
        public readonly int FromNode;
        public readonly int[] ToNodes;

        public Instruction(int fromNode, int[] toNodes)
        {
            this.FromNode = fromNode;
            this.ToNodes = toNodes;
        }
    }
}