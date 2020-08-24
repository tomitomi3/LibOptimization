'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil

''' <summary>
''' unit test for my Linear Algebra  lib
''' </summary>
<TestClass()> Public Class UnitTestLinearAlgebra
    ''' <summary>
    ''' test Create vector
    ''' </summary>
    <TestMethod()> Public Sub Vec_CreateVector()
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
    <TestMethod()> Public Sub Mat_CreateMatrix()
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
    <TestMethod()> Public Sub Mat_Add_VectorMatrix()
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
    <TestMethod()> Public Sub Mat_Mat_Add_MatrixMatrix()
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
    <TestMethod()> Public Sub Mat_Sub_VectorMatrix()
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
    <TestMethod()> Public Sub Mat_Sub_MatrixVector()
        '3x2 ok
        With Nothing
            Dim v As New clsEasyVector(New Double() {1, 1}, clsEasyVector.VectorDirection.COL)
            Dim matV As New clsEasyMatrix(New Double()() {New Double() {1, 1}, New Double() {1, 1}, New Double() {1, 1}})
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
            Dim v As New clsEasyVector(New Double() {1, 1}, clsEasyVector.VectorDirection.ROW)
            Dim matV As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {1, 1, 1}})
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
            Dim v As New clsEasyVector(New Double() {1, 1}, clsEasyVector.VectorDirection.COL)
            Dim matV As New clsEasyMatrix(New Double()() {New Double() {1, 1}, New Double() {1, 1}, New Double() {1, 1}})
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
    ''' Determinant
    ''' </summary>
    <TestMethod()> Public Sub Mat_Determinant()
        With Nothing
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
            MathUtil.clsMathUtil.SwapCol(detMat, 0, 1)
            detMat.PrintValue()
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)
            If d <> -1 Then
                Assert.Fail()
            End If
        End With

        With Nothing
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
            MathUtil.clsMathUtil.SwapCol(detMat, 0, 1)
            detMat.PrintValue()
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)
            If d <> -1 Then
                Assert.Fail()
            End If
        End With

        With Nothing
            'swap
            Dim detMat As New clsEasyMatrix(New Double()() {New Double() {1, 0, 0, 0},
                                                            New Double() {0, 1, 0, 0},
                                                            New Double() {0, 0, 1, 0},
                                                            New Double() {0, 0, 0, 1}})
            detMat.PrintValue()
            Dim d As Double = 0
            d = detMat.Det()
            Console.WriteLine("Determinant:{0}", d)

            If clsMathUtil.IsCloseToValues(d, 1.0) = False Then
                Assert.Fail()
            End If

            'swap col -> sign *-1
            MathUtil.clsMathUtil.SwapCol(detMat, 0, 1)
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
            Dim mat As New MathUtil.clsEasyMatrix(v.Count, True)
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
    <TestMethod()> Public Sub Vec_Product_VectorScalar()
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
    ''' Vector * Vector
    ''' </summary>
    <TestMethod()> Public Sub Vec_Product_VectorVector()
        Dim vec1 = New clsEasyVector(New Double() {1, 2, 3}, clsEasyVector.VectorDirection.ROW)
        Dim vec2 = New clsEasyVector(New Double() {4, 5, 6}, clsEasyVector.VectorDirection.COL)

        With Nothing
            '|v1 v2| * |v3|
            '          |v4|
            Dim vec1vec2 = vec1 * vec2
            If vec1vec2.RowCount <> 1 AndAlso vec1vec2.ColCount <> 1 Then
                vec1vec2.PrintValue(name:="vec1vec2")
                Assert.Fail("error : vector * vector")
            End If
            If MathUtil.clsMathUtil.IsCloseToValues(vec1vec2(0)(0), 32.0) = False Then
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

            Dim correctMat As New clsEasyMatrix(New Double()() {(vec2 * 1.0).ToArray(),
                                                                (vec2 * 2.0).ToArray(),
                                                                (vec2 * 3.0).ToArray()})
            If MathUtil.clsMathUtil.IsNearyEqualMatrix(vec2vec1, correctMat) = False Then
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
    <TestMethod()> Public Sub Mat_Product_MatrixScalar()
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
    <TestMethod()> Public Sub Mat_Product_ScalarMatrix()
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
    <TestMethod()> Public Sub Mat_Product_MatrixMatrix()
        '---------
        'bad
        '---------
        '3x3 * 2x2
        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
            Dim matB = New clsEasyMatrix(dimNum - 1, True)
            Try
                Dim temp = matA * matB
                Assert.Fail("error : 3x3 * 2x2")
            Catch myex As clsException
                'OK
            End Try
        End With

        '2x2 * 3x3
        With Nothing
            Dim dimNum = 3
            Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum - 1)
            Dim matB = New clsEasyMatrix(dimNum, True)
            Try
                Dim temp = matA * matB

                Assert.Fail("error : 2x2 * 3x3")
            Catch myex As clsException
                'OK
            End Try
        End With

        '3x3 * 2x3
        With Nothing
            Dim matA As New clsEasyMatrix(3, True)
            Dim matB As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1},
                                                     New Double() {2, 2, 2}})
            Try
                Dim temp = matA * matB

                Assert.Fail("error : 3x3 * 2x3")
            Catch myex As clsException
                'OK
            End Try
        End With

        '---------
        'OK
        '---------
        For dimNum = 2 To 10 - 1
            With Nothing
                Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
                Dim matB = New clsEasyMatrix(dimNum, True)
                Try
                    Dim temp = matA * matB
                    temp.PrintValue(name:="A*B")

                    If clsMathUtil.IsNearyEqualMatrix(temp, matA) = False Then
                        Assert.Fail("error : {0}x{0}", dimNum)
                    End If
                Catch myex As clsException
                    Assert.Fail()
                End Try
            End With

            With Nothing
                Dim matA = clsMathUtil.CreateRandomSymmetricMatrix(dimNum)
                Dim matB = New clsEasyMatrix(dimNum, False)
                Try
                    Dim temp = matA * matB
                    temp.PrintValue(name:="B(zero)*A")

                    If clsMathUtil.IsNearyEqualMatrix(temp, matB) = False Then
                        Assert.Fail("error : {0}x{0}", dimNum)
                    End If
                Catch myex As clsException
                    Assert.Fail()
                End Try
            End With
        Next

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
                If clsMathUtil.IsNearyEqualMatrix(temp, matA) = False Then
                    Assert.Fail()
                End If
            Catch myex As clsException
                Assert.Fail()
            End Try
        End With
    End Sub

    ''' <summary>
    ''' Transopose
    ''' </summary>
    <TestMethod()> Public Sub Mat_Transopose()
        Dim rng = New Util.clsRandomXorshift()
        For i As Integer = 2 To 50 - 1
            Dim dimNum = i
            Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng)

            '転置
            Dim t = source.T()
            t = t.T()

            'check
            If clsMathUtil.IsNearyEqualMatrix(t, source) = True Then
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
            Dim rng = New Util.clsRandomXorshift()
            Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng)
            Dim product = source * source.Inverse()

            'check
            If clsMathUtil.IsNearyEqualMatrix(product, New clsEasyMatrix(dimNum, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("Inverse fail 1x1")
            End If
        End With

        With Nothing
            Dim source As New clsEasyMatrix(New Double()() {New Double() {3, 1}, New Double() {-2, 5}})
            Dim sourceInv As clsEasyMatrix = source.Inverse()

            'check Identy matrix I = A * A^-1
            Dim productMat = source * sourceInv
            If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(source.RowCount, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("before swap col 2x2")
            End If

            'col swap
            MathUtil.clsMathUtil.SwapCol(source, 0, 1)
            sourceInv = source.Inverse()
            productMat = source * sourceInv
            'check
            If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(3, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("after swap col 2x2")
            End If
        End With

        With Nothing
            Dim source As New clsEasyMatrix(New Double()() {New Double() {3, 1, 1},
                                                    New Double() {5, 1, 3},
                                                    New Double() {2, 0, 1}})
            Dim sourceInv As clsEasyMatrix = source.Inverse()

            'check Identy matrix I = A * A^-1
            Dim productMat = source * sourceInv
            If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(source.RowCount, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("before swap col 3x3")
            End If

            'col swap
            MathUtil.clsMathUtil.SwapCol(source, 0, 1)
            sourceInv = source.Inverse()
            productMat = source * sourceInv
            'check
            If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(3, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
                Assert.Fail("after swap col 3x3")
            End If
        End With

        With Nothing
            'ピボット選択が必須の場合（対角成分が０になる場合）
            Dim source = New MathUtil.clsEasyMatrix(New Double()() {
                                     New Double() {1.0, -1.0, -4.0, 8.0},
                                     New Double() {-1.0, 1.0, 6.0, 6.0},
                                     New Double() {-4.0, 6.0, 1.0, 9.0},
                                     New Double() {8.0, 6.0, 9.0, 1.0}})
            Dim sourceInv As clsEasyMatrix = source.Inverse()

            'check Identy matrix I = A * A^-1
            Dim productMat = source * sourceInv
            If clsMathUtil.IsNearyEqualMatrix(productMat, New clsEasyMatrix(source.RowCount, True)) = True Then
                'OK
            Else
                source.PrintValue(name:="Source matrix")
            End If
        End With

        With Nothing
            Dim rng = New LibOptimization.Util.clsRandomXorshift()
            For i = 0 To 100 - 1
                Dim dimNum = 4
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng, isIncludeZero:=True, isFloating:=True)
                Try
                    Dim souceInv = source.Inverse()
                    Dim product = source * souceInv

                    'check
                    If clsMathUtil.IsNearyEqualMatrix(product, New clsEasyMatrix(dimNum, True)) = True Then
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
            Dim rng = New LibOptimization.Util.clsRandomXorshift()
            For i = 5 To 10 - 1
                Dim dimNum = i
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)
                Try
                    Dim souceInv = source.Inverse()
                    Dim product = source * souceInv

                    'check
                    If clsMathUtil.IsNearyEqualMatrix(product, New clsEasyMatrix(dimNum, True)) = True Then
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
            Dim tempMat As New clsEasyMatrix(New Double()() {New Double() {4.0, 1.0, 1.0},
                                                    New Double() {1.0, 7.0, 1.0},
                                                    New Double() {1.0, 1.0, 5.0}})
            'A = LL^T
            Dim c = tempMat.Cholesky()
            Dim check = c * c.T()

            If clsMathUtil.IsNearyEqualMatrix(tempMat, check) = False Then
                tempMat.PrintValue(name:=String.Format("error source(dim={0})", tempMat.RowCount))
                c.PrintValue(name:="L")
                Assert.Fail()
            End If
        End With

        With Nothing
            Dim tempMat As New clsEasyMatrix(New Double()() {New Double() {9.0, 2.0, 2.0, 6.0},
                                                    New Double() {2.0, 9.0, 7.0, 4.0},
                                                    New Double() {2.0, 7.0, 9.0, 1.0},
                                                    New Double() {6.0, 4.0, 1.0, 8.0}})
            'A = LL^T
            Dim c = tempMat.Cholesky()
            Dim check = c * c.T()

            If clsMathUtil.IsNearyEqualMatrix(tempMat, check) = False Then
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
        Dim rng = New LibOptimization.Util.clsRandomXorshift()
        For j As Integer = 2 To 20 - 1
            For i As Integer = 0 To 50 - 1
                Dim matDim = j
                Dim srcMat = clsMathUtil.CreateRandomSymmetricMatrix(matDim, rng:=rng)

                'eigen
                Dim eigen As Eigen = Nothing
                eigen = srcMat.Eigen()
                Dim retV = eigen.EigenValue
                Dim retM = eigen.EigenVector

                'check
                '固有ベクトルの転置と固有ベクトルの逆行列は同じ＝直交
                ' I = V * V^T
                Dim matI = retM * retM.T
                'matI.PrintValue(name:="EigenVector * EivenVector^T")
                If clsMathUtil.IsNearyEqualMatrix(matI, New clsEasyMatrix(matDim, True)) = False Then
                    srcMat.PrintValue(name:="Source")
                    Assert.Fail("Error eigen() EigenVector * EivenVector^T dim={0} try={1}", matDim, i)
                End If

                'check
                ' Source = V * D * V^T
                Dim temp = retM * retV.ToDiagonalMatrix() * retM.T()
                'temp.PrintValue(name:="V*D*V^T")
                If clsMathUtil.IsNearyEqualMatrix(srcMat, temp) = False Then
                    srcMat.PrintValue(name:="Source")
                    Assert.Fail("Error eigen() V*D*V^T dim={0} try={1}", matDim, i)
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' test LU decomposition
    ''' </summary>
    <TestMethod()> Public Sub Mat_LU()
        Dim rng = New LibOptimization.Util.clsRandomXorshift()
        For j = 3 To 4
            Console.WriteLine("Dim={0} run", j)
            For i = 0 To 1000 - 1
                Dim dimNum = j
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)

                If i Mod 2 = 0 Then
                    source(0)(0) = 0.0
                    source(2)(2) = 0.0
                End If

                Dim resultLU = source.LUP()
                Dim P = resultLU.P
                Dim L = resultLU.L
                Dim U = resultLU.U

                'check
                Dim flg = True
                If clsMathUtil.IsNearyEqualMatrix(source, P * L * U) = True Then
                    'OK
                Else
                    flg = False
                End If
                If flg = False Then
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
            Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)

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
        Dim rng = New LibOptimization.Util.clsRandomXorshift()
        For i = 2 To 10 - 1
            Dim dimNum = i
            For j As Integer = 0 To 300 - 1
                Dim source = clsMathUtil.CreateRandomSymmetricMatrix(dimNum, rng:=rng)
                Try
                    Dim matLUP = source.LUP()
                    If clsMathUtil.IsCloseToZero(matLUP.Det) = True Then
                        Continue For
                    End If

                    '1列目
                    Dim colVec = source.Inverse().Column(0)
                    Dim result = matLUP.Solve((New clsEasyMatrix(dimNum, True))(0))

                    'check
                    If clsMathUtil.IsNearyEqualVector(colVec, result) = True Then
                        'OK
                    Else
                        source.PrintValue(name:="Source vector")
                        colVec.PrintValue()
                        result.PrintValue()
                        Assert.Fail("Solve fail: try={0}", j)
                    End If
                Catch ex As Exception
                    source.PrintValue(name:="Source matrix")
                    Assert.Fail("Solve fail: try={0}", j)
                End Try
            Next
        Next
    End Sub

    ''' <summary>
    ''' test Householder
    ''' </summary>
    <TestMethod()> Public Sub Mat_Householder()
        Try
            Dim source = New MathUtil.clsEasyMatrix(New Double()() {
                                                    New Double() {8, -4, 2, -2},
                                                    New Double() {-4, -2, 4, 9},
                                                    New Double() {2, 4, 3, -6},
                                                    New Double() {-2, 9, -6, 2}})
            Dim correct = New MathUtil.clsEasyMatrix(New Double()() {
                                                    New Double() {8, 4.89898, 0, 0},
                                                    New Double() {4.89898, 4.83333, 9.46778, 0},
                                                    New Double() {0, 9.46778, -6.97526, -1.62287},
                                                    New Double() {0, 0, -1.62287, 5.14193}})
            Dim result = source.Householder()

            'check
            Dim isOK = False
            If clsMathUtil.IsNearyEqualMatrix(result, correct, 0.0001) = True Then
                isOK = True
            End If
            If isOK = False Then
                Assert.Fail()
            End If
        Catch ex As Exception
            Assert.Fail("throw exception")
        End Try
    End Sub

    ''' <summary>
    ''' test Resize()
    ''' </summary>
    <TestMethod()> Public Sub Vec_ResizeVector()
        Dim tempVec = New clsEasyVector(3)
        tempVec.Resize(0)
        Assert.AreEqual(tempVec.Count, 0)
        tempVec.Resize(1)
        Assert.AreEqual(tempVec.Count, 1)
    End Sub

    ''' <summary>
    ''' test Resize()
    ''' </summary>
    <TestMethod()> Public Sub Mat_ResizeMatrix()
        Dim tempMat = New clsEasyMatrix(3)
        tempMat.Resize(0, 0)
        Assert.AreEqual(tempMat.RowCount, 0)
        Assert.AreEqual(tempMat.ColCount, 0)
        tempMat.Resize(1, 2)
        Assert.AreEqual(tempMat.RowCount, 1)
        Assert.AreEqual(tempMat.ColCount, 2)
    End Sub

End Class