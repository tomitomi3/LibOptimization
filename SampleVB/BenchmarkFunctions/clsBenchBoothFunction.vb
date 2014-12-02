''' <summary>
''' Benchmark function
''' Booth Function
''' </summary>
''' <remarks>
''' Minimum:
'''  x = {1,3}
''' 
''' Referrence:
''' http://www-optima.amp.i.kyoto-u.ac.jp/member/student/hedar/Hedar_files/TestGO_files/Page816.htm
''' </remarks>
Public Class clsBenchBoothFunction : Inherits LibOptimization.absObjectiveFunction
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

        Return (ai_var(0) + 2 * ai_var(1) - 7) ^ 2 + (2 * ai_var(0) + ai_var(1) - 5) ^ 2
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
