using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOptimization.Optimization;
using LibOptimization.Util;

namespace SampleCSharp
{
    /// <summary>
    /// My Sphere function(inherit absObjectiveFunction)
    /// </summary>
    class MyObjecticeFunctionForSA : LibOptimization.Optimization.absObjectiveFunction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MyObjecticeFunctionForSA()
        {
        }

        /// <summary>
        /// eval
        /// </summary>
        /// <param name="x">variable</param>
        /// <returns>eval value</returns>
        public override double F(List<double> x)
        {
            double ret = 0.0;
            for (int i = 0; i < this.NumberOfVariable(); i++)
            {
                //model(sphere)
                ret += x[i] * x[i];
            }
            return ret;
        }

        public override List<double> Gradient(List<double> aa)
        {
            return null;
        }

        public override List<List<double>> Hessian(List<double> aa)
        {
            return null;
        }

        public override int NumberOfVariable()
        {
            return 5;
        }
    }

    /// <summary>
    /// Neighbor function for SA
    /// </summary>
    class MyNeighborFunc : LibOptimization.Optimization.absNeighbor
    {
        public MyNeighborFunc()
        {
        }

        //random
        private Random _rng = new Random(DateTime.Now.Millisecond);

        //some parameters...
        public int Range = 2;

        /// <summary>
        /// My neighbor function
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override clsPoint Neighbor(clsPoint p)
        {
            var retP = p.Copy();
            var n = p.GetFunc().NumberOfVariable();

            int i = 0;
            for (; i < n - 2; i++)
            {
                retP[i] = _rng.Next(-this.Range, this.Range); //discrete value
            }

            //all 0
            for (; i < n; i++)
            {
                retP[i] = 0;
            }

            return retP;
        }
    }

    /// <summary>
    /// using SA optimizer
    /// </summary>
    class SimulatedAnnealingSample
    {
        public SimulatedAnnealingSample()
        {
            //nop
        }

        public void Run()
        {
            var func = new MyObjecticeFunctionForSA();
            var opt = new LibOptimization.Optimization.clsOptSimulatedAnnealing(func);

            //initial position using random
            var rng = new LibOptimization.Util.clsRandomXorshift((UInt32)DateTime.Now.Millisecond);
            opt.InitialPosition = new double[] { rng.NextDouble(-2, 2), rng.NextDouble(-2, 2), rng.NextDouble(-2, 2), rng.NextDouble(-2, 2), rng.NextDouble(-2, 2) };

            //parameter for SA
            opt.Temperature = 1;
            opt.StopTemperature = 0.0001;
            opt.CoolingRatio = 0.9995;
            opt.IsUseCriterion = false;
            opt.Iteration = 50000;

            //neighbor function
            {
                //default neightbor function(default)
                //var myNeighborFunc = new LocalRandomSearch();
                //myNeighborFunc.NeighborRange = 0.0001;
                //opt.Neighbor = myNeighborFunc;
            }
            {
                //designed neightbor function
                var myNeighborFunc = new MyNeighborFunc();
                myNeighborFunc.Range = 1;
                opt.Neighbor = myNeighborFunc;
            }
            //init
            opt.Init();
            clsUtil.DebugValue(opt);

            //do optimization
            while (opt.DoIteration(2000) == false)
            {
                clsUtil.DebugValue(opt);
            }
            clsUtil.DebugValue(opt);
        }
        
    }
}
