Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

''' <summary>
''' unit test for my Linear Algebra  lib
''' </summary>
<TestClass()> Public Class UnitTestLinearAlgebra
    ''' <summary>
    ''' test Create vector
    ''' </summary>
    <TestMethod()> Public Sub CreateVector()
        With Nothing
            Dim v As New clsEasyVector(3)
            For i As Integer = 0 To 3 - 1
                Assert.AreEqual(v(i), CType(0, Double))
            Next
        End With

        With Nothing
            Dim v As New clsEasyVector(New Double() {1, 2, 3})
            For i As Integer = 0 To 3 - 1
                Assert.AreEqual(v(i), CType(i + 1, Double))
            Next
        End With
    End Sub

    ''' <summary>
    ''' test Create Matrix
    ''' </summary>
    <TestMethod()> Public Sub CreateMatrix()
        With Nothing
            Dim mat = New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})

            If mat.RowCount <> 3 Then
                Assert.Fail("Row Error")
            End If

            If mat.ColCount <> 3 Then
                Assert.Fail("Col Error")
            End If

            For i As Integer = 0 To 3 - 1
                For j As Integer = 0 To 3 - 1
                    If i = j Then
                        Assert.AreEqual(mat(i)(j), 1.0)
                    Else
                        Assert.AreEqual(mat(i)(j), 0.0)
                    End If
                Next
            Next
        End With

        With Nothing
            Dim mat = New MathUtil.clsEasyMatrix(3, True)

            If mat.RowCount <> 3 Then
                Assert.Fail("Row Error")
            End If

            If mat.ColCount <> 3 Then
                Assert.Fail("Col Error")
            End If

            For i As Integer = 0 To 3 - 1
                For j As Integer = 0 To 3 - 1
                    If i = j Then
                        Assert.AreEqual(mat(i)(j), 1.0)
                    Else
                        Assert.AreEqual(mat(i)(j), 0.0)
                    End If
                Next
            Next
        End With
    End Sub

    ''' <summary>
    ''' Vector + Matrix
    ''' </summary>
    <TestMethod()> Public Sub Add_VectorMatrix()
        '------------------------
        'OK
        '------------------------
        With Nothing
            Dim v As New clsEasyVector(New Double() {1, 1, 1})
            Dim matV As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
            Try
                Dim result = v + matV
                If result(0) = 2.0 AndAlso result(1) = 3.0 AndAlso result(2) = 4.0 Then
                    'OK
                Else
                    Assert.Fail()
                End If
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        '------------------------
        'Bad
        '------------------------
        With Nothing
            Dim v As New clsEasyVector(New Double() {1, 1, 1})
            Dim matV As New clsEasyMatrix(New Double()() {New Double() {1, 2, 3}})
            Try
                v.PrintValue(name:="v")
                matV.PrintValue(name:="matV")

                v.Direction = clsEasyVector.VectorDirection.COL
                Dim result = v + matV

                'error
                Assert.Fail()
            Catch ex As Exception
                'OK
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Matrix + Matrix
    ''' </summary>
    <TestMethod()> Public Sub Add_MatrixMatrix()
        With Nothing
            Try
                Dim v1 = New clsEasyMatrix(4, True)
                Dim v2 = New clsEasyMatrix(5, True)
                Dim temp = v1 + v2

                Assert.Fail()
            Catch ex As Exception
                'OK
            End Try
        End With

        With Nothing
            Try
                Dim v1 = New clsEasyMatrix(5, True)
                Dim v2 = New clsEasyMatrix(5, True)
                Dim temp = v1 + v2
                Dim diag = temp.ToVectorFromDiagonal()
                For Each val As Double In diag
                    Assert.AreEqual(val, 2.0)
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Matrix + vector
    ''' </summary>
    <TestMethod()> Public Sub Add_MatrixVector()
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
    <TestMethod()> Public Sub Sub_VectorMatrix()
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
    <TestMethod()> Public Sub Sub_MatrixVector()
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
    ''' Matrix - Matrix
    ''' </summary>
    <TestMethod()> Public Sub Sub_MatrixMatrix()
        With Nothing
            Try
                Dim v1 = New clsEasyMatrix(4, True)
                Dim v2 = New clsEasyMatrix(5, True)
                Dim temp = v1 - v2

                Assert.Fail()
            Catch ex As Exception
                'OK
            End Try
        End With

        With Nothing
            Try
                Dim v1 = New clsEasyMatrix(5, True)
                Dim v2 = New clsEasyMatrix(5, True)
                Dim temp = v1 - v2
                Dim diag = temp.ToVectorFromDiagonal()
                For Each val As Double In diag
                    Assert.AreEqual(val, 0.0)
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Matrix x Vector
    ''' </summary>
    <TestMethod()> Public Sub Product_MatrixVector()
        '----------------
        'bad
        '----------------
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {0, 1},
                                                  New Double() {0, 0}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        '----------------
        'OK
        '----------------
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.COL)
            Try
                mat.PrintValue()
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 2.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 2, 3}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.COL)
            Try
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 12 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {1, 0},
                                                  New Double() {1, 0}})
            Dim v As New clsEasyVector(New Double() {2, 2}, clsEasyVector.VectorDirection.COL)
            Try
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 2.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Determinant2Order
    ''' </summary>
    <TestMethod()> Public Sub Determinant2Order()
        Dim detMat As New clsEasyMatrix(New Double()() {New Double() {1, 0},
                                                        New Double() {0, 1}})
        detMat.PrintValue()
        Dim d As Double = 0
        d = detMat.Det()
        Console.WriteLine("Determinant:{0}", d)
        If d <> 1 Then
            Assert.Fail()
        End If

        'swap col -> sign *-1
        detMat.SwapCol(0, 1)
        detMat.PrintValue()
        d = detMat.Det()
        Console.WriteLine("Determinant:{0}", d)
        If d <> -1 Then
            Assert.Fail()
        End If
    End Sub

    ''' <summary>
    ''' Determinant3Order
    ''' </summary>
    <TestMethod()> Public Sub Determinant3Order()
        Dim detMat As New clsEasyMatrix(New Double()() {New Double() {1, 0, 0},
                                                        New Double() {0, 1, 0},
                                                        New Double() {0, 0, 1}})
        detMat.PrintValue()
        Dim d As Double = 0
        d = detMat.Det()
        Console.WriteLine("Determinant:{0}", d)
        If d <> 1 Then
            Assert.Fail()
        End If

        'swap col -> sign *-1
        detMat.SwapCol(0, 1)
        detMat.PrintValue()
        d = detMat.Det()
        Console.WriteLine("Determinant:{0}", d)
        If d <> -1 Then
            Assert.Fail()
        End If
    End Sub

    ''' <summary>
    ''' Determinant4Order
    ''' </summary>
    <TestMethod()> Public Sub Determinant4Order()
        Dim detMat As New clsEasyMatrix(New Double()() {New Double() {1, 0, 0, 0},
                                                        New Double() {0, 1, 0, 0},
                                                        New Double() {0, 0, 1, 0},
                                                        New Double() {0, 0, 0, 1}})
        detMat.PrintValue()
        Dim d As Double = 0
        d = detMat.Det()
        Console.WriteLine("Determinant:{0}", d)
        If d <> 1 Then
            Assert.Fail()
        End If

        'swap col -> sign *-1
        detMat.SwapCol(0, 1)
        detMat.PrintValue()
        d = detMat.Det()
        Console.WriteLine("Determinant:{0}", d)
        If d <> -1 Then
            Assert.Fail()
        End If
    End Sub

    ''' <summary>
    ''' Vector * Matrix
    ''' </summary>
    <TestMethod()> Public Sub Product_VectorMatrix()
        '----------------
        'bad
        '----------------
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = v * mat
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {0, 1},
                                                  New Double() {0, 0}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = v * mat
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Try
                Dim temp = v * mat
            Catch myex As clsException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        '----------------
        'OK
        '----------------
        With Nothing
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Try
                Dim temp = v * mat

                'size check
                If temp.Count <> mat.ColCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction <> clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 2.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1},
                                                  New Double() {1},
                                                  New Double() {1}})

            Try
                Dim temp = v * mat

                'size check
                If temp.Count <> mat.ColCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction <> clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 6.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim v As New clsEasyVector(New Double() {2, 2, 2}, clsEasyVector.VectorDirection.ROW)
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {1, 1},
                                                  New Double() {1, 1},
                                                  New Double() {1, 1}})
            Try
                Dim temp = v * mat

                'size check
                If temp.Count <> mat.ColCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction <> clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 6.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim v As New clsEasyVector(New Double() {2, 2}, clsEasyVector.VectorDirection.ROW)
            Dim mat As New MathUtil.clsEasyMatrix(New Double()() {
                                                  New Double() {2, 2, 2},
                                                  New Double() {0, 0, 0}})
            Try
                Dim temp = v * mat

                'size check
                If temp.Count <> mat.ColCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction <> clsEasyVector.VectorDirection.ROW Then
                    Assert.Fail()
                End If

                'value check
                For Each val As Double In temp
                    If val <> 4.0 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Vector * Scalar
    ''' </summary>
    <TestMethod()> Public Sub Product_VectorScalar()
        With Nothing
            Dim v = New clsEasyVector(3)
            Try
                For i As Integer = 0 To v.Count - 1
                    v(i) = 1
                Next

                Dim temp = v * 2

                For Each val As Double In v
                    If val <> 2 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception

            End Try
        End With
    End Sub

    ''' <summary>
    ''' Scalar * Vector
    ''' </summary>
    <TestMethod()> Public Sub Product_ScalarVector()
        With Nothing
            Dim v = New clsEasyVector(3)
            Try
                For i As Integer = 0 To v.Count - 1
                    v(i) = 1
                Next

                Dim temp = v * 2

                For Each val As Double In v
                    If val <> 2 Then
                        Assert.Fail()
                    End If
                Next
            Catch ex As Exception

            End Try
        End With
    End Sub

    ''' <summary>
    ''' Matrix x scalar
    ''' </summary>
    <TestMethod()> Public Sub Product_MatrixScalar()
        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
            Try
                For i As Integer = 0 To matA.RowCount - 1
                    For j As Integer = 0 To matA.ColCount - 1
                        matA(i)(j) = 1
                    Next
                Next
                matA.PrintValue()

                Dim temp = matA * 2.0
                temp.PrintValue()

                For i As Integer = 0 To matA.RowCount - 1
                    For j As Integer = 0 To matA.ColCount - 1
                        If temp(i)(j) <> 2.0 Then
                            Assert.Fail()
                        End If
                    Next
                Next
            Catch myex As clsException
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Scalar x Matrix
    ''' </summary>
    <TestMethod()> Public Sub Product_ScalarMatrix()
        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
            Try
                For i As Integer = 0 To matA.RowCount - 1
                    For j As Integer = 0 To matA.ColCount - 1
                        matA(i)(j) = 1
                    Next
                Next
                matA.PrintValue()

                Dim temp = 2.0 * matA
                temp.PrintValue()

                For i As Integer = 0 To matA.RowCount - 1
                    For j As Integer = 0 To matA.ColCount - 1
                        If temp(i)(j) <> 2.0 Then
                            Assert.Fail()
                        End If
                    Next
                Next
            Catch myex As clsException
                Assert.Fail()
            End Try
        End With
    End Sub


    ''' <summary>
    ''' test Matrix x Matrix
    ''' </summary>
    <TestMethod()> Public Sub Product_MatrixMatrix()
        '---------
        'bad
        '---------
        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
            Dim matB = New clsEasyMatrix(dimNum - 1, True)
            Try
                Dim temp = matA * matB

                Assert.Fail()
            Catch myex As clsException
                'OK
            End Try
        End With

        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum - 1)
            Dim matB = New clsEasyMatrix(dimNum, True)
            Try
                Dim temp = matA * matB

                Assert.Fail()
            Catch myex As clsException
                'OK
            End Try
        End With

        With Nothing
            Dim matA As New clsEasyMatrix(3, True)
            Dim matB As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1},
                                                     New Double() {2, 2, 2}})
            Try
                Dim temp = matA * matB

                Assert.Fail()
            Catch myex As clsException
                'OK
            End Try
        End With

        '---------
        'OK
        '---------
        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
            Dim matB = New clsEasyMatrix(dimNum, True)
            Try
                Dim temp = matA * matB
                temp.PrintValue()

                If clsMathUtil.IsNearyEqualMatrix(temp, matA) = False Then
                    Assert.Fail()
                End If
            Catch myex As clsException
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
            Dim matB = New clsEasyMatrix(dimNum, False)
            Try
                Dim temp = matA * matB
                temp.PrintValue()

                If clsMathUtil.IsNearyEqualMatrix(temp, matB) = False Then
                    Assert.Fail()
                End If
            Catch myex As clsException
                Assert.Fail()
            End Try
        End With

        With Nothing
            Dim matA As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1},
                                                     New Double() {2, 2, 2}})
            Dim matB As New clsEasyMatrix(3, True)
            Try
                Dim temp = matA * matB
                temp.PrintValue()

                If temp.RowCount <> 2 Then
                    Assert.Fail("row error")
                End If

                If temp.ColCount <> 3 Then
                    Assert.Fail("col error")
                End If

                'check value
                For i As Integer = 0 To temp.RowCount - 1
                    For j As Integer = 0 To temp.ColCount - 1
                        If i = 0 Then
                            If temp(i)(j) <> 1 Then
                                Assert.Fail()
                            End If
                        Else
                            If temp(i)(j) <> 2 Then
                                Assert.Fail()
                            End If
                        End If
                    Next
                Next
            Catch myex As clsException
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' test Inverse 1x1
    ''' </summary>
    <TestMethod()> Public Sub InverseMatrix_1x1()
        Dim dimNum = 1
        Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, 123456)
        source.PrintValue(name:="Source matrix")
        source.Inverse().PrintValue(name:="Inverse matrix")

        Dim product = source * source.Inverse()
        product.PrintValue(name:="S*S^-1")

        'check
        If clsMathUtil.IsNearyEqualMatrix(product, New clsEasyMatrix(dimNum, True)) = True Then
            'OK
        Else
            Assert.Fail()
        End If
    End Sub

    ''' <summary>
    ''' test Inverse 2x2
    ''' </summary>
    <TestMethod()> Public Sub InverseMatrix_2x2()
        Dim mat As New clsEasyMatrix(New Double()() {New Double() {3, 1}, New Double() {-2, 5}})
        Dim matInv As clsEasyMatrix = mat.Inverse()

        'check Identy matrix
        ' I = A * A^-1
        Dim productMat = mat * matInv
        'check
        If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(3, True)) = True Then
            'OK
        Else
            Assert.Fail("before swap col")
        End If

        mat.SwapCol(0, 1)
        matInv = mat.Inverse()

        'check Identy matrix
        ' I = A * A^-1
        productMat = mat * matInv
        'check
        If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(3, True)) = True Then
            'OK
        Else
            Assert.Fail("after swap col")
        End If
    End Sub

    ''' <summary>
    ''' test Inverse 3x3
    ''' </summary>
    <TestMethod()> Public Sub InverseMatrix_3x3()
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
        'check
        If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(3, True)) = True Then
            'OK
        Else
            Assert.Fail("before swap col")
        End If

        mat.SwapCol(0, 1)
        matInv = mat.Inverse()

        'check Identy matrix
        ' I = A * A^-1
        productMat = mat * matInv
        'check
        If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(3, True)) = True Then
            'OK
        Else
            Assert.Fail("after swap col")
        End If
    End Sub

    ''' <summary>
    ''' test Inverse 4x4 - 10x10
    ''' </summary>
    <TestMethod()> Public Sub InverseMatrix_4to10()
        For dimNum As Integer = 4 To 10
            Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, 11111)
            source.PrintValue(name:="Source matrix")
            source.Inverse().PrintValue(name:="Inverse matrix")

            Dim product = source * source.Inverse()
            product.PrintValue(name:="S*S^-1")

            'check
            If clsMathUtil.IsNearyEqualMatrix(product, New clsEasyMatrix(dimNum, True)) = True Then
                'OK
            Else
                Assert.Fail("Dim = {0}", dimNum)
            End If
        Next
    End Sub

    ''' <summary>
    ''' test Cholesky decomposition
    ''' </summary>
    <TestMethod()> Public Sub Cholesky()
        Dim tempMat = clsMathUtil.CreateRandomSymmetricMatrix(5, 123456)
        tempMat.PrintValue(name:="Source")

        Dim c = tempMat.Cholesky()
        tempMat.PrintValue(name:="Cholesky decomposition")

        'A = LL^T
        Dim check = c * c.T()

        If clsMathUtil.IsNearyEqualMatrix(tempMat, check) = False Then
            Assert.Fail()
        End If
    End Sub

    ''' <summary>
    ''' test Eigen decomposition
    ''' </summary>
    <TestMethod()> Public Sub Eigen()
        Dim tempMat = clsMathUtil.CreateRandomSymmetricMatrix(5, 123456) '8, 12345 is not calc?
        tempMat.PrintValue(name:="Source")

        Dim retM As clsEasyMatrix = Nothing
        Dim retV As clsEasyVector = Nothing
        Dim suspend As clsEasyMatrix = Nothing
        clsEasyMatrix.Eigen(tempMat, retV, retM, Iteration:=10000)
        Dim retD = retV.ToDiagonalMatrix()

        retM.PrintValue(name:="Eigen V")
        retD.PrintValue(name:="D")
        retM.T().PrintValue(name:="Eigen V^T")
        retM.Inverse().PrintValue(name:="Eigen V^-1")

        'check
        Dim temp As clsEasyMatrix = Nothing
        temp = retM * retV.ToDiagonalMatrix() * retM.T()
        temp.PrintValue(name:="V*D*V^T")
        If clsMathUtil.IsNearyEqualMatrix(tempMat, temp) = False Then
            Assert.Fail()
        End If

        'check
        temp = retM * retV.ToDiagonalMatrix() * retM.Inverse()
        temp.PrintValue(name:="V*D*V^-1")
        If clsMathUtil.IsNearyEqualMatrix(tempMat, temp) = False Then
            Assert.Fail()
        End If
    End Sub

    ''' <summary>
    ''' test Resize()
    ''' </summary>
    <TestMethod()> Public Sub ResizeVectorMatrix()
        Dim tempVec = New clsEasyVector(3)
        tempVec.Resize(0)
        Assert.AreEqual(tempVec.Count, 0)
        tempVec.Resize(1)
        Assert.AreEqual(tempVec.Count, 1)

        Dim tempMat = New clsEasyMatrix(3)
        tempMat.Resize(0, 0)
        Assert.AreEqual(tempMat.RowCount, 0)
        Assert.AreEqual(tempMat.ColCount, 0)
        tempMat.Resize(1, 2)
        Assert.AreEqual(tempMat.RowCount, 1)
        Assert.AreEqual(tempMat.ColCount, 2)
    End Sub
End Class