Imports LibOptimization.MathTool
Imports LibOptimization.MathTool.RNG
Imports LibOptimization.Optimization

Namespace Util
    ''' <summary>
    ''' common use
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsUtil
        ''' <summary>
        ''' Generate Random permutation
        ''' </summary>
        ''' <param name="ai_max">0 to ai_max-1</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer) As List(Of Integer)
            Return RandomPermutaion(0, ai_max, {})
        End Function

        ''' <summary>
        ''' Generate Random permutation
        ''' </summary>
        ''' <param name="ai_max">0 to ai_max-1</param>
        ''' <param name="ai_removeIndex">RemoveIndex</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer, ByVal ai_removeIndex As Integer) As List(Of Integer)
            Return RandomPermutaion(0, ai_max, {ai_removeIndex})
        End Function

        ''' <summary>
        ''' Generate Random permutation with range (ai_min to ai_max-1)
        ''' </summary>
        ''' <param name="ai_min">start value</param>
        ''' <param name="ai_max">ai_max-1</param>
        ''' <param name="ai_removeIndexArray">remove index array</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_min As Integer, ByVal ai_max As Integer, ByVal ai_removeIndexArray() As Integer) As List(Of Integer)
            Dim nLength As Integer = ai_max - ai_min
            If nLength = 0 OrElse nLength < 0 Then
                Return New List(Of Integer)
            End If

            Dim ary As New List(Of Integer)(CInt(nLength * 1.5))
            If ai_removeIndexArray Is Nothing Then
                For ii As Integer = ai_min To ai_max - 1
                    ary.Add(ii)
                Next
            ElseIf ai_removeIndexArray.Length > 0 Then
                Dim removeArIndexFrom As Integer = 0
                Dim removeArIndexTo As Integer = ai_removeIndexArray.Length
                For ii As Integer = ai_min To ai_max - 1
                    If removeArIndexFrom = removeArIndexTo Then
                        ary.Add(ii)
                    Else
                        If ai_removeIndexArray(removeArIndexFrom) = ii Then
                            removeArIndexFrom += 1
                        Else
                            ary.Add(ii)
                        End If
                    End If
                Next
            Else
                For ii As Integer = ai_min To ai_max - 1
                    ary.Add(ii)
                Next
            End If

            'Fisher–Yates shuffle / フィッシャー - イェーツのシャッフル
            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = clsRandomXorshiftSingleton.GetInstance().Next(0, n + 1)
                Dim tmp As Integer = ary(k)
                ary(k) = ary(n)
                ary(n) = tmp
            End While
            Return ary
        End Function

        ''' <summary>
        ''' Random sort
        ''' </summary>
        ''' <param name="arPoint"></param>
        Public Shared Sub RandomizeArray(ByRef arPoint As List(Of clsPoint))
            'Fisher–Yates shuffle / フィッシャー - イェーツのシャッフル
            Dim n As Integer = arPoint.Count
            While n > 1
                n -= 1
                Dim k As Integer = clsRandomXorshiftSingleton.GetInstance().Next(0, n + 1)
                Dim tmp = arPoint(k)
                arPoint(k) = arPoint(n)
                arPoint(n) = tmp
            End While
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_opt"></param>
        ''' <param name="ai_precision"></param>
        ''' <param name="ai_isOutValue"></param>
        ''' <param name="ai_isOnlyIterationCount"></param>
        ''' <remarks></remarks>
        Public Shared Sub DebugValue(ByVal ai_opt As absOptimization,
                                     Optional ai_precision As Integer = 0,
                                     Optional ai_isOutValue As Boolean = True,
                                     Optional ai_isOnlyIterationCount As Boolean = False)
            If ai_opt Is Nothing Then
                Return
            End If

            If ai_isOnlyIterationCount = True Then
                Console.WriteLine("IterationCount:," & String.Format("{0}", ai_opt.IterationCount()))
                Return
            End If

            If ai_isOutValue = True Then
                Console.WriteLine("TargetFunction:" & ai_opt.ObjectiveFunction().GetType().Name & " Dimension:" & ai_opt.ObjectiveFunction().NumberOfVariable.ToString())
                Console.WriteLine("OptimizeMethod:" & ai_opt.GetType().Name)
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_opt.Result.Eval))
                Console.WriteLine("IterationCount:" & String.Format("{0}", ai_opt.IterationCount()))
                Console.WriteLine("Result        :")
                Dim str As New System.Text.StringBuilder()
                For Each value As Double In ai_opt.Result
                    If ai_precision <= 0 Then
                        str.Append(value.ToString())
                    Else
                        str.Append(value.ToString("F3"))
                    End If
                    str.AppendLine("")
                Next
                Console.WriteLine(str.ToString())
            Else
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_opt.Result.Eval))
            End If
        End Sub

        ''' <summary>
        ''' For Debug
        ''' </summary>
        ''' <param name="ai_results"></param>
        ''' <remarks></remarks>
        Public Shared Sub DebugValue(ByVal ai_results As List(Of clsPoint))
            If ai_results Is Nothing OrElse ai_results.Count = 0 Then
                Return
            End If
            For i As Integer = 0 To ai_results.Count - 1
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_results(i).Eval))
            Next
            Console.WriteLine()
        End Sub

        ''' <summary>
        ''' Check Criterion using eval
        ''' </summary>
        ''' <param name="ai_eps">EPS</param>
        ''' <param name="ai_comparisonA"></param>
        ''' <param name="ai_comparisonB"></param>
        ''' <param name="ai_tiny"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Reffrence:
        ''' William H. Press, Saul A. Teukolsky, William T. Vetterling, Brian P. Flannery,
        ''' "NUMRICAL RECIPIES 3rd Edition: The Art of Scientific Computing", Cambridge University Press 2007, pp505-506
        ''' </remarks>
        Public Shared Function IsCriterion(ByVal ai_eps As Double,
                                           ByVal ai_comparisonA As Double, ByVal ai_comparisonB As Double,
                                           Optional ByVal ai_tiny As Double = ConstantValues.SAME_ZERO) As Boolean
            'check division by zero
            Dim denominator = (Math.Abs(ai_comparisonB) + Math.Abs(ai_comparisonA)) + ai_tiny
            If denominator = 0 Then
                Return True
            End If

            'check criterion
            Dim temp = 2.0 * Math.Abs(ai_comparisonB - ai_comparisonA) / denominator
            If temp < ai_eps Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Check Criterion using points
        ''' </summary>
        ''' <param name="ai_eps">EPS</param>
        ''' <param name="ai_comparisonA"></param>
        ''' <param name="ai_comparisonB"></param>
        ''' <param name="ai_tiny"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsCriterion(ByVal ai_eps As Double,
                                           ByVal ai_comparisonA As clsPoint, ByVal ai_comparisonB As clsPoint,
                                           Optional ByVal ai_tiny As Double = ConstantValues.SAME_ZERO) As Boolean
            Return clsUtil.IsCriterion(ai_eps, ai_comparisonA.Eval, ai_comparisonB.Eval, ai_tiny)
        End Function

        ''' <summary>
        ''' Check Criterion using points
        ''' </summary>
        ''' <param name="eps"></param>
        ''' <param name="points"></param>
        ''' <param name="higherNPercentIndex"></param>
        ''' <param name="tiny"></param>
        ''' <returns></returns>
        Public Shared Function IsCriterion(ByVal eps As Double,
                                           ByVal points As List(Of clsPoint),
                                           ByVal higherNPercentIndex As Integer,
                                           Optional ByVal tiny As Double = ConstantValues.SAME_ZERO) As Boolean
            Dim evalIndexs = clsUtil.GetIndexSortedEvalFromPoints(points)

            'global
            Dim bestIndex = evalIndexs(0).Index
            Dim bestEval = points(bestIndex).Eval

            'comparators
            Dim compareIndex = evalIndexs(higherNPercentIndex).Index
            Dim compareEval = points(compareIndex).Eval

            Return clsUtil.IsCriterion(eps, bestEval, compareEval, tiny)
        End Function

        ''' <summary>
        ''' Check Criterion using particles
        ''' </summary>
        ''' <param name="eps"></param>
        ''' <param name="particles"></param>
        ''' <param name="higherNPercentIndex"></param>
        ''' <param name="tiny"></param>
        ''' <returns></returns>
        Public Shared Function IsCriterion(ByVal eps As Double,
                                           ByVal particles As List(Of clsParticle),
                                           ByVal higherNPercentIndex As Integer,
                                           Optional ByVal tiny As Double = ConstantValues.SAME_ZERO) As Boolean
            Dim evalIndexs = clsUtil.GetIndexSortedEvalFromParticles(particles)

            'global
            Dim bestIndex = evalIndexs(0).Index
            Dim bestEval = particles(bestIndex).GetBestEval()

            'comparators
            Dim compareIndex = evalIndexs(higherNPercentIndex).Index
            Dim compareEval = particles(compareIndex).GetBestEval()

            Return clsUtil.IsCriterion(eps, bestEval, compareEval, tiny)
        End Function

        ''' <summary>
        ''' Random position generator
        ''' </summary>
        ''' <param name="ai_min"></param>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GenRandomRange(ByVal ai_min As Double, ByVal ai_max As Double) As Double
            Dim ret = Math.Abs(ai_max - ai_min) * clsRandomXorshiftSingleton.GetInstance().NextDouble() + ai_min
            Return ret
        End Function

        ''' <summary>
        ''' Random position generator
        ''' </summary>
        ''' <param name="oRand"></param>
        ''' <param name="ai_min"></param>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GenRandomRange(ByVal oRand As System.Random, ByVal ai_min As Double, ByVal ai_max As Double) As Double
            Dim ret = Math.Abs(ai_max - ai_min) * oRand.NextDouble() + ai_min
            Return ret
        End Function

        ''' <summary>
        ''' Random position generator(array)
        ''' </summary>
        ''' <param name="func"></param>
        ''' <param name="initp"></param>
        ''' <param name="lower"></param>
        ''' <param name="upper"></param>
        ''' <returns></returns>
        Public Shared Function GenRandomPositionArray(ByVal func As absObjectiveFunction, ByVal initp() As Double, ByVal lower As Double, ByVal upper As Double) As Double()
            Dim ret(func.NumberOfVariable - 1) As Double
            If initp IsNot Nothing AndAlso initp.Length = func.NumberOfVariable Then
                'using InitialPosition
                Dim temp As New List(Of Double)
                For j As Integer = 0 To func.NumberOfVariable - 1
                    Dim value As Double = GenRandomRange(lower, upper) + initp(j)
                    ret(j) = value
                Next
            Else
                'not using InitialPosition
                Dim temp As New List(Of Double)
                For j As Integer = 0 To func.NumberOfVariable - 1
                    Dim value As Double = GenRandomRange(lower, upper)
                    ret(j) = value
                Next
            End If
            Return ret
        End Function

        ''' <summary>
        ''' to csv
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <remarks></remarks>
        Public Shared Sub ToCSV(ByVal arP As clsPoint)
            For Each p In arP
                Console.Write("{0},", p)
            Next
            Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' to csv
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <remarks></remarks>
        Public Shared Sub ToCSV(ByVal arP As List(Of clsPoint))
            For Each p In arP
                clsUtil.ToCSV(p)
            Next
            Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' eval output for debug
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <remarks></remarks>
        Public Shared Sub ToEvalList(ByVal arP As List(Of clsPoint))
            For Each p In arP
                Console.WriteLine("{0}", p.Eval)
            Next
        End Sub

        ''' <summary>
        ''' Limit solution space
        ''' </summary>
        ''' <param name="temp"></param>
        ''' <param name="LowerBounds"></param>
        ''' <param name="UpperBounds"></param>
        ''' <remarks></remarks>
        Public Shared Sub LimitSolutionSpace(ByVal temp As clsPoint, ByVal LowerBounds As Double(), ByVal UpperBounds As Double())
            If UpperBounds IsNot Nothing AndAlso LowerBounds IsNot Nothing Then
                For ii As Integer = 0 To temp.Count - 1
                    Dim upper As Double = 0
                    Dim lower As Double = 0
                    If UpperBounds(ii) > LowerBounds(ii) Then
                        upper = UpperBounds(ii)
                        lower = LowerBounds(ii)
                    ElseIf UpperBounds(ii) < LowerBounds(ii) Then
                        upper = LowerBounds(ii)
                        lower = UpperBounds(ii)
                    Else
                        Throw New Exception("Error! upper bound and lower bound are same.")
                    End If

                    If temp(ii) > lower AndAlso temp(ii) < upper Then
                        'in
                    ElseIf temp(ii) > lower Then
                        temp(ii) = lower
                    ElseIf temp(ii) < upper Then
                        temp(ii) = upper
                    End If
                Next
            End If
            temp.ReEvaluate()
        End Sub

        ''' <summary>
        ''' calc length from each points
        ''' </summary>
        ''' <param name="points"></param>
        ''' <returns></returns>
        Public Shared Function IsExistZeroLength(ByVal points() As clsPoint) As Boolean
            Dim isCanCrossover As Boolean = True
            Dim vec As MathTool.DenseVector = Nothing
            For i As Integer = 0 To points.Length - 2
                vec = points(i) - points(i + 1)
                If vec.NormL1() = 0 Then
                    Return True
                End If
            Next
            vec = points(points.Length - 1) - points(0)
            If vec.NormL1() = 0 Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Overflow check for debug
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns></returns>
        Public Shared Function CheckOverflow(ByVal v As Double) As Boolean
            If Double.IsInfinity(v) = True Then
                Return True
            End If
            If Double.IsNaN(v) = True Then
                Return True
            End If
            If Double.IsNegativeInfinity(v) = True Then
                Return True
            End If
            If Double.IsPositiveInfinity(v) = True Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Overflow check for debug
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Shared Function CheckOverflow(ByVal p As clsPoint) As Boolean
            For Each v In p
                If Double.IsInfinity(v) = True Then
                    Return True
                End If
                If Double.IsNaN(v) = True Then
                    Return True
                End If
                If Double.IsNegativeInfinity(v) = True Then
                    Return True
                End If
                If Double.IsPositiveInfinity(v) = True Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Overflow check for debug
        ''' </summary>
        ''' <param name="listP"></param>
        ''' <returns></returns>
        Public Shared Function CheckOverflow(ByVal listP As List(Of clsPoint)) As Boolean
            For Each temp In listP
                For Each v In temp
                    If Double.IsInfinity(v) = True Then
                        Return True
                    End If
                    If Double.IsNaN(v) = True Then
                        Return True
                    End If
                    If Double.IsNegativeInfinity(v) = True Then
                        Return True
                    End If
                    If Double.IsPositiveInfinity(v) = True Then
                        Return True
                    End If
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Set initial point
        ''' </summary>
        ''' <param name="pupulation"></param>
        ''' <param name="initialPosition"></param>
        Public Shared Sub SetInitialPoint2(ByVal pupulation As List(Of clsPoint), ByVal initialPosition() As Double)
            If pupulation IsNot Nothing AndAlso pupulation.Count > 0 Then
                Dim func = pupulation(0).GetFunc()

                If initialPosition IsNot Nothing AndAlso initialPosition.Length = func.NumberOfVariable Then
                    Dim index As Integer = CInt(pupulation.Count / 10)
                    If index < 1 Then
                        index = 1
                    End If
                    For i As Integer = 0 To index - 1
                        For j As Integer = 0 To func.NumberOfVariable - 1
                            pupulation(i)(j) = initialPosition(j)
                        Next
                        pupulation(i).ReEvaluate()
                    Next
                End If
            End If
        End Sub

        ''' <summary>
        ''' optimizer for UnitTest
        ''' </summary>
        ''' <param name="func"></param>
        ''' <returns></returns>
        Public Shared Function GetOptimizersForUnitTest(ByVal func As absObjectiveFunction) As List(Of absOptimization)
            Dim optimizers As New List(Of absOptimization)
            With Nothing
                Dim optCs = New clsOptCS(func)
                optimizers.Add(optCs)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_1_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_2_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_current_to_Best_1_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_current_to_Best_2_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_current_to_rand_1_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_rand_1_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDE(func)
                opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_rand_2_bin
                opt.Iteration = 500
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptDEJADE(func)
                opt.Iteration = 300
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptES(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptFA(func)
                opt.Iteration = 300
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptHillClimbing(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptNelderMeadANMS(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptNelderMead(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptNelderMeadWiki(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPatternSearch(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSO(func)
                opt.SwarmType = clsOptPSO.EnumSwarmType.GlobalBest
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSO(func)
                opt.SwarmType = clsOptPSO.EnumSwarmType.LocalBest
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSOAIW(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSOChaoticIW(func)
                opt.ChaoticMode = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CDIW
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSOChaoticIW(func)
                opt.ChaoticMode = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CRIW
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSOCPSO(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptPSOLDIW(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptRealGABLX(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptRealGAPCX(func)
                opt.Iteration = 1000
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptRealGAREX(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptRealGASPX(func)
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptRealGAUNDX(func)
                opt.ALPHA = 0.6
                opt.PopulationSize = 100
                opt.ChildrenSize = 50
                opt.Iteration = 700
                optimizers.Add(opt)
            End With
            With Nothing
                Dim opt = New clsOptSimulatedAnnealing(func)
                'opt.NeighborRange = 0.1
                opt.Iteration *= 2
                optimizers.Add(opt)
            End With

            '--------------------------------------------------------------
            '微分が必要
            '--------------------------------------------------------------
            With Nothing
                If TryCast(func, BenchmarkFunction.clsBenchSphere) IsNot Nothing Then
                    optimizers.Add(New clsOptNewtonMethod(func))
                End If
            End With
            With Nothing
                If TryCast(func, BenchmarkFunction.clsBenchSphere) IsNot Nothing Then
                    optimizers.Add(New clsOptSteepestDescent(func))
                End If
            End With

            Return optimizers
        End Function

        ''' <summary>
        ''' LibOptimzation.Results() to my Matrix class
        ''' </summary>
        ''' <param name="results"></param>
        ''' <returns></returns>
        Public Shared Function ToConvertMat(ByVal results As List(Of clsPoint)) As MathTool.DenseMatrix
            Dim row = results.Count
            Dim col = results(0).Count
            Dim ret As New MathTool.DenseMatrix(row, col)
            For i As Integer = 0 To row - 1
                For j As Integer = 0 To col - 1
                    ret(i)(j) = results(i)(j)
                Next
            Next

            Return ret
        End Function

        ''' <summary>
        ''' average
        ''' Memo: same -> (List(of double)).Average 
        ''' </summary>
        ''' <param name="vec"></param>
        ''' <returns></returns>
        Public Shared Function Average(ByVal vec As DenseVector) As Double
            Dim ret As Double = 0.0
            For Each value In vec
                ret += value
            Next
            Return ret / vec.Count
        End Function

        ''' <summary>
        ''' Find current best index from List(of clsPoint)
        ''' </summary>
        ''' <param name="ai_points"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FindCurrentBestIndex(ByVal ai_points As List(Of clsPoint)) As Integer
            Dim bestIndex As Integer = 0
            Dim bestEval = ai_points(0).Eval
            For i = 0 To ai_points.Count - 1
                If ai_points(i).Eval < bestEval Then
                    bestEval = ai_points(i).Eval
                    bestIndex = i
                End If
            Next
            Return bestIndex
        End Function

        ''' <summary>
        ''' Find current best point from List(of clsPoint)
        ''' </summary>
        ''' <param name="ai_points"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function FindCurrentBestPointFromPoints(ByVal ai_points As List(Of clsPoint), Optional ByVal isCopy As Boolean = False) As clsPoint
            Dim i = FindCurrentBestIndex(ai_points)
            If isCopy Then
                Return ai_points(i).Copy()
            Else
                Return ai_points(i)
            End If
        End Function

        ''' <summary>
        ''' Find global best(for PSO)
        ''' </summary>
        ''' <param name="particles"></param>
        ''' <returns></returns>
        Public Shared Function FindCurrentBestIndexFromParticles(ByVal particles As List(Of clsParticle)) As Integer
            Dim bestIndex As Integer = 0
            Dim tempBest As clsPoint = Nothing
            If particles(0).BestPoint.Eval < particles(0).Point.Eval Then
                tempBest = particles(0).BestPoint
            Else
                tempBest = particles(0).Point
            End If
            For i As Integer = 1 To particles.Count - 1
                If particles(i).Point.Eval < tempBest.Eval Then
                    tempBest = particles(i).Point
                    bestIndex = i
                End If
                If particles(i).BestPoint.Eval < tempBest.Eval Then
                    tempBest = particles(i).BestPoint
                    bestIndex = i
                End If
            Next
            Return bestIndex
        End Function

        ''' <summary>
        ''' Eval list
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSortedEvalList(ByVal arP As List(Of clsPoint)) As List(Of clsEval)
            Dim sortedEvalList = New List(Of clsEval)
            For i = 0 To arP.Count - 1
                sortedEvalList.Add(New clsEval(i, arP(i).Eval))
            Next
            sortedEvalList.Sort()
            Return sortedEvalList
        End Function

        ''' <summary>
        ''' Get an index sorted by valuation
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <returns></returns>
        Public Shared Function GetIndexSortedEvalFromPoints(ByVal arP As List(Of clsPoint)) As List(Of clsEval)
            Dim sortedEvalList = New List(Of clsEval)
            For i = 0 To arP.Count - 1
                sortedEvalList.Add(New clsEval(i, arP(i).Eval))
            Next
            sortedEvalList.Sort()
            Return sortedEvalList
        End Function

        ''' <summary>
        ''' Get an index sorted by valuation
        ''' </summary>
        ''' <param name="arP"></param>
        ''' <returns></returns>
        Public Shared Function GetIndexSortedEvalFromParticles(ByVal arP As List(Of clsParticle)) As List(Of clsEval)
            Dim sortedEvalList = New List(Of clsEval)
            For i = 0 To arP.Count - 1
                sortedEvalList.Add(New clsEval(i, arP(i).GetBestEval()))
            Next
            sortedEvalList.Sort()
            Return sortedEvalList
        End Function

        ''' <summary>
        ''' serialize(write)
        ''' </summary>
        ''' <param name="optobj"></param>
        ''' <param name="path"></param>
        Public Shared Sub SerializeOpt(optobj As Object, path As String)
            Using fs = New System.IO.FileStream(path, System.IO.FileMode.Create)
                Dim bf = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
                bf.Serialize(fs, optobj)
            End Using
        End Sub

        ''' <summary>
        ''' deserialize(read)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function DeSerializeOpt(path As String) As Object
            Using fs = New System.IO.FileStream(path, System.IO.FileMode.Open)
                Dim bf = New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
                Return bf.Deserialize(fs)
            End Using
        End Function
    End Class
End Namespace
