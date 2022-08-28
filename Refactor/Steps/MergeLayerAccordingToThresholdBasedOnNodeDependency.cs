using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{
    public class MergeLayerAccordingToThresholdBasedOnNodeDependency : Step<Hierarchies, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Post"; }
        }
        public override string DetailDescription
        {
            get { return string.Format("Merge Layer According to Node Dependency Count with Threshold {0}", threshold); }
        }
        public int threshold;
        public int graphDirection = 1;
        public int mergePreference = 0; //0: small first; 1: big first
        public int mergeDirection = 0; //0: from bottom layer (layer zero); 1: from top layer 
        public int relationKind = 0; //0: node; 1: package
        public int skippedLayerCount = 0;

        public MergeLayerAccordingToThresholdBasedOnNodeDependency(int threshold, int graphDirection = 1, int mergePreference = 0, int mergeDirection = 0, int relationKind = 0, int skippedLayerCount = 0)
        {
            this.threshold = threshold;
            this.graphDirection = graphDirection;
            this.mergePreference = mergePreference;
            this.mergeDirection = mergeDirection;
            this.relationKind = relationKind;
            this.skippedLayerCount = skippedLayerCount;
        }
        private int GetRelationCount(Layer from, Layer to)
        {
            if (relationKind == 0)
                return from.CountOutDegreeTo(to, graphDirection);
            else if (relationKind == 1)
                return from.CountOutPackagesTo(to, graphDirection);
            else
                throw new ArgumentOutOfRangeException("relation kind is not in 0 or 1");
        }
        private bool Judge(int count)
        {
            if (mergePreference == 0)
                return count < threshold;
            else if (mergePreference == 1)
                return count > threshold;
            else if (mergePreference == 2)
                return count <= threshold;
            else if (mergePreference == 3)
                return count >= threshold;
            else
                throw new ArgumentOutOfRangeException("MergePerference not in 0 or 1");
        }
        public override Hierarchies Process(Hierarchies input)
        {
            if (input.Count <= 1) return input;
            if (mergeDirection == 1)
            {
                input.layers.Reverse();
                graphDirection = 1 - graphDirection;
            }

            Hierarchies merged = new Hierarchies();
            for (int i = 0; i < Math.Min(skippedLayerCount, input.Count); i++)
            {
                merged.AddLayer(input.layers[i]);
            }
            if (skippedLayerCount < input.Count)
            {
                Layer layer = input[skippedLayerCount];
                for (int i = skippedLayerCount + 1; i < input.Count; i++)
                {
                    if (Judge(GetRelationCount(input[i], layer)))
                    {
                        layer = layer.Concat(input[i]);
                    }
                    else
                    {
                        merged.AddLayer(layer);
                        layer = input[i];
                    }
                }
                merged.AddLayer(layer);
            }

            if (mergeDirection == 1)
            {
                input.layers.Reverse();
                graphDirection = 1 - graphDirection;
            }
            return merged;
        }
    }
}
