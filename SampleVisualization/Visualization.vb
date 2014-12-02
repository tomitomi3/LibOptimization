Imports System.Drawing.Drawing2D

Public Class Visualization
    Private opt As LibOptimization.absOptimization = Nothing
    Private vertexs As New List(Of List(Of List(Of Double)))
    Private evals As New List(Of List(Of Double))
    Private indexSimplex As Integer = 0

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'ローゼンブロック関数
        Me.opt = New LibOptimization.clsOptNelderMead(New clsBenchRosenblock(2))

        'Init my graphctrl
        Me.graph.Init(-1, -1, 2)
    End Sub

    Private Sub btnInit_Click(sender As System.Object, e As System.EventArgs) Handles btnInit.Click
        'Init Random
        LibOptimization.clsRandomXorshiftSingleton.GetInstance().SetSeed()

        'Init optimization class
        CType(Me.opt, LibOptimization.clsOptNelderMead).Init(New Double()() {New Double() {0, 0}, New Double() {0, 1}, New Double() {1, 0}})

        'Get simplex history
        vertexs.Clear()
        evals.Clear()
        Dim tempSimplex As New List(Of List(Of Double))
        Dim tempSimplexEvals As New List(Of Double)
        For Each p As LibOptimization.clsPoint In CType(opt, LibOptimization.clsOptNelderMead).AllVertexs
            Dim tempPoint As New LibOptimization.clsPoint(p)
            tempSimplex.Add(tempPoint)
            tempSimplexEvals.Add(tempPoint.Eval)
        Next
        Me.vertexs.Add(tempSimplex)
        Me.evals.Add(tempSimplexEvals)
        While (Me.opt.DoIteration(1) = False)
            tempSimplex = New List(Of List(Of Double))
            tempSimplexEvals = New List(Of Double)
            For Each p As LibOptimization.clsPoint In CType(opt, LibOptimization.clsOptNelderMead).AllVertexs
                Dim tempPoint As New LibOptimization.clsPoint(p)
                tempSimplex.Add(tempPoint)
                tempSimplexEvals.Add(tempPoint.Eval)
            Next
            Me.vertexs.Add(tempSimplex)
            Me.evals.Add(tempSimplexEvals)
        End While
        Me.indexSimplex = 0

        Console.WriteLine("{0} , {1} , {2}", Me.evals(Me.indexSimplex)(0), Me.evals(Me.indexSimplex)(1), Me.evals(Me.indexSimplex)(2))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(0)(0), vertexs(Me.indexSimplex)(0)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(1)(0), vertexs(Me.indexSimplex)(1)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(2)(0), vertexs(Me.indexSimplex)(2)(1))

        Me.lblIterationCount.Text = "Count:" & vertexs.Count.ToString()
        Me.graph.Init(-1, -1, 2)
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(0).ToArray(), Color.Red)
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(1).ToArray())
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(2).ToArray())
        Me.graph.AddPolyLine(vertexs(Me.indexSimplex))
        Me.graph.RefreshPbx(True)
    End Sub

    Private Sub btnBack_Click(sender As System.Object, e As System.EventArgs) Handles btnBack.Click
        If Me.NextIteration() = False Then
            Return
        End If

        Console.WriteLine("{0} , {1} , {2}", Me.evals(Me.indexSimplex)(0), Me.evals(Me.indexSimplex)(1), Me.evals(Me.indexSimplex)(2))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(0)(0), vertexs(Me.indexSimplex)(0)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(1)(0), vertexs(Me.indexSimplex)(1)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(2)(0), vertexs(Me.indexSimplex)(2)(1))

        Me.graph.AddPoint(vertexs(Me.indexSimplex)(0).ToArray(), Color.Red)
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(1).ToArray())
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(2).ToArray())
        Me.graph.AddPolyLine(vertexs(Me.indexSimplex))
        Me.graph.RefreshPbx(True)
    End Sub

    Private Sub btnNext_Click(sender As System.Object, e As System.EventArgs) Handles btnNext.Click
        If Me.BackIteration() = False Then
            Return
        End If

        Console.WriteLine("{0} , {1} , {2}", Me.evals(Me.indexSimplex)(0), Me.evals(Me.indexSimplex)(1), Me.evals(Me.indexSimplex)(2))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(0)(0), vertexs(Me.indexSimplex)(0)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(1)(0), vertexs(Me.indexSimplex)(1)(1))
        Console.WriteLine(" {0} , {1}", vertexs(Me.indexSimplex)(2)(0), vertexs(Me.indexSimplex)(2)(1))

        Me.graph.AddPoint(vertexs(Me.indexSimplex)(0).ToArray(), Color.Red)
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(1).ToArray())
        Me.graph.AddPoint(vertexs(Me.indexSimplex)(2).ToArray())
        Me.graph.AddPolyLine(vertexs(Me.indexSimplex))
        Me.graph.RefreshPbx(True)
    End Sub

    Private Function NextIteration() As Boolean
        If indexSimplex <= 0 Then
            Return False
        End If
        indexSimplex -= 1
        Me.lblStep.Text = "Step:" & indexSimplex.ToString()

        Return True
    End Function

    Private Function BackIteration() As Boolean
        If indexSimplex + 1 >= vertexs.Count Then
            Return False
        End If
        indexSimplex += 1
        Me.lblStep.Text = "Step:" & indexSimplex.ToString()

        Return True
    End Function
End Class
