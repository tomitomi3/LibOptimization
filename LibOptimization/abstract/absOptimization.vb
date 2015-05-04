Imports LibOptimization.Util

Namespace Optimization
    ''' <summary>
    ''' Abstarct optimization Class
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class absOptimization
        Protected m_func As absObjectiveFunction = Nothing
        Protected m_iteration As Integer = 0
        Protected m_rand As System.Random = New clsRandomXorshift(CUInt(Environment.TickCount))

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
        ''' Result for debug
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Get all result.
        ''' Do not need to implement this method.
        ''' e.g)Throw New NotImplementedException
        ''' </remarks>
        Public MustOverride ReadOnly Property ResultForDebug() As List(Of clsPoint)

        ''' <summary>
        ''' Recent Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function IsRecentError() As Boolean

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
        ''' Objective Function
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
        ''' Random object
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
        ''' Reset Iteration count
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ResetIterationCount()
            Me.m_iteration = 0
        End Sub
    End Class

End Namespace
