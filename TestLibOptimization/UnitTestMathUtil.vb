Imports System.Text
Imports LibOptimization.MathTool
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass> Public Class UnitTestMathUtil
    Public Const delta = 0.0000000001

    <TestMethod()> Public Sub CoVarianceTest()
        Dim v1 = New DenseVector(New Double() {50, 60, 70, 80, 90})
        Dim v2 = New DenseVector(New Double() {40, 70, 90, 60, 100})
        Dim temp = MathUtil.CoVariance(v1, v2)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 220.0, delta)
    End Sub

    <TestMethod()> Public Sub CreateCovarianceMatrixTest()
        Dim v1 = New DenseVector(New Double() {50, 60, 70, 80, 90})
        Dim v2 = New DenseVector(New Double() {40, 70, 90, 60, 100})

        With Nothing
            Dim mat = New DenseMatrix()
            mat.AddRow(v1)
            mat.AddRow(v2)
            mat = MathUtil.CreateCovarianceMatrix(mat, True)
        End With

        With Nothing
            Dim mat = New DenseMatrix()
            mat.AddRow(v1)
            mat.AddRow(v2)
            mat = MathUtil.CreateCovarianceMatrix(mat, False)
        End With

        With Nothing
            Dim mat = New DenseMatrix()
            mat.AddCol(v1)
            mat.AddCol(v2)
            mat = MathUtil.CreateCovarianceMatrix(mat, True)
        End With

        With Nothing
            Dim mat = New DenseMatrix()
            mat.AddCol(v1)
            mat.AddCol(v2)
            mat = MathUtil.CreateCovarianceMatrix(mat, False)
        End With
    End Sub

    <TestMethod()> Public Sub VarianceTest()
        Dim v1 = New DenseVector(New Double() {1, 2, 3, 4, 5})
        Dim temp = MathUtil.Variance(v1)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 2.0, delta)
    End Sub

    <TestMethod()> Public Sub StddevTest()
        Dim v1 = New DenseVector(New Double() {1, 2, 3, 4, 5})
        Dim temp = MathUtil.Stddev(v1)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 1.4142135623731, delta)
    End Sub

    <TestMethod()> Public Sub CorrelationTest()
        Dim v1 = New DenseVector(New Double() {1, 2, 3, 4, 5})
        Dim v2 = v1 * 2
        Dim temp = MathUtil.Correlation(v1, v2)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 1.0, delta)
    End Sub

    <TestMethod()> Public Sub IsSameMatrixTest()
        Dim matA = MathUtil.CreateRandomASymmetricMatrix(3)

        'zero
        Dim flg = MathUtil.IsSameMatrix(matA, matA)
        Assert.IsTrue(flg)

        'nearly
        Dim matB = New DenseMatrix(matA)
        matB(0)(0) = matB(0)(0) + 0.5
        flg = MathUtil.IsSameMatrix(matA, matB, 0.5)
        Assert.IsTrue(flg)

        'eps is zero
        flg = MathUtil.IsSameMatrix(matA, matA, 0)
        Assert.IsTrue(flg)

        flg = MathUtil.IsSameMatrix(matA, matB, 0)
        Assert.IsFalse(flg)
    End Sub

    <TestMethod()> Public Sub IsSameValuesTest()
        Dim flg = MathUtil.IsSameValues(1, 1)
        Assert.IsTrue(flg)

        flg = MathUtil.IsSameValues(0, 0)
        Assert.IsTrue(flg)

        flg = MathUtil.IsSameValues(-1, -1)
        Assert.IsTrue(flg)

        flg = MathUtil.IsSameValues(1, -1)
        Assert.IsFalse(flg)

        flg = MathUtil.IsSameValues(1, -1, 0.5)
        Assert.IsFalse(flg)

        '誤差を0に指定
        flg = MathUtil.IsSameValues(1, 1, 0)
        Assert.IsTrue(flg)
    End Sub

    <TestMethod()> Public Sub IsSameValuesTest_minimumValue()
        '最小単位の比較
        Dim v1 = Double.Epsilon
        Dim v2 = Double.Epsilon
        Dim flg = MathUtil.IsSameValues(v1, v2, Double.Epsilon) 'Double.Epsilon以下
        Assert.IsTrue(flg)

        v1 = Double.Epsilon
        v2 = Double.Epsilon * 2.0
        flg = MathUtil.IsSameValues(v1, v2, Double.Epsilon) 'Double.Epsilon以下
        Assert.IsTrue(flg)

        v1 = Double.Epsilon
        v2 = Double.Epsilon * 3.0
        flg = MathUtil.IsSameValues(v1, v2, Double.Epsilon) 'Double.Epsilon以下
        Assert.IsFalse(flg)
    End Sub

    <TestMethod()> Public Sub IsSameVecotrTest()
        Dim vecA = New DenseVector(New Double() {1, 2, 3})

        'zero
        Dim flg = MathUtil.IsSameVecotr(vecA, vecA)
        Assert.IsTrue(flg)

        'nearly
        Dim vecB = New DenseVector(vecA)
        vecB(0) = vecB(0) + 0.5
        flg = MathUtil.IsSameVecotr(vecA, vecB, 0.5)
        Assert.IsTrue(flg)

        'eps is zero
        flg = MathUtil.IsSameVecotr(vecA, vecA, 0)
        Assert.IsTrue(flg)

        flg = MathUtil.IsSameVecotr(vecA, vecB, 0)
        Assert.IsFalse(flg)
    End Sub

    <TestMethod()> Public Sub IsSameZeroTest()
        Dim val = 0.0
        Assert.IsTrue(MathUtil.IsSameZero(val), String.Format("error {0}", val.ToString()))

        val = 1.0
        Assert.IsFalse(MathUtil.IsSameZero(val), String.Format("error {0}", val.ToString()))

        val = -1.0
        Assert.IsFalse(MathUtil.IsSameZero(val), String.Format("error {0}", val.ToString()))

        '誤差と同じ 以下なので含む
        val = ConstantValues.MachineEpsiron
        Assert.IsTrue(MathUtil.IsSameZero(val, ConstantValues.MachineEpsiron), String.Format("error {0}", val.ToString()))

        val = -ConstantValues.MachineEpsiron
        Assert.IsTrue(MathUtil.IsSameZero(val, ConstantValues.MachineEpsiron), String.Format("error {0}", val.ToString()))
    End Sub

    <TestMethod()> Public Sub PythagoreanAdditionTest()
        Dim length = Math.Sqrt(1 + 1)

        ' check
        With Nothing
            For i = 1 To 11 - 1
                Dim valA = i
                Dim valB = Math.Pow(i, i * 10.0)

                'check
                Dim normal = Math.Sqrt(valA ^ 2 + valB ^ 2)
                Dim calc = 0.0
                calc = MathUtil.PythagoreanAddition(valA, valB)
                Assert.AreEqual(normal, calc)
                calc = MathUtil.PythagoreanAddition(valB, valA)
                Assert.AreEqual(normal, calc)
            Next
        End With

        With Nothing
            Dim valA = Math.Pow(5.2345, 400)
            Dim valB = Math.Pow(5.2345, 400)

            'check
            Dim source = Math.Sqrt(valA ^ 2 + valB ^ 2)

            ' infになる
            Assert.IsTrue(Double.IsInfinity(source))

            ' 計算できる
            Dim calc1 = MathUtil.PythagoreanAddition(valA, valB)
            Assert.IsTrue(Not Double.IsInfinity(calc1))

            Dim calc2 = MathUtil.PythagoreanAddition(valB, valA)
            Assert.IsTrue(Not Double.IsInfinity(calc2))

            Assert.AreEqual(calc1, calc2)
        End With
    End Sub

End Class
