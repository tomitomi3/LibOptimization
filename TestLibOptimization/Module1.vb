Imports LibOptimization
Imports LibOptimization.MathUtil

'
'あとでテストにする
'

Module Module1

    Sub Main()
        TestVectorMatrix()
    End Sub

    ''' <summary>
    ''' ベクトル、行列テスト計算
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TestVectorMatrix()
        With Nothing
            Dim v As New clsEasyVector(New Double() {1, 1, 1})
            Dim matV As New clsEasyMatrix(New Double()() {New Double() {1}, New Double() {2}, New Double() {3}})

            v = v + matV
            v.PrintValue()
            v = matV + v
            v.PrintValue()

            v = v - matV
            v.PrintValue()
            v = matV - v
            v.PrintValue()

            Console.WriteLine("----------------------------------------------")
            'Product
            Dim temp As clsEasyVector = Nothing
            Dim mat As New clsEasyMatrix(New Double()() {New Double() {1, 1}, New Double() {2, 1}, New Double() {3, 1}})
            Dim mat2 As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {2, 1, 1}, New Double() {3, 1, 1}})

            'v.Direction = clsEasyVector.VectorDirection.ROW
            'Try
            '    temp = mat2 * v
            '    temp.PrintValue()
            'Catch ex As Exception
            'End Try

            'v.Direction = clsEasyVector.VectorDirection.COL
            'temp = mat2 * v
            'mat2.PrintValue()
            'Console.WriteLine()
            'v.PrintValue()
            'Console.WriteLine("Mat *  Vector")
            'If temp IsNot Nothing Then
            '    temp.PrintValue()
            'End If

            'v.Direction = clsEasyVector.VectorDirection.ROW
            'temp = v * mat2
            'mat2.PrintValue()
            'Console.WriteLine()
            'v.PrintValue()
            'Console.WriteLine("Vector * Mat")
            'If temp IsNot Nothing Then
            '    temp.PrintValue()
            'End If

            'v.Direction = clsEasyVector.VectorDirection.COL
            'Try
            '    temp = v * mat2
            '    temp.PrintValue()
            'Catch ex As Exception
            'End Try

            'Console.WriteLine("----------------------------------------------")
            'v.Direction = clsEasyVector.VectorDirection.ROW
            'Try
            '    temp = mat * v
            '    temp.PrintValue()
            'Catch ex As Exception
            'End Try

            'v.Direction = clsEasyVector.VectorDirection.COL
            'Try
            '    temp = mat * v
            '    temp.PrintValue()
            'Catch ex As Exception
            'End Try
            'If temp IsNot Nothing Then
            '    temp.PrintValue()
            'End If

            'v.Direction = clsEasyVector.VectorDirection.ROW
            'temp = v * mat
            'mat.PrintValue()
            'Console.WriteLine()
            'v.PrintValue()
            'Console.WriteLine("Vector * Mat")
            'If temp IsNot Nothing Then
            '    temp.PrintValue()
            'End If

            'v.Direction = clsEasyVector.VectorDirection.COL
            'Try
            '    temp = v * mat
            '    temp.PrintValue()
            'Catch ex As Exception
            'End Try
        End With

        '
        With Nothing
            'Inverse
            '0.5	-0.5	1
            '0.5	 0.5	-2
            '-1      1  	-1
            Dim matInv As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1, 3},
                                                                                    New Double() {2, 2, 5, -5},
                                                                                     New Double() {3, 10, 3, 2},
                                                                                     New Double() {4, 20, 40, 4}})

            'Experiment LU
            Dim matTemp As New clsEasyMatrix(New Double()() {New Double() {0, 1, 2}, New Double() {3, 4, 5}, New Double() {6, 7, 8}})
            'Dim tempLU = New clsLU(matTemp)

            'Dim temptempLU = matTemp.LU(Nothing, Nothing)
            'Dim result = temptempLU.Solve(Nothing)

            'Dim ar As New List(Of Double)
            'For i As Integer = 0 To matTemp.Count - 1
            '    ar.AddRange(matTemp.Column(i))
            'Next
            'Dim piv = New Integer(matTemp.RowCount - 1) {}
            'matTemp.LUOrg(ar, matTemp.RowCount, piv)

            'matTemp.PrintValue()
            'Dim temp = matTemp.LU(matInv, piv)
            'temp.PrintValue(10)

            'Dim rh As New clsShoddyMatrix(20)
            'rh(1)(10) = -100
            'rh(3)(10) = -100
            'rh(6)(19) = -100
            'Dim piv2 = New Integer(20 - 1) {}
            'Dim aaaa = rh.LU(rh, piv2)
            'aaaa.PrintValue()
            'Dim aaa = rh.Det()

            'Dim d = matInv.Det()
            'Dim dd As Double = 1
            'For i As Integer = 0 To 3
            '    dd *= temp(i)(i)
            'Next

        End With

        'Vector check
        With Nothing
            'Vector check
            Dim v As New clsEasyVector(New Double() {1, 1, 1})
            Console.WriteLine(v.NormL1().ToString())
            v.PrintValue()
            v = v + (New clsEasyVector(New Double() {2, 2, 2}))
            v.PrintValue()
            v = v - (New clsEasyVector(New Double() {2, 2, 2}))
            v.PrintValue()
            v = v * 2
            v.PrintValue()
            v = 2 * v
            v.PrintValue()
            v = v / 2
            v.PrintValue()
            v = 2 / v
            v.PrintValue()
            v = (v * 2) ^ 2
            v.PrintValue()

            'Matrix check
            Dim mat As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {2, 1, 1}, New Double() {3, 1, 1}})
            mat.PrintValue()
            mat = mat + (New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {1, 1, 1}, New Double() {1, 1, 1}}))
            mat(1)(1) = 10.0
            mat.PrintValue()
            Console.WriteLine("----------------------------------------------")
            mat = mat * 2
            mat.PrintValue()
            Console.WriteLine("----------------------------------------------")
            mat = 2 * mat
            mat.PrintValue()
            Console.WriteLine("----------------------------------------------")

            Dim matA As New clsEasyMatrix(3, True)
            matA.PrintValue()
            Dim matB As New clsEasyMatrix(New Double()() {New Double() {0, 1, 0}, New Double() {1, 1, 1}, New Double() {1, 1, 1}})
            matA = matA * matB
            matA.PrintValue()

            Console.WriteLine("----------------------------------------------")
            Dim matT As New clsEasyMatrix(New Double()() {New Double() {1, 2, 3}, New Double() {4, 5, 6}, New Double() {7, 8, 9}})
            matT.PrintValue()
            matT.T().PrintValue()
            Dim matT2 As New clsEasyMatrix(New Double()() {New Double() {1, 2, 3}, New Double() {4, 5, 6}})
            matT2.PrintValue()
            matT2.T().PrintValue()
            Console.WriteLine("----------------------------------------------")
            'determinant
            Dim detMat As New clsEasyMatrix(New Double()() {New Double() {3, 1, 1, 2},
                                                                        New Double() {5, 1, 3, 4},
                                                                        New Double() {2, 0, 1, 0},
                                                                        New Double() {1, 3, 2, 1}})
            detMat.PrintValue()
            Dim d As Double = detMat.Det()
            Console.WriteLine(d.ToString()) 'correct  -22
            Console.WriteLine("----------------------------------------------")
            'Inverse
            '0.5	-0.5	1
            '0.5	 0.5	-2
            '-1      1  	-1
            Dim matInv As New clsEasyMatrix(New Double()() {New Double() {3, 1, 1},
                                                                        New Double() {5, 1, 3},
                                                                        New Double() {2, 0, 1}})
            Dim matPivotInv As clsEasyMatrix = matInv.Inverse()
            matInv.PrintValue()
            matPivotInv.PrintValue()
            matPivotInv = matInv * matPivotInv
            matPivotInv.PrintValue()
            Console.WriteLine("----------------------------------------------")
            Dim matPA As New clsEasyMatrix(New Double()() {New Double() {1, 2, 3}})
            Dim matPB As New clsEasyMatrix(New Double()() {New Double() {1},
                                                                       New Double() {4},
                                                                       New Double() {7}})
            matPA.PrintValue()
            matPB.PrintValue()
            matPA = matPA * matPB
            matPA.PrintValue()
            Console.WriteLine("----------------------------------------------")
            Dim vecA As New clsEasyVector(New Double() {1, 2, 3})
            Dim matC As New clsEasyMatrix(New Double()() {New Double() {1, 2, 3},
                                                                       New Double() {4, 5, 6},
                                                                       New Double() {7, 8, 9}})
            'vecA.PrintValue()
            'matC.PrintValue()
            'Dim tempV As clsEasyVector = vecA * matC
            'tempV.PrintValue()
            'Console.WriteLine("----------------------------------------------")
            ''Inner Product
            'With Nothing
            '    Dim va As New clsEasyVector(New Double() {1, 1})
            '    Dim vb As New clsEasyVector(New Double() {0.5, 0})
            '    Console.WriteLine(va.NormL2.ToString())
            '    Console.WriteLine(vb.NormL2.ToString())
            '    Console.WriteLine(va.InnerProduct(vb).ToString())

            '    Dim aa As Double
            '    'aa = va.NormL1 * vb.NormL1 * Math.Cos(45 * Math.PI / 180.0)
            '    'Console.WriteLine(aa.ToString())
            '    aa = va.NormL2 * vb.NormL2 * Math.Cos(45 * Math.PI / 180.0)
            '    Console.WriteLine(aa.ToString())
            'End With
            Console.WriteLine("----------------------------------------------")
            Dim matAA As New clsEasyMatrix(New Double()() {New Double() {1, 1, 1}, New Double() {2, 1, 1}, New Double() {3, 1, 1}})
            Dim vBB As New clsEasyMatrix(New List(Of Double)({1, 1, 1}), clsEasyVector.VectorDirection.COL)
            Dim vAA As New clsEasyVector(New Double() {1, 1, 1})
            matAA.PrintValue()
            vBB.PrintValue()
            vAA.PrintValue()

            Dim tempMatA As clsEasyMatrix
            tempMatA = matAA * vBB
            tempMatA.PrintValue()
            'tempMatA = vBB * matAA
            'tempMatA.PrintValue()
        End With
    End Sub
End Module
