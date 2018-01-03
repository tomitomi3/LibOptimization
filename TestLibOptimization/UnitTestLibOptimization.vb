Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

'LibOptimization
Imports LibOptimization
Imports LibOptimization.MathUtil
Imports LibOptimization.Optimization
Imports LibOptimization.BenchmarkFunction
Imports LibOptimization.Util

<TestClass()> Public Class UnitTestLibOptimization

#Region "Util"
    ''' <summary>
    ''' check random lib
    ''' </summary>
    <TestMethod()> Public Sub TestRandom()
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

#Region "Optimization"
    ''' <summary>
    ''' Sphere関数で最適解になるか確認
    ''' </summary>
    ''' <param name="opt"></param>
    ''' <remarks></remarks>
    Public Sub CheckOptUsingSphere(ByVal opt As absOptimization)
        'check init
        opt.Init()
        Dim errorFlg = opt.IsRecentError()
        Assert.IsFalse(errorFlg)

        'check iterate
        opt.DoIteration()
        errorFlg = opt.IsRecentError()
        Assert.IsFalse(errorFlg)

        'Eval
        If Math.Abs(opt.Result.Eval) > 0.05 Then
            Assert.Fail(String.Format("fail Eval {0}", opt.Result.Eval))
        End If
        Console.WriteLine(String.Format("Success Eval {0}", opt.Result.Eval))

        'Result
        If Math.Abs(opt.Result(0)) > 0.05 OrElse Math.Abs(opt.Result(1)) > 0.05 Then
            Assert.Fail(String.Format("fail Result {0} {1}", opt.Result(0), opt.Result(1)))
        End If
        Console.WriteLine(String.Format("Success Result {0} {1}", opt.Result(0), opt.Result(1)))
    End Sub

    <TestMethod()> Public Sub TestOptimizationCS()
        Dim opt = New clsOptCS(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationDE()
        Dim opt = New clsOptDE(New clsBenchSphere(2))
        opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_1_bin
        Me.CheckOptUsingSphere(opt)

        opt = New clsOptDE(New clsBenchSphere(3))
        opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_best_2_bin
        Me.CheckOptUsingSphere(opt)

        opt = New clsOptDE(New clsBenchSphere(4))
        opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_rand_1_bin
        Me.CheckOptUsingSphere(opt)

        opt = New clsOptDE(New clsBenchSphere(5))
        opt.DEStrategy = clsOptDE.EnumDEStrategyType.DE_rand_2_bin
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationJADE()
        Dim opt = New clsOptDEJADE(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationES()
        Dim opt = New clsOptES(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationFA()
        Dim opt = New clsOptFA(New clsBenchSphere(2))
        opt.Iteration = 300
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationNM()
        Dim opt = New clsOptNelderMead(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationNMWiki()
        Dim opt = New clsOptCS(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationNewton()
        Dim opt = New clsOptNewtonMethod(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationPS()
        Dim opt = New clsOptPatternSearch(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationPSO()
        Dim opt = New clsOptPSO(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationPSOAIW()
        Dim opt = New clsOptPSOAIW(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationPSOCIW()
        Dim opt = New clsOptPSOChaoticIW(New clsBenchSphere(2))
        opt.ChaoticMode = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CDIW
        Me.CheckOptUsingSphere(opt)

        opt = New clsOptPSOChaoticIW(New clsBenchSphere(2))
        opt.ChaoticMode = clsOptPSOChaoticIW.EnumChaoticInertiaWeightMode.CRIW
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationLDIW()
        Dim opt = New clsOptPSOLDIW(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationRGABLX()
        Dim opt = New clsOptRealGABLX(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationRGAPCX()
        Dim opt = New clsOptRealGAPCX(New clsBenchSphere(2))
        opt.Iteration = 1000
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationRGAREX()
        Dim opt = New clsOptRealGAREX(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizatioRGASPX()
        Dim opt = New clsOptRealGASPX(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationRGAUNDX()
        Dim opt = New clsOptRealGAUNDX(New clsBenchSphere(2))
        opt.PopulationSize = 100
        opt.ChildrenSize = 50
        opt.Iteration = 700
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationSA()
        Dim isOnceSuccess As Boolean = False
        For i As Integer = 0 To 10 - 1
            Dim opt = New clsOptSimulatedAnnealing(New clsBenchSphere(2))

            'check init
            opt.Init()
            Dim errorFlg = opt.IsRecentError()
            Assert.IsFalse(errorFlg)

            'check iterate
            opt.DoIteration()
            errorFlg = opt.IsRecentError()
            Assert.IsFalse(errorFlg)

            'Eval
            Dim isSuccess As Boolean = True
            If Math.Abs(opt.Result.Eval) > 0.05 Then
                isSuccess = isSuccess And False
            Else
                isSuccess = isSuccess And True
            End If

            'Result
            If Math.Abs(opt.Result(0)) > 0.05 OrElse Math.Abs(opt.Result(1)) > 0.05 Then
                isSuccess = isSuccess And False
            Else
                isSuccess = isSuccess And True
            End If

            isOnceSuccess = isOnceSuccess Or isSuccess
        Next

        Assert.IsTrue(isOnceSuccess, "fail SimulatedAnnealing")
    End Sub

    <TestMethod()> Public Sub TestOptimizationSD()
        Dim opt = New clsOptSteepestDescent(New clsBenchSphere(2))
        Me.CheckOptUsingSphere(opt)
    End Sub

    <TestMethod()> Public Sub TestOptimizationDEWithBound()
        Dim opt As New clsOptDE(New clsBenchTest2())
        'x1-> 0 to 5, x2-> 0 to 5
        opt.LowerBounds = New Double() {0, 0}
        opt.UpperBounds = New Double() {5, 5}
        opt.Init()
        Dim errorFlg = opt.IsRecentError()
        Assert.IsFalse(errorFlg)

        'check iterate
        opt.DoIteration()
        errorFlg = opt.IsRecentError()
        Assert.IsFalse(errorFlg)

        'Eval
        If -78.99 < opt.Result.Eval AndAlso opt.Result.Eval < -78.98 Then
            'OK
        Else
            Assert.Fail(String.Format("fail Eval {0}", opt.Result.Eval))
        End If
        Console.WriteLine(String.Format("Success Eval {0}", opt.Result.Eval))

        'Result
        If 2.8 < opt.Result(0) AndAlso opt.Result(0) < 2.9 Then
            'OK
        Else
            Assert.Fail(String.Format("fail Result {0} {1}", opt.Result(0), opt.Result(1)))
        End If
        Console.WriteLine(String.Format("Success Result {0} {1}", opt.Result(0), opt.Result(1)))
    End Sub

#End Region
End Class