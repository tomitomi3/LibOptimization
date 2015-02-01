Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Powell function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  x = {0,0,0,0}
    '''  f(x) = 0
    ''' </remarks>
    Public Class clsBenchPowellFunction : Inherits absObjectiveFunction
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Target Function
        ''' </summary>
        ''' <param name="ai_var"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function F(ByVal ai_var As List(Of Double)) As Double
            If ai_var Is Nothing Then
                Return 0
            End If

            Dim x1 As Double = ai_var(0)
            Dim x2 As Double = ai_var(1)
            Dim x3 As Double = ai_var(2)
            Dim x4 As Double = ai_var(3)
            Dim ret As Double = (x1 - 10 * x2) ^ 2 + 5 * (x3 - x4) ^ 2 + (x2 + 2 * x3) ^ 4 + 10 * (x1 - x4) ^ 4
            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides ReadOnly Property NumberOfVariable As Integer
            Get
                Return 4
            End Get
        End Property
    End Class

End Namespace
