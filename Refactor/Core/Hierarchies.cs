namespace Refactor.Core
{
    public class Hierarchies
    {
        public List<Layer> layers = new List<Layer>();
        public Hierarchies()
        { }
        public Hierarchies(List<Layer> layers)
        {
            this.layers = layers;
        }
        public Hierarchies(Hierarchies hierarchies)
        {
            foreach(Layer layer in hierarchies.layers)
                layers.Add(new Layer(layer));
        }
        public int Count { get { return layers.Count; } }
        public static implicit operator Hierarchies(List<Layer> layers)
            => new Hierarchies(layers);
        public static implicit operator List<Layer>(Hierarchies h)
           => h.layers;
        public Layer this[int i]
        {
            get { return layers[i]; }
            set { layers[i] = value; }
        }
        public void RemoveAt(int index)
        {
            this.layers.RemoveAt(index);
        }
        public void AddLayer(Layer layer)
        {
            layers.Add(layer);
        }
        public void Reverse()
        {
            layers.Reverse();
        }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < layers.Count; i++)
            {
                s += "--- Layer : " + i + " ---\n";
                s += layers[i].ToString();
            }
            return s;
        }
    }
}
