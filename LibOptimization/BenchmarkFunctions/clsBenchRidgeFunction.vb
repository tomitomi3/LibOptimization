Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' Ridge Function
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  x = {0,...,0}
    ''' 
    ''' Referrence:
    ''' http://mikilab.doshisha.ac.jp/dia/research/pdga/archive/doc/ga2k_performance.pdf
    ''' </remarks>
    Public Class clsBenchRidgeFunction : Inherits absObjectiveFunction
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
            For i As Integer = 0 To Me.dimension - 1
                ret += ai_var(i)
            Next
            ret = ret ^ 2
            'Sum
            For i As Integer = 0 To 4
                ret += ret
            Next
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

        Public Overrides ReadOnly Property NumberOfVariable As Integer
            Get
                Return Me.dimension
            End Get
        End Property
    End Class

End Namespace
