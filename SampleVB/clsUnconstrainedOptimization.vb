''' <summary>
''' Unconstrained optimization
''' </summary>
''' <remarks></remarks>
Public Class clsUnconstrainedOptimization : Inherits LibOptimization.Optimization.absObjectiveFunction

    ''' <summary>
    ''' polynomial function
    ''' </summary>
    ''' <param name="a"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function F(ByVal a As List(Of Double)) As Double
        Dim eval As Double = 10 - 10 * a(0) + 0.5 * a(0) ^ 2 + a(0) ^ 3 + 0.1 * a(0) ^ 4
        Return eval
    End Function

    Public Overrides Function Gradient(x As List(Of Double)) As List(Of Double)
        Return Nothing
    End Function

    Public Overrides Function Hessian(x As List(Of Double)) As List(Of List(Of Double))
        Return Nothing
    End Function

    Public Overrides ReadOnly Property NumberOfVariable As Integer
        Get
            Return 1
        End Get
    End Property
End Class