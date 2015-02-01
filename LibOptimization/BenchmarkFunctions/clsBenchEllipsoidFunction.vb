Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Ellipsoid function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  x = {0,...,0}
    ''' 
    ''' Refference:
    ''' [1]小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ''' </remarks>
    Public Class clsBenchEllipsoidFunction : Inherits absObjectiveFunction
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

            If Me.dimension <> ai_var.Count Then
                Return 0
            End If

            Dim ret As Double = 0.0
            For i As Integer = 0 To Me.dimension - 1
                Dim temp As Double = (1000 ^ (i / (Me.dimension - 1))) * ai_var(i)
                ret += temp ^ 2
            Next

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
                Return Me.dimension
            End Get
        End Property
    End Class

End Namespace
