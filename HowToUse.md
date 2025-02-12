# How to use LibOptimization

In this tutorial, you will design an objective function to find the minimum value of the [2D Sphere function](https://qiita.com/tomitomi3/items/d4318bf7afbc1c835dda#sphere-function). 
This function is a unimodal convex function with a global minimum.

**Optimization flow using LibOptimization**

1. Install LibOptimization from NuGet.
1. Inherit the **absObjectiveFunction** class and define the objective function.
1. Choose an optimization method and implement the code.
1. Run the optimization.
1. Retrieve the result and evaluate it.

## preparation

Create a console application in C#. In this example, we use C#, but you can also use Visual Basic .NET.

## Step1. Get LibOptimization

Install LibOptimization via NuGet:

URL:https://www.nuget.org/packages/LibOptimization/

```
PM> Install-Package LibOptimization
```

## Step2. Design objective fucntion

Add a class that inherits **absObjectiveFunction** to your solution.

**absObjectiveFunction** is the base class for defining objective functions in LibOptimization.

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
            //If you use the gradient method or Newton method, implement the derivative of the objective function. otherwise, return null.
            return null;
        }

        /// <summary>
        /// Hessian matrix of the objective function
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        public override List<List<double>> Hessian(List<double> x)
        {
            //If you use the Newton method, implement the derivative of the objective function. otherwise, return null.
            return null;
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

The **Gradient(List<double> x)** and **Hessian(List<double> x)** methods define the gradient and Hessian matrix of the objective function.
These are used in gradient-based methods. If they are not explicitly defined, they will be computed numerically using the CalcNumericGradient and CalcNumericHessian functions.

```csharp
        public override List<double> Gradient(List<double> x)
        {
            //Differentiation of sphere function
            // f(x) = x^2
            // df/dx = 2 * x
            var ret = new List<double>();
            var dim = this.NumberOfVariable(); //or x.Count
            for (int i = 0; i < dim; i++)
            {
                ret.Add(2.0 * x[i]);
            }
            return ret;
        }
        
        public override List<List<double>> Hessian(List<double> x)
        {
            // Hessian of sphere function
            // H =
            // | d^2 f/d^2x1 df1/dx2     |
            // | df2/dx1     d^2 f/d^2x2 |
            //
            // omit
            //
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

# Tips

## Not using stopping criteria

The implemented optimization algorithm has a stopping criterion. This stopping criterion is stopped when the best evaluate value is equal to 70% of the population.

**IsUseCriterion** property is **false**.

```csharp
//Set objective function to optimizeclass
var opt = new LibOptimization.Optimization.clsOptPSO(func);

opt.IsUseCriterion = false; //not use criteria

//Initialize(generate initial value)
opt.Init();
```

## Using InitialPosition

When performing optimization, you may want to start from a specific initial position instead of relying solely on a randomly generated population. The InitialPosition property allows you to include a predefined starting point in the population.

```csharp
// objective function
var func = new RosenBrock(2);

var opt = new Optimization.clsOptNelderMead(func);
opt.InitialPosition = new double[] { -10, -10 };
opt.Init();

// Optimization
opt.DoIteration();

// best result
for (int i = 0; i < opt.Result.Count; i++)
{
    Console.WriteLine(opt.Result[i]);
}
```

## Evaluate optimization result per 100 iteration

```csharp
//Evaluate optimization result per 100 iteration
while (opt.DoIteration(100) == false)
{
    clsUtil.DebugValue(opt, ai_isOutValue: false);
}
clsUtil.DebugValue(opt);
```

## Reset Iteration count

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

            //Reset iteration
            opt.Iteration = 0;
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

## ~~Saving and Restoring Optimization Calculations~~

~~You can export and restore the optimization results in binary format. **BinaryFormatter** is used to achieve this functionality.~~

~~LibOptimization.Util.clsUtil.SerializeOpt((absOptimization)opt, "saveOptimization.bin");~~

~~var restoreOpt = LibOptimization.Util.clsUtil.DeSerializeOpt("saveOptimization.bin");~~

I have discontinued the use of BinaryFormatter.
For Save/Restore, please use InitialPosition to restore the most recent results.

## fix Random Number Generator

Fix the seed of the random number generator. It should be used when you want reproducibility.

```csharp
//This RNG is used for random sequence etc.
LibOptimization.Util.clsRandomXorshiftSingleton.GetInstance().SetDefaultSeed();

var func = new RosenBrock(2);
var opt = new LibOptimization.Optimization.clsOptPSO(func);
//This RNG is used for generate itinial value, position etc.
opt.Random = new LibOptimization.Util.clsRandomXorshift();

//init
opt.Init();
```
