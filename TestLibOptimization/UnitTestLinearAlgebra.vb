Imports System.Text
Imports LibOptimization.MathTool
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class UnitTestLinearAlgebra
    ''' <summary>
    ''' test Create vector
    ''' </summary>
    <TestMethod()> Public Sub Vec_CreateVector()
        With Nothing
            Dim v As New DenseVector(3)
            For i As Integer = 0 To 3 - 1
                Assert.AreEqual(v(i), CType(0, Double))
            Next
        End With

        With Nothing
            Dim v As New DenseVector(New Double() {1, 2, 3})
            For i As Integer = 0 To 3 - 1
                Assert.AreEqual(v(i), CType(i + 1, Double))
            Next
        End With
    End Sub

    ''' <summary>
    ''' test Create Matrix
    ''' </summary>
    <TestMethod()> Public Sub Mat_CreateMatrix()
        With Nothing
            Dim mat = New DenseMatrix(New Double()() {
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
            Dim mat = New DenseMatrix(3, True)

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
    <TestMethod()> Public Sub Mat_Add_VectorMatrix()
        '------------------------
        'OK
        '------------------------
        With Nothing
            Dim v As New DenseVector(New Double() {1, 1, 1})
            Dim matV As New DenseMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
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
            Dim v As New DenseVector(New Double() {1, 1, 1})
            Dim matV As New DenseMatrix(New Double()() {New Double() {1, 2, 3}})
            Try
                v.PrintValue(name:="v")
                matV.PrintValue(name:="matV")

                v.Direction = DenseVector.VectorDirection.COL
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
    <TestMethod()> Public Sub Mat_Mat_Add_MatrixMatrix()
        With Nothing
            Try
                Dim v1 = New DenseMatrix(4, True)
                Dim v2 = New DenseMatrix(5, True)
                Dim temp = v1 + v2

                Assert.Fail()
            Catch ex As Exception
                'OK
            End Try
        End With

        With Nothing
            Try
                Dim v1 = New DenseMatrix(5, True)
                Dim v2 = New DenseMatrix(5, True)
                Dim temp = v1 + v2
                Dim diag = temp.ToDiagonalVector()
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
    <TestMethod()> Public Sub Mat_Mat_Add_MatrixVector()
        Dim v As New DenseVector(New Double() {1, 1, 1})
        Dim matV As New DenseMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
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
    <TestMethod()> Public Sub Mat_Sub_VectorMatrix()
        Dim v As New DenseVector(New Double() {1, 1, 1})
        Dim matV As New DenseMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})
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
    <TestMethod()> Public Sub Mat_Sub_MatrixVector()
        '3x2 ok
        With Nothing
            Dim v As New DenseVector(New Double() {1, 1}, DenseVector.VectorDirection.COL)
            Dim matV As New DenseMatrix(New Double()() {New Double() {1, 1}, New Double() {1, 1}, New Double() {1, 1}})
            Dim ret = matV - v
            For Each vec In ret
                For Each value In vec
                    If value = 0.0 Then
                        'nop
                    Else
                        v.PrintValue()
                        matV.PrintValue()
                        Assert.Fail()
                    End If
                Next
            Next
        End With

        '3x2 ok
        With Nothing
            Dim v As New DenseVector(New Double() {1, 1}, DenseVector.VectorDirection.ROW)
            Dim matV As New DenseMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {1, 1, 1}})
            Dim ret = matV - v
            For Each vec In ret
                For Each value In vec
                    If value = 0.0 Then
                        'nop
                    Else
                        v.PrintValue()
                        matV.PrintValue()
                        Assert.Fail()
                    End If
                Next
            Next
        End With

        With Nothing
            Dim v As New DenseVector(New Double() {1, 1}, DenseVector.VectorDirection.COL)
            Dim matV As New DenseMatrix(New Double()() {New Double() {1, 1}, New Double() {1, 1}, New Double() {1, 1}})
            Try
                v.PrintValue()
                matV.PrintValue()
                Assert.Fail()
            Catch ex As Exception
                'OK
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Matrix - Matrix
    ''' </summary>
    <TestMethod()> Public Sub Mat_Sub_MatrixMatrix()
        With Nothing
            Try
                Dim v1 = New DenseMatrix(4, True)
                Dim v2 = New DenseMatrix(5, True)
                Dim temp = v1 - v2

                Assert.Fail()
            Catch ex As Exception
                'OK
            End Try
        End With

        With Nothing
            Try
                Dim v1 = New DenseMatrix(5, True)
                Dim v2 = New DenseMatrix(5, True)
                Dim temp = v1 - v2
                Dim diag = temp.ToDiagonalVector()
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
    <TestMethod()> Public Sub Mat_Product_MatrixVector()
        '----------------
        'bad
        '----------------
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New DenseVector(New Double() {2, 2}, DenseVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As MathException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {0, 1},
                                                  New Double() {0, 0}})
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As MathException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Try
                Dim temp = mat * v
            Catch myex As MathException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        '----------------
        'OK
        '----------------
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.COL)
            Try
                mat.PrintValue()
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = DenseVector.VectorDirection.ROW Then
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
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 2, 3}})
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.COL)
            Try
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = DenseVector.VectorDirection.ROW Then
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
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {1, 0},
                                                  New Double() {1, 0}})
            Dim v As New DenseVector(New Double() {2, 2}, DenseVector.VectorDirection.COL)
            Try
                Dim temp = mat * v

                'size check
                If temp.Count <> mat.RowCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction = DenseVector.VectorDirection.ROW Then
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
    ''' Determinant
    ''' </summary>
    <TestMethod()> Public Sub Mat_Determinant()
        ' 2x2
        With Nothing
            Dim detMat As New DenseMatrix(New Double()() {New Double() {1, 0},
                                                          New Double() {0, 1}})
            detMat.PrintValue()
            Dim d As Double = 0
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)
            If d <> 1 Then
                Assert.Fail()
            End If

            'swap col -> sign *-1
            MathUtil.SwapCol(detMat, 0, 1)
            detMat.PrintValue()
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)
            If d <> -1 Then
                Assert.Fail()
            End If
        End With

        ' 3x3
        With Nothing
            Dim detMat As New DenseMatrix(New Double()() {New Double() {1, 0, 0},
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
            MathUtil.SwapCol(detMat, 0, 1)
            detMat.PrintValue()
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)
            If d <> -1 Then
                Assert.Fail()
            End If
        End With

        ' 4x4
        With Nothing
            'swap
            Dim detMat As New DenseMatrix(New Double()() {New Double() {1, 0, 0, 0},
                                                          New Double() {0, 1, 0, 0},
                                                          New Double() {0, 0, 1, 0},
                                                          New Double() {0, 0, 0, 1}})
            detMat.PrintValue()
            Dim d As Double = 0
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)

            If MathUtil.IsCloseToValues(d, 1.0) = False Then
                Assert.Fail()
            End If

            'swap col -> sign *-1
            MathUtil.SwapCol(detMat, 0, 1)
            detMat.PrintValue()
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)
            If d <> -1 Then
                Assert.Fail()
            End If
        End With
    End Sub

    ''' <summary>
    ''' Vector * Matrix
    ''' </summary>
    <TestMethod()> Public Sub Vec_Product_VectorMatrix()
        '----------------
        'bad
        '----------------
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 1, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New DenseVector(New Double() {2, 2}, DenseVector.VectorDirection.ROW)
            Try
                Dim temp = v * mat
            Catch myex As MathException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0},
                                                  New Double() {0, 1},
                                                  New Double() {0, 0}})
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Try
                Dim temp = v * mat
            Catch myex As MathException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        'bad
        With Nothing
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {1, 0, 0},
                                                  New Double() {0, 0, 1}})
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Try
                Dim temp = v * mat
            Catch myex As MathException
                'OK
                Return
            End Try
            Assert.Fail()
        End With

        '----------------
        'OK
        '----------------
        With Nothing
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Dim mat As New DenseMatrix(v.Count, True)
            Try
                Dim temp = v * mat

                'size check
                If temp.Count <> mat.ColCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction <> DenseVector.VectorDirection.ROW Then
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
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Dim mat As New DenseMatrix(New Double()() {
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
                If temp.Direction <> DenseVector.VectorDirection.ROW Then
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
            Dim v As New DenseVector(New Double() {2, 2, 2}, DenseVector.VectorDirection.ROW)
            Dim mat As New DenseMatrix(New Double()() {
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
                If temp.Direction <> DenseVector.VectorDirection.ROW Then
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
            Dim v As New DenseVector(New Double() {2, 2}, DenseVector.VectorDirection.ROW)
            Dim mat As New DenseMatrix(New Double()() {
                                                  New Double() {2, 2, 2},
                                                  New Double() {0, 0, 0}})
            Try
                Dim temp = v * mat

                'size check
                If temp.Count <> mat.ColCount Then
                    Assert.Fail()
                End If

                'dirction check
                If temp.Direction <> DenseVector.VectorDirection.ROW Then
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
    <TestMethod()> Public Sub Vec_Product_VectorScalar()
        With Nothing
            Dim v = New DenseVector(3)
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
    ''' Vector * Vector
    ''' </summary>
    <TestMethod()> Public Sub Vec_Product_VectorVector()
        Dim vec1 = New DenseVector(New Double() {1, 2, 3}, DenseVector.VectorDirection.ROW)
        Dim vec2 = New DenseVector(New Double() {4, 5, 6}, DenseVector.VectorDirection.COL)

        With Nothing
            '|v1 v2| * |v3|
            '          |v4|
            Dim vec1vec2 = vec1 * vec2
            If vec1vec2.RowCount <> 1 AndAlso vec1vec2.ColCount <> 1 Then
                vec1vec2.PrintValue(name:="vec1vec2")
                Assert.Fail("error : vector * vector")
            End If
            If MathUtil.IsCloseToValues(vec1vec2(0)(0), 32.0) = False Then
                vec1vec2.PrintValue(name:="vec1vec2")
                Assert.Fail("error : vector * vector")
            End If
        End With

        With Nothing
            '|v1| * |v3 v4|
            '|v2|
            Dim vec2vec1 = vec2 * vec1
            If vec2vec1.RowCount <> 3 AndAlso vec2vec1.ColCount <> 3 Then
                vec2vec1.PrintValue(name:="vec2vec1")
                Assert.Fail("error : vector * vector")
            End If

            Dim correctMat As New DenseMatrix(New Double()() {(vec2 * 1.0).ToArray(),
                                                                (vec2 * 2.0).ToArray(),
                                                                (vec2 * 3.0).ToArray()})
            If MathUtil.IsNearyEqualMatrix(vec2vec1, correctMat) = False Then
                vec2vec1.PrintValue(name:="vec2vec1")
                Assert.Fail("error : vector * vector")
            End If
        End With
    End Sub

    ''' <summary>
    ''' Scalar * Vector
    ''' </summary>
    <TestMethod()> Public Sub Mat_Product_ScalarVector()
        With Nothing
            Dim v = New DenseVector(3)
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
    <TestMethod()> Public Sub Mat_Product_MatrixScalar()
        With Nothing
            Dim dimNum = 3
            Dim matA = MathUtil.CreateRandomSymmetricMatrix(dimNum)
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
            Catch myex As MathException
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Scalar x Matrix
    ''' </summary>
    <TestMethod()> Public Sub Mat_Product_ScalarMatrix()
        With Nothing
            Dim dimNum = 3
            Dim matA = MathUtil.CreateRandomSymmetricMatrix(dimNum)
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
            Catch myex As MathException
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' test Matrix x Matrix
    ''' </summary>
    <TestMethod()> Public Sub Mat_Product_MatrixMatrix()
        '---------
        'bad
        '---------
        '3x3 * 2x2
        With Nothing
            Dim dimNum = 3
            Dim matA = MathUtil.CreateRandomSymmetricMatrix(dimNum)
            Dim matB = New DenseMatrix(dimNum - 1, True)
            Try
                Dim temp = matA * matB
                Assert.Fail("error : 3x3 * 2x2")
            Catch myex As MathException
                'OK
            End Try
        End With

        '2x2 * 3x3
        With Nothing
            Dim dimNum = 3
            Dim matA = MathUtil.CreateRandomSymmetricMatrix(dimNum - 1)
            Dim matB = New DenseMatrix(dimNum, True)
            Try
                Dim temp = matA * matB

                Assert.Fail("error : 2x2 * 3x3")
            Catch myex As MathException
                'OK
            End Try
        End With

        '3x3 * 2x3
        With Nothing
            Dim matA As New DenseMatrix(3, True)
            Dim matB As New DenseMatrix(New Double()() {New Double() {1, 1, 1},
                                                     New Double() {2, 2, 2}})
            Try
                Dim temp = matA * matB

                Assert.Fail("error : 3x3 * 2x3")
            Catch myex As MathException
                'OK
            End Try
        End With

        '---------
        'OK
        '---------
        For dimNum = 2 To 10 - 1
            With Nothing
                Dim matA = MathUtil.CreateRandomSymmetricMatrix(dimNum)
                Dim matB = New DenseMatrix(dimNum, True)
                Try
                    Dim temp = matA * matB
                    temp.PrintValue(name:="A*B")

                    If MathUtil.IsNearyEqualMatrix(temp, matA) = False Then
                        Assert.Fail("error : {0}x{0}", dimNum)
                    End If
                Catch myex As MathException
                    Assert.Fail()
                End Try
            End With

            With Nothing
                Dim matA = MathUtil.CreateRandomSymmetricMatrix(dimNum)
                Dim matB = New DenseMatrix(dimNum, False)
                Try
                    Dim temp = matA * matB
                    temp.PrintValue(name:="B(zero)*A")

                    If MathUtil.IsNearyEqualMatrix(temp, matB) = False Then
                        Assert.Fail("error : {0}x{0}", dimNum)
                    End If
                Catch myex As MathException
                    Assert.Fail()
                End Try
            End With
        Next

        With Nothing
            Dim matA As New DenseMatrix(New Double()() {New Double() {1, 1, 1},
                                                     New Double() {2, 2, 2}})
            Dim matB As New DenseMatrix(3, True)
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
                If MathUtil.IsNearyEqualMatrix(temp, matA) = False Then
                    Assert.Fail()
                End If
            Catch myex As MathException
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Transopose
    ''' </summary>
    <TestMethod()> Public Sub Mat_Transopose()
        Dim rng = New Random()
        For i As Integer = 2 To 50 - 1
            Dim dimNum = i
            Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng)

            '転置
            Dim t = source.T()
            t = t.T()

            'check
            If MathUtil.IsNearyEqualMatrix(t, source) = True Then
                'OK
            Else
                source.PrintValue(name:="source")
                t.PrintValue(name:="source^T^T")
                Assert.Fail()
            End If
        Next
    End Sub

    ''' <summary>
    ''' Inverse
    ''' </summary>
    <TestMethod()> Public Sub Mat_Inverse()
        With Nothing
            Dim dimNum = 1
            Dim rng = New Random()
            Dim source = New DenseMatrix(1)
            source(0)(0) = 2
            Dim product = source * source.Inverse()

            'check
            If MathUtil.IsNearyEqualMatrix(product, New DenseMatrix(dimNum, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("Inverse fail 1x1")
            End If
        End With

        With Nothing
            Dim source As New DenseMatrix(New Double()() {New Double() {3, 1}, New Double() {-2, 5}})
            Dim sourceInv As DenseMatrix = source.Inverse()

            'check Identy matrix I = A * A^-1
            Dim productMat = source * sourceInv
            If MathUtil.IsNearyEqualMatrix(productMat, New DenseMatrix(source.RowCount, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("before swap col 2x2")
            End If

            'col swap
            MathUtil.SwapCol(source, 0, 1)
            sourceInv = source.Inverse()
            productMat = source * sourceInv
            'check
            If MathUtil.IsNearyEqualMatrix(productMat, New DenseMatrix(3, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("after swap col 2x2")
            End If
        End With

        With Nothing
            Dim source As New DenseMatrix(New Double()() {New Double() {3, 1, 1},
                                                    New Double() {5, 1, 3},
                                                    New Double() {2, 0, 1}})
            Dim sourceInv As DenseMatrix = source.Inverse()

            'check Identy matrix I = A * A^-1
            Dim productMat = source * sourceInv
            If MathUtil.IsNearyEqualMatrix(productMat, New DenseMatrix(source.RowCount, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("before swap col 3x3")
            End If

            'col swap
            MathUtil.SwapCol(source, 0, 1)
            sourceInv = source.Inverse()
            productMat = source * sourceInv
            'check
            If MathUtil.IsNearyEqualMatrix(productMat, New DenseMatrix(3, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("after swap col 3x3")
            End If
        End With

        With Nothing
            'ピボット選択が必須の場合（対角成分が０になる場合）
            Dim source = New DenseMatrix(New Double()() {
                                     New Double() {1.0, -1.0, -4.0, 8.0},
                                     New Double() {-1.0, 1.0, 6.0, 6.0},
                                     New Double() {-4.0, 6.0, 1.0, 9.0},
                                     New Double() {8.0, 6.0, 9.0, 1.0}})
            Dim sourceInv As DenseMatrix = source.Inverse()

            'check Identy matrix I = A * A^-1
            Dim productMat = source * sourceInv
            If MathUtil.IsNearyEqualMatrix(productMat, New DenseMatrix(source.RowCount, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
            End If
        End With

        With Nothing
            Dim rng = New Random()
            For i = 0 To 10 - 1
                Dim dimNum = 4
                Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng, isIncludeZero:=True, isFloating:=True)
                Try
                    Dim souceInv = source.Inverse()
                    Dim product = source * souceInv

                    'check
                    If MathUtil.IsNearyEqualMatrix(product, New DenseMatrix(dimNum, True)) = True Then
                        'OK
                    Else
                        source.PrintValue(name:="is not equal Source matrix")
                        Assert.Fail("Inverse fail is not equal try:{0}", i)
                    End If
                Catch ex As Exception
                    source.PrintValue(name:="Source matrix")
                    Assert.Fail("Inverse fail try:{0}", i)
                End Try
            Next
        End With

        With Nothing
            Dim rng = New Random()
            For i = 5 To 10 - 1
                Dim dimNum = i
                Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)
                Try
                    Dim souceInv = source.Inverse()
                    Dim product = source * souceInv

                    'check
                    If MathUtil.IsNearyEqualMatrix(product, New DenseMatrix(dimNum, True)) = True Then
                        'OK
                    Else
                        source.PrintValue(name:="Source matrix")
                        Assert.Fail("Inverse fail")
                    End If
                Catch ex As Exception
                    source.PrintValue(name:="Source matrix")
                    Assert.Fail("Inverse fail")
                End Try
            Next
        End With
    End Sub

    ''' <summary>
    ''' test Cholesky decomposition
    ''' </summary>
    <TestMethod()> Public Sub Mat_Cholesky()
        With Nothing
            Dim tempMat As New DenseMatrix(New Double()() {New Double() {4.0, 1.0, 1.0},
                                                    New Double() {1.0, 7.0, 1.0},
                                                    New Double() {1.0, 1.0, 5.0}})
            'A = LL^T
            Dim c = tempMat.Cholesky()
            Dim check = c * c.T()

            If MathUtil.IsNearyEqualMatrix(tempMat, check) = False Then
                tempMat.PrintValue(name:=String.Format("error source(dim={0})", tempMat.RowCount))
                c.PrintValue(name:="L")
                Assert.Fail()
            End If
        End With

        With Nothing
            Dim tempMat As New DenseMatrix(New Double()() {New Double() {9.0, 2.0, 2.0, 6.0},
                                                    New Double() {2.0, 9.0, 7.0, 4.0},
                                                    New Double() {2.0, 7.0, 9.0, 1.0},
                                                    New Double() {6.0, 4.0, 1.0, 8.0}})
            'A = LL^T
            Dim c = tempMat.Cholesky()
            Dim check = c * c.T()

            If MathUtil.IsNearyEqualMatrix(tempMat, check) = False Then
                tempMat.PrintValue(name:=String.Format("error source(dim={0})", tempMat.RowCount))
                c.PrintValue(name:="L")
                Assert.Fail()
            End If
        End With
    End Sub

    ''' <summary>
    ''' test Eigen decomposition
    ''' </summary>
    <TestMethod()> Public Sub Mat_Eigen()
        Dim rng = New Random()

        For i As Integer = 2 To 5
            Dim srcMat = DenseMatrix.CreateDiagonalMatrix(Enumerable.Range(1, i).Select(Function(v) v * 1.0).ToArray())
            srcMat.PrintValue(name:="Source Matrix")

            'eigen
            Dim eigen = srcMat.Eigen()
            Dim retV = eigen.EigenValue
            Dim retM = eigen.EigenVector
            retV.PrintValue(name:="Eigen value")
            retM.PrintValue(name:="Eigen vector")

            'check 元に戻るか Source = V * D * V^T
            Dim temp = retM * retV.ToDiagonalMatrix() * retM.T()
            temp.PrintValue(name:="V * D * V^T")
            If MathUtil.IsNearyEqualMatrix(srcMat, temp) = False Then
                Assert.Fail("Error eigen() V*D*V^T dim={0} try={1}", i, 0)
            End If

            '下記のチェックも
            'a * eigen.EigenVector - eigen.EigenVector * eigen.EigenValue.ToDiagonalMatrix();
        Next

        'ランダム
        For i As Integer = 4 To 6
            Dim matDim = i
            Dim srcMat = MathUtil.CreateRandomSymmetricMatrix(matDim, rng:=rng)
            srcMat.PrintValue(name:="Source Matrix")

            'eigen
            Dim eigen = srcMat.Eigen()
            Dim retV = eigen.EigenValue
            Dim retM = eigen.EigenVector
            retV.PrintValue(name:="Eigen value")
            retM.PrintValue(name:="Eigen vector")

            ''check 固有ベクトルの転置と固有ベクトルの逆行列は同じ＝直交 I = V * V^T
            'Dim matI = retM * retM.T
            ''matI.PrintValue(name:="EigenVector * EivenVector^T")
            'If MathUtil.IsNearyEqualMatrix(matI, New DenseMatrix(matDim, True)) = False Then
            '    srcMat.PrintValue(name:="Source")
            '    Assert.Fail("Error eigen() EigenVector * EivenVector^T dim={0} try={1}", matDim, 0)
            'End If

            'check 元に戻るか Source = V * D * V^T
            Dim temp = retM * retV.ToDiagonalMatrix() * retM.T()
            temp.PrintValue(name:="V * D * V^T")
            If MathUtil.IsNearyEqualMatrix(srcMat, temp) = False Then
                Assert.Fail("Error eigen() V*D*V^T dim={0} try={1}", matDim, 0)
            End If
        Next
    End Sub

    ''' <summary>
    ''' test LU decomposition
    ''' </summary>
    <TestMethod()> Public Sub Mat_LU()
        Dim rng = New Random(12345)
        For j = 2 To 5
            Console.WriteLine("Dim={0} run", j)
            For i = 0 To 100 - 1
                Dim dimNum = j
                Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)

                If j > 2 AndAlso i Mod 2 = 0 Then
                    source(0)(0) = 0.0
                    source(2)(2) = 0.0
                End If

                Dim resultLU = source.LUP()
                Dim P = resultLU.P
                Dim L = resultLU.L
                Dim U = resultLU.U

                'check
                Dim flg = True
                If MathUtil.IsNearyEqualMatrix(source, P * L * U) = False Then
                    Console.WriteLine("No={0} det={1}", i, resultLU.Det)
                    source.PrintValue(name:="souce")
                    P.PrintValue(name:="P")
                    L.PrintValue(name:="L")
                    U.PrintValue(name:="U")
                    Assert.Fail("try no", i)
                End If
            Next
        Next

        With Nothing
            Dim dimNum = 4
            Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)

            '列を0に
            source(2)(0) = 0.0
            source(2)(1) = 0.0
            source(2)(2) = 0.0
            source(2)(3) = 0.0

            Dim resultLU As LU = Nothing
            Try
                resultLU = source.LUP()
                Dim P = resultLU.P
                Dim L = resultLU.L
                Dim U = resultLU.U

                source.PrintValue(name:="souce")
                P.PrintValue(name:="P")
                L.PrintValue(name:="L")
                U.PrintValue(name:="U")
                Assert.Fail("error")
            Catch ex As Exception
                'OK
            End Try
        End With
    End Sub

    ''' <summary>
    ''' test Solve() Ax=b
    ''' </summary>
    <TestMethod()> Public Sub Mat_Solve()
        Dim rng = New Random()
        For i = 2 To 10 - 1
            Dim dimNum = i
            For j As Integer = 0 To 100 - 1
                Dim source = MathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)

                'add 対角成分
                source += New DenseMatrix(dimNum, True)

                Try
                    Dim matLUP = source.LUP()
                    If MathUtil.IsCloseToZero(matLUP.Det) = True Then
                        Continue For
                    End If

                    '1列目
                    Dim colVec = source.Inverse().Column(0)
                    Dim result = matLUP.Solve((New DenseMatrix(dimNum, True))(0))

                    'check
                    If MathUtil.IsNearyEqualVector(colVec, result) = True Then
                        'OK
                    Else
                        source.PrintValue(name:="Source vector")
                        colVec.PrintValue()
                        result.PrintValue()
                        Assert.Fail("Solve fail: try={0}", j)
                    End If
                Catch ex As Exception
                    source.PrintValue(name:="Source matrix")
                    Console.WriteLine(ex.Message)
                    Assert.Fail("Exception. Solve fail: try={0}", j)
                End Try
            Next
        Next
    End Sub

    ''' <summary>
    ''' test Householder
    ''' </summary>
    <Ignore>
    <TestMethod()> Public Sub TestHouseholder_SymmetricMatrix()
        'Try
        '    With Nothing
        '        Dim symmetricMatrix = New DenseMatrix(New Double()() {
        '                                            New Double() {8, -4, 2, -2},
        '                                            New Double() {-4, -2, 4, 9},
        '                                            New Double() {2, 4, 3, -6},
        '                                            New Double() {-2, 9, -6, 2}})

        '        ' 対称行列をハウスホルダー変換 → 三重対角行列
        '        ' 対象行列になり。直交行列となる。 -> H=H^T, H = H^-1
        '        Dim eps = 0.00001
        '        Dim h = symmetricMatrix.Householder()
        '        Assert.IsTrue(h.IsSymmetricMatrix(eps))

        '        Dim h_t = h.T()
        '        Assert.IsTrue(MathUtil.IsNearyEqualMatrix(h, h_t, eps))

        '        'check
        '        Dim correct = New DenseMatrix(New Double()() {
        '                                            New Double() {8, 4.89898, 0, 0},
        '                                            New Double() {4.89898, 4.83333, 9.46778, 0},
        '                                            New Double() {0, 9.46778, -6.97526, -1.62287},
        '                                            New Double() {0, 0, -1.62287, 5.14193}})
        '        Assert.IsTrue(MathUtil.IsNearyEqualMatrix(h, correct, eps))
        '    End With
        'Catch ex As Exception
        '    Assert.Fail("throw exception" + ex.Message)
        'End Try
    End Sub

    ''' <summary>
    ''' test Householder
    ''' </summary>
    <Ignore>
    <TestMethod()> Public Sub TestHouseholder_ASymmetricMatrix()
        'Try
        '    With Nothing
        '        Dim asymmetricMatrix = New DenseMatrix(New Double()() {
        '                                           New Double() {-1.0616, -0.1924, -1.4224, 1.4193},
        '                                           New Double() {2.3505, 0.8886, 0.4882, 0.2916},
        '                                           New Double() {-0.6156, -0.7648, -0.1774, 0.1978},
        '                                           New Double() {0.7481, -1.4023, -0.1961, 1.5877}})

        '        ' 非対称行列をハウスホルダー変換 → ヘッセンベルグ型 Hessenberg
        '        Dim h = asymmetricMatrix.Householder()

        '        Dim correct = New DenseMatrix(New Double()() {
        '                                      New Double() {2.755, 0.6224, 0.951, 0.0888},
        '                                      New Double() {0.0000, -1.7293, -0.3043, 1.415},
        '                                      New Double() {0.0000, 0.0000, -1.1552, 1.6246},
        '                                      New Double() {0.0000, 0.0000, 0.0000, -0.1006}})
        '        Dim eps = 0.00001
        '        Assert.IsTrue(MathUtil.IsNearyEqualMatrix(h, correct, eps))
        '    End With

        'Catch ex As Exception
        '    Assert.Fail("throw exception" + ex.Message)
        'End Try
    End Sub

    ''' <summary>
    ''' test Resize()
    ''' </summary>
    <TestMethod()> Public Sub Vec_ResizeVector()
        Dim tempVec = New DenseVector(3)
        tempVec.Resize(0)
        Assert.AreEqual(tempVec.Count, 0)
        tempVec.Resize(1)
        Assert.AreEqual(tempVec.Count, 1)
    End Sub

    ''' <summary>
    ''' test Resize()
    ''' </summary>
    <TestMethod()> Public Sub Mat_ResizeMatrix()
        Dim tempMat = New DenseMatrix(3)
        tempMat.Resize(0, 0)
        Assert.AreEqual(tempMat.RowCount, 0)
        Assert.AreEqual(tempMat.ColCount, 0)
        tempMat.Resize(1, 2)
        Assert.AreEqual(tempMat.RowCount, 1)
        Assert.AreEqual(tempMat.ColCount, 2)
    End Sub

    <TestMethod()> Public Sub Vec_CrossProduct()
        '0dim
        Try
            Dim v1 = New DenseVector(New Double() {})
            Dim v2 = New DenseVector(New Double() {})
            Dim temp = v1.CrossProduct(v2)
            Assert.Fail("{0} dim not detect exception", v1.Count)
        Catch ex As Exception
            'ok
        End Try

        '1dim
        Try
            Dim v1 = New DenseVector(New Double() {1})
            Dim v2 = New DenseVector(New Double() {1})
            Dim temp = v1.CrossProduct(v2)
            Assert.Fail("{0} dim not detect exception", v1.Count)
        Catch ex As Exception
            'ok
        End Try

        '2dim
        Try
            Dim v1 = New DenseVector(New Double() {1, 1})
            Dim v2 = New DenseVector(New Double() {1, 1})
            Dim temp = v1.CrossProduct(v2)
            Assert.Fail("{0} dim not detect exception", v1.Count)
        Catch ex As Exception
            'ok
        End Try

        '4dim
        Try
            Dim v1 = New DenseVector(New Double() {1, 1, 1, 1})
            Dim v2 = New DenseVector(New Double() {1, 1, 1, 1})
            Dim temp = v1.CrossProduct(v2)
            Assert.Fail("{0} dim not detect exception", v1.Count)
        Catch ex As Exception
            'ok
        End Try

        '3 dim
        With Nothing
            Dim v1 = New DenseVector(New Double() {3, 4, 1})
            Dim v2 = New DenseVector(New Double() {3, 7, 5})
            Try
                Dim temp = v1.CrossProduct(v2)
                Assert.AreEqual(temp(0), 13, 0.0000000001)
                Assert.AreEqual(temp(1), -12, 0.0000000001)
                Assert.AreEqual(temp(2), 9, 0.0000000001)
            Catch ex As Exception
                Assert.Fail("{0} dim detect exception", v1.Count)
            End Try
        End With
    End Sub

    <TestMethod()> Public Sub Mat_AddRow()
        Dim v1 = New DenseVector(New Double() {50, 60, 70, 80, 90})
        Dim v2 = New DenseVector(New Double() {40, 70, 90, 60, 100})

        'error
        With Nothing
            Dim mat = New DenseMatrix()
            Try
                mat.AddCol(v1)
                mat.AddRow(v1)

                Assert.Fail("Error")
            Catch ex As Exception
                'OK
            End Try
        End With

        'correct
        With Nothing
            Dim mat = New DenseMatrix()
            Try
                mat.AddRow(v1)
                mat.AddRow(v1)
                Assert.AreEqual(mat.RowCount, 2)
                Assert.AreEqual(mat.ColCount, 5)
            Catch ex As Exception
                Assert.Fail("Error")
            End Try
        End With
    End Sub

    <TestMethod()> Public Sub Mat_AddCol()
        Dim v1 = New DenseVector(New Double() {50, 60, 70, 80, 90})
        Dim v2 = New DenseVector(New Double() {40, 70, 90, 60, 100})

        With Nothing
            Dim mat = New DenseMatrix()
            Try
                mat.AddRow(v1)
                mat.AddCol(v1)

                Assert.Fail("Error")
            Catch ex As Exception
                'OK
            End Try
        End With

        'correct
        With Nothing
            Dim mat = New DenseMatrix()
            Try
                mat.AddCol(v1)
                mat.AddCol(v1)
                Assert.AreEqual(mat.RowCount, 5)
                Assert.AreEqual(mat.ColCount, 2)
            Catch ex As Exception
                Assert.Fail("Error")
            End Try
        End With
    End Sub

End Class
