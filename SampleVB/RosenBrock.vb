''' <summary>
''' my objective function : inherit objective function
''' </summary>
''' <remarks></remarks>
Public Class RosenBrock : Inherits LibOptimization.Optimization.absObjectiveFunction
    ''' <summary>
    ''' for Rosen block function(Bench mark function)
    ''' </summary>
    ''' <param name="a"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function F(ByVal a As List(Of Double)) As Double
        If a Is Nothing Then
            Return 0.0
        End If
        Dim x1 = a(0)
        Dim x2 = a(1)
        Return 100 * (x2 - x1 * x1) * (x2 - x1 * x1) + (1 - x1) * (1 - x1)
    End Function

    Public Overrides Function Gradient(x As List(Of Double)) As List(Of Double)
        Return Nothing
    End Function

    Public Overrides Function Hessian(x As List(Of Double)) As List(Of List(Of Double))
        Return Nothing
    End Function

    Public Overrides Function NumberOfVariable() As Integer
        Return 2
    End Function
End Class
