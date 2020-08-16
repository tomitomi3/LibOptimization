Imports LibOptimization.Optimization
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Firefly class
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Public Class clsFireFly
        Inherits clsEasyVector
        Implements IComparable

        Private m_func As absObjectiveFunction = Nothing
        Private m_evaluateValue As Double = 0.0

        Public Property Intensity As Double = 0.0

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
        Public Sub New(ByVal ai_vertex As clsFireFly)
            Me.m_func = ai_vertex.GetFunc()
            Me.AddRange(ai_vertex) 'ok
            Me.m_evaluateValue = ai_vertex.Eval
            Me.Intensity = 1 / (Me.m_evaluateValue + 0.0001)
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
            Me.Intensity = 1 / (Me.m_evaluateValue + 0.0001)
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
            Me.Intensity = 1 / (Me.m_evaluateValue + 0.0001)
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
            Me.Intensity = 1 / (Me.m_evaluateValue + 0.0001)
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
            Me.m_evaluateValue = Me.m_func.F(Me)
            Me.Intensity = 1 / (Me.m_evaluateValue + 0.0001)
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
            Dim mineValue As Double = Me.Eval
            Dim compareValue As Double = DirectCast(ai_obj, clsFireFly).Eval
            If mineValue < compareValue Then
                Return -1
            ElseIf mineValue > compareValue Then
                Return 1
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' Re Evaluate
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReEvaluate()
            Me.m_evaluateValue = Me.m_func.F(Me)
            Me.Intensity = 1 / (Me.m_evaluateValue + 0.0001)
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
        ''' Init
        ''' </summary>
        ''' <param name="ai_range">-ai_range to ai_range</param>
        ''' <param name="ai_rand">Random object</param>
        ''' <remarks></remarks>
        Public Sub InitValue(ByVal ai_range As Double, ByVal ai_rand As System.Random)
            For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                Me.Add(Math.Abs(2.0 * ai_range) * ai_rand.NextDouble() - ai_range)
            Next
        End Sub

        ''' <summary>
        ''' Copy
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Copy() As clsFireFly
            Return New clsFireFly(Me)
        End Function

        ''' <summary>
        ''' Copy
        ''' </summary>
        ''' <param name="ai_point"></param>
        ''' <remarks></remarks>
        Public Sub Copy(ByVal ai_point As clsFireFly)
            For i As Integer = 0 To ai_point.Count - 1
                Me(i) = ai_point(i)
            Next
            Me.m_evaluateValue = ai_point.Eval
            Me.Intensity = ai_point.Intensity
        End Sub
    End Class
End Namespace