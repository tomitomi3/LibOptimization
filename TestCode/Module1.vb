Imports LibOptimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

Module Module1

    Sub Main()
        'With Nothing
        '    Dim opt = New LibOptimization.Optimization.DerivativeFree.NelderMead()
        '    opt.Init()
        '    opt.DoIteration()
        '    Util.DebugValue(opt)
        'End With

        'With Nothing
        '    Dim opt = New LibOptimization.Optimization.DerivativeFree.NelderMead()
        '    opt.ObjectiveFunction = New BenchSphere(2)
        '    opt.DoIteration()
        '    Util.DebugValue(opt)
        'End With

        'With Nothing
        '    Dim opt = New LibOptimization.Optimization.DerivativeFree.NelderMead()
        '    opt.DoIteration()
        '    Util.DebugValue(opt)
        'End With

        With Nothing
            While True
                Dim optimization As New Optimization.clsOptRealGAUNDX(New clsBenchSphere(2))
                'optimization.InitialPosition = {1000, 1000, 1, 1, 1, 1, 1, 1, 1, 1}
                optimization.PopulationSize = 100
                optimization.Init()
                clsUtil.DebugValue(optimization, ai_isOutValue:=False)
                While (optimization.DoIteration(10) = False)
                    clsUtil.DebugValue(optimization, ai_isOutValue:=False)
                End While
                clsUtil.DebugValue(optimization)
            End While
        End With
    End Sub

End Module
