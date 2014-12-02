''' <summary>
''' Draw Parameter
''' </summary>
''' <remarks></remarks>
Public MustInherit Class absDrawParameter
    Protected m_type As DrawType = DrawType.No_Set
    Protected m_points As New List(Of System.Drawing.Point)
    Protected m_color As System.Drawing.Color = Color.Black

    Public Enum DrawType
        No_Set
        Point
        Line
        Polygon
        Circle
    End Enum

    Public Sub New()
        'nop
    End Sub

    Public Sub SetPoint(ByVal ai_point As System.Drawing.Point)
        Me.m_points.Clear()
        Me.m_points.Add(ai_point)
    End Sub

    Public Sub SetPoint(ByVal ai_point() As Integer)
        If ai_point.Length < 2 Then
            Return
        End If
        Me.m_points.Clear()
        Me.m_points.Add(New System.Drawing.Point(ai_point(0), ai_point(1)))
    End Sub

    Public Sub AddPoint(ByVal ai_point As System.Drawing.Point)
        Me.m_points.Add(ai_point)
    End Sub

    Public Sub AddPoint(ByVal ai_point() As Integer)
        If ai_point.Length < 2 Then
            Return
        End If
        Me.m_points.Add(New System.Drawing.Point(ai_point(0), ai_point(1)))
    End Sub

    Public Sub SetColor(ByVal ai_color As Color)
        Me.m_color = ai_color
    End Sub

    Public Sub SetColor(ByVal ai_R As Integer, ByVal ai_G As Integer, ByVal ai_B As Integer, Optional ByVal ai_alpha As Integer = 100)
        Me.m_color = Color.FromArgb(ai_alpha, ai_R, ai_G, ai_B)
    End Sub

    Public Function GetDrawType() As DrawType
        Return Me.m_type
    End Function

    Public MustOverride Sub Draw(ByRef ai_g As System.Drawing.Graphics)
End Class

''' <summary>
''' Point
''' </summary>
''' <remarks></remarks>
Public Class clsDrawPoint : Inherits absDrawParameter
    Public Sub New()
        MyBase.m_type = DrawType.Point
    End Sub

    Private m_size As Integer = 1
    Public Sub SetSize(ByVal ai_size As Integer)
        Me.m_size = ai_size
    End Sub

    Public Overrides Sub Draw(ByRef ai_g As System.Drawing.Graphics)
        Dim brs As New System.Drawing.SolidBrush(Me.m_color)
        ai_g.FillEllipse(brs, New Rectangle(Me.m_points(0).X, Me.m_points(0).Y, Me.m_size, Me.m_size))
        brs.Dispose()
    End Sub
End Class

''' <summary>
''' Line
''' </summary>
''' <remarks></remarks>
Public Class clsDrawLine : Inherits absDrawParameter
    Public Sub New()
        MyBase.m_type = DrawType.Line
    End Sub

    Private m_size As Integer = 1
    Public Sub SetSize(ByVal ai_size As Integer)
        Me.m_size = ai_size
    End Sub

    Private m_isClose As Boolean = False
    Public Sub SetClose(ByVal ai_isClose As Boolean)
        Me.m_isClose = ai_isClose
    End Sub

    Public Overrides Sub Draw(ByRef ai_g As System.Drawing.Graphics)
        If Me.m_points.Count <= 1 Then
            Return
        End If
        Dim tempPen As New Pen(Me.m_color, Me.m_size)

        If Me.m_isClose Then
            ai_g.DrawLines(tempPen, m_points.ToArray())
            ai_g.DrawLine(tempPen, m_points(m_points.Count - 1), m_points(0))
        Else
            ai_g.DrawLines(tempPen, m_points.ToArray())
        End If
        tempPen.Dispose()
    End Sub
End Class

''' <summary>
''' Polygon
''' </summary>
''' <remarks></remarks>
Public Class clsDrawPolygon : Inherits absDrawParameter
    Public Sub New()
        MyBase.m_type = DrawType.Polygon
    End Sub

    Private m_isFill As Boolean = False
    Public Sub SetFill(ByVal ai_fill As Boolean)
        Me.m_isFill = ai_fill
    End Sub

    Private m_size As Integer = 1
    Public Sub SetSize(ByVal ai_size As Integer)
        Me.m_size = ai_size
    End Sub

    Public Overrides Sub Draw(ByRef ai_g As System.Drawing.Graphics)

    End Sub
End Class

''' <summary>
''' Circle
''' </summary>
''' <remarks></remarks>
Public Class clsDrawCircle : Inherits absDrawParameter
    Public Sub New()
        MyBase.m_type = DrawType.Circle
    End Sub

    Private m_isFill As Boolean = False
    Public Sub SetFill(ByVal ai_fill As Boolean)
        Me.m_isFill = ai_fill
    End Sub

    Private m_rx As Integer = 1
    Private m_ry As Integer = 1
    Public Sub SetParam(ByVal ai_rx As Integer, ByVal ai_ry As Integer)
        Me.m_rx = ai_rx
        Me.m_ry = ai_ry
    End Sub

    Private m_size As Integer = 1
    Public Sub SetSize(ByVal ai_size As Integer)
        Me.m_size = ai_size
    End Sub

    Public Overrides Sub Draw(ByRef ai_g As System.Drawing.Graphics)

    End Sub
End Class
