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
            Dim a As Double = 1 - clsRandomXorshiftSingleton.GetInstance().NextDouble()
            Dim b As Double = 1 - clsRandomXorshiftSingleton.GetInstance().NextDouble()
            Dim c As Double = Math.Sqrt(-2.0 * Math.Log(a))

            If (0.5 - clsRandomXorshiftSingleton.GetInstance().NextDouble() > 0.0) Then
                Return c * Math.Sin(Math.PI * 2.0 * b) * ai_sigma2 + ai_ave
            Else
                Return c * Math.Cos(Math.PI * 2.0 * b) * ai_sigma2 + ai_ave
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
                Console.WriteLine("IterationCount:," & String.Format("{0}", ai_opt.GetIterationCount()))
                Return
            End If

            If ai_isOutValue = True Then
                Console.WriteLine("TargetFunction:" & ai_opt.GetFunc().GetType().Name & " Dimension:" & ai_opt.GetFunc().NumberOfVariable.ToString())
                Console.WriteLine("OptimizeMethod:" & ai_opt.GetType().Name)
                Console.WriteLine("Eval          :" & String.Format("{0}", ai_opt.Result.Eval))
                Console.WriteLine("IterationCount:" & String.Format("{0}", ai_opt.GetIterationCount()))
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
    End Class
End Namespace