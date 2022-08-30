using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class MergeLayerToCertainCount : Step<Hierarchies, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Post"; }
        }
        public override string DetailDescription
        {
            get { return string.Format("Merge Layer To {0} layers According to Node Dependency Count", finalLayerCount); }
        }
        public override string ChineseDescription
        {
            get
            {
                Dictionary<int, string> mergePreferenceDescriptions;
                mergePreferenceDescriptions = new Dictionary<int, string>()
                {
                    {0, "最小" },
                    {1, "最大" },
                };
                return $"层融合 层间依赖数{mergePreferenceDescriptions[mergePreference]}优先";
            }
        }

        public int finalLayerCount = 4;
        public int graphDirection = 1;
        public int mergePreference = 0; //0: small first; 1: big first
        public int mergeDirection = 0; //0: from bottom layer (layer zero); 1, from top layer 
        public int relationKind = 0; //0: node; 1: package
        public int skippedLayerCount = 0;

        public MergeLayerToCertainCount(int finalLayerCount = 4, int graphDirection = 1, int mergePreference = 0, int mergeDirection = 0, int relationKind = 0, int skippedLayerCount = 0)
        {
            this.finalLayerCount = finalLayerCount;
            this.graphDirection = graphDirection;
            this.mergePreference = mergePreference;
            this.mergeDirection = mergeDirection;
            this.relationKind = relationKind;
            this.skippedLayerCount = Math.Min(skippedLayerCount,finalLayerCount);
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
        public bool Judge(int count, int standard)
        {
            if (mergePreference == 0)
                return count < standard;
            else if (mergePreference == 1)
                return count > standard;
            else if (mergePreference == 2)
                return count <= standard;
            else if (mergePreference == 3)
                return count >= standard;
            else
                throw new ArgumentOutOfRangeException("MergePerference not in 0 or 1");
        }
        public override Hierarchies Process(Hierarchies input)
        {
            Hierarchies merged = new Hierarchies(input);
            if (mergeDirection == 1)
            {
                merged.layers.Reverse();
                graphDirection = 1 - graphDirection;
            }
            //Console.WriteLine("-----start");
            while (finalLayerCount < merged.Count)
            {
                int layerIndex = -1, dependencyCount;
                if (mergePreference % 2 == 0)
                    dependencyCount = int.MaxValue;
                else
                    dependencyCount = -1;

                for (int i = skippedLayerCount+1; i < merged.Count; i++)
                {
                    int count = GetRelationCount(merged[i], merged[i - 1]);
                    //Console.WriteLine(count);
                    if (Judge(count, dependencyCount))
                    {
                        layerIndex = i - 1;
                        dependencyCount = count;
                    }
                }
                if (layerIndex >= 0)
                {
                    merged[layerIndex] = merged[layerIndex].Concat(merged[layerIndex + 1]);
                    merged.RemoveAt(layerIndex + 1);
                }
            }

            if (mergeDirection == 1)
            {
                merged.layers.Reverse();
                graphDirection = 1 - graphDirection;
            }
            return merged;
        }
    }
}
