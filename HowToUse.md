# How to use LibOptimization

This tutrial, We design a objective function to find the minimum value of the [2D Sphere function](https://qiita.com/tomitomi3/items/d4318bf7afbc1c835dda#sphere-function). 
This function is a unimodal convex function and has a global minimum value.

**Optimization flow using LibOptimization**

1. Get LibOptimization your solution from Nuget.
1. You inherit "absObjectiveFunction" class and design the objective function.
1. Choose an optimization method and implement code.
1. Do optimization.
1. Get result and evaluate.

## preparation

Create a console application and development language is C#.
In this example, we use C#. You can also use VisualBasic.NET.

## Step1. Get LibOptimization

URL:https://www.nuget.org/packages/LibOptimization/
```
PM> Install-Package LibOptimization
```

## Step2. Design objective fucntion

Add an objective function class that inherit **absObjectiveFunction** to the solution.

absObjectiveFunction is the base class for objective functions in the LibOptimization.

**SphereFunction.cs**
```csharp
    /// <summary>
    /// objective function inherit absObjectiveFunction
    /// </summary>
    class SphereFunction : LibOptimization.Optimization.absObjectiveFunction
    {
        public SphereFunction()
        {
        }

        /// <summary>
        /// design objective function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double F(List<double> x)
        {
            var ret = 0.0; 
            var dim = this.NumberOfVariable(); //or x.Count
            for (int i = 0; i < dim; i++)
            {
                ret += x[i] * x[i];// x^2
            }
            return ret;
        }

        /// <summary>
        /// Gradient of the objective function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override List<double> Gradient(List<double> x)
        {
            //for Optimization algorithm using gradient
            var ret = new List<double>();
            var dim = this.NumberOfVariable(); //or x.Count
            for (int i = 0; i < dim; i++)
            {
                ret.Add(2.0 * x[i]);
            }
            return ret;

            //If you don't use an optimization algorithm that uses derivatives, return null value.
            return null;

            //Numerical differentiation of the objective function can be easily implemented using the following API.
            return base.NumericDerivative(x);
        }

        /// <summary>
        /// Hessian matrix of the objective function
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        public override List<List<double>> Hessian(List<double> x)
        {
            //for Optimization algorithm using Hessian matrix

            //If you don't use an optimization algorithm that uses derivatives, return null value.
            return null;

            //Numerical Hessian matrix of the objective function can be easily implemented using the following API.
            //Diagonal component stores the second derivative of the objective function.
            return base.NumericHessianToDiagonal(x);
        }

        /// <summary>
        /// The number of dimensions of the objective function
        /// </summary>
        /// <returns></returns>
        public override int NumberOfVariable()
        {
            return 2;
        }
    }
```

## Step3. Choose an optimization method

Write the code to set the evaluation function into the optimization algorithm. Typical code is as follows.

Choosing an optimization algorithm requires experience. In this example, using PSO (Particle Swarm Optimization).

**Program.cs**
```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var func = new SphereFunction();

            //Set objective function to optimizeclass
            var opt = new LibOptimization.Optimization.clsOptPSO(func);

            //Initialize(generate initial value)
            opt.Init();

            //Do Optimization
            opt.DoIteration();

            //Get result
            var result = opt.Result;

            //output console
            Console.WriteLine("Eval : {0}", result.Eval);
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine("{0}", result[i]);
            }
        }
    }
```

## Step4. Buld and Run

Build and run the program. You should see the results in the console after a while.

## Step5. Review and evaluation of results

Check the results obtained.

Are the results extremely large?
Not getting enough iteration?
etc.


