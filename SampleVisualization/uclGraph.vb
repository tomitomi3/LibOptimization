Imports System.Drawing.Drawing2D

Public Class uclGraph
    Private m_isInitilize As Boolean = False
    Private lbx As Double = 0.0
    Private lby As Double = 0.0
    Private rtx As Double = 0.0
    Private rty As Double = 0.0
    Private range As Double = 0.0

    Dim canvas As Bitmap = Nothing
    Dim m_drawStack As New List(Of absDrawParameter)
    Dim m_previousDrawStack As List(Of absDrawParameter)

    Declare Function AllocConsole Lib "kernel32.dll" () As Boolean

    ''' <summary>
    ''' デフォルトコンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。
        AllocConsole()
        Me.canvas = New Bitmap(Me.pbxGraph.Width, Me.pbxGraph.Height)
    End Sub

    ''' <summary>
    ''' 初期化
    ''' </summary>
    ''' <param name="ai_lbx"></param>
    ''' <param name="ai_lby"></param>
    ''' <param name="ai_range"></param>
    ''' <remarks></remarks>
    Public Sub Init(ByVal ai_lbx As Double, ByVal ai_lby As Double, ByVal ai_range As Double)
        Me.m_isInitilize = True
        Me.lbx = ai_lbx
        Me.lby = ai_lby
        Me.rtx = ai_lbx + ai_range
        Me.rty = ai_lby + ai_range
        Me.range = ai_range
        Me.UpdateAxisLable()
    End Sub

    ''' <summary>
    ''' Point
    ''' </summary>
    ''' <param name="ai_p"></param>
    ''' <param name="ai_color"></param>
    ''' <remarks></remarks>
    Public Sub AddPoint(ByVal ai_p As Double(), ByVal ai_color As Color)
        Dim tempDraw As New clsDrawPoint()
        tempDraw.SetSize(5)
        tempDraw.SetColor(ai_color)
        tempDraw.AddPoint(New Integer() {Me.TransformX(ai_p(0)) - 2, Me.TransformY(ai_p(1)) - 2})
        m_drawStack.Add(tempDraw)
    End Sub

    Public Sub AddPoint(ByVal ai_p As Double())
        Dim tempDraw As New clsDrawPoint()
        tempDraw.SetSize(5)
        tempDraw.AddPoint(New Integer() {Me.TransformX(ai_p(0)) - 2, Me.TransformY(ai_p(1)) - 2})
        m_drawStack.Add(tempDraw)
    End Sub

    Public Sub AddPolyLine(ByVal ai_p As Double()())
        Dim tempDraw As New clsDrawLine()
        tempDraw.SetSize(1)
        tempDraw.SetClose(True)
        For i As Integer = 0 To ai_p.Length - 1
            tempDraw.AddPoint(New Integer() {Me.TransformX(ai_p(i)(0)), Me.TransformY(ai_p(i)(1))})
        Next
        m_drawStack.Add(tempDraw)
    End Sub

    Sub AddPolyLine(ByVal ai_p As List(Of List(Of Double)))
        Dim tempDraw As New clsDrawLine()
        tempDraw.SetSize(1)
        tempDraw.SetClose(True)
        For i As Integer = 0 To ai_p.Count - 1
            tempDraw.AddPoint(New Integer() {Me.TransformX(ai_p(i)(0)), Me.TransformY(ai_p(i)(1))})
        Next
        m_drawStack.Add(tempDraw)
    End Sub

    ''' <summary>
    ''' Refresh Picture Box
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RefreshPbx(ByVal ai_isClear As Boolean)
        If Me.canvas Is Nothing Then
            Me.canvas = New Bitmap(Me.pbxGraph.Width, Me.pbxGraph.Height)
        End If

        Me.m_drawStack.Reverse()
        Dim g As Graphics = Graphics.FromImage(canvas)
        Try
            'Erase
            g.FillRectangle(Brushes.White, 0, 0, Me.pbxGraph.Width, Me.pbxGraph.Height)

            'Base
            AddPolyLine(New List(Of List(Of Double))({New List(Of Double)({Me.lbx, 0}), New List(Of Double)({Me.rtx, 0})}))
            AddPolyLine(New List(Of List(Of Double))({New List(Of Double)({0, Me.lby}), New List(Of Double)({0, Me.rty})}))

            'Stack
            For Each dope As absDrawParameter In Me.m_drawStack
                dope.Draw(g)
            Next

            ''Copy
            'm_previousDrawStack = New List(Of absDrawParameter)(m_drawStack)

            Me.pbxGraph.Image = Me.canvas
        Finally
            g.Dispose()
            If ai_isClear = True Then
                Me.m_drawStack.Clear()
            End If
        End Try
    End Sub

    Private Sub UpdateAxisLable()
        Dim temp As Double = 0.0

        'X axis lable
        Me.lblXLeft.Text = Me.lbx.ToString()
        temp = Math.Abs(Me.rtx - Me.lbx) / 2.0 + Me.lbx
        Me.lblXCenter.Text = temp.ToString()
        Me.lblXRight.Text = Me.rtx.ToString()

        'Y axis lable
        Me.lblYTop.Text = Me.rty.ToString()
        temp = Math.Abs(Me.rty - Me.lby) / 2.0 + Me.lby
        Me.lblYCenter.Text = temp.ToString()
        Me.lblYBottom.Text = Me.lby.ToString()
    End Sub

#Region "GraphicOperators"
    'http://blog.livedoor.jp/hisashi327/archives/1849655.html

#End Region

#Region "Graphic Transform"
    ''' <summary>
    ''' 直交座標 X->Pixel
    ''' </summary>
    ''' <param name="ai_x"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TransformX(ByVal ai_x As Double) As Integer
        Dim sf As Double = Me.pbxGraph.Width / Me.range
        Dim diff As Double = ai_x - Me.lbx
        Dim temp As Integer = 0
        If ai_x < Me.lbx Then
            temp = diff * sf
        Else
            temp = Math.Abs(diff * sf)
        End If
        Return temp
    End Function

    ''' <summary>
    ''' 直交座標 Y->Pixel
    ''' </summary>
    ''' <param name="ai_y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TransformY(ByVal ai_y As Double) As Integer
        Dim sf As Double = Me.pbxGraph.Width / Me.range
        Dim diff As Double = ai_y - Me.lby
        Dim temp As Integer = 0
        If ai_y < Me.lby Then
            temp = diff * sf
        Else
            temp = Math.Abs(diff * sf)
        End If
        Return Me.pbxGraph.Height - temp
    End Function
#End Region

#Region "Event Drag&Drop"
    Private isDragNow As Boolean = False
    Private dragStartX As Integer = 0
    Private dragStartY As Integer = 0
    Private dragStartLBX As Double = 0.0
    Private dragStartLBY As Double = 0.0

    Private Sub pbxGraph_MouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles pbxGraph.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            dragStartX = e.X
            dragStartY = Me.pbxGraph.Height - e.Y
            dragStartLBX = Me.lbx
            dragStartLBY = Me.lby
            Console.WriteLine(dragStartX.ToString() & " , " & dragStartY.ToString())
        End If
        isDragNow = False
    End Sub

    Private Sub pbxGraph_MouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles pbxGraph.MouseMove
        If e.Button <> Windows.Forms.MouseButtons.Left Then
            Return
        End If
        isDragNow = True
        Dim sf As Double = Me.pbxGraph.Width / Me.range
        Dim relativeDiffX As Double = (e.X - dragStartX) / sf
        Dim relativeDiffY As Double = (Me.pbxGraph.Height - e.Y - dragStartY) / sf
        Console.WriteLine(relativeDiffX.ToString() & "," & relativeDiffY.ToString())
        Me.Init(dragStartLBX - relativeDiffX, dragStartLBY - relativeDiffY, Me.range)
        Me.RefreshPbx(True)
    End Sub

    Private Sub pbxGraph_MouseUp(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles pbxGraph.MouseUp
        If isDragNow = True Then
            'If e.X < 0 OrElse e.Y < 0 Then
            '    Return
            'End If
            'Me.Init(dragStartLBX, dragStartLBY, Me.range)
            'Me.RefreshPbx(True)
        End If
    End Sub

    Private Sub pbxGraph_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles pbxGraph.MouseEnter
        pbxGraph.Focus()
    End Sub

    Private Sub pbxGraph_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbxGraph.MouseWheel
        Dim sf As Double = Me.pbxGraph.Width / Me.range
        Dim relativeDiffX As Double = e.X / sf
        Dim relativeDiffY As Double = (Me.pbxGraph.Height - e.Y) / sf
        Dim x As Double = lbx + relativeDiffX
        Dim y As Double = lby + relativeDiffY
        Console.WriteLine(x.ToString() & "," & y.ToString())
        If e.Delta < 0 Then
            Dim rangeDiff As Double = 0.1
            If (Me.range - 0.1) > 0 Then
                Me.Init(lbx + rangeDiff / 2, lby + rangeDiff / 2, Me.range - 0.1)
            End If
        ElseIf e.Delta > 0 Then
            Dim rangeDiff As Double = 0.1
            Me.Init(lbx + rangeDiff / 2, lby + rangeDiff / 2, Me.range + 0.1)
        End If
        Me.RefreshPbx(True)
    End Sub

    Private Sub pbxGraph_Resize(sender As System.Object, e As System.EventArgs) Handles pbxGraph.Resize
        Me.Init(1, 1, 1)
    End Sub
#End Region
End Class
