Module Module1
    Sub Main()
        '評価関数
        Dim objectiveFunction = New clsLeastSquaresMethod()
        objectiveFunction.Init("data.csv")

        '最小化
        Dim opt As New LibOptimization.Optimization.clsOptDE(objectiveFunction)
        opt.DEStrategy = LibOptimization.Optimization.clsOptDE.EnumDEStrategyType.DE_best_2_bin
        opt.Init()
        LibOptimization.Util.clsUtil.DebugValue(opt)
        opt.DoIteration()
        LibOptimization.Util.clsUtil.DebugValue(opt)
    End Sub
End Module
