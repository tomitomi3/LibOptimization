Imports LibOptimization.Util

Namespace Optimization
    ''' <summary>
    ''' Abstarct optimization Class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class absOptimization
        ''' <summary>Objective function</summary>
        Protected m_func As absObjectiveFunction = Nothing

        ''' <summary>Iteration count</summary>
        Protected m_iteration As Integer = 0

        ''' <summary>Random object</summary>
        Protected m_rand As System.Random = New clsRandomXorshift(BitConverter.ToUInt32(BitConverter.GetBytes(Environment.TickCount), 0))

        ''' <summary>Error manage class</summary>
        Protected m_error As New clsError

        ''' <summary>Initial position</summary>
        Public Property InitialPosition As Double() = Nothing

        ''' <summary>Upper range of initial value</summary>
        ''' <remarks>This parameters to use when generate a variable</remarks>
        Public Property InitialValueRangeUpper As Double = 5 'parameter range

        ''' <summary>Lower range of initial value</summary>
        ''' <remarks>This parameters to use when generate a variable</remarks>
        Public Property InitialValueRangeLower As Double = -5 'parameter range

        ''' <summary>Use criterion</summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>Memo</summary>
        Public Property Memo As String = String.Empty

        ''' <summary>
        ''' Objective function Property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ObjectiveFunction As absObjectiveFunction
            Get
                Return Me.m_func
            End Get
            Set(ByVal value As absObjectiveFunction)
                Me.m_func = value
            End Set
        End Property

        ''' <summary>
        ''' Random object Property
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Random As System.Random
            Get
                Return Me.m_rand
            End Get
            Set(ByVal value As System.Random)
                Me.m_rand = value
            End Set
        End Property

        ''' <summary>
        ''' Initialize parameter
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Sub Init()

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>true:Stopping Criterion. false:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public MustOverride Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property Result() As clsPoint

        ''' <summary>
        ''' Results
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Get all result.
        ''' Do not need to implement this method.
        ''' e.g)Throw New NotImplementedException
        ''' </remarks>
        Public MustOverride ReadOnly Property Results() As List(Of clsPoint)

        ''' <summary>
        ''' Recent Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function IsRecentError() As Boolean

        ''' <summary>
        ''' Max Iteration
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Property Iteration As Integer

        ''' <summary>
        ''' Iteration count 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IterationCount() As Integer
            Get
                Return Me.m_iteration
            End Get
        End Property

        ''' <summary>
        ''' Reset Iteration count
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ResetIterationCount()
            Me.m_iteration = 0
        End Sub
    End Class

End Namespace
