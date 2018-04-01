Imports LibOptimization
Imports LibOptimization.Optimization
Imports LibOptimization.Util
Imports LibOptimization.BenchmarkFunction

Module Module1
    '------------------------------------------------------------------------------------------------------------------
    'Sample code.
    'LibOptimization is numerical optimization algorithm library for .NET Framework.
    'This library will probably simplify the optimization using C# and VB.Net and other .NET Framework language.
    '------------------------------------------------------------------------------------------------------------------
    Sub Main()
        'Typical use
        With Nothing
            'How to use
            ' 1. You inherit "absObjectiveFunction" class and design objective function.
            ' 2. Choose an optimization method and implement code.
            ' 3. Do optimization!
            ' 4. Get result and evaluate.

            'Instantiation optimization class and set objective function.
            Dim optimization As New Optimization.clsOptDE(New clsBenchSphere(2))
            'Initialize starting value
            optimization.Init()
            'Do optimization
            optimization.DoIteration()
            clsUtil.DebugValue(optimization)
        End With

        'set inital position and inital value range
        With Nothing
            Dim optimization As New Optimization.clsOptDE(New clsBenchSphere(2))
            'set initialposition
            optimization.InitialPosition = New Double() {10, 10}

            'Initial value is generated in the range of -3 to 3.
            optimization.InitialValueRangeLower = -3
            optimization.InitialValueRangeUpper = 3

            'init
            optimization.Init()
            If optimization.IsRecentError() = True Then
                Return
            End If

            'do optimization
            optimization.DoIteration()
            If optimization.IsRecentError() = True Then
                Return
            End If

            'get result
            clsUtil.DebugValue(optimization)
        End With

        'Set initial point. (Not preparation all algorithms.)
        With Nothing
            Dim optimization As New Optimization.clsOptDE(New clsBenchSphere(2))

            'init with add initial point
            optimization.InitialPosition = {1, 1}
            optimization.Init()
            If optimization.IsRecentError() = True Then
                Return
            End If

            'do optimization
            optimization.DoIteration()
            If optimization.IsRecentError() = True Then
                Return
            End If

            'get result
            clsUtil.DebugValue(optimization)
        End With

        'use boundary
        With Nothing
            Dim optimization As New Optimization.clsOptDE(New clsBenchSphere(2))

            'set bpundary
            optimization.UpperBounds = {1, 1}
            optimization.LowerBounds = {2, 2}

            'init
            optimization.Init()
            If optimization.IsRecentError() = True Then
                Return
            End If

            'do optimization
            optimization.DoIteration()
            If optimization.IsRecentError() = True Then
                Return
            End If

            'get result
            clsUtil.DebugValue(optimization)
        End With

        'fix random seed.
        With Nothing
            Util.clsRandomXorshiftSingleton.GetInstance.SetDefaultSeed()
            Dim optimization As New Optimization.clsOptDE(New clsBenchSphere(2))
            optimization.Random = New Util.clsRandomXorshift()
            'init
            optimization.Init()
            'do optimization
            optimization.DoIteration()
            'get result
            clsUtil.DebugValue(optimization)
        End With

        'Evaluate optimization result per 5 iteration.
        With Nothing
            Dim optimization As New Optimization.clsOptDE(New clsBenchSphere(2))
            optimization.Init()
            clsUtil.DebugValue(optimization)
            While (optimization.DoIteration(5) = False)
                clsUtil.DebugValue(optimization, ai_isOutValue:=False)
            End While
            clsUtil.DebugValue(optimization)
        End With

        'Reuse best point
        With Nothing
            Dim optimization As New Optimization.clsOptRealGAREX(New clsBenchDeJongFunction3())

            '1st try
            optimization.Init()
            While (optimization.DoIteration(100) = False)
                clsUtil.DebugValue(optimization, ai_isOutValue:=False)
            End While
            clsUtil.DebugValue(optimization)

            '2nd try reuse
            optimization.InitialPosition = optimization.Result().ToArray()
            optimization.Init()
            While (optimization.DoIteration(100) = False)
                clsUtil.DebugValue(optimization, ai_isOutValue:=False)
            End While
            clsUtil.DebugValue(optimization)
        End With

        'Multi point and MultiThread(Multipoint avoids Local minimum by preparing many values.)
        With Nothing
            'prepare many optimization class.
            Dim multipointNumber As Integer = 30
            Dim listOptimization As New List(Of absOptimization)
            Dim f = New clsBenchAckley(20)
            For i As Integer = 0 To multipointNumber - 1
                Dim tempOpt As New LibOptimization.Optimization.clsOptNelderMead(f)
                tempOpt.ObjectiveFunction = f
                tempOpt.Init()
                listOptimization.Add(tempOpt)
            Next

            'using Parallel.ForEach
            Dim lockObj As New Object()
            Dim best As absOptimization = Nothing
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
            End If
            clsUtil.DebugValue(best)
        End With

        'LeastSquaresMethod 最小二乗法
        With Nothing
            'objective function for LeastSquaresMethod
            Dim objectiveFunction = New LeastSquaresMethod()
            objectiveFunction.Init()

            'set optimizer
            Dim optimization As New Optimization.clsOptDE(objectiveFunction)
            optimization.Init()

            clsUtil.DebugValue(optimization)
            While (optimization.DoIteration(50) = False)
                clsUtil.DebugValue(optimization, ai_isOutValue:=False)
            End While
            clsUtil.DebugValue(optimization)
        End With
    End Sub
End Module
