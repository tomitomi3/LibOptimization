<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Visualization
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
        Me.btnNext = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.lblIterationCount = New System.Windows.Forms.Label()
        Me.lblStep = New System.Windows.Forms.Label()
        Me.graph = New DrawGraph.uclGraph()
        Me.SuspendLayout()
        '
        'btnInit
        '
        Me.btnInit.Location = New System.Drawing.Point(9, 5)
        Me.btnInit.Name = "btnInit"
        Me.btnInit.Size = New System.Drawing.Size(75, 23)
        Me.btnInit.TabIndex = 1
        Me.btnInit.Text = "Init"
        Me.btnInit.UseVisualStyleBackColor = True
        '
        'btnNext
        '
        Me.btnNext.Location = New System.Drawing.Point(216, 5)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(75, 23)
        Me.btnNext.TabIndex = 2
        Me.btnNext.Text = "Next"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.Location = New System.Drawing.Point(135, 5)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(75, 23)
        Me.btnBack.TabIndex = 3
        Me.btnBack.Text = "Back"
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'lblIterationCount
        '
        Me.lblIterationCount.Location = New System.Drawing.Point(306, 5)
        Me.lblIterationCount.Name = "lblIterationCount"
        Me.lblIterationCount.Size = New System.Drawing.Size(123, 18)
        Me.lblIterationCount.TabIndex = 4
        Me.lblIterationCount.Text = "Count:"
        '
        'lblStep
        '
        Me.lblStep.Location = New System.Drawing.Point(435, 5)
        Me.lblStep.Name = "lblStep"
        Me.lblStep.Size = New System.Drawing.Size(100, 23)
        Me.lblStep.TabIndex = 5
        Me.lblStep.Text = "Step:"
        '
        'graph
        '
        Me.graph.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.graph.Location = New System.Drawing.Point(0, 47)
        Me.graph.Name = "graph"
        Me.graph.Size = New System.Drawing.Size(706, 635)
        Me.graph.TabIndex = 6
        '
        'Visualization
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(706, 682)
        Me.Controls.Add(Me.graph)
        Me.Controls.Add(Me.lblStep)
        Me.Controls.Add(Me.lblIterationCount)
        Me.Controls.Add(Me.btnBack)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnInit)
        Me.Name = "Visualization"
        Me.Text = "Visualization"
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents btnInit As System.Windows.Forms.Button
    Private WithEvents btnNext As System.Windows.Forms.Button
    Private WithEvents btnBack As System.Windows.Forms.Button
    Private WithEvents lblIterationCount As System.Windows.Forms.Label
    Private WithEvents lblStep As System.Windows.Forms.Label
    Friend WithEvents graph As DrawGraph.uclGraph

End Class
