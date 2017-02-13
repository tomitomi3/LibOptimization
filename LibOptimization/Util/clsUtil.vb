Imports LibOptimization.Optimization

Namespace Util
    ''' <summary>
    ''' common use
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsUtil
        ''' <summary>
        ''' Normal Distribution
        ''' </summary>
        ''' <param name="ai_ave">Average</param>
        ''' <param name="ai_sigma2">Varianse s^2</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' using Box-Muller method
        ''' </remarks>
        Public Shared Function NormRand(Optional ByVal ai_ave As Double = 0,
                                        Optional ByVal ai_sigma2 As Double = 1) As Double
            Dim x As Double = clsRandomXorshiftSingleton.GetInstance().NextDouble()
            Dim y As Double = clsRandomXorshiftSingleton.GetInstance().NextDouble()

            Dim c As Double = Math.Sqrt(-2.0 * Math.Log(x))
            If (0.5 - clsRandomXorshiftSingleton.GetInstance().NextDouble() > 0.0) Then
                Return c * Math.Sin(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            Else
                Return c * Math.Cos(2.0 * Math.PI * y) * ai_sigma2 + ai_ave
            End If
        End Function

        ''' <summary>
        ''' Cauchy Distribution
        ''' </summary>
        ''' <param name="ai_mu">default:0</param>
        ''' <param name="ai_gamma">default:1</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' http://www.sat.t.u-tokyo.ac.jp/~omi/random_variables_generation.html#Cauchy
        ''' </remarks>
        Public Shared Function CauchyRand(Optional ByVal ai_mu As Double = 0, Optional ByVal ai_gamma As Double = 1) As Double
            Return ai_mu + ai_gamma * Math.Tan(Math.PI * (clsRandomXorshiftSingleton.GetInstance().NextDouble() - 0.5))
        End Function

        ''' <summary>
        ''' Generate Random permutation
        ''' </summary>
        ''' <param name="ai_max">0 to ai_max-1</param>
        ''' <param name="ai_removeIndex">RemoveIndex</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer, Optional ByVal ai_removeIndex As Integer = -1) As List(Of Integer)
            Return RandomPermutaion(0, ai_max, ai_removeIndex)
        End Function

        ''' <summary>
        ''' Generate Random permutation with range (ai_min to ai_max-1)
        ''' </summary>
        ''' <param name="ai_min">start value</param>
        ''' <param name="ai_max">ai_max-1</param>
        ''' <param name="ai_removeIndex">RemoveIndex -1 is invalid</param>
        ''' <returns></returns>
        Public Shared Function RandomPermutaion(ByVal ai_min As Integer, ByVal ai_max As Integer, ByVal ai_removeIndex As Integer) As List(Of Integer)
            Dim nLength As Integer = ai_max - ai_min
            If nLength = 0 OrElse nLength < 0 Then
                Return New List(Of Integer)
            End If

            Dim ary As New List(Of Integer)(CInt(nLength * 1.5))
            If ai_removeIndex <= -1 Then
                For ii As Integer = ai_min To ai_max - 1
                    ary.Add(ii)
                Next
            Else
                For ii As Integer = ai_min To ai_max - 1
                    If ai_removeIndex <> ii Then
                        ary.Add(ii)
                    End If
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
        ''' Check Criterion
        ''' </summary>
        ''' <param name="ai_eps">EPS</param>
        ''' <param name="ai_comparisonA"></param>
        ''' <param name="ai_comparisonB"></param>
        ''' <param name="ai_tiny"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsCriterion(ByVal ai_eps As Double,
                                           ByVal ai_comparisonA As clsPoint, ByVal ai_comparisonB As clsPoint,
                                           Optional ByVal ai_tiny As Double = 0.0000000000001) As Boolean
            Return clsUtil.IsCriterion(ai_eps, ai_comparisonA.Eval, ai_comparisonB.Eval, ai_tiny)
        End Function

        ''' <summary>
        ''' Check Criterion
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
                                           Optional ByVal ai_tiny As Double = 0.0000000000001) As Boolean
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
        ''' Random generator helper
        ''' </summary>
        ''' <param name="oRand"></param>
        ''' <param name="ai_min"></param>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GenRandomRange(ByVal oRand As System.Random, ByVal ai_min As Double, ByVal ai_max As Double) As Double
            Return Math.Abs(ai_max - ai_min) * oRand.NextDouble() + ai_min
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
        ''' Best clsPoint
        ''' </summary>
        ''' <param name="ai_points"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetBestPoint(ByVal ai_points As List(Of clsPoint), Optional ByVal isCopy As Boolean = False) As clsPoint
            If ai_points Is Nothing Then
                Return Nothing
            ElseIf ai_points.Count = 0 Then
                Return Nothing
            ElseIf ai_points.Count = 1 Then
                Return ai_points(0)
            End If

            Dim best = ai_points(0)
            For i As Integer = 1 To ai_points.Count - 1
                If best.Eval > ai_points(i).Eval Then
                    best = ai_points(i)
                End If
            Next

            If isCopy = False Then
                Return best
            Else
                Return best.Copy()
            End If
        End Function

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
                temp.ReEvaluate()
            End If
        End Sub

    End Class
End Namespace