Module Module1
    Sub Main()
        '評価関数
        Dim objectiveFunction = New clsLeastSquaresMethod()
        objectiveFunction.Init("data.csv")

        '最小化
        Dim opt As New LibOptimization.Optimization.clsOptDE(objectiveFunction)
        opt.DEStrategy = LibOptimization.Optimization.clsOptDE.EnumDEStrategyType.DE_best_1_bin
        opt.Init()
        LibOptimization.Util.clsUtil.DebugValue(opt)
        While (opt.DoIteration(50) = False)
            LibOptimization.Util.clsUtil.DebugValue(opt, ai_isOutValue:=False)
        End While
        LibOptimization.Util.clsUtil.DebugValue(opt)
    End Sub
End Module
