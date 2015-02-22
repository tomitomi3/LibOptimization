Imports LibOptimization
Imports LibOptimization.Optimization
Imports LibOptimization.Util
Imports LibOptimization.BenchmarkFunction

Module Module1
    Sub Main()
        'Sample code.
        'LibOptimization is collection of optimization algorithm for .NET Framework.
        'LibOptimization simplifies Optimization.

        'Easy to use
        ' 1. You inherit "absObjectiveFunction" class and design objective function.
        ' 2. Choose an optimization method and implement code.
        ' 3. Do optimization!
        ' 4. Get result and evaluate.

        'Test
        CheckOptimization()
        'OptimizeDeJongFunction()

        'Typical use
        With Nothing
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
        End With

        'Set random class and seed.
        With Nothing
            Dim optimization As New clsOptRealGASPX(New clsBenchSphere(1))
            optimization.Random = New clsRandomXorshift(123456) ' or System.Random(123456)
            optimization.Init()
            optimization.DoIteration()
            If optimization.IsRecentError() = True Then
                Return
            Else
                clsUtil.DebugValue(optimization)
            End If
        End With

        'When you want result every 5 times.
        With Nothing
            Dim optimization As New clsOptSteepestDescent(New clsBenchSphere(2))
            optimization.Init()
            While (optimization.DoIteration(5) = False)
                clsUtil.DebugValue(optimization)
            End While
            clsUtil.DebugValue(optimization, ai_isOnlyIterationCount:=True)
        End With

        'set initial point.
        With Nothing
            Dim optimization As New clsOptSteepestDescent(New clsBenchSphere(2))
            optimization.Init(New Double() {-10, 10})
            While (optimization.DoIteration(5) = False)
                clsUtil.DebugValue(optimization)
            End While
            clsUtil.DebugValue(optimization, ai_isOnlyIterationCount:=True)
        End With

        'You can use other optimization algorithm.
        With Nothing
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
        End With

        'Elite Strategy for RGA
        With Nothing
            'De jong Function3
            Dim optimization1 As New Optimization.clsOptRealGASPX(New BenchmarkFunction.clsBenchDeJongFunction3())
            optimization1.Init()
            For i As Integer = 0 To 2
                optimization1.DoIteration()
                clsUtil.DebugValue(optimization1)
                'Carry over to the new iteration.
                optimization1.UseEliteStrategy(0.1)
            Next
            clsUtil.DebugValue(optimization1)

            'De jong Function5
            Dim optimization2 As New Optimization.clsOptRealGASPX(New clsBenchDeJongFunction5(), ai_randomRange:=65.536, ai_childsSize:=100)
            optimization2.Init()
            For i As Integer = 0 To 5
                optimization2.DoIteration()
                clsUtil.DebugValue(optimization2, ai_isOutValue:=False)
                optimization2.UseEliteStrategy(0.1)
            Next
            clsUtil.DebugValue(optimization2)
        End With

        'Multi point and MultiThread
        ' - Multipoint avoids Local minimum by preparing many values.
        With Nothing
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
            Else
                clsUtil.DebugValue(best)
            End If
        End With
    End Sub

    Private Sub CheckOptimization()
        Dim DIMENSION = 5
        Dim optimization As absOptimization
        optimization = New clsOptSteepestDescent(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptNewtonMethod(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptNelderMead(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptNelderMeadNR(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptPatternSearch(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptRealGAREX(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptRealGASPX(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptPSO(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptPSOLDIW(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptPSOChaoticIW(New clsBenchSphere(DIMENSION))
        CType(optimization, clsOptPSOChaoticIW).PARAM_InertialWeightStrategie = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CDIW
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptPSOChaoticIW(New clsBenchSphere(DIMENSION))
        CType(optimization, clsOptPSOChaoticIW).PARAM_InertialWeightStrategie = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CRIW
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        optimization = New clsOptPSOAIW(New clsBenchSphere(DIMENSION))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization, ai_isOutValue:=False)

        Console.WriteLine("-------------")
    End Sub

    Private Sub ComparisonHJNR()
        Console.WriteLine("HookeJeeves(PatternSearch) - MovePoint")
        With Nothing
            Dim optimization As New clsOptPatternSearch(New clsBenchRosenblock(2))
            optimization.Init(New Double() {-1, 1})
            Console.WriteLine("{0},{1},{2}", optimization.Result.Eval, optimization.Result(0), optimization.Result(1))
            While (optimization.DoIteration(10) = False)
                Console.WriteLine("{0},{1},{2}", optimization.Result.Eval, optimization.Result(0), optimization.Result(1))
            End While
            clsUtil.DebugValue(optimization)
        End With
        Console.WriteLine("Nelder-Mead - MovePoint")
        With Nothing
            Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(2))
            optimization.Init(New Double() {-1, 1})
            Console.WriteLine("{0},{1},{2}", optimization.Result.Eval, optimization.Result(0), optimization.Result(1))
            While (optimization.DoIteration(10) = False)
                Console.WriteLine("{0},{1},{2}", optimization.Result.Eval, optimization.Result(0), optimization.Result(1))
            End While
            clsUtil.DebugValue(optimization)
        End With

        'PatternSearch
        Console.WriteLine("HookeJeeves(PatternSearch)")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptPatternSearch(New clsBenchRosenblock(2))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End With
        Next

        Console.WriteLine("Nelder-Mead")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(2))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End With
        Next

        Console.WriteLine("HookeJeeves(PatternSearch) 10D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptPatternSearch(New clsBenchRosenblock(10))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End With
        Next

        Console.WriteLine("Nelder-Mead 10D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(10))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End With
        Next

        Console.WriteLine("HookeJeeves(PatternSearch) 20D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptPatternSearch(New clsBenchRosenblock(20))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End With
        Next

        Console.WriteLine("Nelder-Mead 20D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(20))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End With
        Next
    End Sub

    Private Sub ComplarisonRGA()
        Console.WriteLine("SPX Rastrigin 20D")
        With Nothing
            Dim optimization As New clsOptRealGASPX(New clsBenchRastriginFunction(20))
            optimization.Init()
            Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            While (optimization.DoIteration(10) = False)
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End While
            If optimization.IsRecentError() = True Then
                Return
            End If
            clsUtil.DebugValue(optimization)
        End With

        Console.WriteLine("REX Rastrigin 20D")
        With Nothing
            Dim optimization As New clsOptRealGAREX(New clsBenchRastriginFunction(20))
            optimization.Init()
            Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            While (optimization.DoIteration(10) = False)
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.IterationCount())
            End While
            If optimization.IsRecentError() = True Then
                Return
            End If
            clsUtil.DebugValue(optimization)
        End With
    End Sub

    Private Sub OptimizeDeJongFunction()
        With Nothing
            Dim opt As New Optimization.clsOptRealGASPX(New clsBenchDeJongFunction1(), ai_randomRange:=5.12)
            opt.Init()
            clsUtil.DebugValue(opt)
            opt.DoIteration()
            clsUtil.DebugValue(opt)
        End With

        With Nothing
            Dim opt As New Optimization.clsOptRealGASPX(New clsBenchDeJongFunction2(), ai_randomRange:=2.048)
            opt.Init()
            clsUtil.DebugValue(opt)
            opt.DoIteration()
            clsUtil.DebugValue(opt)
        End With

        With Nothing
            Dim opt As New Optimization.clsOptRealGASPX(New clsBenchDeJongFunction3(), ai_randomRange:=5.12)
            opt.Init()

            opt.DoIteration()
            clsUtil.DebugValue(opt)

            opt.UseEliteStrategy(0.2) 'Carry over to the new iteration.
            opt.DoIteration()
            clsUtil.DebugValue(opt)
        End With

        With Nothing
            Dim opt As New Optimization.clsOptRealGASPX(New clsBenchDeJongFunction4(), ai_randomRange:=1.28)
            opt.Init()
            clsUtil.DebugValue(opt)
            opt.DoIteration()
            clsUtil.DebugValue(opt)
        End With

        With Nothing
            Dim opt As New Optimization.clsOptRealGASPX(New clsBenchDeJongFunction5(), ai_randomRange:=65.536, ai_childsSize:=100)
            opt.Init()
            For i As Integer = 0 To 2
                opt.DoIteration()
                clsUtil.DebugValue(opt, ai_isOutValue:=False)
                opt.UseEliteStrategy(0.1) 'Carry over to the new iteration.
            Next
            clsUtil.DebugValue(opt)
        End With
    End Sub

End Module
