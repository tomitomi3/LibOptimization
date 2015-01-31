Imports LibOptimization.Optimization

''' <summary>
''' </summary>
''' <remarks>
''' </remarks>
Public Class clsBenchTest : Inherits absObjectiveFunction
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

        Dim ret As Double = ai_var(0) ^ 3 + ai_var(1) ^ 3 - 9 * ai_var(0) * ai_var(1) + 27
        Return ret
    End Function

    Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
        Dim ret As New List(Of Double)
        ret.Add(3 * ai_var(0) ^ 2 - 9 * ai_var(1))
        ret.Add(3 * ai_var(1) ^ 2 - 9 * ai_var(0))
        Return ret
    End Function

    Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
        Dim ret As New List(Of List(Of Double))

        ret.Add(New List(Of Double))
        ret(0).Add(6 * ai_var(0))
        ret(0).Add(0)
        ret.Add(New List(Of Double))
        ret(1).Add(0)
        ret(1).Add(6 * ai_var(1))

        Return ret
    End Function

    Public Overrides ReadOnly Property NumberOfVariable As Integer
        Get
            Return 2
        End Get
    End Property
End Class
