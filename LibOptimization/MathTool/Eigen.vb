Namespace MathTool
    ''' <summary>
    ''' store Eigen values, vector
    ''' </summary>
    <Serializable>
    Public Class Eigen
        ''' <summary>
        ''' for sort
        ''' </summary>
        Private Class ValueDescSort
            Implements IComparable

            Public v As Double = 0.0

            Public idx As Integer = 0

            Public Sub New(ByVal v As Double, ByVal idx As Integer)
                Me.v = v
                Me.idx = idx
            End Sub

            Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
                'Nothing check
                If obj Is Nothing Then
                    Return 1
                End If

                'Type check
                If Not Me.GetType() Is obj.GetType() Then
                    Throw New ArgumentException("Different type", "obj")
                End If

                'Compare descent sort
                Dim mineValue As Double = Me.v
                Dim compareValue As Double = DirectCast(obj, ValueDescSort).v
                If mineValue < compareValue Then
                    Return 1
                ElseIf mineValue > compareValue Then
                    Return -1
                Else
                    Return 0
                End If
            End Function
        End Class

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

        ''' <summary>
        ''' sort by eigen value
        ''' </summary>
        ''' <param name="eigenValue"></param>
        ''' <param name="eigenVector"></param>
        ''' <param name="isColOrder"></param>
        Public Shared Sub EigenSort(ByRef eigenValue As DenseVector, ByRef eigenVector As DenseMatrix, ByVal isColOrder As Boolean)
            Dim n = eigenValue.Count
            Dim colSwapInfo = New List(Of ValueDescSort)
            For i As Integer = 0 To n - 1
                colSwapInfo.Add(New ValueDescSort(eigenValue(i), i))
            Next
            colSwapInfo.Sort()

            If isColOrder = True Then
                Dim newEigenVector = New DenseMatrix(n)
                For j As Integer = 0 To n - 1
                    'eigen value
                    eigenValue(j) = colSwapInfo(j).v

                    'eigen vector
                    Dim k = colSwapInfo(j).idx
                    For i As Integer = 0 To n - 1
                        newEigenVector(i)(j) = eigenVector(i)(k)
                    Next
                Next
                eigenVector = newEigenVector
            Else
                Dim newEigenVector = New DenseMatrix(n)
                For i = 0 To n - 1
                    'eigen value
                    eigenValue(i) = colSwapInfo(i).v

                    'eigen vector
                    Dim k = colSwapInfo(i).idx
                    newEigenVector(i) = eigenVector(k)
                Next
                eigenVector = newEigenVector
            End If
        End Sub
    End Class
End Namespace
