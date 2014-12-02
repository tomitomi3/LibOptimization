<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class uclGraph
    Inherits System.Windows.Forms.UserControl

    'UserControl はコンポーネント一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblYCenter = New System.Windows.Forms.Label()
        Me.lblXCenter = New System.Windows.Forms.Label()
        Me.lblYTop = New System.Windows.Forms.Label()
        Me.lblXRight = New System.Windows.Forms.Label()
        Me.lblXLeft = New System.Windows.Forms.Label()
        Me.pbxGraph = New System.Windows.Forms.PictureBox()
        Me.lblYBottom = New System.Windows.Forms.Label()
        CType(Me.pbxGraph, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblYCenter
        '
        Me.lblYCenter.Location = New System.Drawing.Point(22, 298)
        Me.lblYCenter.Name = "lblYCenter"
        Me.lblYCenter.Size = New System.Drawing.Size(58, 10)
        Me.lblYCenter.TabIndex = 9
        Me.lblYCenter.Text = "( , )"
        Me.lblYCenter.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblXCenter
        '
        Me.lblXCenter.Location = New System.Drawing.Point(379, 607)
        Me.lblXCenter.Name = "lblXCenter"
        Me.lblXCenter.Size = New System.Drawing.Size(75, 11)
        Me.lblXCenter.TabIndex = 11
        Me.lblXCenter.Text = "( , )"
        '
        'lblYTop
        '
        Me.lblYTop.Location = New System.Drawing.Point(22, 4)
        Me.lblYTop.Name = "lblYTop"
        Me.lblYTop.Size = New System.Drawing.Size(58, 10)
        Me.lblYTop.TabIndex = 12
        Me.lblYTop.Text = "( , )"
        Me.lblYTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblXRight
        '
        Me.lblXRight.Location = New System.Drawing.Point(677, 607)
        Me.lblXRight.Name = "lblXRight"
        Me.lblXRight.Size = New System.Drawing.Size(75, 11)
        Me.lblXRight.TabIndex = 10
        Me.lblXRight.Text = "( , )"
        '
        'lblXLeft
        '
        Me.lblXLeft.Location = New System.Drawing.Point(81, 607)
        Me.lblXLeft.Name = "lblXLeft"
        Me.lblXLeft.Size = New System.Drawing.Size(75, 11)
        Me.lblXLeft.TabIndex = 8
        Me.lblXLeft.Text = "( , )"
        '
        'pbxGraph
        '
        Me.pbxGraph.BackColor = System.Drawing.Color.White
        Me.pbxGraph.Location = New System.Drawing.Point(83, 3)
        Me.pbxGraph.Name = "pbxGraph"
        Me.pbxGraph.Size = New System.Drawing.Size(600, 600)
        Me.pbxGraph.TabIndex = 7
        Me.pbxGraph.TabStop = False
        '
        'lblYBottom
        '
        Me.lblYBottom.Location = New System.Drawing.Point(22, 592)
        Me.lblYBottom.Name = "lblYBottom"
        Me.lblYBottom.Size = New System.Drawing.Size(58, 10)
        Me.lblYBottom.TabIndex = 13
        Me.lblYBottom.Text = "( , )"
        Me.lblYBottom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'uclGraph
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblYBottom)
        Me.Controls.Add(Me.lblXCenter)
        Me.Controls.Add(Me.lblXRight)
        Me.Controls.Add(Me.lblXLeft)
        Me.Controls.Add(Me.lblYCenter)
        Me.Controls.Add(Me.lblYTop)
        Me.Controls.Add(Me.pbxGraph)
        Me.Name = "uclGraph"
        Me.Size = New System.Drawing.Size(781, 633)
        CType(Me.pbxGraph, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents lblYCenter As System.Windows.Forms.Label
    Private WithEvents lblXCenter As System.Windows.Forms.Label
    Private WithEvents lblYTop As System.Windows.Forms.Label
    Private WithEvents lblXRight As System.Windows.Forms.Label
    Private WithEvents lblXLeft As System.Windows.Forms.Label
    Private WithEvents pbxGraph As System.Windows.Forms.PictureBox
    Private WithEvents lblYBottom As System.Windows.Forms.Label

End Class
