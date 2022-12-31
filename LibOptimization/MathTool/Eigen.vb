Namespace MathTool
    ''' <summary>
    ''' store Eigen values, vector
    ''' </summary>
    <Serializable>
    Public Class Eigen
        Public Property EigenValue As DenseVector = Nothing

        Public Property EigenVector As DenseMatrix = Nothing

        ''' <summary>
        ''' 収束したかのフラグ
        ''' </summary>
        ''' <returns></returns>
        Public Property IsConversion As Boolean = Nothing

        Private Sub New()
        End Sub

        Public Sub New(ByRef eigeValue As DenseVector, ByRef eigenVec As DenseMatrix)
            Me.EigenValue = eigeValue
            Me.EigenVector = eigenVec
        End Sub

        Public Sub New(ByRef eigeValue As DenseVector, ByRef eigenVec As DenseMatrix, ByVal isconversion As Boolean)
            Me.EigenValue = eigeValue
            Me.EigenVector = eigenVec
            Me.IsConversion = isconversion
        End Sub
    End Class
End Namespace
