Namespace MathTool
    ''' <summary>
    ''' store SVD decomposition
    ''' </summary>
    <Serializable>
    Public Class SVD
        ''' <summary></summary>
        Public Property S As DenseMatrix = Nothing

        ''' <summary></summary>
        Public Property V As DenseVector = Nothing

        ''' <summary></summary>
        Public Property D As DenseMatrix = Nothing

        Private Sub New()
        End Sub

        Public Sub New(ByRef matS As DenseMatrix, ByRef matV As DenseVector, ByRef matD As DenseMatrix)
            Me.S = matS
            Me.V = matV
            Me.D = matD
        End Sub
    End Class
End Namespace
