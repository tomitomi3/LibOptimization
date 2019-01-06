Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

''' <summary>
''' 単体テスト 最適化、線形代数以外
''' </summary>
<TestClass()> Public Class UnitTestOther
#Region "Util"
    ''' <summary>
    ''' check random lib
    ''' </summary>
    <TestMethod()> Public Sub Other_Random()
        Dim rand As System.Random = Nothing
        Try
            rand = New clsRandomXorshift()
        Catch ex As Exception
            Assert.Fail("clsRandomXorshift error")
        End Try

        Try
            Dim temp As Integer = 123456
            rand = New clsRandomXorshift(BitConverter.ToUInt32(BitConverter.GetBytes(temp), 0))
        Catch ex As Exception
            Assert.Fail("clsRandomXorshift seed error using positive value.")
        End Try

        Try
            Dim temp As Integer = -123456
            rand = New clsRandomXorshift(BitConverter.ToUInt32(BitConverter.GetBytes(temp), 0))
        Catch ex As Exception
            Assert.Fail("clsRandomXorshift seed error using negative value.")
        End Try
    End Sub
#End Region
End Class