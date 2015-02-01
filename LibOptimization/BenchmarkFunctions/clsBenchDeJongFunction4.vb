Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' De Jong’s function 4 (qudratic with gauss Function)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class clsBenchDeJongFunction4 : Inherits absObjectiveFunction
        Private dimension As Integer = 0

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <param name="ai_dim">Set dimension</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_dim As Integer)
            Me.dimension = ai_dim
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

            Dim ret As Double = 0

            'not implement

            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException()
        End Function

        Public Overrides ReadOnly Property NumberOfVariable As Integer
            Get
                Return Me.dimension
            End Get
        End Property
    End Class

End Namespace
