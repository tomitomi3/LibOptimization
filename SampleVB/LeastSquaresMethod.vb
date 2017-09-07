Imports LibOptimization

''' <summary>
''' 最小二乗法（最小自乗法）Least Squares Method
''' </summary>
''' <remarks></remarks>
Public Class LeastSquaresMethod : Inherits Optimization.absObjectiveFunction
    ''' <summary>Data</summary>
    Private datas As New List(Of List(Of Double))

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'nop
    End Sub

    ''' <summary>
    ''' 初期化
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Init() As Boolean
        Me.datas.Add(New List(Of Double)({-20, -1900}))
        Me.datas.Add(New List(Of Double)({-19.8, -1918.02392}))
        Me.datas.Add(New List(Of Double)({-18.8, -1953.08032}))
        Me.datas.Add(New List(Of Double)({-7.2, 62.72128}))
        Me.datas.Add(New List(Of Double)({3.8, -359.10232}))
        Me.datas.Add(New List(Of Double)({7.2, -630.78272}))
        Return True
    End Function

#Region "for LibOptimization"
    ''' <summary>
    ''' Model
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function F(x As List(Of Double)) As Double
        Dim sumDiffSquare As Double = 0

        For Each temp In Me.datas
            Dim predict = x(0) * temp(0) ^ 4 + x(1) * temp(0) ^ 3 + x(2) * temp(0) ^ 2 + x(3) * temp(0) + x(4)
            'Dim predict = x(0) / (1 + x(1) * Math.Exp(-x(2) * DateValue())) 'Logistics
            Dim diffSquare = (temp(1) - predict) ^ 2
            sumDiffSquare += diffSquare
        Next

        Return sumDiffSquare
    End Function

    Public Overrides Function Gradient(x As List(Of Double)) As List(Of Double)
        Throw New NotImplementedException
    End Function

    Public Overrides Function Hessian(x As List(Of Double)) As List(Of List(Of Double))
        Throw New NotImplementedException
    End Function

    Public Overrides Function NumberOfVariable() As Integer
        Return 5
    End Function
#End Region
End Class



