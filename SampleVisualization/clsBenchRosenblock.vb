''' <summary>
''' Benchmark function
''' Rosenblock function(Banana function)
''' </summary>
''' <remarks>
''' Features:
'''  -Famous benchmark function.
''' 
''' Minimum:
'''  x = {0,...,0}
''' </remarks>
Public Class clsBenchRosenblock : Inherits LibOptimization.absObjectiveFunction
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
        For i As Integer = 0 To Me.dimension - 2
            ret += 100 * (ai_var(i + 1) - ai_var(i) ^ 2) ^ 2 + (ai_var(i) - 1) ^ 2
        Next

        Return ret
    End Function

    Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
        Return Nothing
    End Function

    Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
        Return Nothing
    End Function

    Public Overrides ReadOnly Property NumberOfVariable As Integer
        Get
            Return Me.dimension
        End Get
    End Property
End Class
