using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using static JuliHelper.DepthManager;

namespace JuliHelper
{
    public static class DepthManager
    {
        public static DepthLayer currentDepth;

        static Dictionary<Type, List<DepthLayer>> depths;

        public static void Initialize(params Type[] depthClassTypes)
        {
            depths = new Dictionary<Type, List<DepthLayer>>();

            for (int i = 0; i < depthClassTypes.Length; i++)
            {
                InitDepthClass(depthClassTypes[i]);
            }
        }

        private static void InitDepthClass(Type type)
        {
            List<DepthLayer> depthList = new List<DepthLayer>();

            var properties = from property in type.GetFields()
                             where Attribute.IsDefined(property, typeof(OrderAttribute))
                             orderby ((OrderAttribute)property
                                       .GetCustomAttributes(typeof(OrderAttribute), false)
                                       .Single()).Order
                             select property;

            int line = -1;
            int lineCount = 0;
            foreach (var property in properties)
            {
                int cLine = ((OrderAttribute)property
                    .GetCustomAttributes(typeof(OrderAttribute), false)
                    .Single()).Order;

                if (cLine > line)
                {
                    line = cLine;
                    lineCount++;
                }
                else if (cLine < line)
                    throw new Exception();
            }

            float partition = 1f / lineCount;
            float depth = 0f;
            line = -1;
            foreach (var property in properties)
            {
                int cLine = ((OrderAttribute)property
                    .GetCustomAttributes(typeof(OrderAttribute), false)
                    .Single()).Order;

                if (cLine > line)
                {
                    // new depth layer
                    if (line != -1) // if first one, don't skip 0f depth
                        depth += partition;
                    line = cLine;

                    DepthLayer d = new DepthLayer(depth);
                    property.SetValue(null, d);
                    depthList.Add(d);
                }
                else
                {
                    property.SetValue(null, depthList.Last());
                }
            }

            depths.Add(type, depthList);
        }

        public static void BeginDraw(Type depthClassType)
        {
            foreach (var item in depths[depthClassType])
            {
                item.ResetCDepth();
            }
        }
    }

    public class DepthLayer
    {
        float depth;
        float cDepth;

        public DepthLayer(float _depth)
        {
            depth = _depth;
        }

        public float GetDrawDepth()
        {
            cDepth = Calculate.NextAfter(cDepth, 1f);
            return cDepth;
        }

        public static implicit operator float(DepthLayer _depth)
        {
            return _depth.GetDrawDepth();
        }

        public override string ToString()
        {
            return depth.ToString();
        }

        internal void ResetCDepth()
        {
            cDepth = depth;
        }

        //public void Set()
        //{
        //    Drawer.depth = this;
        //}

        public void Set(Action action)
        {
            DepthLayer previous = Drawer.depth;
            Drawer.depth = this;
            action();
            Drawer.depth = previous;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order_;
        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }
}
