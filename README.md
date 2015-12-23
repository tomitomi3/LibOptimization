[![Build status](https://ci.appveyor.com/api/projects/status/c4n61lv6x59sfqw5/branch/master?svg=true)](https://ci.appveyor.com/project/tomitomi3/liboptimization/branch/master)

LibOptimization
===============

![LibOptimization Log](https://github.com/tomitomi3/LibOptimization/blob/master/github_pic/logo_liboptimization.png)

LibOptimization is optimization algorithm library for .NET Framework.
This library will probably simplify the optimization using C# and VB.Net and other .NET Framework language.

Implementing optimization algorithm are Steepest Descent Method, Newton Method, Nelder Mead Method (Original ver, Wikipedia ver) ,Hooke and Jeeves of Pattern Search (Direct Search), Real-coded Genetic Algorithm(BLX-alpha and JGG, UNDX and JGG Simplex and JGG, REX and JGG, PCX and G3(Generalize Generation Gap)), Particle Swarm Optimization(Basic PSO, LDIW-PSO, CDIW-PSO, CRIW-PSO, AIW-PSO), Differential Evolution(DE/rand/1/bin, DE/rand/2/bin, DE/best/1/bin, DE/best/2/bin), FireFly, Cuckoo Search(Standard) and Simulated Annealing.

LibOptimizationは制約条件の無い最適化を行う.NET Framework用のライブラリです。実装しているアルゴリズムは最急降下法、ニュートン法、HookeJeevesのパターンサーチ法、Nelder-Mead法（オリジナルの実装、Wikipediaの実装）
、実数値遺伝的アルゴリズム（BLX-α、UNDX、SPX（シンプレクス）、REX、世代交代はJGG、PCX（世代交代はG3））、粒子群最適化（Basic PSO, LDIW-PSO, CDIW-PSO, CRIW-PSO, AIW-PSO）、Differential Evolution(差分進化？ DE/rand/1/bin, DE/rand/2/bin, DE/best/1/bin, DE/best/2/bin)、ホタルアルゴリズム、Cuckoo Search（Matlabコードの移植版）、焼きなまし法です。

See [wiki](https://github.com/tomitomi3/LibOptimization/wiki) for details.

How to get
==========

URL:https://www.nuget.org/packages/LibOptimization/
```
PM> Install-Package LibOptimization
```

License
=======

Microsoft Public License (MS-PL)

http://opensource.org/licenses/MS-PL

Support .NET Framework
============

.NET Framework 4.0

.NET Framework 3.5

.NET Framework 3.0

