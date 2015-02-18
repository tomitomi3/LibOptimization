Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Basic Particle Swarm Optmization
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]James Kennedy and Russell Eberhart, "Particle Swarm Optimization．", Proceedings of IEEE the International Conference on Neural Networks，1995
    ''' [2]Y. Shi and Eberhart, R.C., "A Modified Particle Swarm Optimizer", Proceedings of Congress on Evolu-tionary Computation, 79-73., 1998
    ''' [3]Eberhart, R.C. and Y. Shi, "Comparing inertia weights and constriction factors in particle swarm optimization", In Proceedings of the Congress on Evolutionary Computation, vol. 1, pp. 84–88, IEEE, La Jolla, Calif, USA, July 2000.
    ''' </remarks>
    Public Class clsOptPSO : Inherits absOptimization
#Region "Member"
        'Common parameters
        Private EPS As Double = 0.000000001
        Private MAX_ITERATION As Integer = 5000
        Private INIT_PARAM_RANGE As Double = 5.12 'This Parameter to use when generate a variable
        Private IsUseCriterion As Boolean = True

        'particles
        Private m_swarm As New List(Of clsParticle)

        'PSO Parameters
        Private Size As Integer = 100
        Private Weight As Double = 0.729
        Private C1 As Double = 1.49445
        Private C2 As Double = 1.49445

        'ErrorManage
        Private m_error As New clsError
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Property(Parameter setting)"
        ''' <summary>
        ''' EPS
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_EPS As Double
            Set(value As Double)
                Me.EPS = value
            End Set
        End Property

        ''' <summary>
        ''' Max Iteration
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_MAX_ITERATION As Integer
            Set(value As Integer)
                Me.MAX_ITERATION = value
            End Set
        End Property

        ''' <summary>
        ''' Init range
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_InitRange As Double
            Set(value As Double)
                Me.INIT_PARAM_RANGE = value
            End Set
        End Property

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Common parameter</remarks>
        Public WriteOnly Property PARAM_IsUseCriterion As Boolean
            Set(value As Boolean)
                Me.IsUseCriterion = value
            End Set
        End Property

        ''' <summary>
        ''' Swarm Size
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_Size As Integer
            Set(value As Integer)
                Me.Size = value
            End Set
        End Property

        ''' <summary>
        ''' Inertia weight.
        ''' Weigth=1.0(orignal paper 1995), Weight=0.729(Default setting)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' recommend value is 0.4 to 0.9.
        ''' </remarks>
        Public WriteOnly Property PARAM_Weight As Double
            Set(value As Double)
                Me.Weight = value
            End Set
        End Property

        ''' <summary>
        ''' velocity coefficient(affected by personal best).
        ''' C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_C1 As Double
            Set(value As Double)
                Me.C1 = value
            End Set
        End Property

        ''' <summary>
        ''' velocity coefficient(affected by global best)
        ''' C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public WriteOnly Property PARAM_C2 As Double
            Set(value As Double)
                Me.C2 = value
            End Set
        End Property
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_swarm.Clear()

                'Set initialize value
                For i As Integer = 0 To Me.Size - 1
                    Dim tempPosition = New clsPoint(Me.m_func)
                    Dim tempBestPosition = New clsPoint(Me.m_func)
                    Dim tempVelocity(Me.m_func.NumberOfVariable - 1) As Double
                    For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        tempPosition(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                        tempBestPosition(j) = tempPosition(j)
                        tempVelocity(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
                    Next
                    tempPosition.ReEvaluate()
                    tempBestPosition.ReEvaluate()
                    Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                Next

                'Sort Evaluate
                Me.m_swarm.Sort()

            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do optimize
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'do iterate
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'check iteration count
                If MAX_ITERATION <= m_iteration Then
                    Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                '-------------------------------------------------------------------
                'Particle Swarm Optimize Iteration
                '-------------------------------------------------------------------
                'replace personal best
                For Each particle In Me.m_swarm
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point
                    End If
                Next

                'get global best
                Me.m_swarm.Sort()
                Dim globalBestPoint = Me.m_swarm(0).BestPoint.ToArray()

                'check criterion
                If Me.IsUseCriterion = True Then
                    If IsCriterion(Me.m_swarm(0).BestPoint, Me.m_swarm(Me.Size - 1).BestPoint) < Me.EPS Then
                        Return True
                    End If
                End If

                'update point and velocity
                For Each particle In Me.m_swarm
                    'update a velocity 
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) + _
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) + _
                                   C2 * r2 * (globalBestPoint(i) - particle.Point(i))

                        'range check
                        'If newV < Me.SEARCH_SPACE_RANGE_MIN Then
                        '    newV = Me.SEARCH_SPACE_RANGE_MIN
                        'ElseIf newV > Me.SEARCH_SPACE_RANGE_MAX Then
                        '    newV = Me.SEARCH_SPACE_RANGE_MAX
                        'End If

                        particle.Velocity(i) = newV
                    Next

                    'update a position using velocity
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim newPos = particle.Point(i) + particle.Velocity(i)

                        'range check
                        'If newPos < Me.SEARCH_SPACE_RANGE_MIN Then
                        '    newPos = Me.SEARCH_SPACE_RANGE_MIN
                        'ElseIf newPos > Me.SEARCH_SPACE_RANGE_MAX Then
                        '    newPos = Me.SEARCH_SPACE_RANGE_MAX
                        'End If

                        particle.Point(i) = newPos
                    Next
                    particle.Point.ReEvaluate()
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Recent Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As Optimization.clsPoint
            Get
                Return Me.m_swarm(0).Point
            End Get
        End Property

        ''' <summary>
        ''' for Debug
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property ResultForDebug As List(Of Optimization.clsPoint)
            Get
                Throw New NotImplementedException()
            End Get
        End Property
#End Region

#Region "Private"
        ''' <summary>
        ''' Check Criterion
        ''' </summary>
        ''' <param name="ai_best"></param>
        ''' <param name="ai_worst"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsCriterion(ByVal ai_best As clsPoint, ByVal ai_worst As clsPoint) As Double
            Dim bestEval As Double = ai_best.Eval
            Dim worstEval As Double = ai_worst.Eval
            Dim temp As Double = 2.0 * Math.Abs(worstEval - bestEval) / (Math.Abs(worstEval) + Math.Abs(bestEval) + 0.0000000001)
            Return temp
        End Function
#End Region
    End Class
End Namespace
