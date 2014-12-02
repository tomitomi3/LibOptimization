''' <summary>
''' Benchmark function
''' Easom function
''' </summary>
''' <remarks>
''' Minimum:
'''  x = {pi, pi}
'''  f(x) = -1
''' </remarks>
Public Class clsBenchEasom : Inherits LibOptimization.absObjectiveFunction
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
        Dim ret As Double = -Math.Cos(x1) * Math.Cos(x2) * Math.Exp(-((x1 - Math.PI) ^ 2 + (x2 - Math.PI) ^ 2))
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
            Return 2
        End Get
    End Property
End Class
