using System;
using System.Collections.Generic;
namespace GPImplemenation
{
    public class Chromosome
    {
        public float Fitness { get; set; }
        public Node Node { get; set; }
        public Chromosome() { }
        public Chromosome Clone()
        {
            //Create new chromosome object
            var ch = new Chromosome();
            ch.Fitness = this.Fitness;

            //clone nodes
            ch.Node = Node.Clone();
            return ch;

        }
        public void Generate(int maxLevels, Function[] fSet, string[] tSet)
        {
            this.Fitness = float.MinValue;
            this.Node = new Node();
            this.Node.Level = 0;
            this.Node.Generate(maxLevels, fSet, tSet);
            return;

        }
        public void Crossover(Chromosome ch2, int maxlevels, Function[] fSet, string[] tSet)
        {
            //select random number between 1 and count
            int index1 = Node.Rand.Next(1, NodeCount(this.Node));
            int index2 = Node.Rand.Next(1, NodeCount(ch2.Node));

            //Get random parts
            var p1 = GetNodeAt(this.Node, index1);
            var p2 = GetNodeAt(ch2.Node, index2);

            //clone parts 
            var part1 = p1.Clone();
            var part2 = p2.Clone();

            //share genetic material between parents
            p1.Value = part2.Value;
            p1.Children = part2.Children;
            p2.Value = part1.Value;
            p2.Children = part1.Children;

            //trim branches higher than max levels
            this.Node.Trim(maxlevels, tSet);
            ch2.Node.Trim(maxlevels, tSet);

        }
        public void Mutate(int maxLevels, Function[] funSet, string[] terSet)
        {
            //select random number between 1 and count
            int index = Node.Rand.Next(1, NodeCount(this.Node));

            //start counter from 0
            int count = 0;

            //Collection holds tree nodes
            Stack<Node> dataTree = new Stack<Node>();

            //helper vars
            Node.Level = 0;
            dataTree.Push(this.Node);
            Node n = null;

            while (dataTree.Count > 0)
            {
                //get next n
                n = dataTree.Pop();

                //when the counter is equal to index
                if (count == index)
                {
                    //check if the level < maxLeve
                    if (n.Level == maxLevels)
                    {
                        n.IsFunction = false;
                        n.Value = Node.Rand.Next(0, terSet.Length);
                        n.Children = null;
                    }
                    else
                    {
                        n.Generate(maxLevels, funSet, terSet);
                    }
                }

                if (n.Children != null)
                {
                    n.IsFunction = true;
                    for (int i = n.Children.Length - 1; i >= 0; i--)
                    {
                        n.Children[i].Level = (n.Level + 1);
                        dataTree.Push(n.Children[i]);
                    }

                }
                else
                    n.IsFunction = false;

                //count n
                count++;
            }

        }
        // Helper methods
        private Node GetNodeAt(Node n, int index)
        {
            //Helper vars
            Node temp;
            Stack<Node> nodeTree = new Stack<Node>();
            nodeTree.Push(n);
            int counter = 0;
            //
            while (nodeTree.Count > 0)
            {
                temp = nodeTree.Pop();
                counter++;

                //when random number meets the n count
                if (index == counter)
                {
                    //return clone of  this part
                    return temp.Clone();
                }

                //
                if (temp.Children != null)
                {
                    for (int i = 0; i < temp.Children.Length; i++)
                        nodeTree.Push(temp.Children[i]);
                }
            }
            //this line should not be called
            throw new Exception("No random nodes!");
        }

        private int NodeCount(Node n)
        {
            {
                //first count nodes in tree expression
                int count = 0;
                Node temp;
                Stack<Node> nodeTree = new Stack<Node>();
                nodeTree.Push(n);
                //
                while (nodeTree.Count > 0)
                {
                    temp = nodeTree.Pop();
                    count++;
                    if (temp.Children != null)
                    {
                        for (int i = 0; i < temp.Children.Length; i++)
                            nodeTree.Push(temp.Children[i]);
                    }
                }

                return count;

            }

        }
    }
}