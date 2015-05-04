Namespace Optimization
    ''' <summary>
    ''' Eval
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsEval
        Implements IComparable
        Private m_index As Integer = 0
        Private m_eval As Double = 0

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_index"></param>
        ''' <param name="ai_eval"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_index As Integer, ByVal ai_eval As Double)
            Me.m_index = ai_index
            Me.m_eval = ai_eval
        End Sub

        ''' <summary>
        ''' Eval
        ''' </summary>
        ''' <param name="ai_index"></param>
        ''' <param name="ai_eval"></param>
        ''' <remarks></remarks>
        Public Sub SetEval(ByVal ai_index As Integer, ByVal ai_eval As Double)
            Me.m_index = ai_index
            Me.m_eval = ai_eval
        End Sub

        ''' <summary>
        ''' get evaluate value
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Eval() As Double
            Get
                Return Me.m_eval
            End Get
        End Property

        ''' <summary>
        ''' get index
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Index() As Integer
            Get
                Return Me.m_index
            End Get
        End Property

        ''' <summary>
        ''' Compare(ICompareble)
        ''' </summary>
        ''' <param name="ai_obj"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' larger Me than obj is -1. smaller Me than obj is 1.
        ''' Equal is return to Zero
        ''' </remarks>
        Public Function CompareTo(ByVal ai_obj As Object) As Integer Implements System.IComparable.CompareTo
            'Nothing check
            If ai_obj Is Nothing Then
                Return 1
            End If

            'Type check
            If Not Me.GetType() Is ai_obj.GetType() Then
                Throw New ArgumentException("Different type", "obj")
            End If

            'Compare
            Dim mineValue As Double = Me.m_eval
            Dim compareValue As Double = DirectCast(ai_obj, clsEval).Eval
            If mineValue = compareValue Then
                Return 0
            ElseIf mineValue < compareValue Then
                Return -1
            Else
                Return 1
            End If
        End Function
    End Class
End Namespace