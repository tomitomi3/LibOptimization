Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Easom Function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(pi, pi) = -1
    ''' Range:
    '''  −100≦x1 , x2≦100
    ''' Referrence:
    ''' [1]Test fXin-She Yang, "Test Problems in Optimization", arXiv(http://arxiv.org/abs/1008.0549)
    ''' </remarks>
    Public Class clsBenchEasomFunction : Inherits absObjectiveFunction
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Target Function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function F(ByVal x As List(Of Double)) As Double
            If x Is Nothing Then
                Return 0
            End If

            Dim x1 As Double = x(0)
            Dim x2 As Double = x(1)
            Dim ret As Double = -Math.Cos(x1) * Math.Cos(x2) * Math.Exp(-((x1 - Math.PI) ^ 2 + (x2 - Math.PI) ^ 2))
            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class

End Namespace
