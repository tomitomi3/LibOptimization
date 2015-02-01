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
        'CheckOptimization()

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

        'You can use other optimization method.
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
        Dim optimization As absOptimization
        optimization = New clsOptSteepestDescent(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
        optimization = New clsOptNewtonMethod(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
        optimization = New clsOptNelderMead(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
        optimization = New clsOptNelderMeadNR(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
        optimization = New clsOptPatternSearch(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
        optimization = New clsOptRealGAREX(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
        optimization = New clsOptRealGASPX(New clsBenchSphere(10))
        optimization.Init()
        optimization.DoIteration()
        clsUtil.DebugValue(optimization)
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
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End With
        Next

        Console.WriteLine("Nelder-Mead")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(2))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End With
        Next

        Console.WriteLine("HookeJeeves(PatternSearch) 10D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptPatternSearch(New clsBenchRosenblock(10))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End With
        Next

        Console.WriteLine("Nelder-Mead 10D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(10))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End With
        Next

        Console.WriteLine("HookeJeeves(PatternSearch) 20D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptPatternSearch(New clsBenchRosenblock(20))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End With
        Next

        Console.WriteLine("Nelder-Mead 20D")
        For i As Integer = 0 To 5 - 1
            With Nothing
                Dim optimization As New clsOptNelderMead(New clsBenchRosenblock(20))
                optimization.Init()
                optimization.DoIteration()
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End With
        Next
    End Sub

    Private Sub ComplarisonRGA()
        Console.WriteLine("SPX Rastrigin 20D")
        With Nothing
            Dim optimization As New clsOptRealGASPX(New clsBenchRastriginFunction(20))
            optimization.Init()
            Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            While (optimization.DoIteration(10) = False)
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
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
            Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            While (optimization.DoIteration(10) = False)
                Console.WriteLine("{0},{1}", optimization.Result.Eval, optimization.GetIterationCount())
            End While
            If optimization.IsRecentError() = True Then
                Return
            End If
            clsUtil.DebugValue(optimization)
        End With
    End Sub

End Module
