using System;
using System.Linq;
using System.Collections.Generic;
namespace GPImplemenation
{
    public class Node
    {
        // Properties
        static public Random Rand = new Random((int)DateTime.Now.Ticks);
        public int Value { get; set; }
        public int Level { get; set; }
        public bool IsFunction { get; set; }
        public Node[] Children { get; set; }
        // Operations
        internal Node Clone()
        {
            //create new node object
            var n = new Node();
            //copy node properties
            n.IsFunction = IsFunction;
            n.Level = Level;
            n.Value = Value;
            if (Children == null)
                return n;

            //in case of function node clone its children
            n.Children = new Node[Children.Length];
            for (int i = 0; i < Children.Length; i++)
                n.Children[i] = Children[i].Clone();//recursively call clone operation

            return n;
        }

        internal void Generate(int maxLevels, Function[] fSet, string[] tSet)
        {
            //randomly generate n value (function or terminal )
            if (maxLevels > Level)
            {
                //generate number between 1 and 5. 
                int num = Node.Rand.Next(0, 5);
                //if numb is less than 3 generate function otherwise generate terminal
                if (num < 3)
                {
                    IsFunction = true;
                    //randomly select one function
                    num = Node.Rand.Next(0, fSet.Length);
                    Value = num;
                    //in this case children nodes must be generate too
                    var count = fSet[num].Arity;
                    Children = new Node[count];
                    for (int i = 0; i < count; i++)
                    {
                        var n = new Node();
                        n.Level = Level + 1;
                        Children[i] = n;
                        //recursively call Generate operation
                        n.Generate(maxLevels, fSet, tSet);
                    }
                }
                else
                {
                    IsFunction = false;
                    //randomly select one terminal
                    num = Node.Rand.Next(0, tSet.Length);
                    this.Value = num;
                }
            }
            //force to generate terminal
            else
            {
                IsFunction = false;
                //randomly select one terminal
                int num = Node.Rand.Next(0, tSet.Length);
                this.Value = num;
                this.Children = null;
            }

        }

        internal void Trim(int maxLevels, string[] tSet)
        {
            var dataTree = new Stack<Node>();
            //start with 0 level
            Level = 0;
            dataTree.Push(this);
            // 
            Node n = null;
            while (dataTree.Count > 0)
            {
                //get next n
                n = dataTree.Pop();
                //when the n level is equal to maximum value make terminal
                if (n.Level >= maxLevels)
                {
                    if (n.Children != null)
                        n.Children = null;
                    n.IsFunction = false;
                    n.Value = Rand.Next(0, tSet.Length);
                }
                if (n.Children != null)
                {
                    n.IsFunction = true;
                    for (int i = n.Children.Length - 1; i >= 0; i--)
                    {
                        n.Children[i].Level = ++n.Level;
                        dataTree.Push(n.Children[i]);
                    }
                }
                else
                    n.IsFunction = false;
            }
        }

        public double Evaluate(double[] inputRow)
        {
            double[] args = new double[2];
            var argColl = new Stack<double>();
            //we need to enumerate nodes in reverse order
            foreach (var n in EnumerateTree().Reverse())
            {
                if (n.IsFunction)
                {
                    //extract variable
                    for (int i = 0; i < n.Children.Length; i++)
                        args[i] = argColl.Pop();
                    //evaluate function
                    var retVal = Function.Evaluate(n.Value, args);
                    //add value to stack
                    argColl.Push(retVal);
                }
                else
                {
                    //extract data value from terminal id
                    var dataValue = inputRow[n.Value];
                    argColl.Push(dataValue);
                }
            }
            // return value from stack
            return argColl.Pop();
        }

        // Helper methods
        public string Print(string[] inputRow)
        {
            string[] args = new string[2];
            var argColl = new Stack<string>();

            //we need to enumerate nodes in reverse order
            foreach (var n in EnumerateTree().Reverse())
            {
                if (n.IsFunction)
                {
                    //extract variable
                    for (int i = 0; i < n.Children.Length; i++)
                        args[i] = argColl.Pop();

                    //evaluate function
                    var retVal = Function.GetName(n.Value, args);

                    //add value to stack
                    argColl.Push(retVal);

                }
                else
                {
                    //extract data value from terminal id
                    var dataValue = inputRow[n.Value];
                    argColl.Push(dataValue);
                }
            }
            // return value from stack
            return argColl.Pop();

        }
        internal IEnumerable<Node> EnumerateTree()
        {
            var stack = new Stack<Node>();
            stack.Push(this);
            while (stack.Count > 0)
            {
                var n = stack.Pop();
                yield return n;
                //enumerate children
                if (n.Children != null)
                {
                    for (int i = n.Children.Length - 1; i >= 0; i--)
                        stack.Push(n.Children[i]);
                }

            }
        }

    }
}
