using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public class Layer
    {
        public List<Node> nodes = new List<Node>();
        public int? level = null;

        public Layer()
        {
        }

        public Layer(IEnumerable<Node> nodes)
        {
            this.nodes.AddRange(nodes);
        }
        public void addNode(Node node)
        {
            nodes.Add(node);
        }
        public void removeNode(Node node)
        {
            nodes.Remove(node);
        }
        public bool isDepend(IEnumerable<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                foreach (Node dependency in node.outNodes)
                {
                    if (this.nodes.Contains(dependency))
                        return true;
                }
            }
            return false;
        }
        public override string ToString()
        {
            string s = "Layer:";
            if(level!=null) s+=level.ToString()+"\n";
            foreach (Node node in nodes)
            {
                s += node.ToString() + "\n";
            }
            return s;
        }
    }
}
