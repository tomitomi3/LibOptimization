Imports LibOptimization.Optimization

Namespace BenchmarkFunction
    ''' <summary>
    ''' Benchmark function
    ''' De Jong’s function 4 (qudratic with gauss Function)
    ''' </summary>
    ''' <remarks>
    ''' Minimum:
    '''  x = {0, ..., 0}
    ''' Range
    '''  -1.28 ~ 1.28
    ''' Refference:
    '''  De Jong, K. A., "Analysis of the Behavior of a Class of Genetic Adaptive Systems", PhD dissertation, The University of Michigan, Computer and Communication Sciences Department (1975)
    ''' </remarks>
    <Serializable>
    Public Class clsBenchDeJongFunction4 : Inherits absObjectiveFunction
        Private normRand As New List(Of Double)

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            For i As Integer = 0 To 29
                Me.normRand.Add(Util.clsUtil.NormRand())
            Next
        End Sub

        ''' <summary>
        ''' Target Function
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function F(ByVal x As List(Of Double)) As Double
            If x Is Nothing Then
                Return 0
            End If

            Dim ret As Double = 0
            For i As Integer = 0 To 29
                If (x(i) >= -1.28) AndAlso (x(i) <= 1.28) Then
                    ret += (i + 1) * x(i) * x(i) * x(i) * x(i) + Me.normRand(i)
                Else
                    'out of range
                    ret += Math.Abs(i) 'penarty
                End If
            Next

            Return ret '??? 
        End Function

        Public Overrides Function Gradient(ByVal ai_var As List(Of Double)) As List(Of Double)
            Throw New NotImplementedException
        End Function

        Public Overrides Function Hessian(ByVal ai_var As List(Of Double)) As List(Of List(Of Double))
            Throw New NotImplementedException
        End Function

        Public Overrides Function NumberOfVariable() As Integer
            Return 30
        End Function
    End Class

End Namespace
