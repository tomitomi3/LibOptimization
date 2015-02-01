LibOptimization
===============

LibOptimization is collection of optimization algorithm for .NET Framework

See also wiki(https://github.com/tomitomi3/LibOptimization/wiki)

LibOptimizationは制約条件の無い最適化を行うライブラリです。実装しているアルゴリズムは最急降下法、ニュートン法、HookeJeevesのパターンサーチ法、Nelder-Mead法、実数値遺伝的アルゴリズム（シンプレクス法、REX法）です。

NuGet
=====

URL:https://www.nuget.org/packages/LibOptimization/
```
PM> Install-Package LibOptimization
```

Implement Optimization algorithm
================================

using Derivative

* Steepest Descent Method
* Newton Method

Derivative free

* Nelder Mead Method (Down-Hill simplex method)
* Hooke and Jeeves of Pattern Search (Direct Search)
* Real-coded Genetic Algorithm Simplex and JGG
* Real-coded Genetic Algorithm REX and JGG

How to use
==========

Typecal Use

1. You inherit "absObjectiveFunction" class and design objective function.
1. Choose an optimization method and implement code.
1. Do optimization.
1. Get result and evaluate.

Sample code
===========

* Typical use code
```vb
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
```vb
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
```vb
Dim optimization As New clsOptSteepestDescent(New clsBenchSphere(2))
optimization.Init(New Double() {-10, 10})
While (optimization.DoIteration(5) = False)
    clsUtil.DebugValue(optimization)
End While
clsUtil.DebugValue(optimization, ai_isOnlyIterationCount:=True)
```

* You can use other optimization method(inherit absObjctiveFcuntion).
```vb
Dim optimization As New clsOptRealGASPX(New clsBenchRastriginFunction(20))
optimization.Init()
clsUtil.DebugValue(optimization)
While True
    If optimization.DoIteration(10) = True Then
        Exit While
    End If
    clsUtil.DebugValue(optimization, ai_isOutValue:=False)
End While
If optimization.IsRecentError() = True Then
    Return
End If
clsUtil.DebugValue(optimization)
```

* Multi point and MultiThread
** Multipoint avoids Local minimum by preparing many values.
```vb
'prepare many optimization class.
Dim multipointNumber As Integer = 30
Dim listOptimization As New List(Of absOptimization)
For i As Integer = 0 To multipointNumber - 1
    Dim tempOpt As New clsOptNelderMead(New clsBenchAckley(20))
    tempOpt.Init()
    listOptimization.Add(tempOpt)
Next

'using Parallel.ForEach
Dim lockObj As New Object()
Dim best As LibOptimization.absOptimization = Nothing
Threading.Tasks.Parallel.ForEach(listOptimization, Sub(opt As absOptimization)
                                                       opt.DoIteration()
                                                       'Swap best result
                                                       SyncLock lockObj
                                                           If best Is Nothing Then
                                                               best = opt
                                                           ElseIf best.Result.Eval > opt.Result.Eval Then
                                                               best = opt
                                                           End If
                                                       End SyncLock
                                                   End Sub)

'Check Error
If best.IsRecentError() = True Then
    Return
Else
    clsUtil.DebugValue(best)
End If
```

License
=======

Microsoft Public License (MS-PL)

http://opensource.org/licenses/MS-PL

Requirements
===============

.NET Framework 4.0
