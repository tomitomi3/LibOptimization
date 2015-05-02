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
        ''' Generate Random permutation
        ''' </summary>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer) As List(Of Integer)
            Dim ary As New List(Of Integer)(ai_max)
            For i As Integer = 0 To ai_max - 1
                ary.Add(i)
            Next

            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = CInt(clsRandomXorshiftSingleton.GetInstance().Next(0, n + 1))
                Dim tmp As Integer = ary(k)
                ary(k) = ary(n)
                ary(n) = tmp
            End While
            Return ary
        End Function

        ''' <summary>
        ''' Generate Random permutation with Remove
        ''' </summary>
        ''' <param name="ai_max"></param>
        ''' <param name="ai_removeIndex">RemoveIndex</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RandomPermutaion(ByVal ai_max As Integer, ByVal ai_removeIndex As Integer) As List(Of Integer)
            Dim ary As New List(Of Integer)(ai_max)
            For i As Integer = 0 To ai_max - 1
                If ai_removeIndex <> i Then
                    ary.Add(i)
                End If
            Next

            Dim n As Integer = ary.Count
            While n > 1
                n -= 1
                Dim k As Integer = CInt(clsRandomXorshiftSingleton.GetInstance().Next(0, n + 1))
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
                                     Optional ai_precision As Integer = 0, _
                                     Optional ai_isOutValue As Boolean = True, _
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
        ''' Check Criterion
        ''' </summary>
        ''' <param name="ai_eps">EPS</param>
        ''' <param name="ai_comparisonA"></param>
        ''' <param name="ai_comparisonB"></param>
        ''' <param name="ai_tiny"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsCriterion(ByVal ai_eps As Double, _
                                           ByVal ai_comparisonA As clsPoint, ByVal ai_comparisonB As clsPoint, _
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
        Public Shared Function IsCriterion(ByVal ai_eps As Double, _
                                           ByVal ai_comparisonA As Double, ByVal ai_comparisonB As Double, _
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
    End Class
End Namespace