Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Rastrigin function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  F(0,...,0) = 0
    ''' Range:
    '''  -5.12 to 5.12
    ''' Referrence:
    ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ''' </remarks>
    Public Class clsBenchRastrigin : Inherits absObjectiveFunction
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

            Dim A As Double = 10.0
            Dim ret As Double = 0.0
            For i As Integer = 0 To Me.dimension - 1
                ret += ai_var(i) ^ 2 - A * Math.Cos(2.0 * Math.PI * ai_var(i))
            Next
            ret += A * Me.dimension

            Return ret
        End Function

        Public Overloads Function F(ByVal ai_var() As Double) As Double
            Return Me.F(New List(Of Double)(ai_var))
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return Me.dimension
        End Function
    End Class

End Namespace
