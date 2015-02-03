<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
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
        Me.btnInit = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.oPlot = New OxyPlot.WindowsForms.Plot()
        Me.SuspendLayout()
        '
        'btnInit
        '
        Me.btnInit.Location = New System.Drawing.Point(12, 12)
        Me.btnInit.Name = "btnInit"
        Me.btnInit.Size = New System.Drawing.Size(75, 23)
        Me.btnInit.TabIndex = 1
        Me.btnInit.Text = "Init"
        Me.btnInit.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.Location = New System.Drawing.Point(93, 12)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(75, 23)
        Me.btnBack.TabIndex = 1
        Me.btnBack.Text = "<"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'btnNext
        '
        Me.btnNext.Location = New System.Drawing.Point(174, 12)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(75, 23)
        Me.btnNext.TabIndex = 1
        Me.btnNext.Text = ">"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'oPlot
        '
        Me.oPlot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.oPlot.Location = New System.Drawing.Point(0, 0)
        Me.oPlot.Name = "oPlot"
        Me.oPlot.PanCursor = System.Windows.Forms.Cursors.Hand
        Me.oPlot.Size = New System.Drawing.Size(500, 500)
        Me.oPlot.TabIndex = 2
        Me.oPlot.Text = "Plot1"
        Me.oPlot.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE
        Me.oPlot.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE
        Me.oPlot.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(500, 500)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnBack)
        Me.Controls.Add(Me.btnInit)
        Me.Controls.Add(Me.oPlot)
        Me.Name = "Form1"
        Me.Text = "Visualization"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnInit As System.Windows.Forms.Button
    Friend WithEvents btnBack As System.Windows.Forms.Button
    Friend WithEvents btnNext As System.Windows.Forms.Button
    Friend WithEvents oPlot As OxyPlot.WindowsForms.Plot

End Class
