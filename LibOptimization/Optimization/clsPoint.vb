Imports LibOptimization.Optimization
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Point Class
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsPoint
        Inherits clsShoddyVector
        Implements IComparable

        Private m_func As absObjectiveFunction = Nothing
        Private m_evaluateValue As Double = 0.0

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' copy constructor
        ''' </summary>
        ''' <param name="ai_vertex"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_vertex As clsPoint)
            Me.m_func = ai_vertex.m_func
            Me.AddRange(ai_vertex) 'ok
            Me.m_evaluateValue = ai_vertex.Eval
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
            Me.AddRange(New Double(ai_func.NumberOfVariable - 1) {}) 'ok
            Me.m_evaluateValue = Me.m_func.F(Me)
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <param name="ai_vars"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_vars As List(Of Double))
            Me.m_func = ai_func
            Me.AddRange(ai_vars) 'ok
            Me.m_evaluateValue = Me.m_func.F(Me)
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <param name="ai_vars"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_vars() As Double)
            Me.m_func = ai_func
            Me.AddRange(ai_vars) 'ok
            Me.m_evaluateValue = Me.m_func.F(Me)
        End Sub

        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="ai_func"></param>
        ''' <param name="ai_dim"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, ByVal ai_dim As Integer)
            Me.m_func = ai_func
            Me.AddRange(New Double(ai_dim - 1) {}) 'ok
        End Sub

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
            Dim mineValue As Double = Me.m_evaluateValue
            Dim compareValue As Double = DirectCast(ai_obj, clsPoint).m_evaluateValue
            If mineValue = compareValue Then
                Return 0
            ElseIf mineValue < compareValue Then
                Return -1
            Else
                Return 1
            End If
        End Function

        ''' <summary>
        ''' Re Evaluate
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReEvaluate()
            Me.m_evaluateValue = Me.m_func.F(Me)
        End Sub

        ''' <summary>
        ''' Get Function
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFunc() As absObjectiveFunction
            Return Me.m_func
        End Function

        ''' <summary>
        ''' EvaluateValue
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Eval() As Double
            Get
                Return Me.m_evaluateValue
            End Get
        End Property

        ''' <summary>
        ''' Copy
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Copy() As clsPoint
            Return New clsPoint(Me)
        End Function
    End Class
End Namespace