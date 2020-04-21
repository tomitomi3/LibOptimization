using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOptimization.Optimization;
using LibOptimization.Util;

namespace SampleCSharp
{
    class MyObjectiveFunction : LibOptimization.Optimization.absObjectiveFunction
    {
        private double _maxEval = 0.0;
        private bool _oneShot = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyObjectiveFunction()
        {
        }

        /// <summary>
        /// eval
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double F(List<double> x)
        {
            double ret = 0.0;
            for (int i = 0; i < this.NumberOfVariable(); i++)
            {
                //When even one variable has a negative value, use the recent large evaluation value
                if (x[i] < 0 && _oneShot == false)
                {
                    ret = _maxEval;
                    break;
                }

                //model(sphere)
                ret += x[i] * x[i];
            }

            //update max eval
            if (_oneShot == true)
            {
                _maxEval = ret; //one shot
                _oneShot = false;
            }
            if (_maxEval < ret)
            {
                _maxEval = ret;
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
    /// Sphere function inherit absObjectiveFunction
    /// </summary>
    class SphereFunction : LibOptimization.Optimization.absObjectiveFunction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SphereFunction()
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
    
    class Program
    {
        static void Main(string[] args)
        {
            //Simulated Annealing
            {
                var func = new SphereFunction();
                var opt = new LibOptimization.Optimization.clsOptSimulatedAnnealing(func);

                //initial position using random
                var rng = new LibOptimization.Util.clsRandomXorshift((UInt32)DateTime.Now.Millisecond);
                opt.InitialPosition = new double[] { rng.NextDouble(-2, 2), rng.NextDouble(-2, 2), rng.NextDouble(-2, 2), rng.NextDouble(-2, 2), rng.NextDouble(-2, 2) };

                //parameter for SA
                opt.Temperature = 1;
                opt.StopTemperature = 0.0001;
                opt.CoolingRatio = 0.99;
                opt.Iteration = 30000; //default 20000

                //neighbor
                ((LocalRandomSearch)opt.Neighbor).NeighborRange = 0.001; //neghbor function

                //init
                opt.Init();
                clsUtil.DebugValue(opt);

                //do optimization
                while (opt.DoIteration(2000) == false)
                {
                    clsUtil.DebugValue(opt);

                    //my criterion
                    /*
                    if (Result.Eval < 0.01)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Eval:{0}", opt.Result.Eval);
                    }
                    */
                }
                clsUtil.DebugValue(opt);
            }

            //Typical use
            {
                //Target Function
                var func = new RosenBrock(2);

                //Set Function
                var opt = new LibOptimization.Optimization.clsOptNelderMead(func);
                opt.Init();

                //Optimization
                opt.DoIteration();

                //Check Error
                if (opt.IsRecentError() == true)
                {
                    return;
                }
                else
                {
                    //Get Result
                    clsUtil.DebugValue(opt);
                }
            }

            //Evaluate optimization result per 100 iteration
            {
                var opt = new LibOptimization.Optimization.clsOptDEJADE(new RosenBrock(10));
                opt.Init();
                clsUtil.DebugValue(opt);

                while (opt.DoIteration(100) == false)
                {
                    clsUtil.DebugValue(opt, ai_isOutValue: false);
                }
                clsUtil.DebugValue(opt);
            }

            //Evaluate optimization result per 100 iteration with check my criterion.
            {
                var opt = new LibOptimization.Optimization.clsOptDEJADE(new RosenBrock(10));
                //Disable Internal criterion
                opt.IsUseCriterion = false;

                //Init
                opt.Init();
                clsUtil.DebugValue(opt);

                //do optimization!
                while (opt.DoIteration(100) == false)
                {
                    var eval = opt.Result.Eval;

                    //my criterion
                    if (eval < 0.01)
                    {
                        break;
                    }
                    else
                    {
                        clsUtil.DebugValue(opt, ai_isOutValue: false);
                    }
                }
                clsUtil.DebugValue(opt);
            }

            //Set boundary variable.
            //-20<x1<-1, -15<x2<0
            {
                var opt = new LibOptimization.Optimization.clsOptDEJADE(new RosenBrock(2));
                //move initial position
                opt.InitialPosition = new double[] { -10, -10 };

                //Set boundary variable
                opt.LowerBounds = new double[] { -20, -15 };
                opt.UpperBounds = new double[] { -1, 0 };

                //Init
                opt.Init();
                clsUtil.DebugValue(opt);

                //do optimization!
                while (opt.DoIteration(100) == false)
                {
                    var eval = opt.Result.Eval;

                    //my criterion
                    if (eval < 0.01)
                    {
                        break;
                    }
                    else
                    {
                        clsUtil.DebugValue(opt, ai_isOutValue: false);
                    }
                }
                clsUtil.DebugValue(opt);
            }

            //Optimiztion problem using MyObjectiveFunction
            // min f(x)
            //  s.t. x>0, 170<=x1<=200, 200<=x2<=300, 250<=x3<=400, 370<=x4<=580, 380<=x5<=600
            {
                var func = new MyObjectiveFunction();
                var opt = new LibOptimization.Optimization.clsOptDEJADE(func);

                //Set boundary variable                
                opt.LowerBounds = new double[] { 170, 200, 250, 370, 380 };
                opt.UpperBounds = new double[] { 200, 300, 400, 580, 600 };

                //move initial position
                double[] initialPosition = new double[] { 0, 0, 0, 0, 0 };
                for (int i = 0; i < initialPosition.Length; i++)
                {
                    //center of boundary range.
                    initialPosition[i] = (opt.LowerBounds[i] + opt.UpperBounds[i]) / 2.0;
                }
                opt.InitialPosition = initialPosition;

                //Init
                opt.Init();
                clsUtil.DebugValue(opt);

                //do optimization!
                while (opt.DoIteration(100) == false)
                {
                    var eval = opt.Result.Eval;

                    //my criterion
                    if (eval < 0.01)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Eval:{0}", opt.Result.Eval);
                    }
                }
                clsUtil.DebugValue(opt);
                //return;
            }
        }
    }
}
