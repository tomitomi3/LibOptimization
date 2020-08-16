Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' x1^3 + x2^3 - 0*x1*x2 + 27
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <Serializable>
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

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class

    ''' <summary>
    ''' x1^4 - 20*x1^2 + 20*x1 + x2^4 - 20*x2^2 + 20*x2
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class clsBenchTest2 : Inherits absObjectiveFunction
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

            Dim ret As Double = ai_var(0) ^ 4 - 20 * ai_var(0) ^ 2 + 20 * ai_var(0) + ai_var(1) ^ 4 - 20 * ai_var(1) ^ 2 + 20 * ai_var(1)
            Return ret
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Dim ret As New List(Of Double)
            ret.Add(4 * ai_var(0) ^ 3 - 40 * ai_var(0) + 20)
            ret.Add(4 * ai_var(1) ^ 3 - 40 * ai_var(1) + 20)
            Return ret
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Return Nothing
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 2
        End Function
    End Class
End Namespace
