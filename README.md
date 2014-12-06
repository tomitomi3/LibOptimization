LibOptimization
===============

LibOptimization is collection of optimization algorithm for .NET Framework

See also wiki(https://github.com/tomitomi3/LibOptimization/wiki)

Implement Optimization algorithm
================================

using Derivative

* Steepest Descent Method
* Newton Method
* Derivative free

Nelder Mead Method

* Hooke and Jeeves of Pattern Search (Direct Search)
* Real-coded Genetic Algorithm Simplex and JGG
* Real-coded Genetic Algorithm REX and JGG

How to use
==========

Typecal Use

1. You inherit "absObjectiveFunction" class and design objective function.
1. Choose an optimization method and implement code.
1. Do optimization!
1. Get result and evaluate.

* Sample
```html
'Instantiation optimization class and set objective function.
Dim optimization As New clsOptSteepestDescent(New clsBenchSphere(1))
'Initialize starting value
optimization.Init()
'Do calc
optimization.DoIteration()
'Get result. Check recent error.
If optimization.IsRecentError() = True Then
    Return
Else
    clsUtil.DebugValue(optimization)
End If
```

* When you want result every 5 times.
```
With Nothing
    Dim optimization As New clsOptSteepestDescent(New clsBenchSphere(2))
    optimization.Init()
    While (optimization.DoIteration(5) = False)
clsUtil.DebugValue(optimization)
    End While
    clsUtil.DebugValue(optimization, ai_isOnlyIterationCount:=True)
End With
```

* set initial point
```
Dim optimization As New clsOptSteepestDescent(New clsBenchSphere(2))
optimization.Init(New Double() {-10, 10})
While (optimization.DoIteration(5) = False)
    clsUtil.DebugValue(optimization)
End While
clsUtil.DebugValue(optimization, ai_isOnlyIterationCount:=True)
```

License
=======

Microsoft Public License (MS-PL)

http://opensource.org/licenses/MS-PL

Requirements
===============

.NET Framework 4.0
