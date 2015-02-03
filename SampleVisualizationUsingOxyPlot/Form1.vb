Imports LibOptimization
Imports OxyPlot.Series
Imports OxyPlot
Imports LibOptimization.Optimization

Public Class Form1
    Private opt As Optimization.clsOptNelderMead = Nothing
    Private vertexs As New List(Of List(Of List(Of Double)))
    Private evals As New List(Of List(Of Double))
    Private indexSimplex As Integer = 0

    Declare Function AllocConsole Lib "kernel32.dll" () As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'init plot
        Me.oPlot.Model = New OxyPlot.PlotModel()
        Me.oPlot.Model.PlotType = OxyPlot.PlotType.XY
        'Dim Series = New OxyPlot.Series.FunctionSeries(Function(x As Double) As Double
        '                                                   Return 1 / Math.Sqrt(2 * Math.PI) * Math.Exp(-x * x / 2)
        '                                               End Function, -100, 100, 0.1, "標準正規分布"
        'init opt
        Me.opt = New clsOptNelderMead(New BenchmarkFunction.clsBenchRosenblock(2))
        AllocConsole()
    End Sub

    Private Sub btnInit_Click(sender As Object, e As EventArgs) Handles btnInit.Click
        'init opt
        Me.opt.Init(New Double()() {New Double() {0, 0}, New Double() {0, 1}, New Double() {1, 0}})

        'Get simplex history
        vertexs.Clear()
        evals.Clear()
        Dim tempSimplex As New List(Of List(Of Double))
        Dim tempSimplexEvals As New List(Of Double)
        For Each p As clsPoint In CType(opt, clsOptNelderMead).AllVertexs
            Dim tempPoint As New clsPoint(p)
            tempSimplex.Add(tempPoint)
            tempSimplexEvals.Add(tempPoint.Eval)
        Next
        Me.vertexs.Add(tempSimplex)
        Me.evals.Add(tempSimplexEvals)
        While (Me.opt.DoIteration(1) = False)
            tempSimplex = New List(Of List(Of Double))
            tempSimplexEvals = New List(Of Double)
            For Each p As clsPoint In CType(opt, clsOptNelderMead).AllVertexs
                Dim tempPoint As New clsPoint(p)
                tempSimplex.Add(tempPoint)
                tempSimplexEvals.Add(tempPoint.Eval)
            Next
            Me.vertexs.Add(tempSimplex)
            Me.evals.Add(tempSimplexEvals)
        End While
        Me.indexSimplex = 0

        InitAxis(1.5, 1.5)
    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        If indexSimplex <= 0 Then
            Return
        Else
            indexSimplex -= 1
        End If
        Draw()
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If indexSimplex + 1 >= vertexs.Count Then
            Return
        Else
            indexSimplex += 1
        End If
        Draw()
    End Sub

    Private Sub InitAxis(ByVal ai_x As Double, ByVal ai_y As Double)
        Me.oPlot.Model.Series.Clear()
        Me.oPlot.Model.Axes.Clear()

        Dim x = New Axes.LinearAxis()
        x.Position = Axes.AxisPosition.Bottom
        x.Minimum = -ai_x
        x.Maximum = ai_x
        'x.MajorGridlineStyle = LineStyle.Automatic
        'x.MajorGridlineThickness = 0.5
        'x.MaximumPadding = 0
        'x.MinimumPadding = 0
        Me.oPlot.Model.Axes.Add(x)

        Dim y = New Axes.LinearAxis()
        y.Position = Axes.AxisPosition.Left
        y.Minimum = -ai_y
        y.Maximum = ai_y
        'y.MajorGridlineStyle = LineStyle.Automatic
        'y.MaximumPadding = 0
        'y.MinimumPadding = 0
        Me.oPlot.Model.Axes.Add(y)

        Me.oPlot.InvalidatePlot(True) 'Me.oPlot.OnModelChanged()
    End Sub

    Private Sub Draw()
        Me.oPlot.Model.Series.Clear()

        'Point
        Dim points = New ScatterSeries()
        points.Points.Add(New ScatterPoint(1, 1))
        points.Points.Add(New ScatterPoint(1, 1))
        points.MarkerSize = 2
        points.TextColor = OxyColors.Black
        Me.oPlot.Model.Series.Add(points)

        'Line
        Dim firstSeries = New LineSeries()
        firstSeries.Points.Add(New DataPoint(Me.vertexs(indexSimplex)(0)(0), Me.vertexs(indexSimplex)(0)(1)))
        firstSeries.Points.Add(New DataPoint(Me.vertexs(indexSimplex)(1)(0), Me.vertexs(indexSimplex)(1)(1)))
        firstSeries.Points.Add(New DataPoint(Me.vertexs(indexSimplex)(2)(0), Me.vertexs(indexSimplex)(2)(1)))
        firstSeries.Points.Add(New DataPoint(Me.vertexs(indexSimplex)(0)(0), Me.vertexs(indexSimplex)(0)(1)))
        firstSeries.Color = OxyColors.Red
        firstSeries.StrokeThickness = 1
        Me.oPlot.Model.Series.Add(firstSeries)

        Console.WriteLine("{0} , {1} , {2}", Me.evals(Me.indexSimplex)(0), Me.evals(Me.indexSimplex)(1), Me.evals(Me.indexSimplex)(2))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(0)(0), vertexs(Me.indexSimplex)(0)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(1)(0), vertexs(Me.indexSimplex)(1)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(2)(0), vertexs(Me.indexSimplex)(2)(1))

        Me.oPlot.InvalidatePlot(True) 'Me.oPlot.OnModelChanged()
    End Sub
End Class
