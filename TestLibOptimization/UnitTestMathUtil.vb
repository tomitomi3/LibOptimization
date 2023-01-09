Imports System.Text
Imports LibOptimization.MathTool
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass> Public Class UnitTestMathUtil
    Public Const delta = 0.0000000001

    <TestMethod()> Public Sub CoVariance()
        Dim v1 = New DenseVector(New Double() {50, 60, 70, 80, 90})
        Dim v2 = New DenseVector(New Double() {40, 70, 90, 60, 100})
        Dim temp = MathUtil.CoVar(v1, v2)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 220.0, delta)
    End Sub

    <TestMethod()> Public Sub CoVarianceMatrix()
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

    <TestMethod()> Public Sub Variance()
        Dim v1 = New DenseVector(New Double() {1, 2, 3, 4, 5})
        Dim temp = MathUtil.Var(v1)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 2.0, delta)
    End Sub

    <TestMethod()> Public Sub Stddev()
        Dim v1 = New DenseVector(New Double() {1, 2, 3, 4, 5})
        Dim temp = MathUtil.Stddev(v1)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 1.4142135623731, delta)
    End Sub

    <TestMethod()> Public Sub Correlation()
        Dim v1 = New DenseVector(New Double() {1, 2, 3, 4, 5})
        Dim v2 = v1 * 2
        Dim temp = MathUtil.Cor(v1, v2)
        Console.WriteLine(temp)
        Assert.AreEqual(temp, 1.0, delta)
    End Sub


    <TestMethod()> Public Sub TestIsClose()
        '最小単位の比較
        Dim v1 = Double.Epsilon
        Dim v2 = Double.Epsilon
        Dim flg = MathUtil.IsCloseToValues(v1, v2, Double.Epsilon) 'Double.Epsilon未満
        Assert.IsTrue(flg)

        v1 = Double.Epsilon
        v2 = Double.Epsilon * 2
        flg = MathUtil.IsCloseToValues(v1, v2, Double.Epsilon) 'Double.Epsilonを超える
        Assert.IsFalse(flg)
    End Sub

    <TestMethod()> Public Sub TestPythagoreanAddition()
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
