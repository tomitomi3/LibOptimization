LibOptimization
===============

![LibOptimization Log](https://github.com/tomitomi3/LibOptimization/blob/master/github_pic/logo_liboptimization.png)

LibOptimization is collection of optimization algorithm for .NET Framework

LibOptimizationは制約条件の無い最適化を行うライブラリです。実装しているアルゴリズムは最急降下法、ニュートン法、HookeJeevesのパターンサーチ法、Nelder-Mead法、実数値遺伝的アルゴリズム（シンプレクス法、REX法）です。

NuGet
=====

URL:https://www.nuget.org/packages/LibOptimization/
```
PM> Install-Package LibOptimization
```

Implement Optimization algorithm
================================

require derivative algorithm

* Steepest Descent Method
* Newton Method

derivative free algorithm

* Nelder Mead Method (Down-Hill simplex method)
* Hooke and Jeeves of Pattern Search (Direct Search)
* Real-coded Genetic Algorithm Simplex and JGG(Just Generation Gap)
* Real-coded Genetic Algorithm REX(Real-coded Ensemble Crossover) and JGG

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
============

.NET Framework 4.0


Refference
==========

1. 金谷 健一, "これなら分かる最適化数学―基礎原理から計算手法まで", 共立出版株式会社, 2007年初版第7刷
1. Hooke, R. and Jeeves, T.A., ""Direct search" solution of numerical and statistical problems", Journal of the Association for Computing Machinery (ACM) 8 (2), pp212–229.
1. J.A.Nelder and R.Mead, "A simplex method for function minimization" ,The Computer Journal vol.7, 308–313 (1965)
1. W.H.Press， B.P.Flannery，S.A.Teukolsky，W.T.Vetterlin, "NUMERICAL RECIPES in C 日本語版 C言語による数値計算のレシピ", 株式会社技術評論社, 平成19年 初版 第14刷,
1. 北野宏明編著, “『遺伝的アルゴリズム』”, 産業図書株式会社, 平成5年初版
1. 樋口 隆英, 筒井 茂義, 山村 雅幸, "実数値GAにおけるシンプレクス交叉", 人工知能学会論文誌Vol. 16 (2001) No. 1 pp.147-155
1. 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)

