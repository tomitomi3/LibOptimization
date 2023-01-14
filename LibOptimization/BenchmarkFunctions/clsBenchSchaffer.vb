Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Schaffer function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(0,...,0) = 0
    ''' Range:
    '''  -100 to 100
    ''' Referrence:
    ''' 小林重信, "実数値GAのフロンティア"，人工知能学会誌 Vol. 24, No. 1, pp.147-162 (2009)
    ''' </remarks>
    <Serializable>
    Public Class clsBenchSchaffer : Inherits absObjectiveFunction
        Private dimension As Integer = 0

        ''' <summary>
        ''' Default constructor
        ''' </summary>
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
            For i As Integer = 0 To Me.dimension - 2
                ret += ((ai_var(i) ^ 2 + ai_var(i + 1) ^ 2) ^ 0.25) * (Math.Sin(50 * ((ai_var(i) ^ 2 + ai_var(i + 1) ^ 2)) ^ 0.1) ^ 2 + 1.0)
            Next

            Return ret
        End Function

        Public Overloads Function F(ByVal ai_var() As Double) As Double
            Return Me.F(New List(Of Double)(ai_var))
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double), Optional h As Double = 0.00000001) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double), Optional h As Double = 0.00000001) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return Me.dimension
        End Function
    End Class

End Namespace
