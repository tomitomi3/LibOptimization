[![Build status](https://ci.appveyor.com/api/projects/status/c4n61lv6x59sfqw5/branch/master?svg=true)](https://ci.appveyor.com/project/tomitomi3/liboptimization/branch/master)

I may miss your Issues. When a reply is slow, please give me e-mail.
<pre>
tomi.nori+github atmark gmail.com
</pre>

LibOptimization
===============

![LibOptimization Log](https://github.com/tomitomi3/LibOptimization/blob/master/github_pic/logo_liboptimization.png)

# Introduction

LibOptimization is optimization algorithm library for .NET Framework.
This library will probably simplify the optimization using C# and VB.Net and other .NET Framework language.

LibOptimizationは制約条件の無い最適化を行う.NET Framework用のライブラリです。
実装しているアルゴリズムは最急降下法、ニュートン法、HookeJeevesのパターンサーチ法、Nelder-Mead法（オリジナルの実装、Wikipediaの実装） 、実数値遺伝的アルゴリズム（BLX-α、UNDX、SPX（シンプレクス）、REX、世代交代はJGG、PCX（世代交代はG3））、粒子群最適化（Basic PSO, LDIW-PSO, CDIW-PSO, CRIW-PSO, AIW-PSO）、Differential Evolution(差分進化？ DE/rand/1/bin, DE/rand/2/bin, DE/best/1/bin, DE/best/2/bin)、JADE（自己適応型DE）ホタルアルゴリズム、Cuckoo Search（Matlabコードの移植版）、焼きなまし法です。

## Implement Optimization algorithm

### Require derivative algorithm

* Steepest Descent Method
* Newton Method

### Derivative free algorithm

* Nelder Mead Method (Original ver, Wikipedia ver)
* Hooke and Jeeves of Pattern Search (Direct Search)

### Nature inspired derivative free algorithm

* Real-coded Genetic Algorithm BLX-alpha and JGG(Just Generation Gap)
* Real-coded Genetic Algorithm UNDX(Unimodal Normal Distribution Crossover) and JGG
* Real-coded Genetic Algorithm SPX(Simplex Crossover) and JGG
* Real-coded Genetic Algorithm REX(Real-coded Ensemble Crossover) and JGG
* Real-coded Genetic Algorithm PCX(Parent Centric Recombination) and G3(Generalize Generation Gap)
* Basic Particle Swarm Optimization
* Particle Swarm Optimization using Linear Decrease Inertia Weight
* Particle Swarm Optimization using Chaotic inertia weight(CDIW-PSO, CRIW-PSO)
* Particle Swarm Optmization using adaptive inertia weight
* Differential Evolution(DE/rand/1/bin, DE/rand/2/bin, DE/best/1/bin, DE/best/2/bin)
* JADE(self adaptation DE)
* Standrad Cuckoo Search
* FireFly algorithm

### Derivative free algorithm (Randomized algorithm)

* Simulated Annealing

# How to get

URL:https://www.nuget.org/packages/LibOptimization/
```
PM> Install-Package LibOptimization
```

# How to use

Typical Use

1. You inherit "absObjectiveFunction" class and design objective function.
1. Choose an optimization method and implement code.
1. Do optimization.
1. Get result and evaluate.

# Sample code

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

* Set of initial value and the initial position. Initial value is generated in the range of 2.5 and 3.5.
```vb
        With Nothing
            Dim optimization As New clsOptRealGASPX(New clsBenchSphere(2))
            optimization.InitialPosition = {3, 3}
            optimization.InitialValueRange = 0.5
            optimization.Init()
            While (optimization.DoIteration(5) = False)
                clsUtil.DebugValue(optimization, ai_isOutValue:=False)
            End While
            clsUtil.DebugValue(optimization)
        End With
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

* Multi point and MultiThread. Multipoint avoids Local minimum by preparing many values.
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

* Least squares method (最小二乗法)

You design the evaluation function to minimize residual sum of squares. The following example estimate a parameter of the multinomial expression.

```vb
    Public Overrides Function F(x As List(Of Double)) As Double
        Dim sumDiffSquare As Double = 0

        For Each temp In Me.datas
            'e.g a * x^4 + b * x^3 + c * x^2 + d * x^4 + e
            Dim predict = x(0) * temp(0) ^ 4 + x(1) * temp(0) ^ 3 + x(2) * temp(0) ^ 2 + x(3) * temp(0) + x(4)
            Dim diffSquare = (temp(1) - predict) ^ 2
            sumDiffSquare += diffSquare
        Next

        Return sumDiffSquare
    End Function
```

# License

Microsoft Public License (MS-PL)

http://opensource.org/licenses/MS-PL

# Support .NET Framework
============

.NET Framework 4.0

.NET Framework 3.5

.NET Framework 3.0

# Refference

1. 金谷 健一, "これなら分かる最適化数学―基礎原理から計算手法まで", 共立出版株式会社, 2007年初版第7刷
1. Hooke, R. and Jeeves, T.A., ""Direct search" solution of numerical and statistical problems", Journal of the Association for Computing Machinery (ACM) 8 (2), pp212–229.
1. J.A.Nelder and R.Mead, "A simplex method for function minimization" ,The Computer Journal vol.7, 308–313 (1965)
1. W.H.Press， B.P.Flannery，S.A.Teukolsky，W.T.Vetterlin, "NUMERICAL RECIPES in C 日本語版 C言語による数値計算のレシピ", 株式会社技術評論社, 平成19年 初版 第14刷,
1. 北野宏明編著, "『遺伝的アルゴリズム』", 産業図書株式会社, 平成5年初版
1. 北野宏明 (編集), "遺伝的アルゴリズム 4", 産業図書出版株式会社, 2000年初版
1. 小野功，佐藤浩，小林重信, "単峰性正規分布交叉UNDXを用いた実数値GAによる関数最適化"，人工知能学会誌，Vol. 14，No. 6，pp. 1146-1155 (1999)
1. 樋口 隆英, 筒井 茂義, 山村 雅幸, "実数値GAにおけるシンプレクス交叉", 人工知能学会論文誌Vol. 16 (2001) No. 1 pp.147-155
1. 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
1. James Kennedy and Russell Eberhart, "Particle Swarm Optimization．", Proceedings of IEEE the International Conference on Neural Networks，1995
1. Shi, Y. and Eberhart, R.C., "A Modified Particle Swarm Optimizer", Proceedings of Congress on Evolu-tionary Computation, 79-73., 1998
1. R. C. Eberhart and Y. Shi, "Comparing inertia weights and constriction factors in particle swarm optimization", In Proceedings of the Congress on Evolutionary Computation, vol. 1, pp. 84–88, IEEE, La Jolla, Calif, USA, July 2000.
1. Y. Shi and Russell C. Eberhart, "Empirical Study of Particle Swarm Optimization, Proceeding Congress on Evolutionary Computation 1999, Piscataway, 1945-1949
1. Y. Feng, G. Teng, A. Wang, Y.M. Yao, "Chaotic inertia weight in particle swarm optimization", in: Second International Conference on Innovative Computing, Information and Control (ICICIC 07), 2007, pp. 475–1475.
1. A. Nickabadi, M. M. Ebadzadeh, and R. Safabakhsh, “A novel particle swarm optimization algorithm with adaptive inertia weight,” Applied Soft Computing Journal, vol. 11, no. 4, pp. 3658–3670, 2011.
1. Nelder-Mead法(http://ja.wikipedia.org/wiki/Nelder-Mead%E6%B3%95)
1. Storn, R., Price, K., "Differential Evolution – A Simple and Efficient Heuristic for Global Optimization over Continuous Spaces", Journal of Global Optimization 11: 341–359.
1. Sk. Minhazul Islam, Swagatam Das, "An Adaptive Differential Evolution Algorithm With Novel Mutation and Crossover Strategies for Global Numerical Optimization", IEEE TRANSACTIONS ON SYSTEMS, MAN, AND CYBERNETICS—PART B: CYBERNETICS, VOL. 42, NO. 2, APRIL 2012, pp482-500.
1. Price, K. and Storn, R., "Minimizing the Real Functions of the ICEC’96 contest by Differential Evolution", IEEE International Conference on Evolutionary Computation (ICEC’96), may 1996, pp. 842–844.
1. Xin-She Yang, Suash Deb, "Cuckoo search via Lévy flights.", World Congress on Nature and Biologically Inspired Computing (NaBIC 2009). IEEE Publications. pp. 210–214. arXiv:1003.1594v1.
1. Cuckoo Search (CS) Algorithm (http://www.mathworks.com/matlabcentral/fileexchange/29809-cuckoo-search--cs--algorithm)
1. 焼きなまし法(http://ja.wikipedia.org/wiki/%E7%84%BC%E3%81%8D%E3%81%AA%E3%81%BE%E3%81%97%E6%B3%95)
1. X. S. Yang, “Firefly algorithms for multimodal optimization,” in Proceedings of the 5th International Conference on Stochastic Algorithms: Foundation and Applications (SAGA '09), vol. 5792 of Lecture notes in Computer Science, pp. 169–178, 2009.
1. Firefly Algorithm (http://www.mathworks.com/matlabcentral/fileexchange/29693-firefly-algorithm)
1. Kalyanmoy Deb, Dhiraj Joshi and Ashish Anand, "Real-Coded Evolutionary Algorithms with Parent-Centric Recombination", KanGAL Report No. 2001003
