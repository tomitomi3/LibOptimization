Imports LibOptimization
Imports CsvHelper

''' <summary>
''' 最小二乗法（最小自乗法）
''' </summary>
''' <remarks></remarks>
Public Class clsLeastSquaresMethod : Inherits LibOptimization.Optimization.absObjectiveFunction
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
    ''' <param name="ai_path"></param>
    ''' <remarks></remarks>
    Public Function Init(ByVal ai_path As String) As Boolean
        If System.IO.File.Exists(ai_path) = False Then
            Return False
        End If

        Try
            Using r = New IO.StreamReader(ai_path, Text.Encoding.GetEncoding("SHIFT_JIS"))
                Using csv = New CsvHelper.CsvReader(r)
                    csv.Configuration.HasHeaderRecord = True
                    While csv.Read()
                        datas.Add(New List(Of Double)({csv.CurrentRecord.ElementAt(0), csv.CurrentRecord.ElementAt(1)}))
                    End While
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try

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

    Public Overrides ReadOnly Property NumberOfVariable As Integer
        Get
            Return 5
        End Get
    End Property
#End Region
End Class



