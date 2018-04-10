Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

''' <summary>
''' 単体テスト 線形代数
''' </summary>
<TestClass()> Public Class UnitTestLinearAlgebra
#Region "Vector, Matrix"
    ''' <summary>
    ''' check Matric initialize
    ''' </summary>
    <TestMethod()> Public Sub TestVector()
        Dim v As New clsEasyVector(New Double() {1, 2, 3})
        For i As Integer = 0 To 3 - 1
            Assert.AreEqual(v(i), CType(i + 1, Double))
        Next
    End Sub

    ''' <summary>
    ''' Vector + Matrix
    ''' </summary>
    <TestMethod()> Public Sub TestVectorMatrix_AddVectorMatrix()
        Dim v As New clsEasyVector(New Double() {1, 1, 1})
        Dim matV As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
        Try
            v = v + matV
            If v(0) = 2.0 AndAlso v(1) = 3.0 AndAlso v(2) = 4.0 Then
                'OK
            Else
                Assert.Fail()
            End If
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    ''' <summary>
    ''' Matrix + vector
    ''' </summary>
    <TestMethod()> Public Sub TestVectorMatrix_AddMatrixVector()
        Dim v As New clsEasyVector(New Double() {1, 1, 1})
        Dim matV As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
        Try
            v = matV + v
            If v(0) = 2.0 AndAlso v(1) = 3.0 AndAlso v(2) = 4.0 Then
                'OK
            Else
                Assert.Fail()
            End If
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    ''' <summary>
    ''' Vector - Matrix
    ''' </summary>
    <TestMethod()> Public Sub TestVectorMatrix_SubVectorMatrix()
        Dim v As New clsEasyVector(New Double() {1, 1, 1})
        Dim matV As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
        Try
            v = v - matV
            If v(0) = 0 AndAlso v(1) = -1.0 AndAlso v(2) = -2.0 Then
                'OK
            Else
                Assert.Fail()
            End If
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    ''' <summary>
    ''' Matrix - Vector
    ''' </summary>
    <TestMethod()> Public Sub TestVectorMatrix_SubMatrixVector()
        Dim v As New clsEasyVector(New Double() {1, 1, 1})
        Dim matV As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
        Try
            v = matV - v
            If v(0) = 0.0 AndAlso v(1) = 1.0 AndAlso v(2) = 2.0 Then
                'OK
            Else
                Assert.Fail()
            End If
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    ''' <summary>
    ''' Matrix * Vector
    ''' </summary>
    <TestMethod()> Public Sub TestVectorMatrix_ProductMatrixVector()
        Dim v As New clsEasyVector(New Double() {1, 2, 3})
        Dim mat As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {2, 1, 1}, New Double() {3, 1, 1}})
        Try
            v.Direction = clsEasyVector.VectorDirection.COL
            Dim temp = mat * v
            temp.PrintValue()
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    ''' <summary>
    ''' Vector * Matrix
    ''' </summary>
    <TestMethod()> Public Sub TestVectorMatrix_ProductVectorMatrix_OK()
        Dim v As New clsEasyVector(New Double() {1, 2, 3})
        Dim mat As New clsEasyMatrix(New Double()() {New Double() {4, 5, 6}})
        Try
            v.Direction = clsEasyVector.VectorDirection.COL
            Dim temp = v * mat
            Dim temp2 = mat * v
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    <TestMethod()> Public Sub TestVectorMatrix_ProductMatrixVectorFail()
        Dim v As New clsEasyVector(New Double() {1, 2, 3})
        Dim mat As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {2, 1, 1}, New Double() {3, 1, 1}})
        Try
            v.Direction = clsEasyVector.VectorDirection.ROW
            Dim temp = mat * v
            Assert.Fail()
        Catch ex As Exception
            '例外が投げられるのが正解
        End Try
    End Sub

    <TestMethod()> Public Sub TestMatrix()
        Dim matV As New clsEasyMatrix(New Double()() {New Double() {1, 2, 3}, New Double() {4, 5, 6}, New Double() {7, 8, 9}})
        Dim c As Integer = 1
        For i As Integer = 0 To matV.RowCount - 1
            For j As Integer = 0 To matV.ColCount - 1
                Assert.AreEqual(matV(i)(j), CType(c, Double))
                c += 1
            Next
        Next
    End Sub

    <TestMethod()> Public Sub TestMatrixInverse()
        Dim mat As New clsEasyMatrix(New Double()() {New Double() {3, 1, 1},
                                                     New Double() {5, 1, 3},
                                                     New Double() {2, 0, 1}})
        'Inverse
        '0.5	-0.5	1
        '0.5	 0.5	-2
        '-1      1  	-1
        Dim matInv As clsEasyMatrix = mat.Inverse()

        'check Identy matrix
        ' I = A * A^-1
        Dim productMat = mat * matInv
        For i As Integer = 0 To productMat.RowCount - 1
            For j As Integer = 0 To productMat.ColCount - 1
                If i = j Then
                    If productMat(i)(j) < 0.9999 OrElse productMat(i)(j) > 1.0001 Then
                        Assert.Fail()
                    End If
                Else
                    If productMat(i)(j) < -0.0001 OrElse productMat(i)(j) > 0.0001 Then
                        Assert.Fail()
                    End If
                End If
            Next
        Next
    End Sub

    <TestMethod()> Public Sub TestVectorMatrix()
        Dim v As New clsEasyVector(New Double() {3, 2, 1})
        Dim mat As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})

        Dim temp = v + mat
        For i As Integer = 0 To temp.Count - 1
            Assert.AreEqual(temp(i), CType(4, Double), "v + mat")
        Next

        temp = mat + v
        For i As Integer = 0 To temp.Count - 1
            Assert.AreEqual(temp(i), CType(4, Double), "mat + v")
        Next

        temp = v - mat
        Assert.AreEqual(temp(0), CType(2, Double), "v - mat")
        Assert.AreEqual(temp(1), CType(0, Double), "v - mat")
        Assert.AreEqual(temp(2), CType(-2, Double), "v - mat")

        temp = mat - v
        Assert.AreEqual(temp(0), CType(-2, Double), "mat - v")
        Assert.AreEqual(temp(1), CType(0, Double), "mat - v")
        Assert.AreEqual(temp(2), CType(2, Double), "mat - v")
    End Sub
#End Region
End Class